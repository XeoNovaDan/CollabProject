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
    public class MechanoidReducerExtension : DefModExtension
    {

        public PawnKindDef spawnedPawnKind;

        public IntRange countRange;

        public float manhunterChance = 0f;

    }
}
