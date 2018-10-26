using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Codename_Project_RIM
{
    public class Muffalo_Shell: Projectile
    {
        protected override void Impact(Thing hitThing)
        {
            Map map = base.Map;
            base.Impact(hitThing);
            PawnKindDef kindDef = PawnKindDef.Named("Muffalo");
            Pawn pawn = PawnGenerator.GeneratePawn(kindDef);
            GenSpawn.Spawn(pawn, base.Position, map);
            pawn.TryAddBloodlustHediff(0.3f);
            pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent, null, false, false, null);
            pawn.mindState.exitMapAfterTick = Find.TickManager.TicksGame + Rand.Range(60000, 135000);
        }
    }
}
