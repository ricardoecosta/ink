using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HamstasKitties.UI
{
    /// <summary>
    /// Represents an Item of ListView.
    /// </summary>
    public class ListViewItem : LayerObject
    {
        public ListViewItem(ListView parent, Texture texture)
            : base(parent, texture, Vector2.Zero, Vector2.Zero)
        {
            TouchableSize = Size;
            SubItems = new List<ListViewItem>();
            IsSubItem = false;
            ParentItem = null;
        }

        /// <summary>
        /// Adds new SubItem.
        /// </summary>
        /// <param name="item"></param>
        public void AddSubItem(ListViewItem item)
        {
            if (item != null)
            {
                item.IsSubItem = true;
                item.ParentItem = this;
                SubItems.Add(item);
                if (ParentLayer is ListView)
                {
                    ((ListView)ParentLayer).AddItem(item);
                }
                CheckListViewAsDirty();
            }
        }

        /// <summary>
        /// Removes new SubItem.
        /// </summary>
        /// <param name="item"></param>
        public void RemoveSubItem(ListViewItem item)
        {
            if (item != null)
            {
                SubItems.Remove(item);
                if (ParentLayer is ListView)
                {
                    ((ListView)ParentLayer).RemoveItem(item);
                }
                CheckListViewAsDirty();
            }
        }

        /// <summary>
        /// Gets the sum of width all subitems.
        /// </summary>
        /// <returns></returns>
        public int GetSumOfSubItemsWidth()
        {
            int sum = 0;
            SubItems.ForEach((item) =>
            {
                sum += item.Size.X;
            });
            return sum;
        }

        /// <summary>
        /// Tells to ListView update the ListItems size in the next update.
        /// </summary>
        private void CheckListViewAsDirty()
        {
            if (ParentLayer is ListView)
            {
                //((ListView)ParentLayer).ForceItemsMetricsUpdate = true;
            }
        }

        #region Inherited form LayerObject

        public override void Dispose()
        {
            SubItems.ForEach((item) =>
            {
                if (ParentLayer is ListView)
                {
                    ((ListView)ParentLayer).RemoveItem(item);
                }
                item.Dispose();
            });
            SubItems.Clear();
            base.Dispose();
        }

        public override Rectangle GetCollisionArea()
        {
            return new Rectangle((int)(AbsolutePosition.X - Origin.X),
                                 (int)(AbsolutePosition.Y - Origin.Y),
                                 TouchableSize.X, TouchableSize.Y);
        }
        #endregion

        public Point TouchableSize { get; set; }
        public List<ListViewItem> SubItems { get; protected set; }
        public bool IsSubItem { get; private set; }
        public ListViewItem ParentItem { get; private set; }
    }
}
