using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentPipelineTest
{
    [ContentProcessor(DisplayName = "BasicTileAndTilesetProcessor")]
    public class BasicTileAndTilesetProcessor : ContentProcessor<BasicTileAndTilesetContent, BasicTileAndTilesetContent>
    {
        public override BasicTileAndTilesetContent Process(BasicTileAndTilesetContent content,
            ContentProcessorContext context)
        {
            context.Logger.LogMessage($"\n=== Processing Tileset: {content.Name} ===");
            LogTilesetState(content, context, "Pre-processing");

            // Process tileset texture
            if (!string.IsNullOrEmpty(content.Image))
            {
                string texturePath = GetTexturePath(content.Image, content.Filename, context);
                context.Logger.LogMessage($"Processing texture: {texturePath}");

                try
                {
                    content.Texture = context.BuildAndLoadAsset<TextureContent, Texture2DContent>(
                        new ExternalReference<TextureContent>(texturePath),
                        "TextureProcessor"
                    );

                    if (content.Texture?.Mipmaps.Count > 0)
                    {
                        content.TexWidth = content.Texture.Mipmaps[0].Width;
                        content.TexHeight = content.Texture.Mipmaps[0].Height;
                        context.Logger.LogMessage($"Texture processed successfully:");
                        context.Logger.LogMessage($" Width: {content.TexWidth}");
                        context.Logger.LogMessage($" Height: {content.TexHeight}");
                    }
                    else
                    {
                        context.Logger.LogWarning("", new ContentIdentity(), "No mipmaps found in processed texture!");
                    }
                }
                catch (Exception ex)
                {
                    context.Logger.LogImportantMessage($"Error processing texture: {ex.Message}");
                    throw;
                }
            }

            // Process individual tiles
            if (content.Tiles != null && content.Tiles.Count > 0)
            {
                context.Logger.LogMessage("\nProcessing Tiles:");
                foreach (var tileKvp in content.Tiles)
                {
                    var tile = tileKvp.Value;
                    context.Logger.LogMessage($"\nTile ID: {tile.Id}");
                    context.Logger.LogMessage($" Type: {tile.Type ?? "none"}");
                    context.Logger.LogMessage($" Probability: {tile.Probability}");

                    if (tile.Properties.Count > 0)
                    {
                        context.Logger.LogMessage(" Properties:");
                        foreach (var prop in tile.Properties)
                        {
                            context.Logger.LogMessage($"  {prop.Key}: {prop.Value}");
                        }
                    }
                }
            }

            LogTilesetState(content, context, "Post-processing");
            return content;
        }

        private static string GetTexturePath(string imageSource, string mapFilename, ContentProcessorContext context)
        {
            context.Logger.LogMessage(
                $"GetTexturePath: Starting with imageSource={imageSource}, mapFilename={mapFilename}");

            // Use the directory of the map file as the content directory
            string contentDir = Path.GetDirectoryName(mapFilename) ?? string.Empty;
            context.Logger.LogMessage($"GetTexturePath: Content directory={contentDir}");

            // Resolve the absolute path of the image
            string absolutePath = Path.GetFullPath(Path.Combine(contentDir, imageSource));
            context.Logger.LogMessage($"GetTexturePath: Absolute path={absolutePath}");

            // Get the path relative to the content root
            string relativePath = absolutePath.Replace(context.OutputDirectory, "")
                .TrimStart(Path.DirectorySeparatorChar);
            string processedPath = relativePath.Replace('\\', '/');
            context.Logger.LogMessage($"GetTexturePath: Final processed path={processedPath}");

            return processedPath;
        }

        private static void LogTilesetState(BasicTileAndTilesetContent content, ContentProcessorContext context,
            string stage)
        {
            context.Logger.LogMessage($"\n{stage} State:");
            context.Logger.LogMessage($" Name: {content.Name}");
            context.Logger.LogMessage($" FirstTileId: {content.FirstTileId}");
            context.Logger.LogMessage($" TileWidth: {content.TileWidth}");
            context.Logger.LogMessage($" TileHeight: {content.TileHeight}");
            context.Logger.LogMessage($" Spacing: {content.Spacing}");
            context.Logger.LogMessage($" Margin: {content.Margin}");
            context.Logger.LogMessage($" TexWidth: {content.TexWidth}");
            context.Logger.LogMessage($" TexHeight: {content.TexHeight}");
            context.Logger.LogMessage($" Has Texture: {content.Texture != null}");
            context.Logger.LogMessage($" Number of Tiles: {content.Tiles?.Count ?? 0}");
        }
    }
}
