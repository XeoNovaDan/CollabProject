using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;
using Harmony;

namespace Codename_Project_RIM
{
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {

        private static readonly Type patchType = typeof(HarmonyPatches);

        static HarmonyPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("ProjectRIMTeam.ProjectRIM");

            harmony.Patch(AccessTools.Method(typeof(Verb_Shoot), "TryCastShot"), null,
                new HarmonyMethod(patchType, nameof(Postfix_Verb_Shoot_TryCastShot)));

            harmony.Patch(AccessTools.Method(typeof(Pawn), nameof(Pawn.Kill)),
                new HarmonyMethod(patchType, nameof(Prefix_Pawn_Kill)), null);

            // Currently not working properly
            //harmony.Patch(AccessTools.Method(typeof(PawnApparelGenerator), "GenerateWorkingPossibleApparelSetFor"),
            //    new HarmonyMethod(patchType, nameof(Prefix_PawnApparelGenerator_GenerateWorkingPossibleApparelSetFor)), null);

            harmony.Patch(AccessTools.Method(typeof(QualityUtility), nameof(QualityUtility.GenerateQualityCreatedByPawn), new[] { typeof(Pawn), typeof(SkillDef) }), null,
                new HarmonyMethod(patchType, nameof(Postfix_QualityUtility_GenerateQualityCreatedByPawn)));

            harmony.Patch(AccessTools.Method(typeof(Projectile), nameof(Projectile.Launch), new[] { typeof(Thing), typeof(Vector3), typeof(LocalTargetInfo), typeof(LocalTargetInfo),
            typeof(ProjectileHitFlags), typeof(Thing), typeof(ThingDef)}),
                new HarmonyMethod(patchType, nameof(Prefix_Projectile_Launch)), null);

        }

        // Dakka mote thrower
        public static void Postfix_Verb_Shoot_TryCastShot(Verb_Shoot __instance, bool __result)
        {
            PostWarmupMote extension = __instance.EquipmentSource.def.GetModExtension<PostWarmupMote>() ?? PostWarmupMote.defaultValues;
            if (__result && extension.throwMote)
                // I did try the [MustTranslate] attribute, but it didn't actually work
                MoteMaker.ThrowText(__instance.caster.DrawPos, __instance.caster.Map, extension.moteTextTranslationKey.Translate(), 4f);
        }

        // Plot armour thickens :thooiiink:
        public static bool Prefix_Pawn_Kill(Pawn __instance)
        {
            if (__instance.apparel != null)
                foreach (Apparel app in __instance.apparel.GetDirectlyHeldThings())
                    if (app.def == PR_ThingDefOf.Apparel_PlotArmor)
                        return false;
            return true;
        }

        public static void Prefix_PawnApparelGenerator_GenerateWorkingPossibleApparelSetFor(Pawn pawn)
        {
            if (Rand.Chance(PlotArmorUtility.CombatPowerToPlotArmorChance.Evaluate(pawn.kindDef.combatPower)))
            {
                List<ThingStuffPair> allApparelPairs = Traverse.Create(typeof(PawnApparelGenerator)).Field("allApparelPairs").GetValue<List<ThingStuffPair>>();
                Traverse workingSetTraverse = Traverse.Create(typeof(PawnApparelGenerator)).Field("workingSet");
                var workingSet = workingSetTraverse.GetValue<List<object>>();
                workingSet.Add(allApparelPairs.Where(pa => pa.thing == PR_ThingDefOf.Apparel_PlotArmor).RandomElementByWeight(pa => pa.Commonality));
                workingSetTraverse.SetValue(workingSet);
            }
        }

        public static void Postfix_QualityUtility_GenerateQualityCreatedByPawn(QualityCategory __result, Pawn pawn, SkillDef relevantSkill)
        {
            if (__result >= QualityCategory.Legendary)
            {
                bool generatePlotArmor = (relevantSkill == SkillDefOf.Artistic) ?
                    Rand.Chance(PlotArmorUtility.GenerateChanceFromLegendaryArt) : Rand.Chance(PlotArmorUtility.GenerateChanceFromLegendaryItem);
                if (generatePlotArmor)
                {
                    ThingDef stuff = GenStuff.RandomStuffByCommonalityFor(PR_ThingDefOf.Apparel_PlotArmor);
                    Thing plotArmor = ThingMaker.MakeThing(PR_ThingDefOf.Apparel_PlotArmor, stuff);
                    plotArmor.TryGetComp<CompQuality>().SetQuality(QualityUtility.AllQualityCategories.RandomElement(), ArtGenerationContext.Colony);
                    GenSpawn.Spawn(plotArmor, pawn.Position, pawn.Map);
                    Messages.Message("PlotArmorCreated".Translate(pawn.LabelShort), plotArmor, MessageTypeDefOf.PositiveEvent);
                }
            }
        }

        public static void Prefix_Projectile_Launch(Vector3 origin, ref LocalTargetInfo usedTarget, LocalTargetInfo intendedTarget)
        {
            if (intendedTarget.Thing is Pawn pawn && pawn.RaceProps.Humanlike && !PR_StatDefOf.WallBuildChance.Worker.IsDisabledFor(pawn))
            {
                float buildChance = pawn.GetStatValue(PR_StatDefOf.WallBuildChance);
                Map map = pawn.Map;
                IntVec3 pawnPos = pawn.PositionHeld;
                IntVec3 wallPos;
                if (origin == null)
                    wallPos = GenRadial.RadialCellsAround(pawnPos, 1.9f, false).RandomElement();
                else
                {
                    float angleFlat = (origin.ToIntVec3() - pawnPos).AngleFlat;
                    if (angleFlat > 335f || angleFlat <= 45f)
                        wallPos = pawnPos + IntVec3.North;
                    else if (angleFlat <= 135f)
                        wallPos = pawnPos + IntVec3.East;
                    else if (angleFlat <= 225f)
                        wallPos = pawnPos + IntVec3.South;
                    else
                        wallPos = pawnPos + IntVec3.West;
                }
                if (Rand.Chance(buildChance) && wallPos.GetFirstThing(map, ThingDefOf.Wall) == null)
                {
                    wallPos.GetThingList(map).Clear();
                    Building wall = (Building)ThingMaker.MakeThing(ThingDefOf.Wall, GenStuff.AllowedStuffsFor(ThingDefOf.Wall).RandomElement());
                    GenSpawn.Spawn(wall, wallPos, map);
                    MoteMaker.ThrowText(pawn.DrawPos, map, "BuildoffSuccessful".Translate(), 4f);
                    usedTarget = wall;
                }
            }
        }

    }
}
