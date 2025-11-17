using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace DressForTheWeather
{
    [HarmonyPatch(typeof(IncidentWorker_Raid), nameof(IncidentWorker_Raid.TryGenerateRaidInfo))]
    public static class IncidentWorker_Raid_TryGenerateRaidInfo
    {
        public static void Postfix(
            ref List<Pawn> pawns,
            IncidentParms parms)
        {
            if (pawns is null || parms is null || parms.target is null)
            {
                return;
            }

            //get map from parms
            Map map = (Map)parms.target;

            if (PawnsArriveUtilities.Settings.RaidersComePrepared)
            {
                PawnsArriveUtilities.InitializeApparelPairs();
                foreach (Pawn pawn in pawns)
                {
                    PawnsArriveUtilities.DressPawnAppropriately(pawn, map);
                }
            }
        }
    }
}