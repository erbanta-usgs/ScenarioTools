using System;
using System.Collections.Generic;
using System.Text;

using HPdf;
using ScenarioTools.Reporting;
using ScenarioTools.Util;

namespace ScenarioTools.PdfWriting
{
    public class TableWriter
    {
        private const float ROW_HEIGHT = 20.0f;
        private const float COLUMN_WIDTH = 100.0f;
        private const float COLUMN_DATE_AND_TIME_WIDTH = 140.0f;
        private const float PAGE_TOP_BUFFER = 80.0f;
        private const float PAGE_BOTTOM_BUFFER = 80.0f;

        private static HPdfFont fontHelvetica = null;
        private static HPdfFont fontHelveticaBold = null;
        private static HPdfFont fontSymbol = null;

        private static void makeFonts(HPdfDoc pdf)
        {
            // Get the fonts.
            fontHelvetica = pdf.GetFont("Helvetica", null);
            fontHelveticaBold = pdf.GetFont("Helvetica-Bold", null);
            fontSymbol = pdf.GetFont("Symbol", null);
        }

        public static int WriteTable(HPdfDoc pdf, SADocument document, ReportElementTable reportElement, 
                                     int pageNumber, HPdfFont footerFont)
        {
            // Make the fonts.
            makeFonts(pdf);

            // Create a new page.
            HPdfPage page = pdf.AddPage();

            // Determine the number of pages that this table will require.            
            float pageWidth = page.GetWidth();
            float pageHeight = page.GetHeight();
            float heightAvailableForTable = pageHeight - PAGE_TOP_BUFFER - PAGE_BOTTOM_BUFFER;

            bool isTruncated;
            int numDataRowsInFullTable;
            string[,] tableData = reportElement.GetTableData(false, out isTruncated, -1, out numDataRowsInFullTable);
            int numColumnsInTable = tableData.GetLength(0);
            int numRowsInTable = tableData.GetLength(1); // Includes cells populated with column headings

            // Need to find height required for heading row before numRowsPerPage can be assigned
            float headingRowHeight = ROW_HEIGHT; // Default height for heading row
            int numHeadingLines = 1;             // Default number of lines needed to draw headings
            int numLinesThisHeading;
            float fontSizeCells = 12.0f;

            // Populate an array with the headings
            int j;
            string heading;
            string[][] headingLines = new string[numColumnsInTable][];
            page.SetFontAndSize(fontHelveticaBold, fontSizeCells);
            for (j = 0; j < numColumnsInTable; j++)
            {
                heading = tableData[j, 0];
                // Parse column heading into lines of text
                headingLines[j] = WrapText(heading, COLUMN_WIDTH, page);
                numLinesThisHeading = headingLines[j].Length;
                if (numLinesThisHeading > numHeadingLines)
                {
                    numHeadingLines = numLinesThisHeading;
                }
            }
            if (numHeadingLines > 1)
            {
                headingRowHeight = ((numHeadingLines * 1.3f) + 0.3f) * fontSizeCells;
            }

            float heightAvailableForData = heightAvailableForTable - headingRowHeight;
            int numRowsPerPage = (int)Math.Floor(heightAvailableForData / ROW_HEIGHT);
            int numPages = (int)Math.Ceiling((numRowsInTable - 1) / (float)numRowsPerPage);

            // Write the pages.
            int topRowOfPage = 1;
            for (int k = 0; k < numPages; k++)
            {
                // If this is not the first page, make a new page (the first page had to be made to get the width and height).
                if (k != 0)
                {
                    page = pdf.AddPage();
                }

                // Set the title font.
                float titleFontHeight = 24.0f;
                page.SetFontAndSize(fontHelvetica, titleFontHeight);

                // Draw the title.
                string title = reportElement.Name + (numPages == 1 ? "" : " (page " + (k + 1) + " of " + numPages + ")");
                float tw = page.TextWidth(title);
                page.BeginText();
                page.MoveTextPos((page.GetWidth() - tw) / 2, page.GetHeight() - (PAGE_TOP_BUFFER + titleFontHeight) / 2.0f);
                page.ShowText(title);
                page.EndText();

                // Determine how many rows need to be drawn. 
                // Ensure that this number is fewer or equal to the 
                // maximum rows per page and that a single
                // row is not orphaned on the last page.
                int numDataRowsThisPage = numRowsInTable - topRowOfPage;
                if (numDataRowsThisPage > numRowsPerPage - 1)
                {
                    numDataRowsThisPage = numRowsPerPage - 1;
                }
                if (numRowsInTable - numDataRowsThisPage - topRowOfPage == 1)
                {
                    numDataRowsThisPage--;
                }

                // Determine the width of the table. This will be more sophisticated in the future, which is why it's written as a loop.
                float tableWidth = 0.0f;
                float[] columnWidths = new float[numColumnsInTable];
                for (int i = 0; i < numColumnsInTable; i++)
                {
                    if (i == 0 && reportElement.IncludeTimeInTable)
                    {
                        columnWidths[i] = COLUMN_DATE_AND_TIME_WIDTH;
                    }
                    else
                    {
                        columnWidths[i] = COLUMN_WIDTH;
                    }
                    tableWidth += columnWidths[i];
                }

                // Draw the horizontal lines for the table.
                float tableLeft = (pageWidth - tableWidth) / 2.0f;
                float tableRight = (pageWidth + tableWidth) / 2.0f;
                float yc;

                // Draw lines above and below headings
                yc = pageHeight - PAGE_TOP_BUFFER;
                page.MoveTo(tableLeft, yc);
                page.LineTo(tableRight, yc);
                page.Stroke();
                yc = yc - headingRowHeight;
                page.MoveTo(tableLeft, yc);
                page.LineTo(tableRight, yc);
                page.Stroke();

                // Draw line below each data row
                for (int i = 0; i < numDataRowsThisPage; i++)
                {
                    yc = yc - ROW_HEIGHT;
                    page.MoveTo(tableLeft, yc);
                    page.LineTo(tableRight, yc);
                    page.Stroke();
                }

                // Draw the vertical lines for the table.
                float tableTop = pageHeight - PAGE_TOP_BUFFER;
                float tableBottom = tableTop - (numDataRowsThisPage * ROW_HEIGHT + headingRowHeight);
                float xc = tableLeft;
                for (int i = 0; i < numColumnsInTable + 1; i++)
                {
                    page.MoveTo(xc, tableTop);
                    page.LineTo(xc, tableBottom);
                    page.Stroke();

                    if (i < numColumnsInTable)
                    {
                        xc += columnWidths[i];
                    }
                }

                // Draw the column headings
                int rowIndex = 0;
                string headingLine;
                float xcLeft = tableLeft;
                float cellTextWidth;
                page.SetFontAndSize(fontHelveticaBold, fontSizeCells);
                for (int i = 0; i < numColumnsInTable; i++)
                {
                    for (int l = 0; l < headingLines[i].Length; l++)
                    {
                        headingLine = headingLines[i][l];
                        yc = pageHeight - PAGE_TOP_BUFFER - (1.3f * l + 0.7f) * fontSizeCells;
                        // Write the text.
                        cellTextWidth = page.TextWidth(headingLine);
                        page.BeginText();
                        page.MoveTextPos(xcLeft + columnWidths[i] / 2.0f - cellTextWidth / 2.0f, yc - fontSizeCells / 2.0f);
                        page.ShowText(headingLine);
                        page.EndText();
                    }

                    // Advance the value that tracks the left side of the current column.
                    if (i < numColumnsInTable)
                    {
                        xcLeft += columnWidths[i];
                    }
                }

                // Draw the data cell contents.
                for (j = 1; j < numDataRowsThisPage + 1; j++)
                {
                    // Determine the row index.
                    rowIndex = topRowOfPage + j - 1;
                    yc = pageHeight - PAGE_TOP_BUFFER - headingRowHeight - (j - 0.5f) * (ROW_HEIGHT);

                    HPdfFont rowFont = j == 0 ? fontHelveticaBold : fontHelvetica;

                    xcLeft = tableLeft;
                    for (int i = 0; i < numColumnsInTable; i++)
                    {
                        // Start with the default size of the cell font.
                        float cellFontSize = fontSizeCells;
                        page.SetFontAndSize(rowFont, cellFontSize);

                        // Determine a font size that allows the cell value to fit in the cell.
                        string cellValue = tableData[i, rowIndex];
                        while (page.TextWidth(cellValue) > columnWidths[i] && cellFontSize > 2.0f)
                        {
                            cellFontSize -= 0.5f;
                            page.SetFontAndSize(rowFont, cellFontSize);
                        }

                        // Write the text.
                        cellTextWidth = page.TextWidth(cellValue);
                        page.BeginText();
                        page.MoveTextPos(xcLeft + columnWidths[i] / 2.0f - cellTextWidth / 2.0f, yc - cellFontSize / 2.0f);
                        page.ShowText(cellValue);
                        page.EndText();

                        // Advance the value that tracks the left side of the current column.
                        if (i < numColumnsInTable)
                        {
                            xcLeft += columnWidths[i];
                        }
                    }
                }

                Console.WriteLine(numDataRowsThisPage + " rows this page");

                // Advance the top row of the page.
                topRowOfPage += numDataRowsThisPage;

                // Write the footer.
                PdfHelper.WriteFooter(page, document, pageNumber, footerFont);

                // Advance the page number.
                pageNumber++;
            }

            // Return the number of pages.
            return numPages;
        }

        /// <summary>
        /// Parse a string to an array of lines that fit in a specified width, 
        /// given the font and font size properties of a specified HPdfPage.
        /// </summary>
        /// <param name="text">String to be parsed</param>
        /// <param name="width">Width within which each line must fit</param>
        /// <param name="page">HPdfPage containing font and font size properties</param>
        /// <returns>String array containing parsed lines</returns>
        private static string[] WrapText(string text, float width, HPdfPage page)
        {
            List<string> headingsList = new List<string>();
            char[] separator = new char[1];
            separator[0] = ' ';
            string[] words = text.Split(separator);
            int numWords = words.Length;
            int firstWord = 0;
            int lastWord = numWords - 1;
            bool done = false;
            string line = "";
            while (!done)
            {
                line = StringUtil.MakeString(words, firstWord, lastWord);
                if (page.TextWidth(line) > width)
                {
                    if (lastWord > firstWord)
                    {
                        lastWord--;
                    }
                    else if (firstWord == lastWord)
                    {
                        // Store current word even though it doesn't fit
                        headingsList.Add(line);
                        if (lastWord < numWords - 1)
                        {
                            firstWord = lastWord + 1;
                            lastWord = numWords - 1;
                        }
                        else
                        {
                            done = true;
                        }
                    }
                }
                else
                {
                    headingsList.Add(line);
                    if (lastWord < numWords - 1)
                    {
                        firstWord = lastWord + 1;
                        lastWord = numWords - 1;
                    }
                    else
                    {
                        done = true;
                    }
                }
            }
            string[] headingLines = headingsList.ToArray();
            return headingLines;
        }
    }
}
