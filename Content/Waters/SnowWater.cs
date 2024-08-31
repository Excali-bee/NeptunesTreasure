using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NeptunesTreasure.Common.Interfaces;
using NeptunesTreasure.Utils;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeptunesTreasure.Content.Waters
{
    public class SnowWater() : IWater
    {
        public List<int> GetWaterID()
        {
            return [Water.Snow];
        }
        public void AI(ModProjectile proj)
        {
            proj.Projectile.tileCollide = false;
            proj.Projectile.aiStyle = ProjAIStyleID.Boomerang;
            proj.Projectile.timeLeft = 1200;
        }
        public void OnHitNPC(ModProjectile proj, NPC target)
        {
            target.AddBuff(BuffID.Frozen, Helper.Ticks(1));
        }
        public void OnSpawn(ModProjectile proj)
        {
            proj.Projectile.penetrate = 5;
        }
        public void OnTileCollide(ModProjectile proj, Vector2 oldVelocity) { }
    }
}