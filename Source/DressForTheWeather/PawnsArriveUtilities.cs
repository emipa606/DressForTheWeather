using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace DressForTheWeather;

public static class PawnsArriveUtilities
{
    private const int maxTries = 100;

    private static List<ThingStuffPair> allApparelPairs;

    private static DressForTheWeatherSettings settings;


    private static bool ApparelRequirementCanUseStuff(SpecificApparelRequirement req, ThingStuffPair pair)
    {
        if (req.Stuff == null || !PawnApparelGenerator.ApparelRequirementHandlesThing(req, pair.thing))
        {
            return true;
        }

        return pair.stuff != null && req.Stuff == pair.stuff;
    }


    private static bool CanUseStuff(Pawn pawn, ThingStuffPair pair)
    {
        List<SpecificApparelRequirement> apparelRequirements = pawn.kindDef.specificApparelRequirements;
        if (apparelRequirements != null)
        {
            for (int index = 0; index < apparelRequirements.Count; ++index)
            {
                if (!ApparelRequirementCanUseStuff(apparelRequirements[index], pair))
                {
                    return false;
                }
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

        bool isWearingGasMask = false;
        ThingDef gasMaskDef = null;
        //if pawn is not wearing a gas mask and should have one and biotech is enabled
        if (ModLister.BiotechInstalled &&
            (map.pollutionGrid.TotalPollutionPercent > 0.5 ||
                map.GameConditionManager.ConditionIsActive(GameConditionDefOf.ToxicFallout)))
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
                ThingStuffPair tsp = allApparelPairs!.Where(p => p.thing == gasMaskDef).RandomElementByWeight(p => p.Commonality);
                gasMask = (Apparel)ThingMaker.MakeThing(tsp.thing, tsp.stuff);
            }
            //initialize apparel
            gasMask.InitializeComps();
            //add apparel to pawn
            pawn.apparel.Wear(gasMask, false);
            PawnGenerator.PostProcessGeneratedGear(gasMask, pawn);
            isWearingGasMask = true;
        }

        bool isWearingVacSuit = false;
        //if pawn is not wearing a vacsuit and should have one and oddysey is enabled
        if (ModLister.OdysseyInstalled && map.Biome.inVacuum && pawn.Faction?.def?.techLevel >= TechLevel.Spacer)
        {
            //create new vacsuit and helmet
            var vacSuitDef = ThingDefOf.Apparel_Vacsuit;
            if(pawn.DevelopmentalStage == DevelopmentalStage.Baby || pawn.DevelopmentalStage == DevelopmentalStage.Child)
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
        float minTemp = pawn.GetStatValue(StatDefOf.ComfyTemperatureMin);
        //get maximum comfortable temperature for pawn
        float maxTemp = pawn.GetStatValue(StatDefOf.ComfyTemperatureMax);
        //get current temperature
        float currentTemp = map.mapTemperature.OutdoorTemp;
        int tries = 0;

        while (currentTemp < minTemp && tries < maxTries)
        {
            tries++;
            //generate random apparel
            Apparel apparel = GetNewApparel(pawn);
            if (apparel is null)
            {
                continue;
            }

            //get cold insulation
            float coldInsulation = apparel.GetStatValue(StatDefOf.Insulation_Cold);
            //compare to currently worn apparel in same slot
            List<Apparel> replacedApparel = GetApparelReplacedBy(pawn, apparel).ToList();

            if(replacedApparel.Any(a => !Settings.ReplaceableFilter.Allows(a)))
            {
                continue;
            }

            if(isWearingGasMask && replacedApparel.Any(apparel => apparel.def == gasMaskDef))
            {
                continue;
            }


            if (isWearingVacSuit && replacedApparel.Any(apparel => apparel.def == ThingDefOf.Apparel_Vacsuit || apparel.def == ThingDefOf.Apparel_VacsuitHelmet || apparel.def == DefDatabase<ThingDef>.GetNamed("Apparel_VacsuitChildren")))
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
            Apparel apparel = GetNewApparel(pawn);
            if (apparel is null)
            {
                continue;
            }
            //get heat insulation
            float heatInsulation = apparel.GetStatValue(StatDefOf.Insulation_Heat);
            //compare to currently worn apparel in same slot

            List<Apparel> replacedApparel = GetApparelReplacedBy(pawn, apparel).ToList();

            if (replacedApparel.Any(a => !Settings.ReplaceableFilter.Allows(a)))
            {
                continue;
            }

            if (isWearingGasMask && replacedApparel.Any(apparel => apparel.def == gasMaskDef))
            {
                continue;
            }


            if (isWearingVacSuit && replacedApparel.Any(apparel => apparel.def == ThingDefOf.Apparel_Vacsuit || apparel.def == ThingDefOf.Apparel_VacsuitHelmet))
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

    private static IEnumerable<Apparel> GetApparelReplacedBy(Pawn pawn, Apparel apparel)
    {
        return pawn.apparel.WornApparel
            .Where(
                x => x.def.apparel.bodyPartGroups.Intersect(apparel.def.apparel.bodyPartGroups).Any() &&
                    x.def.apparel.layers.Intersect(apparel.def.apparel.layers).Any());
    }

    private static Apparel GetNewApparel(Pawn pawn)
    {
        while (true)
        {
            //get pawn faction tech level
            TechLevel techLevel = pawn.Faction?.def.techLevel ?? TechLevel.Archotech;
            //Log tech level

            if (!allApparelPairs!.Where(p => p.thing.techLevel <= techLevel && CanUseStuff(pawn, p))
                .TryRandomElementByWeight(p => p.Commonality, out ThingStuffPair pair))
            {
                return null;
            }

            if (pair.thing is null)
            {
                return null;
            }

            //Create new apparel
            Apparel apparel = (Apparel)ThingMaker.MakeThing(pair.thing, pair.stuff);
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
                ThingStuffPair.AllWith(
            td => td.IsApparel &&
                !td.apparel.layers.Contains(ApparelLayerDefOf.Belt) &&
                !td.apparel.mechanitorApparel &&
                td.apparel.canBeGeneratedToSatisfyWarmth &&
                Settings.ApparelFilter.Allows(td));
    }

    public static DressForTheWeatherSettings Settings => settings ??=
        LoadedModManager.GetMod<DressForTheWeatherMod>().GetSettings<DressForTheWeatherSettings>();
}
