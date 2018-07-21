using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace Codename_Project_RIM
{

    public static class ThrumboTransformerUtility
    {

        public static void TryTransferHediffs(Pawn fromPawn, ref Pawn toPawn)
        {
            List<Hediff> pawnHediffs = fromPawn.health.hediffSet.GetHediffs<Hediff>().ToList();
            if (!pawnHediffs.NullOrEmpty())
            {
                foreach (Hediff hediff in pawnHediffs)
                {
                    if (hediff.CanBeAddedToThrumbo())
                        toPawn.health.AddHediff(hediff);
                }
            }
        }

        public static bool CanBeAddedToThrumbo(this Hediff hediff)
        {
            return hediff.def != PR_HediffDefOf.ThrumboHornGrowth && hediff.def != PR_HediffDefOf.ThrumboCrackAddiction && !(hediff is Hediff_AddedPart) && !(hediff is Hediff_Implant);
        }

    }

}
