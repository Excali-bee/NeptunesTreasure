using NeptunesTreasure.Utils;

namespace NeptunesTreasure.Common
{
    public abstract class WaterProjectile : ModProjectile
    {
        public int WaterStyle => Main.waterStyle;

        public delegate void Effect();

        public override void OnSpawn(IEntitySource source, Effect[] effects, int[] waterStyle)
        {
            foreach (Effect e in effects)
            {
                e(waterStyle[effects.IndexOf(e)]);
            }
        }

        public void OnWetProjectile()
        {

        }

        public void ProjectileMovement(int waterStyle, Vector2 position, float speed)
        {

        }

        public override void AI()
        {
            if (Projectile.wet)
            {
                OnWetProjectile();
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
    }
}