using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Verse;

namespace DressForTheWeather;

public class DressForTheWeatherMod : Mod
{
    private static List<SpecialThingFilterDef> specialThingFilterDefs;

    private static List<SpecialThingFilterDef> SpecialThingFilterDefs
    {
        get
        {
            if (specialThingFilterDefs is not null)
            {
                return specialThingFilterDefs;
            }

            specialThingFilterDefs = new List<SpecialThingFilterDef>
            {
                SpecialThingFilterDefOf.AllowDeadmansApparel,
                SpecialThingFilterDefOf.AllowNonDeadmansApparel,
                SpecialThingFilterDefOf.AllowFresh,
                SpecialThingFilterDef.Named("AllowSmeltableApparel"),
                SpecialThingFilterDef.Named("AllowNonSmeltableApparel"),
                SpecialThingFilterDef.Named("AllowBurnableApparel"),
                SpecialThingFilterDef.Named("AllowNonBurnableApparel"),
                SpecialThingFilterDef.Named("AllowBiocodedApparel"),
                SpecialThingFilterDef.Named("AllowNonBiocodedApparel")
            };
            return specialThingFilterDefs;
        }
    }

    private static ThingFilter apparelGlobalFilter;

    private static ThingFilter ApparelGlobalFilter
    {
        get
        {
            if (apparelGlobalFilter is not null)
            {
                return apparelGlobalFilter;
            }

            apparelGlobalFilter = new ThingFilter();
            apparelGlobalFilter.SetAllow(ThingCategoryDefOf.Apparel, true);
            apparelGlobalFilter.allowedHitPointsConfigurable = false;
            apparelGlobalFilter.allowedQualitiesConfigurable = false;
            apparelGlobalFilter.DisplayRootCategory = new TreeNode_ThingCategory(ThingCategoryDefOf.Apparel);
            return apparelGlobalFilter;
        }
    }

    private static DressForTheWeatherSettings settings;

    private static DressForTheWeatherSettings Settings => settings ??=
        LoadedModManager.GetMod<DressForTheWeatherMod>().GetSettings<DressForTheWeatherSettings>();

    private readonly ThingFilterUI.UIState apparelThingFilterState = new();

    public DressForTheWeatherMod(ModContentPack content) : base(content)
    {
        new Harmony("DanielWedemeyer.DressForTheWeather").PatchAll(Assembly.GetExecutingAssembly());
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        ApparelGlobalFilter.allowedQualitiesConfigurable = false;
        Widgets.Label(inRect with { width = inRect.width / 2, height = 24 }, "DressForTheWeather_Raiders".Translate());
        Widgets.Checkbox(new Vector2(inRect.x + inRect.width - 24, inRect.y), ref Settings.RaidersComePrepared);

        Rect rect4 = new(0.0f, inRect.y + 40f, inRect.width / 2, inRect.height - 40f);

        if(Current.Game == null)
        {
            Widgets.Label(rect4, "DressForTheWeather_SettingsAvailableInGame".Translate());
            return;
        }

        Widgets.BeginGroup(rect4);
        Widgets.Label(new Rect(0.0f, 0.0f, rect4.width, 24f), "DressForTheWeather_ApparelFilter".Translate());

        ThingFilterUI.DoThingFilterConfigWindow(
            new Rect(0, 40, 300f, (float)(rect4.height - 45.0 - 10.0 - 40f)),
            apparelThingFilterState,
            Settings.ApparelFilter,
            ApparelGlobalFilter,
            16,
            forceHideHitPointsConfig: true,
            forceHiddenFilters:
                SpecialThingFilterDefs);
        Widgets.EndGroup();

        Rect rect5 = new(inRect.width / 2, inRect.y + 40f, inRect.width / 2, inRect.height - 40f);

        Widgets.BeginGroup(rect5);
        Widgets.Label(new Rect(0.0f, 0.0f, rect5.width, 24f), "DressForTheWeather_ReplaceableFilter".Translate());
        ThingFilterUI.DoThingFilterConfigWindow(
            new Rect(0, 40, 300f, (float)(rect5.height - 45.0 - 10.0 - 40f)),
            apparelThingFilterState,
            Settings.ReplaceableFilter,
            ApparelGlobalFilter,
            16,
            forceHideHitPointsConfig: true,
            forceHiddenFilters: SpecialThingFilterDefs);
        Widgets.EndGroup();

        base.DoSettingsWindowContents(inRect);
    }

    public override string SettingsCategory() { return "Dress For The Weather"; }
}
