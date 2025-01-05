using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PipelineTestExample
{
    public class BasicTiledMTLG
    {
        /// <summary>
        /// The Map's width and height
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The Map's height
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The Map's tile width
        /// </summary>
        public int TileWidth { get; set; }

        /// <summary>
        /// The Map's tile height
        /// </summary>
        public int TileHeight { get; set; }

        /// <summary>
        /// The Map's properties
        /// </summary>
        public Dictionary<string, string> Properties { get; set; }

        /// <summary>
        /// The Map's Groups
        /// </summary>
        public Dictionary<string, BasicTiledGroupMTLG> Groups { get; set; }

        /// <summary>
        /// The Map's Layers
        /// </summary>
        public Dictionary<string, BasicLayerMTLG> Layers { get; set; }

        /// <summary>
        /// The Map's Tilesets
        /// </summary>
        public Dictionary<string, BasicTilesetMTLG> Tilesets { get; set; }
    }

    /// <summary>
    /// The Map's tileset class
    /// </summary>
    public class BasicTilesetMTLG
    {
        public class TilePropertyList : Dictionary<string, string>;

        public string Name { get; set; }

        public int FirstTileId { get; set; }

        public int TileWidth { get; set; }

        public int TileHeight { get; set; }

        public int Spacing { get; set; }

        public int Margin { get; set; }

        public Dictionary<int, TilePropertyList> TileProperties { get; set; }

        public string Image { get; set; }

        protected Texture2D Texture { get; set; }

        protected int TexWidth { get; set; }

        protected int TexHeight { get; set; }

        public TilePropertyList GetTileProperties(int index)
        {
            index -= FirstTileId;

            if (index < 0)
                return null;

            TileProperties.TryGetValue(index, out TilePropertyList result);

            return result;
        }

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

        /// <summary>
        /// Converts a map position into a rectangle providing the
        /// bounds of the tile in the TileSet texture.
        /// </summary>
        /// <param name="index">The tile index</param>
        /// <param name="rect">The bounds of the tile in the tileset texture</param>
        /// <returns>True if the tile index exists in the tileset</returns>
        internal bool MapTileToRect(int index, ref Rectangle rect)
        {
            index -= FirstTileId;

            if (index < 0)
                return false;

            int rowSize = TexWidth / (TileWidth + Spacing);
            int row = index / rowSize;
            int numRows = TexHeight / (TileHeight + Spacing);
            if (row >= numRows)
                return false;

            int col = index % rowSize;

            rect.X = col * TileWidth + col * Spacing + Margin;
            rect.Y = row * TileHeight + row * Spacing + Margin;
            rect.Width = TileWidth;
            rect.Height = TileHeight;
            return true;
        }
    }

    public class BasicLayerMTLG
    {
        public static uint FlippedHorizontallyFlag { get; set; }
        public static uint FlippedVerticallyFlag { get; set; }
        public static uint FlippedDiagonallyFlag { get; set; }

        public static byte HorizontalFlipDrawFlag { get; set; }
        public static byte VerticalFlipDrawFlag { get; set; }
        public static byte DiagonallyFlipDrawFlag { get; set; }

        public SortedList<string, string> Properties { get; set; }

        public struct TileInfo
        {
            public Texture2D Texture { get; set; }
            public Rectangle Rectangle { get; set; }
        }

        public string Name { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public float Opacity { get; set; }

        public int[] Tiles { get; set; }

        public byte[] FlipAndRotate { get; set; }

        public TileInfo[] TileInfoCache { get; set; }
    }

    /// <summary>
    /// A class representing a map object
    /// </summary>
    /// <remarks>
    /// A map object represents an object in a map; it has a 
    /// position, width, and height, and a collection of properties.
    /// It can be used for spawn locations, triggers, etc.
    /// In this implementation, it also has a texture 
    /// </remarks>
    public class BasicTiledObjectMTLG
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
    public class BasicTiledObjectGroupMTLG
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

    public class BasicTiledGroupMTLG
    {
        public Dictionary<string, BasicTiledObjectGroupObject> ObjectGroups { get; set; }
        public Dictionary<string, string> Properties { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public bool Locked { get; set; }
    }
}
