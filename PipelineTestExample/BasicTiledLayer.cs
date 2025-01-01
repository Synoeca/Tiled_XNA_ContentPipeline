﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Xna.Framework;

namespace PipelineTestExample
{
    public class BasicTiledLayer
    {
        /*
         * High-order bits in the tile data indicate tile flipping
         */
        //private const uint FlippedHorizontallyFlag = 0x80000000;
        //private const uint FlippedVerticallyFlag = 0x40000000;
        //private const uint FlippedDiagonallyFlag = 0x20000000;

        //internal const byte HorizontalFlipDrawFlag = 1;
        //internal const byte VerticalFlipDrawFlag = 2;
        //internal const byte DiagonallyFlipDrawFlag = 4;

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
