using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App2.SolidWorksPackage.Simulation.MaterialWorker
{
    public class MaterialManager
    {

        private MaterialDBParser parser;

        public Dictionary<string, Material> materials;

        public MaterialManager()
        {

            parser = new MaterialDBParser();

            materials = new Dictionary<string, Material>();

        }

        public void LoadDBMaterials(string path)
        {

            IEnumerable<Material> materials = parser.GetMaterials(path);

            foreach (Material material in materials)
            {

                this.materials.Add(material.name, material);

            }

        }

        //public TreeNode[] GetTreeNodeView()
        //{

        //    List<TreeNode> result = new List<TreeNode>();

        //    Dictionary<string, TreeNode> categories = new Dictionary<string, TreeNode>();

        //    foreach (string name in materials.Keys)
        //    {

        //        Material material = materials[name];

        //        string category = material.category;

        //        TreeNode categoryNode;

        //        if (!categories.TryGetValue(category, out categoryNode))
        //        {

        //            categoryNode = new TreeNode(category);

        //            categories.Add(category, categoryNode);

        //            result.Add(categoryNode);

        //        }

        //        categoryNode.Nodes.Add(name);

        //    }

        //    return result.ToArray();

        //}

    }
}
