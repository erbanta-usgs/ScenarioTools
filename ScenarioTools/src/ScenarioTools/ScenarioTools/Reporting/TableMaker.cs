using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ScenarioTools.Reporting
{
    public class TableMaker
    {
        public static Image MakeTableImage(string name, string[,] table, bool isTruncated, int rowsInFullTable, bool includeTimeInTable)
        {
            // Determine the table image size.
            const float topOfTable = 40;
            const float minRowHeight = 30;
            const float defaultColumnWidth = 140;
            int nColumns = table.GetLength(0);
            int nRows = table.GetLength(1);
            int nRowsDisplayed = nRows;
            float defaultColumn0Width = defaultColumnWidth;
            if (includeTimeInTable)
            {
                defaultColumn0Width = 220;
            }

            // Declarations
            bool roomAvailable = true;
            RectangleF rectF;
            float fontHeight = 12.0f;
            Font font = new Font(FontFamily.GenericSansSerif, fontHeight);
            Brush brush = Brushes.Black;
            StringFormat stringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            float cellHeight;
            float tableHeight = 0.0f;

            // Define array of column widths
            float[] columnWidths = new float[nColumns];
            columnWidths[0] = defaultColumn0Width;
            for (int i = 1; i < nColumns; i++)
            {
                columnWidths[i] = defaultColumnWidth;
            }

            // Define array of row heights and calculate required height of each row
            float[] rowHeights = new float[nRows];

            // Make a preliminary Image and Graphics for calculating row heights
            Image imageTemp = new Bitmap(1200, 900);
            using (System.Drawing.Graphics gTemp = System.Drawing.Graphics.FromImage(imageTemp))
            {
                // Iterate through all rows
                for (int i = 0; i < nRows; i++)
                {
                    rowHeights[i] = minRowHeight;

                    // Iterate through all columns
                    for (int j = 0; j < nColumns; j++)
                    {
                        float lineHeight = gTemp.MeasureString(table[j, i], font).Height;
                        rectF = new RectangleF(0.0f, 0.0f, columnWidths[j], rowHeights[i]);

                        // Find number of lines that DrawString will require.
                        int numLines = CountWrappedLines(table[j, i], rectF, font, brush, stringFormat);
                        cellHeight = numLines * (lineHeight + 0.5f) + 0.5f;
                        if (cellHeight > rowHeights[i])
                        {
                            rowHeights[i] = cellHeight;
                        }
                    }
                    tableHeight = tableHeight + rowHeights[i];
                }
            }

            // Create an image to work with
            int imageWidth = 1;
            for (int i = 0; i < nColumns; i++)
            {
                imageWidth = imageWidth + (int)columnWidths[i];
            }
            int imageHeight = 2 + (int)(topOfTable + tableHeight);
            if (isTruncated)
            {
                imageHeight = imageHeight + (int)(minRowHeight * 2.5f);
            }

            int maxTableHeight = imageHeight - Convert.ToInt32(topOfTable) - 1;
            float defaultBottomOfTable = imageHeight - (isTruncated ? (int)minRowHeight * 2 : 0);

            Pen pen = Pens.Black;

            // Make the image and blank the background.
            Image image = new Bitmap(imageWidth, imageHeight);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image))
            {
                GraphicsUnit graphicsUnit = g.PageUnit;

                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillRectangle(Brushes.White, 0, 0, imageWidth, imageHeight);

                // Draw the title.
                float titleFontHeight = 14.0f;
                Font titleFont = new Font(FontFamily.GenericSansSerif, titleFontHeight, FontStyle.Bold);

                // Adjust font to fit title on one line across top of image
                while (g.MeasureString(name, titleFont).Width > imageWidth)
                {
                    titleFontHeight -= 1.0f;
                    titleFont = new Font(FontFamily.GenericSansSerif, titleFontHeight, FontStyle.Bold);
                }

                // Use the Graphics object to draw the title to the image.
                g.DrawString(name, titleFont, brush, imageWidth / 2.0f, topOfTable / 2.0f, stringFormat);

                // Draw the cell contents.
                fontHeight = 12.0f;
                float yRowTop = topOfTable;
                font = new Font(FontFamily.GenericSansSerif, fontHeight);
                float xc;
                for (int j = 0; j < nRows; j++)
                {
                    float yc = (j + 0.5f) * minRowHeight + topOfTable;
                    float yCellText = yRowTop + 0.5f;
                    xc = 0.0f;
                    for (int i = 0; i < nColumns; i++)
                    {
                        if (i > 0)
                        {
                            xc = xc + columnWidths[i - 1];
                        }
                        // Assign default row height
                        cellHeight = rowHeights[j];

                        // Wrap heading text in rectangles
                        rectF = new RectangleF(xc, yCellText, columnWidths[i], cellHeight);
                        if (g.MeasureString(table[i, j], font).Width > columnWidths[i])
                        {
                            yCellText = yRowTop + 0.2f;
                        }
                        // Determine if height of image will accomodate current cell.
                        // If not, truncate table at current row
                        if ((yRowTop + cellHeight) > defaultBottomOfTable)
                        {
                            roomAvailable = false;
                        }
                        if (roomAvailable)
                        {
                            rectF = new RectangleF(xc, yCellText, columnWidths[i], cellHeight);
                            // Draw the cell contents.
                            g.DrawString(table[i, j], font, brush, rectF, stringFormat);
                        }
                        else
                        {
                            rowHeights[j] = 0.0f;
                        }
                    }
                    yRowTop = yRowTop + rowHeights[j];
                }

                // Determine height of table
                float tableHeightAsDrawn = 0.0f;
                for (int j = 0; j < nRowsDisplayed; j++)
                {
                    tableHeightAsDrawn = tableHeightAsDrawn + rowHeights[j];
                }
                float bottomOfTableAsDrawn = topOfTable + tableHeightAsDrawn;
                // Draw the vertical lines.
                float x = 0.0f;
                for (int i = 0; i < nColumns + 1; i++)
                {
                    if (i > 0)
                    {
                        x = x + columnWidths[i - 1];
                    }
                    g.DrawLine(pen, x, topOfTable, x, bottomOfTableAsDrawn);
                }

                // Draw the horizontal lines.
                float y = topOfTable;

                // Draw top line
                g.DrawLine(pen, 0, y, imageWidth - 1, y);

                // Draw line at bottom of each row
                for (int i = 0; i < nRows; i++)
                {
                    if (rowHeights[i] == 0.0f)
                    {
                        break;
                    }
                    y = y + rowHeights[i];
                    g.DrawLine(pen, 0, y, imageWidth - 1, y);
                }

                // If the table is truncated, write "Table truncated for preview" message.
                if (isTruncated)
                {
                    string[] messages = { "Table truncated for preview", rowsInFullTable + " data rows in full table" };
                    for (int i = 0; i < 2; i++)
                    {
                        string message = messages[i];
                        float yc = ((nRows + i) + 0.5f) * minRowHeight + topOfTable;
                        yc = bottomOfTableAsDrawn + (i + 1) * minRowHeight;

                        // Try to use the default font. If necessary, create a new, reduced font.
                        float messageFontHeight = fontHeight;
                        Font messageFont = font;
                        while (g.MeasureString(message, messageFont).Width > imageWidth && messageFontHeight >= 2.0f)
                        {
                            messageFontHeight -= 1.0f;
                            messageFont = new Font(FontFamily.GenericSansSerif, messageFontHeight);
                        }

                        // Draw the cell contents.
                        xc = imageWidth / 2.0f;
                        g.DrawString(message, messageFont, brush, xc, yc, stringFormat);
                    }
                }
            }

            // Return the result.
            return image;
        }

        /// <summary>
        /// Return number of lines that will be needed by a call to Graphics.DrawString, 
        /// if rectangle height does not constrain wrapping.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="rectF"></param>
        /// <param name="font"></param>
        /// <param name="brush"></param>
        /// <param name="stringFormat"></param>
        /// <returns></returns>
        private static int CountWrappedLines(string text, RectangleF rectF, Font font, Brush brush, StringFormat stringFormat)
        {
            StringFormat tempStringFormat = new StringFormat(stringFormat);
            tempStringFormat.Trimming = StringTrimming.Word;
            float maxHeight = 600.0f;
            RectangleF tempRectangleF = new RectangleF(rectF.X, rectF.Y, rectF.Width, maxHeight);
            int charactersFitted, linesFilled;
            int width = Convert.ToInt32(rectF.Width);
            int height = Convert.ToInt32(rectF.Height);
            Image image = new Bitmap(width, height);   
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image);
            g.MeasureString(text, font, tempRectangleF.Size, stringFormat, out charactersFitted, out linesFilled);
            tempStringFormat.Dispose();
            image.Dispose();
            g.Dispose();
            return linesFilled;
        }
    }
}
