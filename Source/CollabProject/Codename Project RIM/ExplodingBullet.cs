using System;
using Verse;
using Verse.Sound;
using RimWorld;
using UnityEngine;

namespace Codename_Project_RIM
{
    public class ExplodingBullet : Projectile
    {
        public override void Tick()
        {
            base.Tick();
            if (this.origin.x != this.Position.x || this.origin.z != this.Position.z)
            {
                GenExplosion.DoExplosion(this.Position, base.Map, 0.2f, DamageDefOf.Flame, null, null, null, null, null, 0f, 1, false, null, 0f, 1);
            }
        }
        void DoExplosion(Map map)
        {
            int[,] design = new int[,]
                {
                    {0,0,0,0,0,0,0,0,0 },
                    {0,1,1,1,1,1,1,1,0 },
                    {0,1,0,0,0,0,0,1,0 },
                    {0,1,0,1,0,1,0,1,0 },
                    {0,1,0,0,0,0,0,1,0 },
                    {0,1,1,0,0,0,1,1,0 },
                    {0,0,1,1,1,1,1,0,0 },
                    {0,0,1,0,0,0,1,0,0 },
                    {0,0,1,1,1,1,1,0,0 },
                };
            for (int x = 0; x < design.GetLength(0); x += 1)
            {
                for (int y = 0; y < design.GetLength(1); y += 1)
                {
                    if (design[x, y] == 1)
                    {
                        IntVec3 newExplosion = new IntVec3(this.Position.x - y + 4, this.Position.y, this.Position.z - x + 4);
                        GenExplosion.DoExplosion(newExplosion, map, 0.2f, DamageDefOf.Bomb, null, null, null, null, null, 0f, 1, false, null, 0f, 1);
                    }

                }
            }



        }

        protected override void Impact(Thing hitThing)
        {
            Map map = base.Map;
            base.Impact(hitThing);
            if (hitThing != null)
            {

                DoExplosion(map);
            }
            else
            {
                DoExplosion(map);
                SoundDefOf.BulletImpactGround.PlayOneShot(new TargetInfo(base.Position, map, false));
                MoteMaker.MakeStaticMote(this.ExactPosition, map, ThingDefOf.Mote_ShotHit_Dirt, 1f);

            }
        }
    }
}
