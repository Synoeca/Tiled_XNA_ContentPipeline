using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ContentPipelineTest
{
    [ContentProcessor(DisplayName = "BasicTiledMTLProcessor")]
    public class BasicTiledMTLProcessor : ContentProcessor<BasicTiledMTLContent, BasicTiledMTLContent>
    {
        public override BasicTiledMTLContent Process(BasicTiledMTLContent mtl, ContentProcessorContext context)
        {
            context.Logger.LogMessage("\n=== Starting Tilemap Processing ===");
            context.Logger.LogMessage($"Input Map State:");
            context.Logger.LogMessage($"  Filename: {mtl.Filename}");
            context.Logger.LogMessage($"  Dimensions: {mtl.Width}x{mtl.Height}");
            context.Logger.LogMessage($"  Tile Dimensions: {mtl.TileWidth}x{mtl.TileHeight}");
            context.Logger.LogMessage($"  Number of Tilesets: {mtl.Tilesets.Count}");
            //context.Logger.LogMessage($"  Number of Layers: {tiledMapTileset.Layers.Count}");
            //context.Logger.LogMessage($"  Number of Object Groups: {tiledMapTileset.ObjectGroups.Count}");

            //context.Logger.LogMessage($"\n=== Processing Tileset: {tiledMapTileset.Name} ===");
            //LogTilesetState(tiledMapTileset.Tilesets, context, "Pre-processing");

            foreach (KeyValuePair<string, BasicParalleledTilesetContent> tilesetEntry in mtl.Tilesets)
            {
                context.Logger.LogMessage($"\nKey: {tilesetEntry.Key}");
                context.Logger.LogMessage($"Value: {tilesetEntry.Value}");
                LogTilesetState(tilesetEntry.Value, context, "Pre-processing");

                if (!string.IsNullOrEmpty(tilesetEntry.Value.Image))
                {
                    string texturePath = GetTexturePath(tilesetEntry.Value.Image, tilesetEntry.Value.Filename, context);
                    context.Logger.LogMessage($"Processing texture: {texturePath}");

                    try
                    {
                        tilesetEntry.Value.Texture = context.BuildAndLoadAsset<TextureContent, Texture2DContent>(
                            new ExternalReference<TextureContent>(texturePath),
                            "TextureProcessor"
                        );

                        if (tilesetEntry.Value.Texture?.Mipmaps.Count > 0)
                        {
                            //tilesetEntry.Value.TexWidth = tilesetEntry.Value.Texture.Mipmaps[0].Width;
                            //tilesetEntry.Value.TexHeight = tilesetEntry.Value.Texture.Mipmaps[0].Height;
                            context.Logger.LogMessage($"Texture processed successfully:");
                            context.Logger.LogMessage($" Width: {tilesetEntry.Value.TexWidth}");
                            context.Logger.LogMessage($" Height: {tilesetEntry.Value.TexHeight}");
                        }
                        else
                        {
                            context.Logger.LogWarning("", new ContentIdentity(),
                                "No mipmaps found in processed texture!");
                        }
                    }
                    catch (Exception ex)
                    {
                        context.Logger.LogImportantMessage($"Error processing texture: {ex.Message}");
                        throw;
                    }

                    LogTilesetState(tilesetEntry.Value, context, "Post-processing");
                }
            }

            foreach (KeyValuePair<string, BasicParalleledLayerContent> layerEntry in mtl.Layers)
            {
                context.Logger.LogMessage($"\nKey: {layerEntry.Key}");
                context.Logger.LogMessage($"Value: {layerEntry.Value}");

                context.Logger.LogMessage($"\n=== Processing Layer: {layerEntry.Value.Name} ===");
                context.Logger.LogMessage($"  Dimensions: {layerEntry.Value.Width}x{layerEntry.Value.Height}");
                context.Logger.LogMessage($"  Opacity: {layerEntry.Value.Opacity}");
                context.Logger.LogMessage($"  Tiles Count: {layerEntry.Value.Tiles?.Length ?? 0}");
                context.Logger.LogMessage($"  FlipAndRotate Count: {layerEntry.Value.FlipAndRotate?.Length ?? 0}");
                context.Logger.LogMessage($"  Properties Count: {layerEntry.Value.Properties?.Count ?? 0}");
                context.Logger.LogMessage($"  FlippedHorizontallyFlag: {BasicParalleledLayerContent.FlippedHorizontallyFlag}");
                context.Logger.LogMessage($"  FlippedVerticallyFlag: {BasicParalleledLayerContent.FlippedVerticallyFlag}");
                context.Logger.LogMessage($"  FlippedDiagonallyFlag: {BasicParalleledLayerContent.FlippedDiagonallyFlag}");
                context.Logger.LogMessage($"  HorizontalFlipDrawFlag: {BasicParalleledLayerContent.HorizontalFlipDrawFlag}");
                context.Logger.LogMessage($"  VerticalFlipDrawFlag: {BasicParalleledLayerContent.VerticalFlipDrawFlag}");
                context.Logger.LogMessage($"  DiagonallyFlipDrawFlag: {BasicParalleledLayerContent.DiagonallyFlipDrawFlag}");
                context.Logger.LogMessage($"  TileInfoCache Count: {layerEntry.Value.TileInfoCache?.Length ?? 0}");
                context.Logger.LogMessage($"  Filename: {layerEntry.Value.Filename}");
            }

            return mtl;
        }

        private static string GetTexturePath(string imageSource, string mapFilename, ContentProcessorContext context)
        {
            context.Logger.LogMessage($"GetTexturePath: Starting with imageSource={imageSource}, mapFilename={mapFilename}");

            // Use the directory of the map file as the content directory
            string contentDir = Path.GetDirectoryName(mapFilename) ?? string.Empty;
            context.Logger.LogMessage($"GetTexturePath: Content directory={contentDir}");

            // Resolve the absolute path of the image
            string absolutePath = Path.GetFullPath(Path.Combine(contentDir, imageSource));
            context.Logger.LogMessage($"GetTexturePath: Absolute path={absolutePath}");

            // Get the path relative to the content root
            string relativePath = absolutePath.Replace(context.OutputDirectory, "").TrimStart(Path.DirectorySeparatorChar);
            string processedPath = relativePath.Replace('\\', '/');
            context.Logger.LogMessage($"GetTexturePath: Final processed path={processedPath}");

            return processedPath;
        }


        private static void LogTilesetState(BasicParalleledTilesetContent tileset, ContentProcessorContext context, string stage)
        {
            context.Logger.LogMessage($"\n{stage} Tileset State:");
            context.Logger.LogMessage($" Name: {tileset.Name}");
            context.Logger.LogMessage($" FirstTileId: {tileset.FirstTileId}");
            context.Logger.LogMessage($" TileWidth: {tileset.TileWidth}");
            context.Logger.LogMessage($" TileHeight: {tileset.TileHeight}");
            context.Logger.LogMessage($" Spacing: {tileset.Spacing}");
            context.Logger.LogMessage($" Margin: {tileset.Margin}");
            context.Logger.LogMessage($" TexWidth: {tileset.TexWidth}");
            context.Logger.LogMessage($" TexHeight: {tileset.TexHeight}");
            context.Logger.LogMessage($" Has Texture: {tileset.Texture != null}\n");
        }
    }
}
