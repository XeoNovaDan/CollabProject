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
    public static class PlotArmorUtility
    {

        public const float GenerateChanceFromLegendaryArt = 0.8f;
        public const float GenerateChanceFromLegendaryItem = 0.35f;

        public static readonly SimpleCurve CombatPowerToPlotArmorChance = new SimpleCurve
        {
            new CurvePoint(0, 0.1f),
            new CurvePoint(25, 0.01f),
            new CurvePoint(50, 0.001f),
            new CurvePoint(51, 0)
        };

    }
}
