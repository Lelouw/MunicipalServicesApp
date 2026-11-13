using System;
using System.Collections.Generic;
using MunicipalServicesApp.DataStructures;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Services
{
    /// <summary>
    /// General tree of City of Jozi services. Departments are children of the city,
    /// categories are children of a department, and each request is a leaf under its category.
    /// Choosing Water on the screen is a walk to that node and a read of its leaves.
    /// </summary>
    public sealed class ServiceCatalogue
    {
        private readonly Dictionary<string, GeneralTreeNode<CatalogueItem>> categories;

        private ServiceCatalogue(GeneralTreeNode<CatalogueItem> root, Dictionary<string, GeneralTreeNode<CatalogueItem>> categories)
        {
            Root = root;
            this.categories = categories;
        }

        public GeneralTreeNode<CatalogueItem> Root { get; private set; }

        public static ServiceCatalogue Create()
        {
            var categories = new Dictionary<string, GeneralTreeNode<CatalogueItem>>(StringComparer.OrdinalIgnoreCase);
            var root = new GeneralTreeNode<CatalogueItem>(new CatalogueItem { Name = "City of Jozi" });
            AddDepartment(root, categories, "Water and Sanitation", "Water", "Sanitation");
            AddDepartment(root, categories, "Roads and Transport", "Roads");
            AddDepartment(root, categories, "Energy", "Electricity", "Street Lighting");
            AddDepartment(root, categories, "Waste and Environment", "Waste Collection", "Parks and Recreation");
            AddDepartment(root, categories, "Other services", "Utilities", "Other");
            return new ServiceCatalogue(root, categories);
        }

        public void AttachRequest(string category, string requestId)
        {
            GeneralTreeNode<CatalogueItem> categoryNode;
            if (string.IsNullOrWhiteSpace(category) || !categories.TryGetValue(category, out categoryNode))
            {
                categoryNode = categories["Other"];
            }

            for (int i = 0; i < categoryNode.Children.Count; i++)
            {
                if (string.Equals(categoryNode.Children[i].Value.RequestId, requestId, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            categoryNode.AddChild(new CatalogueItem
            {
                Name = requestId,
                RequestId = requestId
            });
        }

        public IList<string> RequestIds(GeneralTreeNode<CatalogueItem> node)
        {
            var ids = new List<string>();
            if (node == null)
            {
                return ids;
            }

            Collect(node, ids);
            return ids;
        }

        public int CountRequests(GeneralTreeNode<CatalogueItem> node)
        {
            return RequestIds(node).Count;
        }

        private static void Collect(GeneralTreeNode<CatalogueItem> node, IList<string> ids)
        {
            if (node.Value != null && node.Value.IsRequest)
            {
                ids.Add(node.Value.RequestId);
                return;
            }

            for (int i = 0; i < node.Children.Count; i++)
            {
                Collect(node.Children[i], ids);
            }
        }

        private static void AddDepartment(
            GeneralTreeNode<CatalogueItem> root,
            Dictionary<string, GeneralTreeNode<CatalogueItem>> categories,
            string department,
            params string[] categoryNames)
        {
            GeneralTreeNode<CatalogueItem> departmentNode = root.AddChild(new CatalogueItem { Name = department });
            for (int i = 0; i < categoryNames.Length; i++)
            {
                GeneralTreeNode<CatalogueItem> categoryNode = departmentNode.AddChild(new CatalogueItem
                {
                    Name = categoryNames[i]
                });
                categories[categoryNames[i]] = categoryNode;
            }
        }
    }
}
