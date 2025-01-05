using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ContentPipelineTest
{
    [ContentImporter(".tmx", DisplayName = "BasicTiledMTLImporter", DefaultProcessor = "BasicTiledMTLProcessor")]
    public class BasicTiledMTLImporter : ContentImporter<BasicTiledMTLContent>
    {
        public override BasicTiledMTLContent Import(string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage($"=== Starting TMX Import for {filename} ===");
            BasicTiledMTLContent result = new()
            {
                Filename = Path.GetFullPath(filename),
                Tilesets = new Dictionary<string, BasicParalleledTilesetContent>(),
                Layers = new Dictionary<string, BasicParalleledLayerContent>()
            };

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
                            case "map":
                                result.Width = int.Parse(reader.GetAttribute("width"));
                                result.Height = int.Parse(reader.GetAttribute("height"));
                                result.TileWidth = int.Parse(reader.GetAttribute("tilewidth"));
                                result.TileHeight = int.Parse(reader.GetAttribute("tileheight"));

                                context.Logger.LogMessage($"Map dimensions: {result.Width}x{result.Height}");
                                context.Logger.LogMessage($"Tile dimensions: {result.TileWidth}x{result.TileHeight}");
                                break;

                            case "tileset":
                                using (XmlReader st = reader.ReadSubtree())
                                {
                                    st.Read();
                                    context.Logger.LogMessage("Loading tileset...");
                                    BasicParalleledTilesetContent tileset = LoadBasicTileset(st, context);
                                    result.Tilesets[tileset.Name] = tileset;
                                    context.Logger.LogMessage($"tileset.Name: {tileset.Name} (FirstTileId: {tileset.FirstTileId})");
                                    context.Logger.LogMessage($"Loaded tileset: {tileset.Name} (FirstTileId: {tileset.FirstTileId})");
                                }
                                break;


                            case "layer":
                                using (XmlReader layerReader = reader.ReadSubtree())
                                {
                                    layerReader.Read();
                                    context.Logger.LogMessage("Loading layer...");
                                    BasicParalleledLayerContent layer = LoadBasicLayer(layerReader, filename, context);
                                    if (layer != null)
                                    {
                                        result.Layers[layer.Name] = layer;
                                        context.Logger.LogMessage($"Loaded layer: {layer.Name} ({layer.Width}x{layer.Height})");
                                    }
                                    else
                                    {
                                        context.Logger.LogMessage("Couldn't load layer!");
                                    }
                                }
                                break;

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

        private BasicParalleledTilesetContent LoadBasicTileset(XmlReader reader, ContentImporterContext context)
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

            BasicParalleledTilesetContent result = new();
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

                                    if (!result.TileProperties.TryGetValue(currentTileId, out BasicParalleledTilesetContent.TilePropertyList props))
                                    {
                                        props = new BasicParalleledTilesetContent.TilePropertyList();
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

        private BasicParalleledLayerContent LoadBasicLayer(XmlReader reader, string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage("\n=== Starting Layer Loading ===");
            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";
            BasicParalleledLayerContent result = new();
            BasicParalleledLayerContent.FlippedHorizontallyFlag = 0x80000000;
            BasicParalleledLayerContent.FlippedVerticallyFlag = 0x40000000;
            BasicParalleledLayerContent.FlippedDiagonallyFlag = 0x20000000;
            BasicParalleledLayerContent.HorizontalFlipDrawFlag = 1;
            BasicParalleledLayerContent.VerticalFlipDrawFlag = 2;
            BasicParalleledLayerContent.DiagonallyFlipDrawFlag = 4;
            result.Filename = Path.GetFullPath(filename);

            context.Logger.LogMessage("Reading initial attributes...");
            if (reader.GetAttribute("name") != null)
            {
                result.Name = reader.GetAttribute("name");
            }
            if (reader.GetAttribute("width") != null)
            {
                result.Width = int.Parse(reader.GetAttribute("width") ?? throw new InvalidOperationException());
            }
            if (reader.GetAttribute("height") != null)
            {
                result.Height = int.Parse(reader.GetAttribute("height") ?? throw new InvalidOperationException());
            }
            if (reader.GetAttribute("opacity") != null)
            {
                result.Opacity = float.Parse(reader.GetAttribute("opacity") ?? throw new InvalidOperationException(), NumberStyles.Any, ci);
                context.Logger.LogMessage($"Layer Opacity: {result.Opacity}");
            }
            else
            {
                result.Opacity = 1.0f;
                context.Logger.LogMessage("Layer Opacity not specified, using default value: 1.0");
            }

            result.Tiles = new int[result.Width * result.Height];
            result.FlipAndRotate = new byte[result.Width * result.Height];

            while (!reader.EOF)
            {
                string name = reader.Name;

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (name)
                        {
                            case "data":
                                {
                                    if (reader.GetAttribute("encoding") == null)
                                    {
                                        using XmlReader st = reader.ReadSubtree();
                                        int i = 0;
                                        while (!st.EOF)
                                        {
                                            switch (st.NodeType)
                                            {
                                                case XmlNodeType.Element:
                                                    if (st.Name == "tile")
                                                    {
                                                        if (i < result.Tiles.Length)
                                                        {
                                                            result.Tiles[i] = int.Parse(st.GetAttribute("gid"));
                                                            i++;
                                                        }
                                                    }

                                                    break;
                                                case XmlNodeType.EndElement:
                                                    break;
                                            }

                                            st.Read();
                                        }
                                    }
                                    else
                                    {
                                        string encoding = reader.GetAttribute("encoding");
                                        string compressor = reader.GetAttribute("compression");
                                        switch (encoding)
                                        {
                                            case "base64":
                                                {
                                                    int dataSize = (result.Width * result.Height * 4) + 1024;
                                                    byte[] buffer = new byte[dataSize];
                                                    reader.ReadElementContentAsBase64(buffer, 0, dataSize);

                                                    Stream stream = new MemoryStream(buffer, false);
                                                    switch (compressor)
                                                    {
                                                        case "gzip":
                                                            stream = new GZipStream(stream, CompressionMode.Decompress, false);
                                                            break;
                                                        case "zlib":
                                                            stream = new GZipStream(stream, CompressionMode.Decompress, false);
                                                            break;
                                                    }

                                                    using (stream)
                                                    using (BinaryReader br = new(stream))
                                                    {
                                                        for (int i = 0; i < result.Tiles.Length; i++)
                                                        {
                                                            uint tileData = br.ReadUInt32();

                                                            // The data contain flip information as well as the tileset index
                                                            byte flipAndRotateFlags = 0;
                                                            if ((tileData & BasicTiledLayerContent.FlippedHorizontallyFlag) != 0)
                                                            {
                                                                flipAndRotateFlags |= BasicTiledLayerContent.HorizontalFlipDrawFlag;
                                                            }

                                                            if ((tileData & BasicTiledLayerContent.FlippedVerticallyFlag) != 0)
                                                            {
                                                                flipAndRotateFlags |= BasicTiledLayerContent.VerticalFlipDrawFlag;
                                                            }

                                                            if ((tileData & BasicTiledLayerContent.FlippedDiagonallyFlag) != 0)
                                                            {
                                                                flipAndRotateFlags |= BasicTiledLayerContent.DiagonallyFlipDrawFlag;
                                                            }

                                                            result.FlipAndRotate[i] = flipAndRotateFlags;

                                                            // Clear the flip bits before storing the tile data
                                                            tileData &= ~(BasicTiledLayerContent.FlippedHorizontallyFlag |
                                                                          BasicTiledLayerContent.FlippedVerticallyFlag |
                                                                          BasicTiledLayerContent.FlippedDiagonallyFlag);
                                                            result.Tiles[i] = (int)tileData;
                                                        }
                                                    }

                                                    continue;
                                                }

                                            default:
                                                throw new Exception("Unrecognized encoding.");
                                        }
                                    }
                                }
                                break;
                            case "properties":
                                {
                                    using XmlReader st = reader.ReadSubtree();
                                    while (!st.EOF)
                                    {
                                        switch (st.NodeType)
                                        {
                                            case XmlNodeType.Element:
                                                if (st.Name == "property")
                                                {
                                                    if (st.GetAttribute("name") != null)
                                                    {
                                                        result.Properties.Add(st.GetAttribute("name"), st.GetAttribute("value"));
                                                    }
                                                }

                                                break;
                                            case XmlNodeType.EndElement:
                                                break;
                                        }

                                        st.Read();
                                    }
                                }
                                break;
                        }

                        break;
                    case XmlNodeType.EndElement:
                        break;
                }

                reader.Read();
            }

            return result;
        }
    }
}
