﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeptunesTreasure.Utils
{
    /// <summary>
    /// This class is used to facilitate the usage of projectiles.
    /// </summary>
    public static class ProjectileExtras
    {
        /// <summary>
        /// Applies gravity to the vertical movement of the projectile.
        /// </summary>
        /// <param name="velocityY">The vertical velocity of the projectile.</param>
        /// <param name="power">The strength of the gravity to be applied. The default value is 0.1f.</param>
        /// <param name="limit">The maximum limit of the vertical velocity. The default value is 16.</param>
        /// <returns>The vertical velocity of the projectile after applying gravity.</returns>
        public static float ApplyGravity(float velocityY, float power = 0.1f, float limit = 16)
        {
            if (velocityY > limit)
            {
                return limit;
            }
            else
            {
                return velocityY += power;
            }
        }

        /// <summary>
        /// Applies bouncing effect to the projectile.
        /// </summary>
        /// <param name="proj">The projectile to be affected.</param>
        /// <param name="oldVelocity">The previous velocity of the projectile.</param>
        public static void ApplyBounce(ModProjectile proj, Vector2 oldVelocity)
        {
            proj.Projectile.penetrate--;
            if (proj.Projectile.penetrate <= 0)
            {
                proj.Projectile.Kill();
            }
            else
            {
                Collision.HitTiles(proj.Projectile.position, proj.Projectile.velocity, proj.Projectile.width, proj.Projectile.height);
                SoundEngine.PlaySound(SoundID.Item10, proj.Projectile.position);

                // If the projectile hits the left or right side of the block, invert the velocity on the X axis
                if (Math.Abs(proj.Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                {
                    proj.Projectile.velocity.X = -oldVelocity.X;
                }

                // If the projectile hits the top or bottom side of the block, invert the velocity on the Y axis
                if (Math.Abs(proj.Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                {
                    proj.Projectile.velocity.Y = -oldVelocity.Y;
                }
            }
        }

        /// <summary>
        /// Applies orbiting around the player to the projectile.
        /// </summary>
        /// <param name="projectile">The projectile to be affected.</param>
        /// <param name="player">The player around which the projectile will orbit.</param>
        /// <param name="radius">The radius of the orbit.</param>
        /// <param name="speed">The rotation speed of the projectile.</param>
        public static void ApplyOrbitingPlayer(ModProjectile projectile, Player player, float radius, float speed)
        {
            double deg = projectile.Projectile.ai[1];
            double rad = deg * (Math.PI / 180);
            double dist = radius;

            projectile.Projectile.position.X = player.Center.X - (int)(Math.Cos(rad) * dist) - projectile.Projectile.width / 2;
            projectile.Projectile.position.Y = player.Center.Y - (int)(Math.Sin(rad) * dist) - projectile.Projectile.height / 2;
            projectile.Projectile.ai[1] += speed;
        }

        public static void ApplyWaveMovement(ModProjectile proj, float frequency = 0.2f, float amplitude = 5f)
        {
            float direction = MathF.Atan2(proj.Projectile.velocity.Y, proj.Projectile.velocity.X);

            proj.Projectile.position.X += proj.Projectile.velocity.X / 1.5f;
            proj.Projectile.position.Y += proj.Projectile.velocity.Y / 1.5f;

            // Movement formula
            proj.Projectile.position.X += (float)Math.Sin(proj.Projectile.timeLeft * frequency) * amplitude * (float)Math.Cos(direction + MathF.PI / 2);
            proj.Projectile.position.Y += (float)Math.Sin(proj.Projectile.timeLeft * frequency) * amplitude * (float)Math.Sin(direction + MathF.PI / 2);
        }
    }
}
