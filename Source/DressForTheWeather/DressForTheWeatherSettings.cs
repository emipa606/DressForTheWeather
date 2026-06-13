using System.Collections.Generic;
using System.Xml;
using RimWorld;
using Verse;

namespace DressForTheWeather;

[StaticConstructorOnStartup]
public class DressForTheWeatherSettings : ModSettings
{
    private ThingFilter apparelFilter;
    public bool RaidersComePrepared;


    public ThingFilter ApparelFilter
    {
        get
        {
            if (apparelFilter is not null)
            {
                return apparelFilter;
            }

            apparelFilter = new ThingFilter();
            apparelFilter.SetAllow(ThingCategoryDefOf.Apparel, true);
            apparelFilter.SetAllow(ThingCategoryDefOf.ApparelArmor, false);
            apparelFilter.SetAllow(ThingCategoryDefOf.ArmorHeadgear, false);
            apparelFilter.SetAllow(DefDatabase<ThingCategoryDef>.GetNamed("ApparelUtility"), false);
            return apparelFilter;
        }
        set => apparelFilter = value;
    }

    public ThingFilter ReplaceableFilter
    {
        get
        {
            if (field is not null)
            {
                return field;
            }

            field = new ThingFilter();
            field.SetAllow(ThingCategoryDefOf.Apparel, true);
            apparelFilter.SetAllow(DefDatabase<ThingCategoryDef>.GetNamed("ApparelUtility"), false);
            field.SetAllow(ThingCategoryDefOf.ApparelArmor, false);
            field.SetAllow(ThingCategoryDefOf.ArmorHeadgear, false);
            field.SetAllow(DefDatabase<ThingDef>.GetNamed("Apparel_AdvancedHelmet"), true);
            field.SetAllow(DefDatabase<ThingDef>.GetNamed("Apparel_SimpleHelmet"), true);
            field.SetAllow(DefDatabase<ThingDef>.GetNamed("Apparel_FlakVest"), true);
            field.SetAllow(DefDatabase<ThingDef>.GetNamed("Apparel_FlakPants"), true);
            field.SetAllow(DefDatabase<ThingDef>.GetNamed("Apparel_FlakJacket"), true);
            //gas mask
            if (ModLister.BiotechInstalled)
            {
                field.SetAllow(DefDatabase<ThingDef>.GetNamedSilentFail("Apparel_GasMask"), false);
            }

            return field;
        }
        set;
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref RaidersComePrepared, nameof(RaidersComePrepared));

        if (Scribe.mode == LoadSaveMode.LoadingVars)
        {
            CleanFilterXmlOfUnknownDefs(nameof(ApparelFilter));
            CleanFilterXmlOfUnknownDefs(nameof(ReplaceableFilter));
        }

        var filter = ApparelFilter;
        Scribe_Deep.Look(ref filter, nameof(ApparelFilter));
        ApparelFilter = filter;

        var filter2 = ReplaceableFilter;
        Scribe_Deep.Look(ref filter2, nameof(ReplaceableFilter));
        ReplaceableFilter = filter2;

        base.ExposeData();
    }

    private static void CleanFilterXmlOfUnknownDefs(string filterNodeName)
    {
        var allowedDefsNode = Scribe.loader?.curXmlParent?[filterNodeName]?["allowedDefs"];
        if (allowedDefsNode == null)
            return;

        var toRemove = new List<XmlNode>();
        foreach (XmlNode liNode in allowedDefsNode.ChildNodes)
        {
            var defName = liNode.InnerText?.Trim();
            if (!string.IsNullOrEmpty(defName) && DefDatabase<ThingDef>.GetNamedSilentFail(defName) == null)
                toRemove.Add(liNode);
        }

        foreach (var node in toRemove)
            allowedDefsNode.RemoveChild(node);
    }
}