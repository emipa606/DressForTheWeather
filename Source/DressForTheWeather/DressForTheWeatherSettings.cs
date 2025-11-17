using RimWorld;
using Verse;

namespace DressForTheWeather;

[StaticConstructorOnStartup]
public class DressForTheWeatherSettings : ModSettings
{
    public bool RaidersComePrepared;
    private ThingFilter apparelFilter;
    private ThingFilter replaceableFilter;


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
        private set { apparelFilter = value; }
    }

    public ThingFilter ReplaceableFilter
    {
        get
        {
            if (replaceableFilter is not null)
            {
                return replaceableFilter;
            }

            replaceableFilter = new ThingFilter();
            replaceableFilter.SetAllow(ThingCategoryDefOf.Apparel, true);
            apparelFilter.SetAllow(DefDatabase<ThingCategoryDef>.GetNamed("ApparelUtility"), false);
            replaceableFilter.SetAllow(ThingCategoryDefOf.ApparelArmor, false);
            replaceableFilter.SetAllow(ThingCategoryDefOf.ArmorHeadgear, false);
            replaceableFilter.SetAllow(DefDatabase<ThingDef>.GetNamed("Apparel_AdvancedHelmet"), true);
            replaceableFilter.SetAllow(DefDatabase<ThingDef>.GetNamed("Apparel_SimpleHelmet"), true);
            replaceableFilter.SetAllow(DefDatabase<ThingDef>.GetNamed("Apparel_FlakVest"), true);
            replaceableFilter.SetAllow(DefDatabase<ThingDef>.GetNamed("Apparel_FlakPants"), true);
            replaceableFilter.SetAllow(DefDatabase<ThingDef>.GetNamed("Apparel_FlakJacket"), true);
            //gas mask
            replaceableFilter.SetAllow(DefDatabase<ThingDef>.GetNamed("Apparel_GasMask"), false);
            return replaceableFilter;
        }
        private set { replaceableFilter = value; }
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref RaidersComePrepared, nameof(RaidersComePrepared), false);
        ThingFilter filter = ApparelFilter;
        Scribe_Deep.Look(ref filter, nameof(ApparelFilter));
        ApparelFilter = filter;

        ThingFilter filter2 = ReplaceableFilter;
        Scribe_Deep.Look(ref filter2, nameof(ReplaceableFilter));
        ReplaceableFilter = filter2;

        base.ExposeData();
    }
}
