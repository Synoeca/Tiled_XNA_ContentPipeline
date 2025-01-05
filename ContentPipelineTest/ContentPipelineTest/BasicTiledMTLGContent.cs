using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ContentPipelineTest
{
    [ContentSerializerRuntimeType("PipelineTestExample.BasicTiledMTLG, PipelineTestExample")]
    public class BasicTiledMTLGContent
    {
        public int Width;

        public int Height;

        public int TileWidth;

        public int TileHeight;

        [ContentSerializerIgnore]
        public string Filename;

        public Dictionary<string, string> Properties;

        public Dictionary<string, BasicTiledGroupMTLGContent> Groups;

        public Dictionary<string, BasicLayerMTLGContent> Layers;

        public Dictionary<string, BasicTilesetMTLGContent> Tilesets;
    }

    [ContentSerializerRuntimeType("PipelineTestExample.BasicTilesetMTLG, PipelineTestExample")]
    public class BasicTilesetMTLGContent
    {
        [ContentSerializerRuntimeType("PipelineTestExample.BasicTilesetMTLG+TilePropertyList, PipelineTestExample")]
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

    [ContentSerializerRuntimeType("PipelineTestExample.BasicLayerMTLG, PipelineTestExample")]
    public class BasicLayerMTLGContent
    {
        public static uint FlippedHorizontallyFlag;
        public static uint FlippedVerticallyFlag;
        public static uint FlippedDiagonallyFlag;

        public static byte HorizontalFlipDrawFlag;
        public static byte VerticalFlipDrawFlag;
        public static byte DiagonallyFlipDrawFlag;

        public SortedList<string, string> Properties;

        [ContentSerializerRuntimeType("PipelineTestExample.BasicLayerMTLG+TileInfo, PipelineTestExample")]
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

    [ContentSerializerRuntimeType("PipelineTestExample.BasicTiledObjectMTLG, PipelineTestExample")]
    public class BasicTiledObjectMTLGContent
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

    [ContentSerializerRuntimeType("PipelineTestExample.BasicTiledObjectGroupMTLG, PipelineTestExample")]
    public class BasicTiledObjectGroupMTLGContent
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

    [ContentSerializerRuntimeType("PipelineTestExample.BasicTiledGroupMTLG, PipelineTestExample")]
    public class BasicTiledGroupMTLGContent
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
