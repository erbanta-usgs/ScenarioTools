using System;
using System.Collections.Generic;

namespace ScenarioTools.Scene
{
    public class TagStore : ICloneable
    {
        private List <TagLink> _items = new List<TagLink>();

        /// <summary>
        /// Constructor for TagStore class
        /// </summary>
        public TagStore()
        {
            //_items = new List<TagLink>();
        }

        public List<TagLink> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
            }
        }

        public int Count
        {
            get
            {
                return _items.Count;
            }
        }

        public int GetNextTag()
        {
            int i = 0;
            foreach (TagLink link in _items)
            {
                if (link.Tag > i) i = link.Tag;
            }
            i++;
            return i;
        }

        public TagLink GetLinkByTag(int tag)
        {
            TagLink nullLink = null;
            foreach (TagLink link in _items)
            {
                if (link.Tag == tag) return link;
            }
            return nullLink;
        }

        public TagLink GetLinkByIndex(int index)
        {
            TagLink nullLink = null;
            if (index > -1 && index < _items.Count)
            {
                return _items[index];
            }
            else
            {
                return nullLink;
            }
        }

        /// <summary>
        /// Return link with largest index
        /// </summary>
        /// <returns></returns>
        public TagLink GetLastLink()
        {
            TagLink nullLink = null;
            if (_items.Count > 0)
            {
                return _items[_items.Count - 1];
            }
            else
            {
                return nullLink;
            }
        }

        public void Connect(TagLink link)
        {
            link.Tag = this.GetNextTag();
            //link.TreeNode.Tag = link.Tag;
            //link.ScenarioElement.Tag = link.Tag;
            link.ScenarioElement.Link = link;
            _items.Add(link);
        }

        public bool IsValid(int tag)
        {
            bool OK = false;
            foreach (TagLink link in _items)
            {
                if (link.Tag == tag) OK = true;
            }
            return OK;
        }

        public object Clone()
        {
            TagStore tagStore = new TagStore();
            foreach (TagLink link in _items)
            {
                tagStore.Items.Add((TagLink)link.Clone());
            }
            return tagStore;
        }
    }
}
