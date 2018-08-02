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
    public class CompTargetEffect_ReduceMechanoid : CompTargetEffect
    {

        // Janky, but it works
        private List<Pawn> oldPawnCache = new List<Pawn>();
        private List<Pawn> newPawnCache = new List<Pawn>();
        private List<IntVec3> locCache = new List<IntVec3>();
        private List<bool> manhunterCache = new List<bool>();
        private Map map = null;

        public override void DoEffectOn(Pawn user, Thing target)
        {
            MechanoidReducerExtension extension = target.def.GetModExtension<MechanoidReducerExtension>();
            map = target.Map;

            // Update the caches
            for (int i = 0; i < extension.countRange.RandomInRange; i++)
            {
                newPawnCache.Add(PawnGenerator.GeneratePawn(extension.spawnedPawnKind));
                locCache.Add(target.Position);
                manhunterCache.Add(Rand.Chance(extension.manhunterChance));
            }
            oldPawnCache.Add((Pawn)target);
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            // Map will only not be null if the mechanoid reducer was actually used
            if (map != null)
            {
                // Destroy old pawns
                foreach (Pawn pawn in oldPawnCache)
                    pawn.Destroy();

                // Spawn new pawns
                for (int i = 0; i < newPawnCache.Count; i++)
                {
                    Pawn newPawn = (Pawn)GenSpawn.Spawn(newPawnCache[i], locCache[i], map);
                    if (manhunterCache[i])
                        newPawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter);
                }
            }
        }

    }
}
