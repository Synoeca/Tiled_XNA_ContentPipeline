using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PipelineTestExample
{
    /// <summary>
    /// A class representing a map object
    /// </summary>
    /// <remarks>
    /// A map object represents an object in a map; it has a 
    /// position, width, and height, and a collection of properties.
    /// It can be used for spawn locations, triggers, etc.
    /// In this implementation, it also has a texture 
    /// </remarks>
    public class BasicTiledObject
    {
        public Dictionary<string, string> Properties { get; set; }

        public string Name { get; set; }
        public string Image { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        public Texture2D Texture { get; set; }
        public int TexWidth { get; set; }
        public int TexHeight { get; set; }

        public Texture2D TileTexture
        {
            get => Texture;
            set
            {
                Texture = value;
                TexWidth = value.Width;
                TexHeight = value.Height;
            }
        }
    }

    /// <summary>
    /// A class representing a group of map Objects
    /// </summary>
    public class BasicTiledObjectGroupObject
    {
        public Dictionary<string, BasicTiledObject> Objects { get; set; }
        public Dictionary<string, string> ObjectProperties { get; set; }

        public string Name { get; set; }
        public string Class { get; set; }
        public float Opacity { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public float ParallaxX { get; set; }
        public float ParallaxY { get; set; }

        public Color? Color { get; set; }
        public Color? TintColor { get; set; }
        public bool Visible { get; set; }
        public bool Locked { get; set; }
        public int Id { get; set; }
    }

    public class BasicTiledGroup
    {
        public Dictionary<string, BasicTiledObjectGroupObject> ObjectGroups { get; set; }
        public Dictionary<string, string> Properties { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public bool Locked { get; set; }
    }
}
