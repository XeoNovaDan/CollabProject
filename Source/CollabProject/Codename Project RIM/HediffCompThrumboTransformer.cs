using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace Codename_Project_RIM
{
    public class HediffCompThrumboTransformer : HediffComp
    {

        private const int ThrumboTransformCheckInterval = 60;

        public HediffCompProperties_ThrumboTransformer Props
        {
            get
            {
                return (HediffCompProperties_ThrumboTransformer)props;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (parent.CurStageIndex > 0 && Pawn.IsHashIntervalTick(ThrumboTransformCheckInterval) &&
                Rand.MTBEventOccurs(Props.mtbDaysTransformIntoThrumbo, GenDate.TicksPerDay, ThrumboTransformCheckInterval))
            {

                // Initial thrumbo generation
                Pawn newThrumbo = PawnGenerator.GeneratePawn(request: new PawnGenerationRequest(PawnKindDefOf.Thrumbo, Faction.OfPlayer, fixedBiologicalAge: Pawn.ageTracker.AgeBiologicalYearsFloat,
                    fixedChronologicalAge: Pawn.ageTracker.AgeChronologicalYearsFloat, fixedGender: Pawn.gender));

                // Set thrumbo needs
                newThrumbo.needs.food.CurLevel = Pawn.needs.food.CurLevel;
                newThrumbo.needs.rest.CurLevel = Pawn.needs.rest.CurLevel;

                // Set thrumbo skills
                newThrumbo.training.SetWantedRecursive(TrainableDefOf.Obedience, true);
                newThrumbo.training.Train(TrainableDefOf.Obedience, null, true);
                
                // Name thrumbo
                newThrumbo.Name = Pawn.Name;

                // Transfer hediffs from colonists to thrumbo where possible
                ThrumboTransformerUtility.TryTransferHediffs(Pawn, ref newThrumbo, Props);

                // Try to bond with another colonist + set up letter text
                string thrumboTransformLetterLabel = "LetterThrumboCrackTransformationLabel".Translate();
                string thrumboTransformLetter = "LetterThrumboCrackTransformation".Translate(new object[] { Pawn.Label, Pawn.gender.GetPossessive() });
                if (ThrumboTransformerUtility.TryGivePostTransformationBondRelation(ref newThrumbo, Pawn, out Pawn otherPawn))
                {
                    thrumboTransformLetterLabel += " " + "BondBrackets".Translate();
                    thrumboTransformLetter += "\n\n" + "TransformedThrumboBonded".Translate(new object[] { Pawn.gender.GetPronoun().CapitalizeFirst(), otherPawn.LabelShort });
                }

                // Spawn thrumbo and remove pawn
                Pawn thrumbo = (Pawn)GenSpawn.Spawn(newThrumbo, Pawn.PositionHeld, Pawn.MapHeld);
                thrumbo.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter, forceWake: true, transitionSilently: true);
                Find.LetterStack.ReceiveLetter(thrumboTransformLetterLabel, thrumboTransformLetter, LetterDefOf.ThreatSmall);
                Pawn.Destroy();
                Find.TickManager.slower.SignalForceNormalSpeedShort();
            }
        }

    }

}