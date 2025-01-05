using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PipelineTestExample
{
    public class BasicTiledMTL
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
        /// The Map's Layers
        /// </summary>
        public Dictionary<string, BasicParalleledLayer> Layers { get; set; }

        /// <summary>
        /// The Map's Tilesets
        /// </summary>
        public Dictionary<string, BasicParalleledTileset> Tilesets { get; set; }
    }

    /// <summary>
    /// The Map's tileset class
    /// </summary>
    public class BasicParalleledTileset
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

    public class BasicParalleledLayer
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
}
