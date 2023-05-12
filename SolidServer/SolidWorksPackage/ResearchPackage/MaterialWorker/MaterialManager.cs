using SolidServer.SolidWorksApplicationPackage;
using System;
using System.Collections.Generic;
using System.Xml;

namespace SolidServer.SolidWorksPackage.ResearchPackage
{
    public class MaterialManager
    {
        private const string XML_NODE_MATERIAL_CATEGORY = "classification";
        private const string XML_NODE_MATERIAL_CATEGORY_ATTR_NAME = "name";

        private const string XML_NODE_MATERIAL = "material";
        private const string XML_NODE_MATERIAL_ATTR_NAME = "name";

        private const string XML_NODE_MATERIAL_PHYSICAL_PROPERTIE = "physicalproperties";

        private const string XML_NODE_MATERIAL_PHYSICAL_PROPERTIE_EX = "EX";
        private const string XML_NODE_MATERIAL_PHYSICAL_PROPERTIE_NUXY = "NUXY";
        private const string XML_NODE_MATERIAL_PHYSICAL_PROPERTIE_GXY = "GXY";
        private const string XML_NODE_MATERIAL_PHYSICAL_PROPERTIE_ALPX = "ALPX";
        private const string XML_NODE_MATERIAL_PHYSICAL_PROPERTIE_DENS = "DENS";
        private const string XML_NODE_MATERIAL_PHYSICAL_PROPERTIE_KX = "KX";
        private const string XML_NODE_MATERIAL_PHYSICAL_PROPERTIE_C = "C";
        private const string XML_NODE_MATERIAL_PHYSICAL_PROPERTIE_SIGXT = "SIGXT";
        private const string XML_NODE_MATERIAL_PHYSICAL_PROPERTIE_SIGYLD = "SIGYLD";

        private const string XML_NODE_MATERIAL_PHYSICAL_PROPERTIE_ATTR_NAME = "displayname";
        private const string XML_NODE_MATERIAL_PHYSICAL_PROPERTIE_ATTR_VALUE = "value";


        public static Dictionary<string, Material> GetMaterials()
        {
            var materialDictionary = new Dictionary<string, Material>();
            var paths = SolidWorksAppWorker.GetPathsMaterialDataBase();

            foreach (var path in paths)
            {
                foreach (Material material in GetMaterialsFromPath( path))
                {
                    materialDictionary.Add(material.name, material);
                }
            }
            return materialDictionary;
        }

        private static List<Material> GetMaterialsFromPath(string path)
        {
            var xDoc = new XmlDocument();
            xDoc.Load(path);
            var xRoot = xDoc.DocumentElement;
            List<Material> materials = new List<Material>();

            foreach (XmlNode xNode in xRoot)
            {

                if (xNode.Name == XML_NODE_MATERIAL_CATEGORY)
                {
                    string category = xNode.Attributes.GetNamedItem(XML_NODE_MATERIAL_CATEGORY_ATTR_NAME).Value;
                    materials.AddRange(GetMaterialsFromXmlChildNodes(xNode.ChildNodes, category));
                }
            }

            return materials;
        }

        private static HashSet<Material> GetMaterialsFromXmlChildNodes(XmlNodeList xNodeList, string categoryName)
        {
            HashSet<Material> result = new HashSet<Material>();

            foreach (XmlNode xNode in xNodeList)
            {
                if (xNode.Name == XML_NODE_MATERIAL)
                {
                    string materialName = xNode.Attributes.GetNamedItem(XML_NODE_MATERIAL_ATTR_NAME).Value;
                    double[] physicalProperties = GetMaterialPhysicalProperties(xNode.ChildNodes);
                    Material material = new Material(categoryName, materialName, physicalProperties);
                    result.Add(material);
                }
            }
            return result;
        }
        private static double[] GetMaterialPhysicalProperties(XmlNodeList xNodeList)
        {
            double[] result = new double[9]; // всего 9 физических параметров
            foreach (XmlNode xNode in xNodeList)
            {
                if (xNode.Name == XML_NODE_MATERIAL_PHYSICAL_PROPERTIE)
                {
                    XmlNodeList xNodeListPhysicalProperties = xNode.ChildNodes;

                    result[0] = GetMaterialPhysicalPropertieValue(xNodeListPhysicalProperties,
                        XML_NODE_MATERIAL_PHYSICAL_PROPERTIE_EX);

                    result[1] = GetMaterialPhysicalPropertieValue(xNodeListPhysicalProperties,
                        XML_NODE_MATERIAL_PHYSICAL_PROPERTIE_NUXY);

                    result[2] = GetMaterialPhysicalPropertieValue(xNodeListPhysicalProperties,
                        XML_NODE_MATERIAL_PHYSICAL_PROPERTIE_GXY);

                    result[3] = GetMaterialPhysicalPropertieValue(xNodeListPhysicalProperties,
                        XML_NODE_MATERIAL_PHYSICAL_PROPERTIE_ALPX);

                    result[4] = GetMaterialPhysicalPropertieValue(xNodeListPhysicalProperties,
                        XML_NODE_MATERIAL_PHYSICAL_PROPERTIE_DENS);

                    result[5] = GetMaterialPhysicalPropertieValue(xNodeListPhysicalProperties,
                        XML_NODE_MATERIAL_PHYSICAL_PROPERTIE_KX);

                    result[6] = GetMaterialPhysicalPropertieValue(xNodeListPhysicalProperties,
                        XML_NODE_MATERIAL_PHYSICAL_PROPERTIE_C);

                    result[7] = GetMaterialPhysicalPropertieValue(xNodeListPhysicalProperties,
                        XML_NODE_MATERIAL_PHYSICAL_PROPERTIE_SIGXT);

                    result[8] = GetMaterialPhysicalPropertieValue(xNodeListPhysicalProperties,
                        XML_NODE_MATERIAL_PHYSICAL_PROPERTIE_SIGYLD);
                    break;
                }
            }
            return result;
        }

        private static double GetMaterialPhysicalPropertieValue(XmlNodeList xNodeList, string propertie)
        {
            double result = 0;

            foreach (XmlNode xNode in xNodeList)
            {
                if (xNode.Name == propertie)
                {
                    string value = xNode.Attributes.GetNamedItem(XML_NODE_MATERIAL_PHYSICAL_PROPERTIE_ATTR_VALUE).Value;
                    value = value.Replace('.', ',');
                    result = Convert.ToDouble(value);
                    break;
                }
            }
            return result;
        }
    }
}
