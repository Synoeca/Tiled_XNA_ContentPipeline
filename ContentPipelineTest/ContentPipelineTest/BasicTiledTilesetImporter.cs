using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace ContentPipelineTest
{
    [ContentImporter(".tmx", DisplayName = "BasicTiledTilesetImporter", DefaultProcessor = "BasicTiledTilesetProcessor")]
    public class BasicTiledTilesetImporter : ContentImporter<BasicTiledTilesetContent>
    {
        public override BasicTiledTilesetContent Import(string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage($"=== Starting TMX Import for {filename} ===");
            BasicTiledTilesetContent result = new();
            result.Filename = Path.GetFullPath(filename);
            result.TileProperties = new Dictionary<int, BasicTiledTilesetContent.TilePropertyList>();
            XmlReaderSettings settings = new();
            settings.DtdProcessing = DtdProcessing.Parse;

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
                                    BasicTiledTilesetContent tileset = LoadBasicTileset(st, context);
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

        private BasicTiledTilesetContent LoadBasicTileset(XmlReader reader, ContentImporterContext context)
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

            BasicTiledTilesetContent result = new();
            result.Name = reader.GetAttribute("name")!;
            result.FirstTileId = ParseIntAttribute(reader, "firstgid");
            result.TileWidth = ParseIntAttribute(reader, "tilewidth");
            result.TileHeight = ParseIntAttribute(reader, "tileheight");
            result.Margin = ParseIntAttribute(reader, "margin");
            result.Spacing = ParseIntAttribute(reader, "spacing");
            result.Filename = reader.GetAttribute("name")!;

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
                context.Logger.LogMessage($"\nNode: {reader.Name} (Type: {reader.NodeType})");
                string name = reader.Name;

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (name)
                        {
                            case "image":
                                string source = reader.GetAttribute("source");
                                string width = reader.GetAttribute("width");
                                string height = reader.GetAttribute("height");
                                result.Image = source;

                                context.Logger.LogMessage("Found image element:");
                                context.Logger.LogMessage($"  Source: {source}");
                                context.Logger.LogMessage($"  Width: {width}");
                                context.Logger.LogMessage($"  Height: {height}");
                                break;

                            case "tile":
                                string idAttr = reader.GetAttribute("id");
                                currentTileId = int.Parse(idAttr ?? throw new InvalidOperationException($"Tile missing id attribute"));
                                if (currentTileId != -1)
                                {
                                    string propName = reader.GetAttribute("name");
                                    string propValue = reader.GetAttribute("value");

                                    if (!result.TileProperties.TryGetValue(currentTileId, out BasicTiledTilesetContent.TilePropertyList props))
                                    {
                                        props = new BasicTiledTilesetContent.TilePropertyList();
                                        result.TileProperties[currentTileId] = props;
                                    }

                                    props[propName ?? throw new InvalidOperationException("Property missing name attribute")] = propValue;

                                    context.Logger.LogMessage($"Added property to tile {currentTileId}:");
                                    context.Logger.LogMessage($"  Name: {propName}");
                                    context.Logger.LogMessage($"  Value: {propValue}");
                                }
                                break;
                        }
                        break;

                    case XmlNodeType.EndElement:
                        if (name == "tile")
                        {
                            context.Logger.LogMessage($"Finished processing tile {currentTileId}");
                            currentTileId = -1;
                        }
                        else if (name == "tileset")
                        {
                            context.Logger.LogMessage("\nFinished processing tileset");
                            context.Logger.LogMessage($"Final image path: {result.Image}");
                            //context.Logger.LogMessage($"Total properties: {result.TileProperties.Count}");
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
