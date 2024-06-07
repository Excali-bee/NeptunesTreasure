﻿using MagicTridents.Content.Dusts;
using MagicTridents.Utils;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MagicTridents.Content.Projectiles
{
    public class AquaticArrow : ModProjectile
    {
        private bool EnterOnWater = false;
        private bool happen = false;
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
            Projectile.ignoreWater = true;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;

        //<summary> when the projectile enter in the water, transform to a homing projectile </summary>
        public override void OnSpawn(IEntitySource source)
        {
            switch (Main.waterStyle)
            {
                case Water.Hallow:
                    Projectile.NewProjectileDirect(new EntitySource_TileBreak(2, 2), Main.player[Projectile.owner].Center + new Vector2(-50, 0), new Vector2(-10, 0), ModContent.ProjectileType<NeptuneFragment>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Projectile.NewProjectileDirect(new EntitySource_TileBreak(2, 2), Main.player[Projectile.owner].Center + new Vector2(+50, 0), new Vector2(+10, 0), ModContent.ProjectileType<NeptuneFragment>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    break;
                case Water.Jungle:
                    Projectile.tileCollide = true;
                    Projectile.penetrate = 3;
                    break;
                case Water.Snow:
                    Projectile.tileCollide = false;
                    Projectile.aiStyle = ProjAIStyleID.Boomerang;
                    Projectile.timeLeft = 1200;
                    Projectile.penetrate = 200;
                    break;
                case Water.Desert:
                    Projectile.tileCollide = false;
                    break;
                case Water.Desert2:
                    Projectile.tileCollide = false;
                    break;
            }

            Projectile.ai[0] = 0;
            Projectile.ai[1] = 0;
            happen = false;
        }
        public override void AI()
        {
            Dust dust = Dust.NewDustPerfect(Projectile.position, ModContent.DustType<WaterBubble>(), Vector2.Zero);
            Lighting.AddLight(Projectile.position, dust.color.R / 255, dust.color.G / 255, dust.color.B / 255);
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

            if (Projectile.soundDelay == 0 && Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) > 2f)
            {
                Projectile.soundDelay = Helper.Ticks(1);
                SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
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
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player p = Main.player[Projectile.owner];

            if (target.life <= 0 && (Main.waterStyle is Water.Crimsom || Main.waterStyle is Water.BloodMoon))
            {
                p.Heal((int)(3 * p.GetDamage(DamageClass.Magic).Multiplicative));
            }

            if (Main.waterStyle is Water.Snow)
            {
                target.AddBuff(BuffID.Frozen, Helper.Ticks(2));
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

            double deg = (double)Projectile.ai[0];
            double rad = deg * (Math.PI / 180);

            switch (Main.waterStyle)
            {
                case Water.Jungle:
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
            }
        }
        private void Empower()
        {
            Projectile.ai[1]++;

            if (Projectile.ai[1] >= 25)
            {
                HomingProjectile();
            }
        }
    }
}