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
    public class BanDamageProps : DefModExtension
    {

        public static readonly BanDamageProps defaultValues = new BanDamageProps();

        public float banChance = 1f;

        public DamageDef explosionDamageDef;

        public float explosionRadius = 1f;

        public float haywireBanChance = 0f;

        public float haywireBanPlayerPawnChance = 0f;

        public float haywireBanMostValuablePawnChance = 0f;

    }
}
