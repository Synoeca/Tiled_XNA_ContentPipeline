using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentPipelineTest
{
    [ContentProcessor(DisplayName = "BasicTiledObjectGroupObjectProcessor")]
    public class BasicTiledObjectGroupObjectProcessor : ContentProcessor<BasicTiledGroupContent, BasicTiledGroupContent>
    {
        public override BasicTiledGroupContent Process(BasicTiledGroupContent group, ContentProcessorContext context)
        {
            context.Logger.LogMessage("\n=== Starting Group Processing ===");
            context.Logger.LogMessage($"Input Group State:");
            context.Logger.LogMessage($"  Name: {group.Name}");
            context.Logger.LogMessage($"  Id: {group.Id}");
            context.Logger.LogMessage($"  Locked: {group.Locked}");
            context.Logger.LogMessage($"  Number of Object Groups: {group.ObjectGroups?.Count ?? 0}");
            context.Logger.LogMessage($"  Number of Properties: {group.Properties?.Count ?? 0}");

            // Process each object group in the group
            if (group.ObjectGroups != null)
            {
                foreach (KeyValuePair<string, BasicTiledObjectGroupObjectContent> objGroupEntry in group.ObjectGroups)
                {
                    context.Logger.LogMessage($"\nProcessing Object Group: {objGroupEntry.Key}");
                    BasicTiledObjectGroupObjectContent objectGroup = objGroupEntry.Value;
                    LogObjectGroupState(objectGroup, context, "Pre-processing");

                    // Process each object in the object group
                    if (objectGroup.Objects != null)
                    {
                        foreach (KeyValuePair<string, BasicTiledObjectContent> objectEntry in objectGroup.Objects)
                        {
                            context.Logger.LogMessage($"\nProcessing Object: {objectEntry.Key}");
                            BasicTiledObjectContent obj = objectEntry.Value;
                            LogObjectState(obj, context, "Pre-processing");

                            if (!string.IsNullOrEmpty(obj.Image))
                            {
                                ProcessObjectTexture(obj, group.Filename, context);
                            }

                            LogObjectState(obj, context, "Post-processing");
                        }
                    }

                    LogObjectGroupState(objectGroup, context, "Post-processing");
                }
            }

            return group;
        }

        private void ProcessObjectTexture(BasicTiledObjectContent obj, string baseFilename, ContentProcessorContext context)
        {
            string texturePath = GetTexturePath(obj.Image, baseFilename, context);
            context.Logger.LogMessage($"Processing texture for Object {obj.Name}: {texturePath}");

            try
            {
                Texture2DContent texture = context.BuildAndLoadAsset<TextureContent, Texture2DContent>(
                    new ExternalReference<TextureContent>(texturePath),
                    "TextureProcessor"
                );

                if (texture?.Mipmaps.Count > 0)
                {
                    obj.Texture = texture;
                    obj.TexWidth = texture.Mipmaps[0].Width;
                    obj.TexHeight = texture.Mipmaps[0].Height;

                    context.Logger.LogMessage($"Texture processed successfully:");
                    context.Logger.LogMessage($" Width: {obj.TexWidth}");
                    context.Logger.LogMessage($" Height: {obj.TexHeight}");
                }
                else
                {
                    context.Logger.LogWarning("", new ContentIdentity(),
                        $"No mipmaps found in processed texture for Object {obj.Name}!");
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogImportantMessage($"Error processing texture for Object {obj.Name}: {ex.Message}");
                throw;
            }
        }

        private static string GetTexturePath(string imageSource, string baseFilename, ContentProcessorContext context)
        {
            context.Logger.LogMessage($"GetTexturePath: Starting with imageSource={imageSource}, baseFilename={baseFilename}");
            string contentDir = Path.GetDirectoryName(baseFilename) ?? string.Empty;
            string absolutePath = Path.GetFullPath(Path.Combine(contentDir, imageSource));
            string relativePath = absolutePath.Replace(context.OutputDirectory, "").TrimStart(Path.DirectorySeparatorChar);
            return relativePath.Replace('\\', '/');
        }

        private static void LogObjectState(BasicTiledObjectContent obj, ContentProcessorContext context, string stage)
        {
            context.Logger.LogMessage($"\n{stage} Object State:");
            context.Logger.LogMessage($" Name: {obj.Name}");
            context.Logger.LogMessage($" Position: ({obj.X}, {obj.Y})");
            context.Logger.LogMessage($" Dimensions: {obj.Width}x{obj.Height}");
            context.Logger.LogMessage($" Image: {obj.Image}");
            context.Logger.LogMessage($" Properties Count: {obj.Properties?.Count ?? 0}");
            context.Logger.LogMessage($" TexWidth: {obj.TexWidth}");
            context.Logger.LogMessage($" TexHeight: {obj.TexHeight}\n");
        }

        private static void LogObjectGroupState(BasicTiledObjectGroupObjectContent objGroup, ContentProcessorContext context, string stage)
        {
            context.Logger.LogMessage($"\n{stage} Object Group State:");
            context.Logger.LogMessage($" Name: {objGroup.Name}");
            context.Logger.LogMessage($" Class: {objGroup.Class}");
            context.Logger.LogMessage($" Position: ({objGroup.OffsetX}, {objGroup.OffsetY})");
            context.Logger.LogMessage($" Opacity: {objGroup.Opacity}");
            context.Logger.LogMessage($" ParallaxX: {objGroup.ParallaxX}");
            context.Logger.LogMessage($" ParallaxY: {objGroup.ParallaxY}");
            //context.Logger.LogMessage($" Color: {objGroup.Color}");
            //context.Logger.LogMessage($" TintColor: {objGroup.TintColor}");
            //if (objGroup.Color != null)
            //{
            //    context.Logger.LogMessage($"  color A: {objGroup.Color} -> {objGroup.Color.Value.A}");
            //    context.Logger.LogMessage($"  color B: {objGroup.Color} -> {objGroup.Color.Value.B}");
            //    context.Logger.LogMessage($"  color G: {objGroup.Color} -> {objGroup.Color.Value.G}");
            //    context.Logger.LogMessage($"  color R: {objGroup.Color} -> {objGroup.Color.Value.R}");
            //}
            //else
            //{
            //    context.Logger.LogMessage($"  color is null!!");
            //}
            //if (objGroup.TintColor != null)
            //{
            //    context.Logger.LogMessage($"  Tint color A: {objGroup.TintColor} -> {objGroup.TintColor.Value.A}");
            //    context.Logger.LogMessage($"  Tint color B: {objGroup.TintColor} -> {objGroup.TintColor.Value.B}");
            //    context.Logger.LogMessage($"  Tint color G: {objGroup.TintColor} -> {objGroup.TintColor.Value.G}");
            //    context.Logger.LogMessage($"  Tint color R: {objGroup.TintColor} -> {objGroup.TintColor.Value.R}");
            //}
            //else
            //{
            //    context.Logger.LogMessage($"  tint color is null!!");
            //}
            context.Logger.LogMessage($" Visible: {objGroup.Visible}");
            context.Logger.LogMessage($" Locked: {objGroup.Locked}");
            context.Logger.LogMessage($" Properties Count: {objGroup.ObjectProperties?.Count ?? 0}");
            context.Logger.LogMessage($" Objects Count: {objGroup.Objects?.Count ?? 0}\n");
        }
    }
}
