using NeptunesTreasure.Content.Dusts;
using NeptunesTreasure.Utils;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using NeptunesTreasure.Common.Interfaces;
using System.Collections.Generic;
using NeptunesTreasure.Content.Waters;

namespace NeptunesTreasure.Content.Projectiles
{
    //TODO: CORRUPTION -> Pensar em algo

    public class AquaticArrow : ModProjectile
    {
        private readonly List<IWater> Waters = [new JungleWater(), new CavernWater(), new DesertWater(), new SnowWater(), new HallowWater()];
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

        public override void OnSpawn(IEntitySource source)
        {
            foreach (IWater water in Waters)
            {
                if (water.GetWaterID().Contains(Main.waterStyle))
                {
                    water.OnSpawn(this);
                }
            }

            SoundEngine.PlaySound(SoundID.Item21, Projectile.position);
        }

        public override void AI()
        {
            foreach (IWater water in Waters)
            {
                if (water.GetWaterID().Contains(Main.waterStyle))
                {
                    water.AI(this);
                }
            }

            Dust waterBubble = Dust.NewDustPerfect(Projectile.position, ModContent.DustType<WaterBubble>(), Vector2.Zero);
            Lighting.AddLight(Projectile.position, waterBubble.color.R / 255, waterBubble.color.G / 255, waterBubble.color.B / 255);
            Projectile.rotation = Projectile.velocity.ToRotation();

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
            foreach (IWater water in Waters)
            {
                if (water.GetWaterID().Contains(Main.waterStyle))
                {
                    water.OnHitNPC(proj: this, target);
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            foreach (IWater water in Waters)
            {
                if (water.GetWaterID().Contains(Main.waterStyle))
                {
                    water.OnTileCollide(proj: this, oldVelocity);
                }
            }

            return OnTileCollide(oldVelocity);
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