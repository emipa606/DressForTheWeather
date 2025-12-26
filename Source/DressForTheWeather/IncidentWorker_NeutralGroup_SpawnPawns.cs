using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DressForTheWeather;

[HarmonyPatch(typeof(IncidentWorker_NeutralGroup), "SpawnPawns")]
public static class IncidentWorker_NeutralGroup_SpawnPawns
{
    [HarmonyPostfix]
    public static void Postfix(ref List<Pawn> __result, IncidentParms parms)
    {
        PawnsArriveUtilities.InitializeApparelPairs();
        //get map from parms
        var map = (Map)parms.target;

        foreach (var pawn in __result)
        {
            PawnsArriveUtilities.DressPawnAppropriately(pawn, map);
        }
    }
}