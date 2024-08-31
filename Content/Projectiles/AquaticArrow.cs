﻿using NeptunesTreasure.Content.Dusts;
using NeptunesTreasure.Utils;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeptunesTreasure.Content.Projectiles
{
    //TODO: CORRUPTION -> Pensar em algo

    public class AquaticArrow : ModProjectile
    {
        private bool EnterOnWater = false;
        private bool active = false;

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.light = 1f;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.damage = 20;
            Projectile.timeLeft = 600;
            Projectile.netUpdate = true;
            Projectile.velocity *= 2;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;

        //<summary> when the projectile enter in the water, transform to a homing projectile </summary>
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item21, Projectile.position);

            switch (Main.waterStyle)
            {
                case Water.Hallow:
                    Projectile.NewProjectileDirect(new EntitySource_TileBreak(2, 2), Projectile.Center, Projectile.velocity.RotatedBy(0.261799), ModContent.ProjectileType<AquaticShard>(), Projectile.damage / 3, Projectile.knockBack, Projectile.owner);
                    Projectile.NewProjectileDirect(new EntitySource_TileBreak(2, 2), Projectile.Center, Projectile.velocity.RotatedBy(-0.261799), ModContent.ProjectileType<AquaticShard>(), Projectile.damage / 3, Projectile.knockBack, Projectile.owner);
                    break;
                case Water.Jungle:
                    Projectile.penetrate = 5;
                    break;
                case Water.Snow:
                    Projectile.velocity *= 2;
                    break;
            }
        }
        public override void AI()
        {
            Dust waterBubble = Dust.NewDustPerfect(Projectile.position, ModContent.DustType<WaterBubble>(), Vector2.Zero);
            Lighting.AddLight(Projectile.position, waterBubble.color.R / 255, waterBubble.color.G / 255, waterBubble.color.B / 255);
            Projectile.rotation = Projectile.velocity.ToRotation();

            WaterEffect();

            if (Projectile.wet)
            {
                if (!EnterOnWater)
                {
                    OnWetProjectile();
                    EnterOnWater = true;
                }
                active = true;
            }

            if (active)
            {
                Empower();
            }
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 40; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(0.5f, 0.5f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<WaterBubble>(), speed * 5);
                d.noGravity = true;

                Lighting.AddLight(Projectile.position, Water.GetWaterColor().ToVector3());
            }

            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player p = Main.player[Projectile.owner];

            if (target.life <= 0 && (Main.waterStyle is Water.Crimsom || Main.waterStyle is Water.BloodMoon))
            {
                p.Heal((p.statDefense / 20) + 1);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Main.waterStyle is Water.Jungle)
            {
                ProjectileExtras.ApplyBounce(this, oldVelocity);
                return false;
            }

            return base.OnTileCollide(oldVelocity);
        }

        private void HomingProjectile()
        {
            Vector2 move = Vector2.Zero;
            float distance = 400f;
            bool isTarget = false;

            for (int k = 0; k < 100; k++)
            {
                if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5)
                {
                    Vector2 newMove = Main.npc[k].Center - Projectile.Center;
                    float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                    if (distanceTo < distance)
                    {
                        move = newMove;
                        distance = distanceTo;
                        isTarget = true;
                    }
                }
            }
            if (isTarget)
            {
                AdjustMagnitude(ref move);
                Projectile.velocity = (10 * Projectile.velocity + move) / 11f;
                AdjustMagnitude(ref Projectile.velocity);
            }
        }
        private static void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 6f)
            {
                vector *= 6f / magnitude;
            }
        }
        private void OnWetProjectile()
        {
            for (int i = 0; i < 40; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<WaterBubble>(), speed * 5);
                d.noGravity = true;
            }
        }
        private void WaterEffect()
        {
            Player p = Main.player[Projectile.owner];

            float frequency = 0.2f; // Frequência do movimento senoidal
            float amplitude = 5f; // Amplitude do movimento senoidal
            float direction = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X);


            switch (Main.waterStyle)
            {
                case Water.Jungle:
                    Projectile.tileCollide = true;
                    Projectile.velocity.Y = ProjectileExtras.ApplyGravity(Projectile.velocity.Y);
                    break;
                case Water.Desert:
                    ProjectileExtras.ApplyOrbitingPlayer(this, p, 64, 1.5f);
                    Projectile.tileCollide = false;
                    break;
                case Water.Desert2:
                    ProjectileExtras.ApplyOrbitingPlayer(this, p, 64, 1.5f);
                    Projectile.tileCollide = false;
                    break;
                case Water.Cavern:
                    Projectile.tileCollide = false;
                    Projectile.position.X += Projectile.velocity.X / 1.5f;
                    Projectile.position.Y += Projectile.velocity.Y / 1.5f;

                    // Movimento de onda perpendicular à direção do disparo
                    Projectile.position.X += (float)Math.Sin(Projectile.timeLeft * frequency) * amplitude * (float)Math.Cos(direction + MathF.PI / 2);
                    Projectile.position.Y += (float)Math.Sin(Projectile.timeLeft * frequency) * amplitude * (float)Math.Sin(direction + MathF.PI / 2);
                    break;
                case Water.Cavern2:
                    Projectile.tileCollide = false;
                    Projectile.position.X += Projectile.velocity.X / 1.5f;
                    Projectile.position.Y += Projectile.velocity.Y / 1.5f;

                    // Movimento de onda perpendicular à direção do disparo
                    Projectile.position.X += (float)Math.Sin(Projectile.timeLeft * frequency) * amplitude * (float)Math.Cos(direction + MathF.PI / 2);
                    Projectile.position.Y += (float)Math.Sin(Projectile.timeLeft * frequency) * amplitude * (float)Math.Sin(direction + MathF.PI / 2);
                    break;
                case Water.Snow:
                    Projectile.tileCollide = false;
                    Projectile.aiStyle = ProjAIStyleID.Boomerang;
                    Projectile.timeLeft = 1200;
                    Projectile.penetrate = 200;
                    break;
            }
        }
        private void Empower()
        {
            Projectile.ai[1]++;

            if (Projectile.ai[1] >= 25)
            {
                HomingProjectile();
                Projectile.velocity *= 1.0001f;
            }
        }
    }
}