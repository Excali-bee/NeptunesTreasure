using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TritonsHydrants.Common.Interfaces;
using TritonsHydrants.Utils;
using Terraria;
using Terraria.ModLoader;

namespace TritonsHydrants.Content.Waters
{
    public class CrimsomWater() : IWater
    {
        public void AI(ModProjectile proj) { }

        public List<int> GetWaterID()
        {
            return [Water.Jungle];
        }

        public void OnHitNPC(ModProjectile proj, NPC target)
        {
            Player p = Main.player[proj.Projectile.owner];

            if (target.life <= 0 && (Main.waterStyle is Water.Crimsom || Main.waterStyle is Water.BloodMoon))
            {
                p.Heal((p.statDefense / 20) + 1);
            }
        }

        public void OnSpawn(ModProjectile proj)
        {
            proj.Projectile.penetrate = 2;
        }

        public void OnTileCollide(ModProjectile proj, Vector2 oldVelocity) { }
    }
}