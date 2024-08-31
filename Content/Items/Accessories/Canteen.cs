using NeptunesTreasure.Utils;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader.IO;
using System.Collections.Generic;

namespace NeptunesTreasure.Content.Items.Accessories
{
    public class Canteen : ModItem
    {
        private int WaterContent = 0;
        private Color WaterColor = Color.White;

        public override void SetDefaults()
        {
            // Size settings
            Item.width = 28;
            Item.height = 30;

            // Item Properties
            Item.accessory = true;
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.WaterBucket, 3)
                .AddIngredient(ItemID.Silk, 50)
                .AddIngredient(ItemID.BlackLens, 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (Item.wet)
            {
                WaterContent = Main.waterStyle;
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (WaterContent is not 0)
            {
                Main.waterStyle = WaterContent;
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new(Mod, "Face", $"Content: {Water.GetWaterName(WaterContent)}")
            {
                OverrideColor = new Color(100, 100, 255)
            };
            tooltips.Add(line);
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D waterIcon = ModContent.Request<Texture2D>("NeptunesTreasure/Common/Textures/WaterIcon").Value;
            Vector2 offset = new(+21, -4);

            //If the canteen is empty, the water color is white
            if (WaterContent is 0)
            {
                WaterColor = Color.White;
            }
            else
            {
                WaterColor = Water.GetWaterColor(WaterContent);
            }

            spriteBatch.Draw(waterIcon, position + offset, null, WaterColor, 0f, origin, scale, SpriteEffects.None, 1f);
        }

        #region Saving and Loading
        public override void LoadData(TagCompound tag)
        {
            tag.TryGet("WaterContent", out WaterContent);
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Set("WaterContent", WaterContent);
        }
        #endregion
    }
}
