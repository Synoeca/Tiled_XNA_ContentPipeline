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
    [ContentSerializerRuntimeType("PipelineTestExample.BasicTiledLayer, PipelineTestExample")]
    public class BasicTiledLayerContent
    {
        public static uint FlippedHorizontallyFlag;
        public static uint FlippedVerticallyFlag;
        public static uint FlippedDiagonallyFlag;

        public static byte HorizontalFlipDrawFlag;
        public static byte VerticalFlipDrawFlag;
        public static byte DiagonallyFlipDrawFlag;

        public SortedList<string, string> Properties;

        [ContentSerializerRuntimeType("PipelineTestExample.BasicTiledLayer+TileInfo, PipelineTestExample")]
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
