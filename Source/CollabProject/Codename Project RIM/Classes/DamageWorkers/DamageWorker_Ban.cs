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
    public class DamageWorker_Ban : DamageWorker_Blunt
    {

        public BanDamageProps BanProps => def.GetModExtension<BanDamageProps>() ?? BanDamageProps.defaultValues;

        protected override void ApplySpecialEffectsToPart(Pawn pawn, float totalDamage, DamageInfo dinfo, DamageResult result)
        {
            if (Rand.Chance(BanProps.banChance))
            {
                Thing instigator = dinfo.Instigator;
                Pawn pawnToBan = null;

                bool haywire = false;

                if (Rand.Chance(BanProps.haywireBanChance))
                {
                    haywire = true;

                    IEnumerable<Pawn> bannablePawns = (Rand.Chance(BanProps.haywireBanPlayerPawnChance)) ?
                        instigator.Map.mapPawns.AllPawnsSpawned.Where(p => p.Faction == Faction.OfPlayer) : instigator.Map.mapPawns.AllPawnsSpawned;

                    pawnToBan = (Rand.Chance(BanProps.haywireBanMostValuablePawnChance)) ? GetMostValuablePawn(bannablePawns) : bannablePawns.RandomElement();
                }

                else
                    pawnToBan = pawn;

                BanPawn(pawnToBan, instigator, haywire);
            }

            else
                base.ApplySpecialEffectsToPart(pawn, totalDamage, dinfo, result);
        }

        private Pawn GetMostValuablePawn(IEnumerable<Pawn> bannablePawns)
        {
            Pawn pawnToBan = bannablePawns.RandomElement();
            float totalMarketValue = GetTotalMarketValue(pawnToBan);
            foreach (Pawn pawn in bannablePawns)
            {
                float totalMarketValueNew = GetTotalMarketValue(pawn);
                if (totalMarketValueNew > totalMarketValue)
                {
                    totalMarketValue = totalMarketValueNew;
                    pawnToBan = pawn;
                }
            }
            return pawnToBan;
        }

        private float GetTotalMarketValue(Pawn pawn)
        {
            float marketValue = pawn.MarketValue;
            if (pawn.equipment is Pawn_EquipmentTracker equipmentTracker)
                foreach (Thing equipment in equipmentTracker.AllEquipmentListForReading)
                    marketValue += equipment.MarketValue;
            if (pawn.apparel is Pawn_ApparelTracker apparelTracker)
                foreach (Thing apparel in apparelTracker.WornApparel)
                    marketValue += apparel.MarketValue;
            return marketValue;
        }

        public void BanPawn(Pawn pawnToBan, Thing instigator, bool haywire)
        {
            Map map = pawnToBan.Map;
            IntVec3 center = pawnToBan.Position;
            if (haywire)
                Messages.Message("BanhammerGoneHaywire".Translate(instigator.LabelShort), MessageTypeDefOf.CautionInput);
            if (BanMessage(pawnToBan, instigator, haywire, new LookTargets(center, map), out Message message))
                Messages.Message(message);
            pawnToBan.Destroy();
            if (BanProps.explosionDamageDef != null)
                GenExplosion.DoExplosion(center, map, BanProps.explosionRadius, BanProps.explosionDamageDef, null);
        }

        private bool BanMessage(Pawn pawnToBan, Thing instigator, bool haywire, LookTargets lookTargets, out Message message)
        {
            message = null;
            string ptbLabel = pawnToBan.LabelShort;
            string iLabel = instigator.LabelShort;
            if (pawnToBan.Faction == Faction.OfPlayer)
                message = new Message("PawnBannedMessage".Translate(ptbLabel, iLabel), MessageTypeDefOf.NegativeEvent, lookTargets);
            else
            {
                string text = "";
                if (pawnToBan.Faction != null && pawnToBan.RaceProps.Humanlike)
                    text = (haywire) ? "RandomNonPlayerHumanlikeBannedMessage".Translate(pawnToBan.KindLabel, ptbLabel, iLabel) : "PawnBannedMessage".Translate(ptbLabel, iLabel);
                else
                    text = (haywire) ? "RandomPawnBannedMessage".Translate(pawnToBan.KindLabel, iLabel) : "PawnBannedMessage".Translate(ptbLabel.CapitalizeFirst(), iLabel);
                message = new Message(text, MessageTypeDefOf.CautionInput, lookTargets);
            }
            if (pawnToBan.IsWildMan())
            {
                string text = (haywire) ? "RandomWildManBannedMessage".Translate(ptbLabel, iLabel) : "PawnBannedMessage".Translate(ptbLabel, iLabel);
                message = new Message("RandomWildManBannedMessage".Translate(ptbLabel, iLabel), MessageTypeDefOf.CautionInput, lookTargets);
            }
            return message != null;
        }

    }
}
