using System.Collections.Generic;

namespace ScenarioTools.Scene
{
    public static class TagUtilities
    {
        /// <summary>
        /// Instantiate and connect links between TreeNode nodes and _scenarioElements items
        /// in tree branch starting at TreeNode node defined for Link property of taggableItem.
        /// </summary>
        /// <param name="taggableItem">Any Scenario, Package, or FeatureSet in 
        /// a List of ITaggable.  The taggableItem argument must have its Link property 
        /// defined.</param>
        /// <param name="taggableList">List of ITaggable to which taggableItem belongs</param>
        public static void ConnectList(ITaggable taggableItem, List<ITaggable> owner)
        {
            TagLink newLink = taggableItem.Link;
            int i = 0;
            if (taggableItem.Items != null)
            {
                foreach (ITaggable item in taggableItem.Items)
                {
                    owner.Add(item);
                    // Following code will be handy if I want to show 
                    // individual features on tree view
                    //while (newLink.TreeNode.Nodes.Count <= i)
                    //{
                    //    newLink.TreeNode.Nodes.Add(item.Name);
                    //}
                    if (i < newLink.TreeNode.Nodes.Count)
                    {
                        TagLink link = new TagLink(newLink.TreeNode.Nodes[i], item, newLink.Owner);
                        newLink.Owner.Connect(link);
                    }
                    if (item is Package)
                    {
                        item.Parent = newLink.ScenarioElement; // ????
                    }
                    if (item is FeatureSet)
                    {
                        item.Parent = newLink.ScenarioElement; // ????
                    }
                    i++;
                    ConnectList(item, owner);
                }
            }
        }
    }
}
