using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ScenarioTools.Scene
{
    public enum ElementType
    {
        NoType = -1,
        Scenario = 0,
        Package = 1,
        FeatureSet = 2,
        Feature = 3
    }

    /// <summary>
    /// ITaggable enables an object (e.g. a scenario element such as a 
    /// scenario, package, or feature set) to be linked to a TreeView 
    /// node using the node's Tag property.  When a scenario element
    /// is linked to a tree node, the Tag property of the scenario
    /// element, the tree node, and the link must have the same value.
    /// Each Tag property must be unique in its collection.  
    /// The ConnectList method of MainForm can be used to set up links
    /// properly.
    /// </summary>
    public interface ITaggable : ICloneable, IDescribable
    {
        /// <summary>
        /// Get a new, unique Tag from the TagStore
        /// </summary>
        /// <returns></returns>
        int GetNewTag();

        int Tag { get; set; }

        string Name { get; set; }

        ElementType ElemType { get; }

        TagLink Link { get; set; }

        List<ITaggable> Items { get; set; }

        ITaggable Parent { get; set; }

        /// <summary>
        /// Copy Contents of taggableItem argument to this instance
        /// </summary>
        /// <param name="taggableItem"></param>
        void AssignFrom(ITaggable taggableItem);

        /// <summary>
        /// Short for recursive tag.  When this method is invoked, each scenario
        /// element belonging (recursively) to the instance should get a new tag
        /// and assign it to its corresponding TreeNode.
        /// </summary>
        void ReTag();

        void ConnectList(List<ITaggable> owner);

        void ConvertRelativePaths(string oldDirectoryPath, string newDirectoryPath);

        void SelectTreeNode(TreeView treeView);
    }
}
