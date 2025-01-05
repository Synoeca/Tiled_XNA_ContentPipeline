using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ContentPipelineTest
{
    [ContentProcessor(DisplayName = "BasicTiledLayerProcessor")]
    public class BasicTiledLayerProcessor : ContentProcessor<BasicTiledLayerContent, BasicTiledLayerContent>
    {
        public override BasicTiledLayerContent Process(BasicTiledLayerContent layer, ContentProcessorContext context)
        {
            context.Logger.LogMessage($"\n=== Processing Layer: {layer.Name} ===");
            context.Logger.LogMessage($"  Dimensions: {layer.Width}x{layer.Height}");
            context.Logger.LogMessage($"  Opacity: {layer.Opacity}");
            context.Logger.LogMessage($"  Tiles Count: {layer.Tiles?.Length ?? 0}");
            context.Logger.LogMessage($"  FlipAndRotate Count: {layer.FlipAndRotate?.Length ?? 0}");
            context.Logger.LogMessage($"  Properties Count: {layer.Properties?.Count ?? 0}");
            context.Logger.LogMessage($"  FlippedHorizontallyFlag: {BasicTiledLayerContent.FlippedHorizontallyFlag}");
            context.Logger.LogMessage($"  FlippedVerticallyFlag: {BasicTiledLayerContent.FlippedVerticallyFlag}");
            context.Logger.LogMessage($"  FlippedDiagonallyFlag: {BasicTiledLayerContent.FlippedDiagonallyFlag}");
            context.Logger.LogMessage($"  HorizontalFlipDrawFlag: {BasicTiledLayerContent.HorizontalFlipDrawFlag}");
            context.Logger.LogMessage($"  VerticalFlipDrawFlag: {BasicTiledLayerContent.VerticalFlipDrawFlag}");
            context.Logger.LogMessage($"  DiagonallyFlipDrawFlag: {BasicTiledLayerContent.DiagonallyFlipDrawFlag}");
            context.Logger.LogMessage($"  TileInfoCache Count: {layer.TileInfoCache?.Length ?? 0}");
            context.Logger.LogMessage($"  Filename: {layer.Filename}");


            if (layer.Tiles != null && layer.Tiles.Length > 0)
            {
                context.Logger.LogMessage("  Tile Data:");
                StringBuilder tileDataBuilder = new StringBuilder();
                int tilesPerLine = Math.Min(layer.Width, 20); // Adjust this value to control how many tiles per line to display

                for (int i = 0; i < layer.Tiles.Length; i++)
                {
                    tileDataBuilder.Append(layer.Tiles[i]);

                    if (i < layer.Tiles.Length - 1)
                    {
                        tileDataBuilder.Append(",");
                    }

                    if ((i + 1) % tilesPerLine == 0 || i == layer.Tiles.Length - 1)
                    {
                        context.Logger.LogMessage($"    {tileDataBuilder}");
                        tileDataBuilder.Clear();
                    }
                }
            }
            else
            {
                context.Logger.LogMessage("  No tile data available.");
            }

            if (layer.Properties != null && layer.Properties.Count > 0)
            {
                context.Logger.LogMessage("  Properties:");
                foreach (KeyValuePair<string, string> prop in layer.Properties)
                {
                    context.Logger.LogMessage($"    {prop.Key}: {prop.Value}");
                }
            }

            if (layer.TileInfoCache != null && layer.TileInfoCache.Length > 0)
            {
                context.Logger.LogMessage("  TileInfoCache Details:");
                for (int i = 0; i < layer.TileInfoCache.Length; i++)
                {
                    BasicTiledLayerContent.TileInfo tileInfo = layer.TileInfoCache[i];
                    context.Logger.LogMessage($"    Tile {i}:");
                    context.Logger.LogMessage($"      Texture: {(tileInfo.Texture != null ? "Present" : "Null")}");
                    context.Logger.LogMessage($"      Rectangle: X={tileInfo.Rectangle.X}, Y={tileInfo.Rectangle.Y}, Width={tileInfo.Rectangle.Width}, Height={tileInfo.Rectangle.Height}");
                }
            }
            return layer;
        }
    }
}
