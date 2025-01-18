using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace ContentPipelineTest
{
    [ContentSerializerRuntimeType("PipelineTestExample.BasicTile, PipelineTestExample")]
    public class BasicTileContent
    {
        public int Id;

        public string Type;

        public float Probability;

        public Dictionary<string, string> Properties;
    }

    [ContentSerializerRuntimeType("PipelineTestExample.BasicTileAndTileset, PipelineTestExample")]
    public class BasicTileAndTilesetContent
    {
        public string Name;

        public int FirstTileId;

        public int TileWidth;

        public int TileHeight;

        public int Spacing;

        public int Margin;

        public Dictionary<int, BasicTileContent> Tiles;

        public string Image;

        public Texture2DContent Texture;

        public int TexWidth;

        public int TexHeight;

        public Texture2DContent TileTexture;

        [ContentSerializerIgnore]
        public string Filename;
    }
}
