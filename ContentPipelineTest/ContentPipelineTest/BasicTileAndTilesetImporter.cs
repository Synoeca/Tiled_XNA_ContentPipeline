using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace ContentPipelineTest
{
    [ContentImporter(".tmx", DisplayName = "BasicTileAndTilesetImporter", DefaultProcessor = "BasicTileAndTilesetProcessor")]
    public class BasicTileAndTilesetImporter : ContentImporter<BasicTileAndTilesetContent>
    {
        public override BasicTileAndTilesetContent Import(string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage($"=== Starting TMX Import for {filename} ===");
            BasicTileAndTilesetContent result = new()
            {
                Filename = Path.GetFullPath(filename),
                Tiles = new Dictionary<int, BasicTileContent>()
                
            };
            XmlReaderSettings settings = new()
            {
                DtdProcessing = DtdProcessing.Parse
            };

            using StreamReader stream = File.OpenText(filename);
            using XmlReader reader = XmlReader.Create(stream, settings);
            while (reader.Read())
            {
                string name = reader.Name;

                switch (reader.NodeType)
                {
                    case XmlNodeType.DocumentType:
                        if (name != "map")
                        {
                            context.Logger.LogImportantMessage("Invalid map format - document type is not 'map'");
                            throw new Exception("Invalid map format");
                        }
                        break;

                    case XmlNodeType.Element:
                        switch (name)
                        {
                            //case "map":
                            //    result.Width = int.Parse(reader.GetAttribute("width"));
                            //    result.Height = int.Parse(reader.GetAttribute("height"));
                            //    result.TileWidth = int.Parse(reader.GetAttribute("tilewidth"));
                            //    result.TileHeight = int.Parse(reader.GetAttribute("tileheight"));

                            //    context.Logger.LogMessage($"Map dimensions: {result.Width}x{result.Height}");
                            //    context.Logger.LogMessage($"Tile dimensions: {result.TileWidth}x{result.TileHeight}");
                            //    break;

                            case "tileset":
                                using (XmlReader st = reader.ReadSubtree())
                                {
                                    st.Read();
                                    context.Logger.LogMessage("Loading tileset...");
                                    BasicTileAndTilesetContent tileset = LoadBasicTileset(st, context);
                                    result = tileset;
                                    context.Logger.LogMessage($"tileset.Name: {tileset.Name} (FirstTileId: {tileset.FirstTileId})");
                                    context.Logger.LogMessage($"Loaded tileset: {tileset.Name} (FirstTileId: {tileset.FirstTileId})");
                                }
                                break;

                                //case "layer":
                                //    using (XmlReader st = reader.ReadSubtree())
                                //    {
                                //        st.Read();
                                //        context.Logger.LogMessage("Loading layer...");
                                //        BasicMap.BasicLayer layer = LoadBasicLayer(st);
                                //        if (layer != null)
                                //        {
                                //            result.Layers.Add(layer.Name, layer);
                                //            context.Logger.LogMessage($"Loaded layer: {layer.Name} ({layer.Width}x{layer.Height})");
                                //        }
                                //    }
                                //    break;

                                //case "objectgroup":
                                //    using (XmlReader st = reader.ReadSubtree())
                                //    {
                                //        st.Read();
                                //        context.Logger.LogMessage("Loading object group...");
                                //        BasicMap.BasicObjectGroup objectgroup = LoadBasicObjectGroup(st);
                                //        result.ObjectGroups.Add(objectgroup.Name, objectgroup);
                                //        context.Logger.LogMessage($"Loaded object group: {objectgroup.Name} (Objects: {objectgroup.Objects.Count})");
                                //    }
                                //    break;

                                //case "properties":
                                //    using (XmlReader st = reader.ReadSubtree())
                                //    {
                                //        context.Logger.LogMessage("Loading map properties...");
                                //        int propertyCount = 0;
                                //        while (!st.EOF)
                                //        {
                                //            if (st.NodeType == XmlNodeType.Element && st.Name == "property")
                                //            {
                                //                string propName = st.GetAttribute("name");
                                //                string propValue = st.GetAttribute("value");
                                //                if (propName != null)
                                //                {
                                //                    result.Properties.Add(propName, propValue);
                                //                    propertyCount++;
                                //                }
                                //            }
                                //            st.Read();
                                //        }
                                //        context.Logger.LogMessage($"Loaded {propertyCount} map properties");
                                //    }
                                //    break;
                        }
                        break;
                }
            }
            return result;
        }
        private BasicTileAndTilesetContent LoadBasicTileset(XmlReader reader, ContentImporterContext context)
        {
            context.Logger.LogMessage("\n=== Starting Tileset Loading ===");
            context.Logger.LogMessage("Reading initial attributes...");

            // Log raw attribute values before parsing
            context.Logger.LogMessage("Raw XML Attributes:");
            context.Logger.LogMessage($"  name: {reader.GetAttribute("name")}");
            context.Logger.LogMessage($"  firstgid: {reader.GetAttribute("firstgid")}");
            context.Logger.LogMessage($"  tilewidth: {reader.GetAttribute("tilewidth")}");
            context.Logger.LogMessage($"  tileheight: {reader.GetAttribute("tileheight")}");
            context.Logger.LogMessage($"  margin: {reader.GetAttribute("margin")}");
            context.Logger.LogMessage($"  spacing: {reader.GetAttribute("spacing")}");
            context.Logger.LogMessage($"  tilecount: {reader.GetAttribute("tilecount")}");
            context.Logger.LogMessage($"  columns: {reader.GetAttribute("columns")}");

            BasicTileAndTilesetContent result = new()
            {
                Name = reader.GetAttribute("name")!,
                FirstTileId = ParseIntAttribute(reader, "firstgid"),
                TileWidth = ParseIntAttribute(reader, "tilewidth"),
                TileHeight = ParseIntAttribute(reader, "tileheight"),
                Margin = ParseIntAttribute(reader, "margin"),
                Spacing = ParseIntAttribute(reader, "spacing"),
                Tiles = new Dictionary<int, BasicTileContent>(),
                Filename = reader.GetAttribute("name")!
            };

            context.Logger.LogMessage("\nParsed initial values:");
            context.Logger.LogMessage($"  Name: {result.Name}");
            context.Logger.LogMessage($"  FirstTileId: {result.FirstTileId}");
            context.Logger.LogMessage($"  TileWidth: {result.TileWidth}");
            context.Logger.LogMessage($"  TileHeight: {result.TileHeight}");
            context.Logger.LogMessage($"  Margin: {result.Margin}");
            context.Logger.LogMessage($"  Spacing: {result.Spacing}");

            int currentTileId = -1;
            context.Logger.LogMessage("\nProcessing tileset child elements...");

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case "image":
                                string source = reader.GetAttribute("source");
                                string width = reader.GetAttribute("width");
                                string height = reader.GetAttribute("height");
                                result.Image = source;
                                result.TexWidth = Convert.ToInt32(width);
                                result.TexHeight = Convert.ToInt32(height);
                                

                                context.Logger.LogMessage("Found image element:");
                                context.Logger.LogMessage($"  Source: {source}");
                                context.Logger.LogMessage($"  Width: {width}");
                                context.Logger.LogMessage($"  Height: {height}");
                                break;

                            case "tile":
                                string idAttr = reader.GetAttribute("id");
                                int tileId = int.Parse(idAttr ?? throw new InvalidOperationException($"Tile missing id attribute"));

                                // Create new tile instance
                                BasicTileContent tile = new()
                                {
                                    Id = tileId,
                                    Properties = new Dictionary<string, string>()
                                };

                                // Get tile type if specified
                                string tileType = reader.GetAttribute("type");
                                if (!string.IsNullOrEmpty(tileType))
                                {
                                    tile.Type = tileType;
                                }

                                // Get probability if specified
                                string probability = reader.GetAttribute("probability");
                                if (!string.IsNullOrEmpty(probability) && float.TryParse(probability, out float prob))
                                {
                                    tile.Probability = prob;
                                }
                                else
                                {
                                    tile.Probability = 1.000f;
                                }

                                context.Logger.LogMessage($"\nProcessing tile {tileId}:");
                                context.Logger.LogMessage($"  Type: {tileType ?? "none"}");
                                context.Logger.LogMessage($"  Probability: {probability ?? "1"}");

                                // Process properties if they exist
                                using (XmlReader tileReader = reader.ReadSubtree())
                                {
                                    while (tileReader.Read())
                                    {
                                        if (tileReader.NodeType == XmlNodeType.Element && tileReader.Name == "properties")
                                        {
                                            using (XmlReader propertiesReader = tileReader.ReadSubtree())
                                            {
                                                while (propertiesReader.Read())
                                                {
                                                    if (propertiesReader.NodeType == XmlNodeType.Element &&
                                                        propertiesReader.Name == "property")
                                                    {
                                                        string propName = propertiesReader.GetAttribute("name");
                                                        string propType = propertiesReader.GetAttribute("type");
                                                        string propValue = propertiesReader.GetAttribute("value");

                                                        if (!string.IsNullOrEmpty(propName) && !string.IsNullOrEmpty(propValue))
                                                        {
                                                            tile.Properties[propName] = propValue;
                                                            context.Logger.LogMessage($"    Property: {propName} ({propType ?? "string"}) = {propValue}");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                // Add the tile to the tileset
                                result.Tiles[tileId] = tile;
                                break;
                        }
                        break;

                    case XmlNodeType.EndElement:
                        if (reader.Name == "tileset")
                        {
                            context.Logger.LogMessage("\n=== Completed Tileset Loading ===");
                            return result;
                        }
                        break;
                }
            }

            context.Logger.LogMessage("=== Completed Tileset Loading ===");
            return result;
        }

        private static int ParseIntAttribute(XmlReader reader, string attributeName, int defaultValue = 0)
        {
            string value = reader.GetAttribute(attributeName);
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            if (int.TryParse(value, out int result))
            {
                return result;
            }
            else
            {
                throw new InvalidOperationException($"Failed to parse {attributeName} attribute. Raw value: '{value}'");
            }
        }
    }
}
