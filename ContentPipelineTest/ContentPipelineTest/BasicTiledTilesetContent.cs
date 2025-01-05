using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace ContentPipelineTest
{
    [ContentSerializerRuntimeType("PipelineTestExample.BasicTiledTileset, PipelineTestExample")]
    public class BasicTiledTilesetContent
    {
        [ContentSerializerRuntimeType("PipelineTestExample.BasicTiledTileset+TilePropertyList, PipelineTestExample")]
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
}
