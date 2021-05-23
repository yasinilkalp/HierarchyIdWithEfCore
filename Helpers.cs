using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HierarchyIdWithEfCore
{
    public class Helpers
    {
        public static List<TreeView> GetTree(List<Category> fields)
        {
            var x = GetTreeMethod("/", fields);

            return x;
        }

        private static List<TreeView> GetTreeMethod(string nodeStr, List<Category> lstCollection)
        {
            List<TreeView> lst = new List<TreeView>();
            HierarchyId node = HierarchyId.Parse(nodeStr);
            var lastItemInCurrentLevel = GetChilds(node, lstCollection);

            foreach (var item in lastItemInCurrentLevel)
            {
                TreeView tr = new TreeView
                {
                    title = item.title,
                    id = item.id,
                    node = item.node
                };
                tr.children = GetTreeMethod(item.node.ToString(), lstCollection);
                lst.Add(tr);
            }

            return lst;
        }

        private static List<TreeView> GetChilds(HierarchyId node, List<Category> lstCollection)
        {
            List<TreeView> child = lstCollection.Where(x => x.HierarchyId.ToString() != "/" && x.HierarchyId.GetAncestor(1).ToString() == node.ToString()).Select(q => new TreeView { id = q.Id, node = q.HierarchyId, title = q.Name }).ToList();
            return child;

        }


        public class TreeView
        {
            public long id { get; set; }
            public string title { get; set; }
            public HierarchyId node { get; set; }
            public string Hierarchy { get { return node.ToString(); } }
            public List<TreeView> children { get; set; }
        }
    }
}
