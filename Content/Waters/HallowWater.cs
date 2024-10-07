using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TritonsHydrants.Common.Interfaces;
using TritonsHydrants.Content.Projectiles;
using TritonsHydrants.Utils;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace TritonsHydrants.Content.Waters
{
    public class HallowWater() : IWater
    {
        public void AI(ModProjectile proj)
        {
            throw new System.NotImplementedException();
        }

        public List<int> GetWaterID()
        {
            return [Water.Jungle];
        }

        public void OnHitNPC(ModProjectile proj, NPC target) { }

        public void OnSpawn(ModProjectile proj)
        {
            Projectile.NewProjectileDirect(new EntitySource_TileBreak(2, 2), proj.Projectile.Center, proj.Projectile.velocity.RotatedBy(0.261799), ModContent.ProjectileType<AquaticShard>(), proj.Projectile.damage / 3, proj.Projectile.knockBack, proj.Projectile.owner);
            Projectile.NewProjectileDirect(new EntitySource_TileBreak(2, 2), proj.Projectile.Center, proj.Projectile.velocity.RotatedBy(-0.261799), ModContent.ProjectileType<AquaticShard>(), proj.Projectile.damage / 3, proj.Projectile.knockBack, proj.Projectile.owner);
        }

        public void OnTileCollide(ModProjectile proj, Vector2 oldVelocity) { }
    }
}