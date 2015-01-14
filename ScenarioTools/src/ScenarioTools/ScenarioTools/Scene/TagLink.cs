using System;
using System.Windows.Forms;

namespace ScenarioTools.Scene
{
    public class TagLink : ICloneable
    {
        private int _tag;
        private TreeNode _treeNode;
        private ITaggable _scenarioElement;
        private TagStore _owner;

        /// <summary>
        /// Constructor for TagLink class
        /// </summary>
        /// <param name="tag">Tag used to link to a TreeNode</param>
        /// <param name="treeNode">TreeNode to which link is assigned</param>
        /// <param name="scenarioElement">Element (Scenario, Package, or FeatureSet)
        /// that is linked to TreeNode</param>
        /// <param name="owner">TagStore that keeps track of all TagLinks</param>
        public TagLink(int tag, TreeNode treeNode, ITaggable scenarioElement, TagStore owner)
        {
            _treeNode = treeNode;
            _scenarioElement = scenarioElement;
            _owner = owner;
            this.Tag = tag;
        }

        /// <summary>
        /// Constructor for class TagLink
        /// </summary>
        /// <param name="treeNode">TreeNode to which link is assigned</param>
        /// <param name="scenarioElement">Element (Scenario, Package, or FeatureSet)
        /// that is linked to TreeNode</param>
        /// <param name="owner">TagStore that keeps track of all TagLinks</param>
        public TagLink(TreeNode treeNode, ITaggable scenarioElement, TagStore owner)
        {
            _treeNode = treeNode;
            _scenarioElement = scenarioElement;
            _owner = owner;
            this.Tag = owner.GetNextTag();
        }

        /// <summary>
        /// Default constructor for class TagLink
        /// </summary>
        public TagLink()
        {
            _treeNode = null;
            _scenarioElement = null;
            _owner = null;
            _tag = 0;
        }

        public int Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
                if (_scenarioElement != null) { this._scenarioElement.Tag = value; }
                if (_treeNode != null) { this._treeNode.Tag = value; }
            }
        }

        public TreeNode TreeNode
        {
            get
            {
                return _treeNode;
            }
            set
            {
                _treeNode = value;
            }
        }

        public ITaggable ScenarioElement
        {
            get
            {
                return _scenarioElement;
            }
            set
            {
                _scenarioElement = value;
            }
        }

        public TagStore Owner
        {
            get
            {
                return _owner;
            }
            set
            {
                _owner = value;
            }
        }

        public void AssignFrom(TagLink taglinkSource)
        {
            ScenarioElement = taglinkSource.ScenarioElement;
            TreeNode = taglinkSource.TreeNode;
            Owner = taglinkSource.Owner;
            Tag = taglinkSource.Tag;
        }

        public object Clone()
        {
            // After invoking TagLink.Clone, calling routine likely will need to 
            // reassign ScenarioElement and TreeNode, and their Tag values.
            TagLink newLink = new TagLink();
            newLink.ScenarioElement = this._scenarioElement;
            newLink.TreeNode = this._treeNode;
            newLink.Owner = this._owner;
            newLink.Tag = this._tag; // operates on _scenarioElement and _treeNode as well as _tag
            return newLink;
        }

        public void ReTag()
        {
            this.Tag = this.Owner.GetNextTag();
        }

    }
}
