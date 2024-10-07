using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TritonsHydrants.Common.Interfaces;
using TritonsHydrants.Utils;
using Terraria;
using Terraria.ModLoader;

namespace TritonsHydrants.Content.Waters
{
    public class CavernWater : IWater
    {
        public void AI(ModProjectile proj) { }

        public List<int> GetWaterID()
        {
            return [Water.Cavern];
        }
        public void OnHitNPC(ModProjectile proj, NPC target) { }
        public void OnSpawn(ModProjectile projectile) { }

        public void OnTileCollide(ModProjectile proj, Vector2 oldVelocity) { }
    }
}