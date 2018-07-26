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

        public List<float> MTBDaysList => Props.shipIncidentMTBDaysPairs.Values.ToList();

        public int ShipPartMTBDaysLength => MTBDaysList.Count;

        public bool CanAttractShipParts
        {
            get
            {
                CompPowerTrader powerTraderComp = parent.TryGetComp<CompPowerTrader>();
                return ticksUntilCanRefire <= 0 && (powerTraderComp != null) ? powerTraderComp.PowerOn : true;
            }
        }

        public Dictionary<int, IncidentDef> ShipPartsIndexed
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

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (Prefs.DevMode)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Debug: Force ship part incident",
                    action = delegate
                    {
                        TryFireShipPartIncidents(true);
                    }
                };
            }
        }

        public override void CompTick()
        {
            ticksUntilCanRefire--;
            if (CanAttractShipParts)
            {
                TryFireShipPartIncidents();
            }
        }

        private void TryFireShipPartIncidents(bool forced = false)
        {
            List<IncidentDef> incidentsToFire = GetIncidentsToFire(forced).ToList();

            if (!incidentsToFire.NullOrEmpty())
            {
                foreach (IncidentDef incident in incidentsToFire)
                {
                    IncidentParms parms = StorytellerUtility.DefaultParmsNow(incident.category, Find.Maps.Where(x => x.IsPlayerHome).RandomElement());
                    incident.Worker.TryExecute(parms);
                }
                ticksUntilCanRefire = Mathf.RoundToInt(Props.minRefireDays * GenDate.TicksPerDay);
            }
        }

        private IEnumerable<IncidentDef> GetIncidentsToFire(bool forced)
        {
            for (int i = 0; i < ShipPartMTBDaysLength; i++)
            {
                if ((forced && Rand.Value >= 0.5f) || !forced && Rand.MTBEventOccurs(MTBDaysList[i], GenDate.TicksPerDay, 60f))
                    yield return ShipPartsIndexed[i];
            }
        }

    }
}
