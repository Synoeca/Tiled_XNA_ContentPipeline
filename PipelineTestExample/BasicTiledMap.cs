using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineTestExample
{
    public class BasicTiledMap
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
    }
}
