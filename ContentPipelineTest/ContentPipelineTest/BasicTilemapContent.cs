using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace ContentPipelineTest
{
    [ContentSerializerRuntimeType("PipelineTestExample.BasicTilemap, PipelineTestExample")]
    public class BasicTilemapContent
    {
        /// <summary>Map dimensions</summary>
        public int MapWidth;

        /// <summary>Map dimensions</summary>
        public int MapHeight;

        /// <summary>Tile dimensions</summary>
        public int TileWidth;

        /// <summary>Tile dimensions</summary>
        public int TileHeight;

        /// <summary>The tileset texture</summary>
        public Texture2DContent TilesetTexture;

        /// <summary>The tileset data</summary>
        public Rectangle[] Tiles;

        /// <summary>The map data</summary>
        public int[] TileIndices;

        /// <summary>The map filename</summary>
        [ContentSerializerIgnore]
        public string mapFilename;

        /// <summary> The tileset image filename </summary>
        [ContentSerializerIgnore]
        public string TilesetImageFilename;
    }
}
