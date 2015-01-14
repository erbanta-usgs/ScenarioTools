using System.Windows.Forms;

namespace ScenarioAnalyzer
{
    public class IconHelper
    {
        private static ImageList iconList;

        public const int ERROR_INDEX = 0;
        public const int WARNING_INDEX = 1;
        public const int REFRESH_INDEX_0 = 2;
        public const int REFRESH_INDEX_1 = 3;
        public const int REFRESH_INDEX_2 = 4;
        public const int REFRESH_INDEX_3 = 5;
        public const int BLANK_INDEX = 6;
        public const int NORMAL_INDEX = 7;
        public const int MAP_INDEX = 8;
        public const int PUMA_MAP_INDEX = 8;
        public const int GRAPH_INDEX = 9;
        public const int TABLE_INDEX = 10;

        public static ImageList GetIconList()
        {
            // If the icon list has not been made, make it.
            if (iconList == null)
            {
                // Make an empty list.
                iconList = new ImageList();

                // Add the icons.
                iconList.Images.Add(Properties.Resources.icon_error);
                iconList.Images.Add(Properties.Resources.icon_warning);
                iconList.Images.Add(Properties.Resources.icon_refresh_0);
                iconList.Images.Add(Properties.Resources.icon_refresh_1);
                iconList.Images.Add(Properties.Resources.icon_refresh_2);
                iconList.Images.Add(Properties.Resources.icon_refresh_3);
                iconList.Images.Add(Properties.Resources.icon_blank);
                iconList.Images.Add(Properties.Resources.icon_normal);
                iconList.Images.Add(Properties.Resources.icon_map);
                iconList.Images.Add(Properties.Resources.icon_graph);
                iconList.Images.Add(Properties.Resources.icon_table);
            }

            // Return the icon list.
            return iconList;
        }
    }
}
