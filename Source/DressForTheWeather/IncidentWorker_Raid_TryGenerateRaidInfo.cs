using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DressForTheWeather;

[HarmonyPatch(typeof(IncidentWorker_Raid), nameof(IncidentWorker_Raid.TryGenerateRaidInfo))]
public static class IncidentWorker_Raid_TryGenerateRaidInfo
{
    public static void Postfix(
        ref List<Pawn> pawns,
        IncidentParms parms)
    {
        if (pawns is null || parms?.target is null)
        {
            return;
        }

        //get map from parms
        var map = (Map)parms.target;

        if (!PawnsArriveUtilities.Settings.RaidersComePrepared)
        {
            return;
        }

        PawnsArriveUtilities.InitializeApparelPairs();
        foreach (var pawn in pawns)
        {
            PawnsArriveUtilities.DressPawnAppropriately(pawn, map);
        }
    }
}