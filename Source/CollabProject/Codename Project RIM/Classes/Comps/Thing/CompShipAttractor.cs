using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace Codename_Project_RIM
{
    public class CompShipAttractor : ThingComp
    {

        private int ticksUntilCanRefire = 0;

        public CompProperties_ShipAttractor Props => (CompProperties_ShipAttractor)props;

        private List<float> MTBDaysList => Props.shipIncidentMTBDaysPairs.Values.ToList();

        private int ShipPartMTBDaysLength => MTBDaysList.Count;

        private Dictionary<int, IncidentDef> ShipPartsIndexed
        {
            get
            {
                Dictionary<int, IncidentDef> dict = new Dictionary<int, IncidentDef>();
                List<IncidentDef> keyList = Props.shipIncidentMTBDaysPairs.Keys.ToList();
                for (int i = 0; i < ShipPartMTBDaysLength; i++)
                {
                    dict.Add(i, keyList[i]);
                }
                return dict;
            }
        }

        public override void CompTick()
        {
            ticksUntilCanRefire--;
            if (ticksUntilCanRefire <= 0)
            {
                int i = 0;
                bool canFireIncident = false;
                while (i < ShipPartMTBDaysLength)
                {
                    if (Rand.MTBEventOccurs(MTBDaysList[i], GenDate.TicksPerDay, 1f))
                    {
                        canFireIncident = true;
                        break;
                    }
                    i++;
                }
                if (canFireIncident)
                {
                    IncidentDef incident = ShipPartsIndexed[i];
                    IncidentParms parms = StorytellerUtility.DefaultParmsNow(incident.category, Find.Maps.Where(x => x.IsPlayerHome).RandomElement());
                    incident.Worker.TryExecute(parms);
                    ticksUntilCanRefire = Mathf.RoundToInt(Props.minRefireDays * GenDate.TicksPerDay);
                }
            }
        }

    }
}
