using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TritonsHydrants.Common.Interfaces;
using TritonsHydrants.Utils;
using Terraria;
using Terraria.ModLoader;

namespace TritonsHydrants.Content.Waters
{
    public class DesertWater() : IWater
    {
        public void AI(ModProjectile proj) { }

        public List<int> GetWaterID()
        {
            return [Water.Desert];
        }

        public void OnHitNPC(ModProjectile proj, NPC target) { }

        public void OnSpawn(ModProjectile proj)
        {
            proj.Projectile.penetrate = 2;
        }

        public void OnTileCollide(ModProjectile proj, Vector2 oldVelocity) { }
    }
}