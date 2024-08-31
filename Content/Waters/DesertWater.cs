using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NeptunesTreasure.Common.Interfaces;
using NeptunesTreasure.Utils;
using Terraria;
using Terraria.ModLoader;

namespace NeptunesTreasure.Content.Waters
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