using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System;
using System.IO;

namespace ContentPipelineTest
{
    [ContentProcessor(DisplayName = "BasicTiledTilesetProcessor")]
    public class BasicTiledTilesetProcessor : ContentProcessor<BasicTiledTilesetContent, BasicTiledTilesetContent>
    {
        public override BasicTiledTilesetContent Process(BasicTiledTilesetContent tileset, ContentProcessorContext context)
        {
            context.Logger.LogMessage($"\n=== Processing Tileset: {tileset.Name} ===");
            LogTilesetState(tileset, context, "Pre-processing");

            if (!string.IsNullOrEmpty(tileset.Image))
            {
                string texturePath = GetTexturePath(tileset.Image, tileset.Filename, context);
                context.Logger.LogMessage($"Processing texture: {texturePath}");

                try
                {
                    tileset.Texture = context.BuildAndLoadAsset<TextureContent, Texture2DContent>(
                        new ExternalReference<TextureContent>(texturePath),
                        "TextureProcessor"
                    );

                    if (tileset.Texture?.Mipmaps.Count > 0)
                    {
                        //tileset.TexWidth = tileset.Texture.Mipmaps[0].Width;
                        //tileset.TexHeight = tileset.Texture.Mipmaps[0].Height;
                        context.Logger.LogMessage($"Texture processed successfully:");
                        context.Logger.LogMessage($" Width: {tileset.TexWidth}");
                        context.Logger.LogMessage($" Height: {tileset.TexHeight}");
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

            LogTilesetState(tileset, context, "Post-processing");
            return tileset;
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


        private static void LogTilesetState(BasicTiledTilesetContent tileset, ContentProcessorContext context, string stage)
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
            context.Logger.LogMessage($" Has Texture: {tileset.Texture != null}");
        }
    }
}
