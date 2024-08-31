
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace NeptunesTreasure.Common.Interfaces
{
    public interface IWater
    {
        List<int> GetWaterID();
        void OnSpawn(ModProjectile proj);
        void OnHitNPC(ModProjectile proj, NPC target);
        void AI(ModProjectile proj);
        void OnTileCollide(ModProjectile proj, Vector2 oldVelocity);
    }
}