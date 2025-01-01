﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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
        /// The Map's tileset class
        /// </summary>
        public class BasicTiledTileset
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

        /// <summary>
        /// The Map's Tilesets
        /// </summary>
        public Dictionary<string, BasicTiledTileset> Tilesets { get; set; }

        /// <summary>
        /// The Map's Layer class
        /// </summary>
        public class BasicTiledLayer
        {
            public static uint FlippedHorizontallyFlag { get; set; }
            public static uint FlippedVerticallyFlag { get; set; }
            public static uint FlippedDiagonallyFlag { get; set; }

            public static byte HorizontalFlipDrawFlag { get; set; }
            public static byte VerticalFlipDrawFlag { get; set; }
            public static byte DiagonallyFlipDrawFlag { get; set; }

            public Dictionary<string, string> Properties { get; set; }

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

            /// <summary>
            /// Gets the tile index of the tile at position (<paramref name="x"/>,<paramref name="y"/>)
            /// in the layer
            /// </summary>
            /// <param name="x">The tile's x-position in the layer</param>
            /// <param name="y">The tile's y-position in the layer</param>
            /// <returns>The index of the tile in the tileset(s)</returns>
            public int GetTile(int x, int y)
            {
                if ((x < 0) || (y < 0) || (x >= Width) || (y >= Height))
                    throw new InvalidOperationException();

                int index = (y * Width) + x;
                return Tiles[index];
            }

            /// <summary>
            /// Caches the information about each specific tile in the layer
            /// (its texture and bounds within that texture) in a list indexed 
            /// by the tile index for quick retreival/processing
            /// </summary>
            /// <param name="tilesets">The list of tilesets containing tiles to cache</param>
            protected void BuildTileInfoCache(IList<BasicTiledTileset> tilesets)
            {
                Rectangle rect = new();
                List<TileInfo> cache = [];
                int i = 1;

            next:
                foreach (BasicTiledTileset ts in tilesets)
                {
                    if (ts.MapTileToRect(i, ref rect))
                    {
                        cache.Add(new TileInfo
                        {
                            Texture = ts.TileTexture,
                            Rectangle = rect
                        });
                        i += 1;
                        goto next;
                    }
                }

                TileInfoCache = cache.ToArray();
            }

            /// <summary>
            /// Draws the layer
            /// </summary>
            /// <param name="batch">The SpriteBatch to draw with</param>
            /// <param name="tilesets">A list of tilesets associated with the layer</param>
            /// <param name="rectangle">The viewport to render within</param>
            /// <param name="viewportPosition">The viewport's position in the layer</param>
            /// <param name="tileWidth">The width of a tile</param>
            /// <param name="tileHeight">The height of a tile</param>
            public void Draw(SpriteBatch batch, IList<BasicTiledTileset> tilesets, Rectangle rectangle, Vector2 viewportPosition, int tileWidth, int tileHeight)
            {
                if (TileInfoCache == null)
                    BuildTileInfoCache(tilesets);

                // Draw all tiles in the layer
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        int i = (y * Width) + x;

                        byte flipAndRotate = FlipAndRotate[i];
                        SpriteEffects flipEffect = SpriteEffects.None;
                        float rotation = 0f;

                        // Handle flip and rotation flags
                        if ((flipAndRotate & HorizontalFlipDrawFlag) != 0)
                            flipEffect |= SpriteEffects.FlipHorizontally;
                        if ((flipAndRotate & VerticalFlipDrawFlag) != 0)
                            flipEffect |= SpriteEffects.FlipVertically;
                        if ((flipAndRotate & DiagonallyFlipDrawFlag) != 0)
                        {
                            if ((flipAndRotate & HorizontalFlipDrawFlag) != 0 &&
                                 (flipAndRotate & VerticalFlipDrawFlag) != 0)
                            {
                                rotation = (float)(Math.PI / 2);
                                flipEffect ^= SpriteEffects.FlipVertically;
                            }
                            else if ((flipAndRotate & HorizontalFlipDrawFlag) != 0)
                            {
                                rotation = (float)-(Math.PI / 2);
                                flipEffect ^= SpriteEffects.FlipVertically;
                            }
                            else if ((flipAndRotate & VerticalFlipDrawFlag) != 0)
                            {
                                rotation = (float)(Math.PI / 2);
                                flipEffect ^= SpriteEffects.FlipHorizontally;
                            }
                            else
                            {
                                rotation = -(float)(Math.PI / 2);
                                flipEffect ^= SpriteEffects.FlipHorizontally;
                            }
                        }

                        int index = Tiles[i] - 1;
                        if (index >= 0 && index < TileInfoCache!.Length)
                        {
                            TileInfo info = TileInfoCache[index];

                            // Position tiles relative to ground level
                            Vector2 position = new(
                                x * tileWidth,    // X position is straightforward left-to-right
                                y * tileHeight    // Y position is top-to-bottom
                            );

                            batch.Draw(
                                info.Texture,
                                position,
                                info.Rectangle,
                                Color.White * Opacity,
                                rotation,
                                Vector2.Zero, // Don't use center origin since it positions manually
                                1f,
                                flipEffect,
                                0
                            );
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The Map's Layers
        /// </summary>
        public Dictionary<string, BasicTiledLayer> Layers { get; set; }
    }
}
