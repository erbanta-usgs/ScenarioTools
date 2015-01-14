using System;
using System.Collections.ObjectModel;
using System.Drawing;

using USGS.Puma.UI.MapViewer;

namespace ScenarioTools.Reporting
{
    public class STMapLegend : MapLegend
    {
        public STMapLegend()
            : base()
        {
            //Height = 400;
            Font font = new Font("Arial", 11.0f);
            this.Font = font;
        }

        public Image RenderAsImage()
        {
            // Ned TODO: Write STMapLegend.RenderAsImage
            return null;
        }

        public void FitWidth(int newWidth)
        {
            foreach (MapLegendItem item in Items)
            {
                if (item.Parent.Width < newWidth)
                {
                    item.Parent.Width = newWidth;
                }
                item.Width = newWidth;
            }
        }

        new public void AddAndLayoutItems(Collection<GraphicLayer> items)
        {
            MapLegendItemCollection legendItems = new MapLegendItemCollection();
            if (items != null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i] is FeatureLayer)
                    {
                        MapLegendItem item = new MapLegendItem(items[i]);
                        legendItems.Add(item);
                    }
                }
            }
            AddAndLayoutItems(legendItems);
        }
        new public void AddAndLayoutItems(MapLegendItemCollection items)
        {
            Items.Clear();
            if (items != null)
            {
                this.SuspendLayout();
                this.panelBody.SuspendLayout();
                this.panelBody.Controls.Clear();

                for (int i = 0; i < items.Count; i++)
                {
                    Items.Add(items[i]);
                }

                int y = 0;
                int spaceHeight = 6;
                for (int i = Items.Count - 1; i >= 0; i--)
                {
                    Items[i].Dock = System.Windows.Forms.DockStyle.Top;
                    panelBody.Controls.Add(Items[i]);
                    Items[i].LayerVisibilityChanged += new EventHandler<EventArgs>(MapLegend_LayerVisibilityChanged);
                    y += Items[i].Height;
                    {
                        System.Windows.Forms.Panel spacerPanel = new System.Windows.Forms.Panel();
                        spacerPanel.Size = new System.Drawing.Size(panelBody.Width, spaceHeight);
                        spacerPanel.Dock = System.Windows.Forms.DockStyle.Top;
                        panelBody.Controls.Add(spacerPanel);
                        y += spacerPanel.Height;
                    }
                }
                panelBody.Height = y;
                this.FitWidth(this.Width);
                this.Controls.Remove(panelBody);
                this.Controls.Add(panelBody);
                this.panelBody.ResumeLayout(false);
                this.panelBody.PerformLayout();
                this.ResumeLayout(false);
                this.PerformLayout();
            }
        }

        #region Event Handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MapLegend_LayerVisibilityChanged(object sender, EventArgs e)
        {
            OnLayerVisibilityChanged(e);
        }
        #endregion
    }
}
