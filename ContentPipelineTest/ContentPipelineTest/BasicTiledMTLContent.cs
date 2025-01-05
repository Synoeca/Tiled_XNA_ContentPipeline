using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ContentPipelineTest
{
    [ContentSerializerRuntimeType("PipelineTestExample.BasicTiledMTL, PipelineTestExample")]
    public class BasicTiledMTLContent
    {
        public int Width;

        public int Height;

        public int TileWidth;

        public int TileHeight;

        [ContentSerializerIgnore]
        public string Filename;

        public Dictionary<string, BasicParalleledLayerContent> Layers;

        public Dictionary<string, BasicParalleledTilesetContent> Tilesets;


    }

    [ContentSerializerRuntimeType("PipelineTestExample.BasicParalleledTileset, PipelineTestExample")]
    public class BasicParalleledTilesetContent
    {
        [ContentSerializerRuntimeType("PipelineTestExample.BasicParalleledTileset+TilePropertyList, PipelineTestExample")]
        public class TilePropertyList : Dictionary<string, string>;

        public string Name;

        public int FirstTileId;

        public int TileWidth;

        public int TileHeight;

        public int Spacing;

        public int Margin;

        public Dictionary<int, TilePropertyList> TileProperties;

        public string Image;

        public Texture2DContent Texture;

        public int TexWidth;

        public int TexHeight;

        public Texture2DContent TileTexture;

        [ContentSerializerIgnore]
        public string Filename;
    }

    [ContentSerializerRuntimeType("PipelineTestExample.BasicParalleledLayer, PipelineTestExample")]
    public class BasicParalleledLayerContent
    {
        public static uint FlippedHorizontallyFlag;
        public static uint FlippedVerticallyFlag;
        public static uint FlippedDiagonallyFlag;

        public static byte HorizontalFlipDrawFlag;
        public static byte VerticalFlipDrawFlag;
        public static byte DiagonallyFlipDrawFlag;

        public SortedList<string, string> Properties;

        [ContentSerializerRuntimeType("PipelineTestExample.BasicParalleledLayer+TileInfo, PipelineTestExample")]
        public struct TileInfo
        {
            public Texture2DContent Texture;
            public Rectangle Rectangle;
        }

        public string Name;

        public int Width;

        public int Height;

        public float Opacity;

        public int[] Tiles;

        public byte[] FlipAndRotate;

        public TileInfo[] TileInfoCache;

        [ContentSerializerIgnore]
        public string Filename;
    }
}
