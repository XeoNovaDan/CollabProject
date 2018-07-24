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
    public class HediffCompProperties_ThrumboTransformer : HediffCompProperties
    {

        public HediffCompProperties_ThrumboTransformer()
        {
            compClass = typeof(HediffCompThrumboTransformer);
        }

        public float mtbDaysTransformIntoThrumbo = 0f;

        public Dictionary<BodyPartDef, BodyPartDef> partConversionsByDefNames;

        public Dictionary<string, string> partConversionsByCustomLabels;

        public List<BodyPartDef> partConversionBlacklist;

    }
}
