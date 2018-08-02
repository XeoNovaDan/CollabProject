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
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {

        private static readonly Type patchType = typeof(HarmonyPatches);

        static HarmonyPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("ProjectRIMTeam.ProjectRIM");

            harmony.Patch(AccessTools.Method(typeof(Verb_Shoot), "TryCastShot"), null,
                new HarmonyMethod(patchType, nameof(Postfix_TryCastShot)));
        }

        // Dakka mote thrower
        public static void Postfix_TryCastShot(Verb_Shoot __instance, bool __result)
        {
            PostWarmupMote extension = __instance.EquipmentSource.def.GetModExtension<PostWarmupMote>() ?? PostWarmupMote.defaultValues;
            if (__result && extension.throwMote)
                // I did try the [MustTranslate] attribute, but it didn't actually work
                MoteMaker.ThrowText(__instance.caster.DrawPos, __instance.caster.Map, extension.moteTextTranslationKey.Translate(), 4f);
        }

    }
}
