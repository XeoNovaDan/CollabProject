using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;
using Harmony;

namespace Codename_Project_RIM
{
    public static class LaunchedAnimalUtility
    {

        public static void TryAddBloodlustHediff(this Pawn pawn, float chance)
        {
            if (!pawn.health.hediffSet.HasHediff(PR_HediffDefOf.LaunchedAnimalBloodlust) && Rand.Chance(chance))
                pawn.health.AddHediff(PR_HediffDefOf.LaunchedAnimalBloodlust);
        }

    }
}
