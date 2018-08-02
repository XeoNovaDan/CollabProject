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
    public class CompTargetable_AllMechanoidsOnTheMap : CompTargetable_AllPawnsOnTheMap
    {

        protected override TargetingParameters GetTargetingParameters()
        {
            TargetingParameters targetParams = base.GetTargetingParameters();
            targetParams.validator = delegate(TargetInfo targ)
            {
                if (!BaseTargetValidator(targ.Thing))
                    return false;
                else
                {
                    return (targ.Thing is Pawn pawn && pawn.RaceProps.IsMechanoid && pawn.def.HasModExtension<MechanoidReducerExtension>());
                }
            };
            return targetParams;
        }

    }
}
