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
    public class PostWarmupMote : DefModExtension
    {

        public static readonly PostWarmupMote defaultValues = new PostWarmupMote();

        public bool throwMote = true;

        public string moteTextTranslationKey = "Dakka";

    }
}
