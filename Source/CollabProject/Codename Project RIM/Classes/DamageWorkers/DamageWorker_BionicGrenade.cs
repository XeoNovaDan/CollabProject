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
    class DamageWorker_BionicGrenade : DamageWorker_AddInjury
    {

        private const float BaseChanceToAddHediff = 0.5f;

        protected override void ExplosionDamageThing(Explosion explosion, Thing t, List<Thing> damagedThings, IntVec3 cell)
        {
            base.ExplosionDamageThing(explosion, t, damagedThings, cell);
            List<HediffDef> validHediffs = DefDatabase<HediffDef>.AllDefs.Where(h => h.hediffClass == typeof(Hediff_AddedPart) || h.hediffClass == typeof(Hediff_Implant)).ToList();
            float hediffChance = BaseChanceToAddHediff / validHediffs.Count;
            foreach (Thing thing in damagedThings)
                if (thing is Pawn pawn && pawn.health is Pawn_HealthTracker health)
                    foreach (HediffDef hediff in validHediffs)
                        if (Rand.Chance(hediffChance))
                        {
                            BodyPartRecord part = pawn.RaceProps.body.AllParts.RandomElement();
                            if (!health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(part))
                                health.AddHediff(hediff, part);
                        }
        }

    }
}
