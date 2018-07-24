using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace Codename_Project_RIM
{
    public class CompProperties_ShipAttractor : CompProperties
    {

        public CompProperties_ShipAttractor()
        {
            compClass = typeof(CompShipAttractor);
        }

        public Dictionary<IncidentDef, float> shipIncidentMTBDaysPairs;

        public float minRefireDays = 0f;

    }
}
