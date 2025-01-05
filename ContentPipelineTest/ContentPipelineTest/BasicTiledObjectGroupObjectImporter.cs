using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Xna.Framework;

namespace ContentPipelineTest
{
    [ContentImporter(".tmx", DisplayName = "BasicTiledObjectGroupObjectImporter", DefaultProcessor = "BasicTiledObjectGroupObjectProcessor")]
    public class BasicTiledObjectGroupObjectImporter : ContentImporter<BasicTiledGroupContent>
    {
        public override BasicTiledGroupContent Import(string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage($"=== Starting TMX Import for {filename} ===");
            BasicTiledGroupContent result = new()
            {
                Filename = Path.GetFullPath(filename),
                ObjectGroups = new Dictionary<string, BasicTiledObjectGroupObjectContent>(),
                Properties = new Dictionary<string, string>()
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

                            //case "tileset":
                            //    using (XmlReader st = reader.ReadSubtree())
                            //    {
                            //        st.Read();
                            //        context.Logger.LogMessage("Loading tileset...");
                            //        BasicParallelTileset tileset = LoadBasicTileset(st, context);
                            //        result.Tilesets[tileset.Name] = tileset;
                            //        context.Logger.LogMessage($"tileset.Name: {tileset.Name} (FirstTileId: {tileset.FirstTileId})");
                            //        context.Logger.LogMessage($"Loaded tileset: {tileset.Name} (FirstTileId: {tileset.FirstTileId})");
                            //    }
                            //    break;


                            //case "layer":
                            //    using (XmlReader layerReader = reader.ReadSubtree())
                            //    {
                            //        layerReader.Read();
                            //        context.Logger.LogMessage("Loading layer...");
                            //        BasicParallelLayer layer = LoadBasicLayer(layerReader, filename, context);
                            //        if (layer != null)
                            //        {
                            //            result.Layers[layer.Name] = layer;
                            //            context.Logger.LogMessage($"Loaded layer: {layer.Name} ({layer.Width}x{layer.Height})");
                            //        }
                            //        else
                            //        {
                            //            context.Logger.LogMessage("Couldn't load layer!");
                            //        }
                            //    }
                            //    break;

                            case "group":
                                using (XmlReader st = reader.ReadSubtree())
                                {
                                    st.Read();
                                    context.Logger.LogMessage("Loading group...");
                                    result = LoadGroup(st, context);
                                    context.Logger.LogMessage($"Loaded group: {result.Name}");
                                }
                                break;

                            //case "objectgroup":
                            //    using (XmlReader st = reader.ReadSubtree())
                            //    {
                            //        st.Read();
                            //        context.Logger.LogMessage("Loading object group...");
                            //        BasicTiledObjectGroupObjectContent objectgroup = LoadBasicObjectGroup(st);
                            //        //result.ObjectGroups.Add(objectgroup.Name, objectgroup);
                            //        result = objectgroup;
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

        public BasicTiledGroupContent LoadGroup(XmlReader reader, ContentImporterContext context)
        {
            context.Logger.LogMessage("Loading group...");

            BasicTiledGroupContent group = new()
            {
                ObjectGroups = new Dictionary<string, BasicTiledObjectGroupObjectContent>(),
                Properties = new Dictionary<string, string>()
            };

            group.Id = ParseIntAttribute(reader, "id");
            group.Name = reader.GetAttribute("name") ?? string.Empty;
            group.Locked = ParseBoolAttribute(reader, "locked");

            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case "properties":
                                using (XmlReader st = reader.ReadSubtree())
                                {
                                    st.Read();
                                    LoadProperties(st, group.Properties, context);
                                }
                                break;

                            case "objectgroup":
                                using (XmlReader st = reader.ReadSubtree())
                                {
                                    st.Read();
                                    context.Logger.LogMessage("Loading object group...");
                                    BasicTiledObjectGroupObjectContent objectGroup = LoadBasicObjectGroup(st, context);
                                    if (objectGroup != null)
                                    {
                                        group.ObjectGroups[objectGroup.Name] = objectGroup;
                                        context.Logger.LogMessage($"Added object group: {objectGroup.Name}");
                                    }
                                }
                                break;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if (reader.Name == "group")
                        {
                            return group;
                        }
                        break;
                }

                reader.Read();
            }

            return group;
        }

        public BasicTiledObjectGroupObjectContent LoadBasicObjectGroup(XmlReader reader, ContentImporterContext context)
        {
            context.Logger.LogMessage("\n=== Starting LoadBasicObjectGroup ===");

            BasicTiledObjectGroupObjectContent result = new()
            {
                Objects = new Dictionary<string, BasicTiledObjectContent>(),
                ObjectProperties = new Dictionary<string, string>()
            };

            // Read and log all attributes
            context.Logger.LogMessage("Reading objectgroup attributes:");

            // ID
            result.Id = ParseIntAttribute(reader, "id");
            context.Logger.LogMessage($"  id: {result.Id}");

            // Name
            result.Name = reader.GetAttribute("name") ?? string.Empty;
            context.Logger.LogMessage($"  name: {result.Name}");

            // Class
            result.Class = reader.GetAttribute("class");
            context.Logger.LogMessage($"  class: {result.Class}");

            // Color
            string colorStr = reader.GetAttribute("color");
            result.Color = ParseColor(colorStr);
            if (result.Color != null)
            {
                context.Logger.LogMessage($"  color A: {colorStr} -> {result.Color.Value.A}");
                context.Logger.LogMessage($"  color B: {colorStr} -> {result.Color.Value.B}");
                context.Logger.LogMessage($"  color G: {colorStr} -> {result.Color.Value.G}");
                context.Logger.LogMessage($"  color R: {colorStr} -> {result.Color.Value.R}");
            }
            else
            {
                context.Logger.LogMessage($"  color is null!!");
            }
            

            // Tint Color
            string tintColorStr = reader.GetAttribute("tintcolor");
            result.TintColor = ParseColor(tintColorStr);
            if (result.TintColor != null)
            {
                context.Logger.LogMessage($"  color A: {tintColorStr} -> {result.TintColor.Value.A}");
                context.Logger.LogMessage($"  color B: {tintColorStr} -> {result.TintColor.Value.B}");
                context.Logger.LogMessage($"  color G: {tintColorStr} -> {result.TintColor.Value.G}");
                context.Logger.LogMessage($"  color R: {tintColorStr} -> {result.TintColor.Value.R}");
            }
            else
            {
                context.Logger.LogMessage($"  tint color is null!!");
            }
           

            // Visible (0 = false, 1 = true)
            result.Visible = ParseBoolAttribute(reader, "visible", true);  // Note: reversed logic in TMX
            context.Logger.LogMessage($"  visible: {result.Visible} -> {result.Visible}");

            // Locked
            result.Locked = ParseBoolAttribute(reader, "locked");
            context.Logger.LogMessage($"  locked: {result.Locked}");

            // Opacity
            result.Opacity = ParseFloatAttribute(reader, "opacity", 1.0f);
            context.Logger.LogMessage($"  opacity: {result.Opacity}");

            // Offset
            result.OffsetX = ParseFloatAttribute(reader, "offsetx", 0.0f);
            result.OffsetY = ParseFloatAttribute(reader, "offsety", 0.0f);
            context.Logger.LogMessage($"  offset: ({result.OffsetX}, {result.OffsetY})");

            // Parallax
            result.ParallaxX = ParseFloatAttribute(reader, "parallaxx", 1.0f);
            result.ParallaxY = ParseFloatAttribute(reader, "parallaxy", 1.0f);
            context.Logger.LogMessage($"  parallax: ({result.ParallaxX}, {result.ParallaxY})");

            // Process child elements (properties and objects)
            while (!reader.EOF)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "properties":
                            using (XmlReader st = reader.ReadSubtree())
                            {
                                st.Read();
                                context.Logger.LogMessage("Processing properties:");
                                LoadProperties(st, result.ObjectProperties, context);
                            }
                            break;

                        case "object":
                            using (XmlReader st = reader.ReadSubtree())
                            {
                                st.Read();
                                context.Logger.LogMessage("Processing object:");
                                BasicTiledObjectContent obj = LoadBasicObject(st);
                                if (!result.Objects.TryAdd(obj.Name, obj))
                                {
                                    int count = result.Objects.Keys.Count(k => k.StartsWith(obj.Name));
                                    string newName = $"{obj.Name}_{count}";
                                    result.Objects.Add(newName, obj);
                                    context.Logger.LogMessage($"  renamed duplicate object to: {newName}");
                                }
                                context.Logger.LogMessage($"  added object: {obj.Name}");
                            }
                            break;
                    }
                }
                reader.Read();
            }

            context.Logger.LogMessage("\n=== Object Group Summary ===");
            context.Logger.LogMessage($"Name: {result.Name}");
            context.Logger.LogMessage($"ID: {result.Id}");
            context.Logger.LogMessage($"Position: ({result.OffsetX}, {result.OffsetY})");
            context.Logger.LogMessage($"Parallax: ({result.ParallaxX}, {result.ParallaxY})");
            context.Logger.LogMessage($"Opacity: {result.Opacity}");
            context.Logger.LogMessage($"Visible: {result.Visible}");
            context.Logger.LogMessage($"Locked: {result.Locked}");
            if (result.Color != null)
            {
                context.Logger.LogMessage($"  color A: {colorStr} -> {result.Color.Value.A}");
                context.Logger.LogMessage($"  color B: {colorStr} -> {result.Color.Value.B}");
                context.Logger.LogMessage($"  color G: {colorStr} -> {result.Color.Value.G}");
                context.Logger.LogMessage($"  color R: {colorStr} -> {result.Color.Value.R}");
            }
            else
            {
                context.Logger.LogMessage($"  color is null!!");
            }
            if (result.TintColor != null)
            {
                context.Logger.LogMessage($"  Tint color A: {tintColorStr} -> {result.TintColor.Value.A}");
                context.Logger.LogMessage($"  Tint color B: {tintColorStr} -> {result.TintColor.Value.B}");
                context.Logger.LogMessage($"  Tint color G: {tintColorStr} -> {result.TintColor.Value.G}");
                context.Logger.LogMessage($"  Tint color R: {tintColorStr} -> {result.TintColor.Value.R}");
            }
            else
            {
                context.Logger.LogMessage($"  tint color is null!!");
            }
            context.Logger.LogMessage($"Properties Count: {result.ObjectProperties.Count}");
            context.Logger.LogMessage($"Objects Count: {result.Objects.Count}");

            return result;
        }

        public BasicTiledObjectContent LoadBasicObject(XmlReader reader)
        {
            BasicTiledObjectContent result = new()
            {
                Name = reader.GetAttribute("name"),
                X = int.Parse(reader.GetAttribute("x") ?? throw new InvalidOperationException()),
                Y = int.Parse(reader.GetAttribute("y") ?? throw new InvalidOperationException())
            };

            /*
             * Height and width are optional on objects
             */
            if (int.TryParse(reader.GetAttribute("width"), out int width))
            {
                result.Width = width;
            }

            if (int.TryParse(reader.GetAttribute("height"), out int height))
            {
                result.Height = height;
            }

            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "properties")
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
                                                result.Properties.Add(st.GetAttribute("name") ?? throw new InvalidOperationException(), st.GetAttribute("value"));
                                            }
                                        }

                                        break;
                                    case XmlNodeType.EndElement:
                                        break;
                                }

                                st.Read();
                            }
                        }
                        if (reader.Name == "image")
                        {
                            result.Image = reader.GetAttribute("source");
                        }

                        break;
                    case XmlNodeType.EndElement:
                        break;
                }

                reader.Read();
            }

            return result;
        }

        private void LoadProperties(XmlReader reader, Dictionary<string, string> properties, ContentImporterContext context)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "property")
                {
                    string name = reader.GetAttribute("name");
                    string value = reader.GetAttribute("value");
                    if (!string.IsNullOrEmpty(name))
                    {
                        properties[name] = value ?? string.Empty;
                        context.Logger.LogMessage($"Added property: {name} = {value}");
                    }
                }
            }
        }

        private static int ParseIntAttribute(XmlReader reader, string attributeName, int defaultValue = 0)
        {
            string value = reader.GetAttribute(attributeName);
            return string.IsNullOrEmpty(value) ? defaultValue : int.Parse(value);
        }

        private static float ParseFloatAttribute(XmlReader reader, string attributeName, float defaultValue)
        {
            string value = reader.GetAttribute(attributeName);
            return string.IsNullOrEmpty(value) ? defaultValue :
                float.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        private static bool ParseBoolAttribute(XmlReader reader, string attributeName, bool defaultValue = false)
        {
            string value = reader.GetAttribute(attributeName);
            if (string.IsNullOrEmpty(value)) return defaultValue;

            // Handle numeric boolean values (1 = true, 0 = false)
            if (value == "1") return true;
            if (value == "0") return false;

            // Fall back to standard boolean parsing for "true"/"false" strings
            return bool.Parse(value);
        }

        private static Color? ParseColor(string colorStr)
        {
            if (string.IsNullOrEmpty(colorStr))
                return null;

            if (colorStr.StartsWith("#"))
                colorStr = colorStr.Substring(1);

            if (colorStr.Length == 6)
            {
                // RGB format
                int r = Convert.ToInt32(colorStr.Substring(0, 2), 16);
                int g = Convert.ToInt32(colorStr.Substring(2, 2), 16);
                int b = Convert.ToInt32(colorStr.Substring(4, 2), 16);
                return new Color(r, g, b);
            }
            else if (colorStr.Length == 8)
            {
                // ARGB format
                int a = Convert.ToInt32(colorStr.Substring(0, 2), 16);
                int r = Convert.ToInt32(colorStr.Substring(2, 2), 16);
                int g = Convert.ToInt32(colorStr.Substring(4, 2), 16);
                int b = Convert.ToInt32(colorStr.Substring(6, 2), 16);
                return new Color(r, g, b, a);
            }

            return null;
        }
    }
}
