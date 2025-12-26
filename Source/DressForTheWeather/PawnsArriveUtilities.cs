using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace DressForTheWeather;

public static class PawnsArriveUtilities
{
    private const int maxTries = 100;

    private static List<ThingStuffPair> allApparelPairs;

    private static DressForTheWeatherSettings settings;

    private static readonly WeatherDef poisonForestDef =
        DefDatabase<WeatherDef>.GetNamedSilentFail("PoisonForestSpores");

    public static DressForTheWeatherSettings Settings => settings ??=
        LoadedModManager.GetMod<DressForTheWeatherMod>().GetSettings<DressForTheWeatherSettings>();


    private static bool apparelRequirementCanUseStuff(SpecificApparelRequirement req, ThingStuffPair pair)
    {
        if (req.Stuff == null || !PawnApparelGenerator.ApparelRequirementHandlesThing(req, pair.thing))
        {
            return true;
        }

        return pair.stuff != null && req.Stuff == pair.stuff;
    }


    private static bool canUseStuff(Pawn pawn, ThingStuffPair pair)
    {
        var apparelRequirements = pawn.kindDef.specificApparelRequirements;
        if (apparelRequirements == null)
        {
            return pair.stuff == null ||
                   pawn.Faction == null ||
                   pawn.kindDef.ignoreFactionApparelStuffRequirements ||
                   pawn.Faction.def.CanUseStuffForApparel(pair.stuff);
        }

        foreach (var apparelRequirement in apparelRequirements)
        {
            if (!apparelRequirementCanUseStuff(apparelRequirement, pair))
            {
                return false;
            }
        }

        return pair.stuff == null ||
               pawn.Faction == null ||
               pawn.kindDef.ignoreFactionApparelStuffRequirements ||
               pawn.Faction.def.CanUseStuffForApparel(pair.stuff);
    }


    public static void DressPawnAppropriately(Pawn pawn, Map map)
    {
        if (pawn.apparel is null)
        {
            return;
        }

        var isWearingGasMask = false;
        ThingDef gasMaskDef = null;
        //if pawn is not wearing a gas mask and should have one and biotech or advanced biomes are enabled
        if (ModLister.BiotechInstalled &&
            (map.pollutionGrid.TotalPollutionPercent > 0.5 ||
             map.GameConditionManager.ConditionIsActive(GameConditionDefOf.ToxicFallout)) ||
            poisonForestDef != null && map.weatherManager.curWeather == poisonForestDef)
        {
            Apparel gasMask;
            if (pawn.Faction?.def?.techLevel >= TechLevel.Industrial)
            {
                //get gas mask
                gasMaskDef = ThingDefOf.Apparel_GasMask;
                //create new gas mask
                gasMask = (Apparel)ThingMaker.MakeThing(gasMaskDef);
            }
            else
            {
                gasMaskDef = DefDatabase<ThingDef>.GetNamed("Apparel_WarVeil");
                var tsp = allApparelPairs!.Where(p => p.thing == gasMaskDef).RandomElementByWeight(p => p.Commonality);
                gasMask = (Apparel)ThingMaker.MakeThing(tsp.thing, tsp.stuff);
            }

            //initialize apparel
            gasMask.InitializeComps();
            //add apparel to pawn
            pawn.apparel.Wear(gasMask, false);
            PawnGenerator.PostProcessGeneratedGear(gasMask, pawn);
            isWearingGasMask = true;
        }

        var isWearingVacSuit = false;
        //if pawn is not wearing a vacsuit and should have one and oddysey is enabled
        if (ModLister.OdysseyInstalled && map.Biome.inVacuum && pawn.Faction?.def?.techLevel >= TechLevel.Spacer)
        {
            //create new vacsuit and helmet
            var vacSuitDef = ThingDefOf.Apparel_Vacsuit;
            if (pawn.DevelopmentalStage is DevelopmentalStage.Baby or DevelopmentalStage.Child)
            {
                vacSuitDef = DefDatabase<ThingDef>.GetNamed("Apparel_VacsuitChildren");
            }

            var vacSuit = (Apparel)ThingMaker.MakeThing(vacSuitDef);
            var vacSuitHelmet = (Apparel)ThingMaker.MakeThing(ThingDefOf.Apparel_VacsuitHelmet);

            //initialize apparel
            vacSuit.InitializeComps();
            vacSuitHelmet.InitializeComps();

            //add apparel to pawn
            pawn.apparel.Wear(vacSuit, false);
            pawn.apparel.Wear(vacSuitHelmet, false);
            PawnGenerator.PostProcessGeneratedGear(vacSuit, pawn);
            PawnGenerator.PostProcessGeneratedGear(vacSuitHelmet, pawn);
            isWearingVacSuit = true;
        }

        //get minimum comfortable temperature for pawn
        var minTemp = pawn.GetStatValue(StatDefOf.ComfyTemperatureMin);
        //get maximum comfortable temperature for pawn
        var maxTemp = pawn.GetStatValue(StatDefOf.ComfyTemperatureMax);
        //get current temperature
        var currentTemp = map.mapTemperature.OutdoorTemp;
        var tries = 0;

        while (currentTemp < minTemp && tries < maxTries)
        {
            tries++;
            //generate random apparel
            var apparel = getNewApparel(pawn);
            if (apparel is null)
            {
                continue;
            }

            //get cold insulation
            var coldInsulation = apparel.GetStatValue(StatDefOf.Insulation_Cold);
            //compare to currently worn apparel in same slot
            var replacedApparel = getApparelReplacedBy(pawn, apparel).ToList();

            if (replacedApparel.Any(a => !Settings.ReplaceableFilter.Allows(a)))
            {
                continue;
            }

            if (isWearingGasMask && replacedApparel.Any(found => found.def == gasMaskDef))
            {
                continue;
            }


            if (isWearingVacSuit && replacedApparel.Any(found =>
                    found.def == ThingDefOf.Apparel_Vacsuit || found.def == ThingDefOf.Apparel_VacsuitHelmet ||
                    found.def == DefDatabase<ThingDef>.GetNamed("Apparel_VacsuitChildren")))
            {
                continue;
            }

            //if currently worn apparel has more cold insulation, don't wear the new apparel
            if (replacedApparel.Sum(a => a.GetStatValue(StatDefOf.Insulation_Cold)) > coldInsulation)
            {
                continue;
            }

            pawn.apparel.Wear(apparel, false);
        }

        while (currentTemp > maxTemp && tries < maxTries)
        {
            tries++;
            //generate random apparels
            var apparel = getNewApparel(pawn);
            if (apparel is null)
            {
                continue;
            }

            //get heat insulation
            var heatInsulation = apparel.GetStatValue(StatDefOf.Insulation_Heat);
            //compare to currently worn apparel in same slot

            var replacedApparel = getApparelReplacedBy(pawn, apparel).ToList();

            if (replacedApparel.Any(a => !Settings.ReplaceableFilter.Allows(a)))
            {
                continue;
            }

            if (isWearingGasMask && replacedApparel.Any(found => found.def == gasMaskDef))
            {
                continue;
            }


            if (isWearingVacSuit && replacedApparel.Any(found =>
                    found.def == ThingDefOf.Apparel_Vacsuit || found.def == ThingDefOf.Apparel_VacsuitHelmet))
            {
                continue;
            }

            //if currently worn apparel has more heat insulation, don't wear the new apparel
            if (replacedApparel.Sum(a => a.GetStatValue(StatDefOf.Insulation_Heat)) > heatInsulation)
            {
                continue;
            }

            pawn.apparel.Wear(apparel, false);
            //write to log
            Log.Message($"Wearing {apparel.def.label} for {pawn.Name}");
        }
    }

    private static IEnumerable<Apparel> getApparelReplacedBy(Pawn pawn, Apparel apparel)
    {
        return pawn.apparel.WornApparel
            .Where(x => x.def.apparel.bodyPartGroups.Intersect(apparel.def.apparel.bodyPartGroups).Any() &&
                        x.def.apparel.layers.Intersect(apparel.def.apparel.layers).Any());
    }

    private static Apparel getNewApparel(Pawn pawn)
    {
        while (true)
        {
            //get pawn faction tech level
            var techLevel = pawn.Faction?.def.techLevel ?? TechLevel.Archotech;
            //Log tech level

            if (!allApparelPairs!.Where(p => p.thing.techLevel <= techLevel && canUseStuff(pawn, p))
                    .TryRandomElementByWeight(p => p.Commonality, out var pair))
            {
                return null;
            }

            if (pair.thing is null)
            {
                return null;
            }

            //Create new apparel
            var apparel = (Apparel)ThingMaker.MakeThing(pair.thing, pair.stuff);
            if (!apparel.PawnCanWear(pawn))
            {
                continue;
            }

            apparel.InitializeComps();
            PawnGenerator.PostProcessGeneratedGear(apparel, pawn);
            return apparel;
        }
    }

    public static void InitializeApparelPairs()
    {
        allApparelPairs =
            ThingStuffPair.AllWith(td => td.IsApparel &&
                                         !td.apparel.layers.Contains(ApparelLayerDefOf.Belt) &&
                                         !td.apparel.mechanitorApparel &&
                                         td.apparel.canBeGeneratedToSatisfyWarmth &&
                                         Settings.ApparelFilter.Allows(td));
    }
}