using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ContentPipelineTest
{
    [ContentImporter(".tmx", DisplayName = "BasicTiledMapImporter", DefaultProcessor = "BasicTiledMapProcessor")]
    public class BasicTiledMapImporter : ContentImporter<BasicTiledMapContent>
    {
        public override BasicTiledMapContent Import(string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage($"=== Starting TMX Import for {filename} ===");
            BasicTiledMapContent result = new();
            result.Filename = Path.GetFullPath(filename);
            ;
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

                            //case "tileset":
                            //    using (XmlReader st = reader.ReadSubtree())
                            //    {
                            //        st.Read();
                            //        context.Logger.LogMessage("Loading tileset...");
                            //        BasicMap.BasicTileset tileset = LoadBasicTileset(st, context);
                            //        result.Tilesets.Add(tileset.Name, tileset);
                            //        context.Logger.LogMessage($"tileset.Name: {tileset.Name} (FirstTileId: {tileset.FirstTileId})");
                            //        context.Logger.LogMessage($"Loaded tileset: {tileset.Name} (FirstTileId: {tileset.FirstTileId})");
                            //    }
                            //    break;

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
    }
}
