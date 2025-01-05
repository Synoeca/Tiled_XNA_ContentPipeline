using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ContentPipelineTest
{
    [ContentSerializerRuntimeType("PipelineTestExample.BasicTiledObject, PipelineTestExample")]
    public class BasicTiledObjectContent
    {
        public Dictionary<string, string> Properties;

        public string Name;
        public string Image;
        public int Width;
        public int Height;
        public int X;
        public int Y;

        public Texture2DContent Texture;
        public int TexWidth;
        public int TexHeight;

        public Texture2DContent TileTexture;
    }

    [ContentSerializerRuntimeType("PipelineTestExample.BasicTiledObjectGroupObject, PipelineTestExample")]
    public class BasicTiledObjectGroupObjectContent
    {
        public Dictionary<string, BasicTiledObjectContent> Objects;
        public Dictionary<string, string> ObjectProperties;

        public string Name;
        public string Class;
        public float Opacity;
        public float OffsetX;
        public float OffsetY;
        public float ParallaxX;
        public float ParallaxY;
        public Color? Color;
        public Color? TintColor;
        public bool Visible;
        public bool Locked;
        public int Id;
    }

    [ContentSerializerRuntimeType("PipelineTestExample.BasicTiledGroup, PipelineTestExample")]
    public class BasicTiledGroupContent
    {
        public Dictionary<string, BasicTiledObjectGroupObjectContent> ObjectGroups;
        public Dictionary<string, string> Properties;
        public string Name;
        public int Id;
        public bool Locked;
        [ContentSerializerIgnore]
        public string Filename;
    }
}
