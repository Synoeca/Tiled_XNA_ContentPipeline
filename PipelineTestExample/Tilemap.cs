using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PipelineTestExample
{
    public class Tilemap
    {
        /// <summary>The map filename</summary>
        private string _mapFilename;

        /// <summary>The tileset texture</summary>
        private Texture2D _tilesetTexture;

        /// <summary>The map and tile dimensions</summary>
        private int _tileWidth, _tileHeight, _mapWidth, _mapHeight;

        /// <summary>The tileset data</summary>
        private Rectangle[] _tiles;

        /// <summary>The map data</summary>
        private int[] _map;

        /// <summary>
        /// Creates a new tilemap instance
        /// </summary>
        /// <param name="mapFilename">The filename of the map file to load</param>
        public Tilemap(string mapFilename)
        {
            _mapFilename = mapFilename;
        }

        /// <summary>
        /// Loads the tilemap content
        /// </summary>
        /// <param name="content">The ContentManager to use for loading</param>
        public void LoadContent(ContentManager content)
        {
            // Read in the map file
            string data = File.ReadAllText(Path.Join(content.RootDirectory, _mapFilename));
            string[] lines = data.Split('\n');

            // First line is tileset image file name 
            string tilesetFileName = lines[0].Trim();
            _tilesetTexture = content.Load<Texture2D>(tilesetFileName);

            // Second line is tile size
            string[] secondLine = lines[1].Split(',');
            _tileWidth = int.Parse(secondLine[0]);
            _tileHeight = int.Parse(secondLine[1]);

            // Now that we know the tile size and tileset
            // image, we can determine tile bounds
            int tilesetColumns = _tilesetTexture.Width / _tileWidth;
            int tilesetRows = _tilesetTexture.Height / _tileHeight;
            _tiles = new Rectangle[tilesetColumns * tilesetRows];
            for (int y = 0; y < tilesetRows; y++)
            {
                for (int x = 0; x < tilesetColumns; x++)
                {
                    _tiles[y * tilesetColumns + x] = new Rectangle(
                        x * _tileWidth,  // upper left-hand x coordinate
                        y * _tileHeight, // upper left-hand y coordinate
                        _tileWidth,      // width
                        _tileHeight      // height
                    );
                }
            }

            // Third line is map size (in tiles)
            string[] thirdLine = lines[2].Split(',');
            _mapWidth = int.Parse(thirdLine[0]);
            _mapHeight = int.Parse(thirdLine[1]);

            // Fourth line is map data
            _map = new int[_mapWidth * _mapHeight];
            string[] fourthLine = lines[3].Split(',');
            for (int i = 0; i < _mapWidth * _mapHeight; i++)
            {
                _map[i] = int.Parse(fourthLine[i]);
            }
        }

        /// <summary>
        /// Draws the tilemap
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="spriteBatch">The SpriteBatch to use for drawing</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int y = 0; y < _mapHeight; y++)
            {
                for (int x = 0; x < _mapWidth; x++)
                {
                    // Indexes start at 1, so shift for array coordinates
                    int index = _map[y * _mapWidth + x] - 1;
                    // Index of -1 (shifted from 0) should not be drawn
                    if (index == -1) continue;

                    spriteBatch.Draw(
                        _tilesetTexture,
                        new Vector2(
                            x * _tileWidth,
                            y * _tileHeight
                        ),
                        _tiles[index],
                        Color.White
                    );
                }
            }
        }
    }
}