using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;

using HPdf;
using ScenarioTools.Reporting;

namespace ScenarioTools.PdfWriting
{
    class ChartWriter
    {
        #region Constants
        private const int CHART_TOP_BUFFER = 0;
        private const int CHART_V_BUFFER = 10;
        private const int CHART_LEFT_BUFFER = 15;
        private const float POINT_MARKER_SIZE = 4.5f;
        private const double BUFFER_PIXELS_DOMAIN = 10;
        private const double BUFFER_PIXELS_RANGE = 10;
        private const int TICK_MARK_LENGTH = 5;

        private const int BUFFER_LABELS_X_AXIS = -20;
        private const int SIDE_BUFFER = 100;
        private const int TOP_BUFFER = 50;
        private const int V_BUFFER_TITLE_TO_CHART = 20;
        private const int V_BUFFER_TITLE_TO_FIRST_CHART = 30;
        private const int V_BUFFER_BETWEEN_CHARTS = 30;
        private const float X_AXIS_LABEL_TITLE_BUFFER = 4.0f;
        private const float Y_LABEL_CHART_BUFFER = 2.0f;
        private const string Y_AXIS_LABEL = "";
        #endregion Constants

        public static int WriteChart(HPdfDoc pdf, SADocument document, ReportElementChart reportElement, 
                                     int pageNumber, HPdfFont fontHelvetica, HPdfFont fontHelveticaBold)
        {

            // Create a new page.
            HPdfPage page = pdf.AddPage();
            int startPageNumber = pageNumber;

            // Get the width and height of the page.
            float pageWidth = page.GetWidth();
            float pageHeight = page.GetHeight();

            // Set the title font.
            page.SetFontAndSize(fontHelvetica, 24.0f);

            // Draw the title.
            string chartTitle = reportElement.Name;
            float tw = page.TextWidth(chartTitle);

            // Determine the height available for the graphs.
            float graphsTop = (pageHeight - (TOP_BUFFER + 24 + V_BUFFER_TITLE_TO_FIRST_CHART));
            float graphsBottom = TOP_BUFFER;
            float availableHeight = graphsTop - graphsBottom;
            float graphHeight = availableHeight / 3;
            float graphWidth = pageWidth - 2 * SIDE_BUFFER;

            // Draw the chart.
            drawChart(pdf, page, SIDE_BUFFER, graphsTop, graphWidth, graphHeight, chartTitle, reportElement, fontHelvetica);

            // Draw the legend.
            float top = graphsTop - graphHeight - 40;
            float legendHeight = drawLegend(pdf, page, page.GetWidth(), top, reportElement, fontHelvetica);
            top -= legendHeight + 60;

            // Draw the metadata.
            int metadataHeight = PdfHelper.DrawMetadata(pdf, ref page, top, reportElement, fontHelvetica, fontHelveticaBold, document, ref pageNumber);

            // Write the footer.
            PdfHelper.WriteFooter(page, document, pageNumber, fontHelveticaBold);

            // Return the number of pages.
            return 1 + pageNumber - startPageNumber;
        }

        private static void drawChart(HPdfDoc pdf, HPdfPage page, float left, float top, 
                                      float fWidth, float fHeight, string graphTitle,
                                      ReportElementChart reportElement, HPdfFont fontHelvetica)
        {

            // Set the title font.
            HPdfFont font = pdf.GetFont("Helvetica", null);
            page.SetFontAndSize(font, 16.0f);
            float titleHeight = 16.0f; // (1.f/72.f) * (font.GetCapHeight + font.GetDescent);

            // Draw the title.
            float tw = page.TextWidth(graphTitle);
            page.BeginText();
            page.MoveTextPos((page.GetWidth() - tw) / 2, top);
            page.ShowText(graphTitle);
            page.EndText();

            // This is the space that will be occupied by the y-axis labels.
            int yLabelsSpace = 0;

            // Determine the bounds of the graph.
            float graphHeight = fHeight - 2 * CHART_TOP_BUFFER - CHART_V_BUFFER - titleHeight;
            float graphLeft = left + CHART_LEFT_BUFFER + yLabelsSpace;
//            float graphBottom = top - (CHART_TOP_BUFFER + CHART_V_BUFFER + graphHeight + titleHeight + V_BUFFER_TITLE_TO_CHART);
            float graphBottom = top - (CHART_TOP_BUFFER + CHART_V_BUFFER + graphHeight + V_BUFFER_TITLE_TO_CHART);
            float graphWidth = fWidth - 2 * CHART_LEFT_BUFFER - yLabelsSpace;

            // Draw the chart to the PDF page
            Chart chart = reportElement.Chart;
            Image image = reportElement.GetImage();
            Bitmap bitmap = new Bitmap(image);
            ImageFormat imageFormat = ImageFormat.Bmp;

            #region Process image byte data
            // Byte array from MS Chart contains a header plus 4 bytes per pixel
            byte[] imageData = ConvertImageToByteArray(bitmap, imageFormat);

            // Remove bitmap header data from imageData array
            int len = imageData.GetLength(0);
            int width = reportElement.Chart.Width;
            int height = reportElement.Chart.Height;
            // MS Chart includes 4 bytes for each pixel (but uses only 3 for RGB)
            int dim = width * height * 4;  
            int bytesToRemove = len - dim;
            byte[] rawData = new byte[dim];
            for (int i = 0; i < dim; i++)
            {
                rawData[i] = imageData[i + bytesToRemove];
            }

            // Allocate byte array with three bytes per pixel
            int rgbDim = 3 * width * height;
            byte[] rgbData = new byte[rgbDim];
            int ii = 0;
            int kk;

            // Adapt byte order from MS Chart order to HPdf order
            // Reverse the row order
            for (int row = height-1; row >= 0; row--) 
            {
                // Column order is correct as is
                for (int col = 0; col < width; col++) 
                {
                    // Reverse the component (RGB) order
                    for (int comp = 2; comp >= 0; comp--) 
                    {
                        kk = 3 * (row*width + col) + comp;
                        rgbData[kk] = rawData[ii];
                        ii++;
                    }
                    ii++; // Skip every 4th byte in rawData array
                }
            }
            #endregion Process image byte data

            uint bitsPerComponent = 8; // Valid values are: 1, 2, 4, 8
            HPdfColorSpace colorSpace = HPdfColorSpace.HPDF_CS_DEVICE_RGB;
            HPdfImage hpdfImage = pdf.LoadRawImageFromMem(rgbData, width, 
                                      height, colorSpace, bitsPerComponent);
            IntPtr p = hpdfImage.GetHandle();
            float x = left;
            float y = top - fHeight;
            y = graphBottom; // experiment
            try
            {
                page.DrawImage(hpdfImage, x, y, fWidth, fHeight);
            }
            catch (Exception e)
            {
            }
        }

        private static float drawLegend(HPdfDoc pdf, HPdfPage page, float pageWidth, float top, 
                                        IReportElement reportElement, HPdfFont fontHelvetica)
        {
            //float legendHeight = 0.0f;
            //return legendHeight;
            const float BUFFER_V_INTERNAL = 5.0f;
            const float BUFFER_H_INTERNAL = 4.0f;
            const float BUFFER_V_EDGES = 5.0f;
            const float BUFFER_H_EDGES = 5.0f;
            const float LABEL_HEIGHT = 12.0f;
            const float ICON_WIDTH = 20.0f;

            // Determine the width of the widest label.
            float maxLabelWidth = 0;
            int numEntries = reportElement.NumDataSeries;
            page.SetFontAndSize(fontHelvetica, LABEL_HEIGHT);
            for (int i = 0; i < numEntries; i++)
            {
                string name = reportElement.GetDataSeries(i).Name;
                maxLabelWidth = Math.Max(maxLabelWidth, page.TextWidth(name));
            }

            // Calculate the table width.
            float tableWidth = BUFFER_H_EDGES * 2 + BUFFER_H_INTERNAL + ICON_WIDTH + maxLabelWidth;

            //page.SetFontAndSize(fontCustom, labelHeight);
            page.SetFontAndSize(fontHelvetica, LABEL_HEIGHT);

            // Determine the height of the table.
            float tableHeight = numEntries * LABEL_HEIGHT + (numEntries - 1) * BUFFER_V_INTERNAL + 2 * BUFFER_V_EDGES;

            // Draw the icons.
            float tableLeft = (pageWidth - tableWidth) / 2.0f;
            float tableBottom = top - tableHeight;
            float xc = tableLeft + BUFFER_H_EDGES;
            for (int i = 0; i < numEntries; i++)
            {
                // Get the data series from the report element.
                DataSeries dataSeries = reportElement.GetDataSeries(i);

                // Calculate the base y coordinate.
                float yc = tableBottom + (numEntries - i - 1) * (LABEL_HEIGHT + BUFFER_V_INTERNAL) + BUFFER_V_EDGES;

                // If there is a line, draw it.
                if (!ColorUtil.ColorEquals(dataSeries.LineSeriesColor, Color.Transparent))
                {
                    float xc0 = xc;
                    float xc1 = xc0 + ICON_WIDTH;
                    float ycc = yc + LABEL_HEIGHT / 2.0f - 2.0f;

                    // Set the stroke.
                    Color c = dataSeries.LineSeriesColor;
                    float lineWidth = 0.1f;
                    page.SetRGBStroke(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f);
                    page.SetLineWidth(lineWidth);
                    page.SetLineCap(HPdfLineCap.HPDF_ROUND_END);
                    page.SetLineJoin(HPdfLineJoin.HPDF_ROUND_JOIN);

                    // Draw the line.
                    page.MoveTo(xc0, ycc);
                    page.LineTo(xc1, ycc);
                    page.Stroke();
                }

                // If there is a point, draw it.
                if (!ColorUtil.ColorEquals(dataSeries.PointSeriesColor, Color.Transparent))
                {
                    float markerRadius = POINT_MARKER_SIZE / 2.0f;
                    float xcc = xc + ICON_WIDTH / 2.0f;
                    float ycc = yc + LABEL_HEIGHT / 2.0f - 2.0f;

                    // Set the stroke.
                    Color c = dataSeries.PointSeriesColor;
                    float lineWidth = 0.1f;
                    page.SetRGBStroke(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f);
                    page.SetLineWidth(lineWidth);
                    page.SetLineCap(HPdfLineCap.HPDF_ROUND_END);
                    page.SetLineJoin(HPdfLineJoin.HPDF_ROUND_JOIN);

                    // Draw the circle.
                    page.Circle(xcc, ycc, markerRadius);
                    page.Stroke();
                }
            }

            // Set the stroke back to black.
            page.SetRGBStroke(0.0f, 0.0f, 0.0f);

            // Draw the labels.
            xc = tableLeft + BUFFER_H_EDGES + ICON_WIDTH + BUFFER_H_INTERNAL;
            for (int i = 0; i < numEntries; i++)
            {
                float yc = tableBottom + (numEntries - i - 1) * (LABEL_HEIGHT + BUFFER_V_INTERNAL) + BUFFER_V_EDGES;
                PdfHelper.DrawString(page, reportElement.GetDataSeries(i).Name, xc, yc, fontHelvetica);
            }

            // Draw the box.
            page.SetLineWidth(1.0f);
            page.Rectangle(tableLeft, tableBottom, tableWidth, tableHeight);
            page.Stroke();

            // Return the legend height.
            return tableHeight;
        }

        //private static int drawMetadata(HPdfDoc pdf, ref HPdfPage page, float top, IReportElement reportElement, 
        //                                HPdfFont fontHelvetica, HPdfFont fontHelveticaBold, SADocument document, 
        //                                ref int pageNumber)
        //{
        //    // Set the font and other constants.
        //    const float LABEL_HEIGHT_TITLE = 12.0f;
        //    const float LABEL_HEIGHT_META = 10.0f;
        //    const float LEFT_BUFFER = 60.0f;
        //    const float BUFFER_V = 2.0f;
        //    const float BOTTOM_TEXT = 150.0f;  // Triggers switch to new column or new page

        //    bool rightColumn = false;
        //    float startTop = top;

        //    float pageWidth = page.GetWidth();
        //    float pageHeight = page.GetHeight();
        //    float left;

        //    // Draw the metadata for each data series.
        //    for (int i = 0; i < reportElement.NumDataSeries; i++)
        //    {
        //        if (top < BOTTOM_TEXT) // was (i == 2)
        //        {
        //            if (rightColumn)
        //            {
        //                // Write "continued" note
        //                page.BeginText();
        //                left = page.GetWidth() / 2.0f;
        //                page.MoveTextPos(left, top);
        //                page.ShowText("(continued on next page)");
        //                page.EndText();

        //                // Write footer
        //                PdfHelper.WriteFooter(page, document, pageNumber, fontHelveticaBold);

        //                // Add new page and increment page number
        //                page = pdf.AddPage();
        //                pageNumber++;

        //                // Start at top of new page
        //                startTop = pageHeight - (TOP_BUFFER + 24 + V_BUFFER_TITLE_TO_CHART);
        //            }
        //            top = startTop;
        //            rightColumn = !rightColumn;
        //        }

        //        // Get the data series.
        //        DataSeries dataSeries = reportElement.GetDataSeries(i);

        //        // Draw the data series name.
        //        if (rightColumn)
        //        {
        //            left = page.GetWidth() / 2.0f;
        //        }
        //        else
        //        {
        //            left = LEFT_BUFFER;
        //        }
        //        page.SetFontAndSize(fontHelveticaBold, LABEL_HEIGHT_TITLE);
        //        DrawString(page, dataSeries.Name, left, top, fontHelvetica);

        //        // Decrement top.
        //        top -= LABEL_HEIGHT_TITLE + BUFFER_V;

        //        // Draw the data-provider metadata.
        //        page.SetFontAndSize(fontHelvetica, LABEL_HEIGHT_META);
        //        float maxWidth = left < pageWidth / 2.0f ? pageWidth / 2.0f - left - 20.0f : pageWidth - left - 20.0f;
        //        string[] providerMeta = dataSeries.DataProvider.GetMetadata();
        //        for (int j = 0; j < providerMeta.Length; j++)
        //        {
        //            int numLines = DrawStringSmart(page, providerMeta[j], left, top, maxWidth, LABEL_HEIGHT_META + BUFFER_V);
        //            top -= (LABEL_HEIGHT_META + BUFFER_V) * numLines;
        //        }
        //        top -= LABEL_HEIGHT_TITLE + BUFFER_V;
        //    }

        //    return 0;
        //}

        //public static void DrawString(HPdfPage page, string s, float xc, float yc, HPdfFont fontHelvetica)
        //{
        //    // Get the sections of the string.
        //    string[] sections = getSections(s);

        //    // Make a stack for symbols and super/subscripts.
        //    List<bool> symbols = new List<bool>();
        //    List<bool> scripts = new List<bool>();  // true means superscript, false means subscript

        //    // Iterate through the elements. Change the writer state according to the tags, and write the text elements.
        //    for (int i = 0; i < sections.Length; i++)
        //    {
        //        string section = sections[i];

        //        // If this is a tag, process it and update the settings.
        //        if (section.StartsWith("<"))
        //        {
        //            // If this is a close tag, pop from the appropriate stack and update the settings.
        //            if (section.StartsWith("</"))
        //            {
        //                section = section.ToLower();
        //                if (section.Equals("</symbol>"))
        //                {
        //                    symbols.RemoveAt(symbols.Count - 1);
        //                }
        //                else
        //                {
        //                    scripts.RemoveAt(scripts.Count - 1);
        //                }
        //            }

        //            // If this is an open tag, push it on the appropriate stack and update the settings.
        //            else
        //            {
        //                // Push the tag to the appropriate 
        //                section = section.ToLower();
        //                if (section.Equals("<symbol>"))
        //                {
        //                    symbols.Add(true);
        //                }
        //                else
        //                {
        //                    scripts.Add(section.Equals("<super>"));
        //                }
        //            }

        //            // Update the settings.
        //            //HPdfFont font = symbols.Count > 0 ? fontCustom : fontHelvetica;
        //            HPdfFont font = fontHelvetica;
        //            float fontSize = scripts.Count > 0 ? 5.0f : 9.0f;
        //            page.SetFontAndSize(font, fontSize);

        //            // Here, update the position.
        //            float offset = scripts.Count == 0 ? 0.0f : (scripts[0].Equals("<super>") ? 2.0f : -2.0f);
        //            page.SetTextRaise(offset);

        //        }

        //        // Otherwise, draw the string.
        //        else
        //        {
        //            //Console.WriteLine("wrote: " + section);
        //            page.BeginText();
        //            page.MoveTextPos(xc, yc);
        //            page.ShowText(section);
        //            page.EndText();

        //            xc += page.TextWidth(section);
        //        }
        //    }
        //}

        //public static int DrawStringSmart(HPdfPage page, string s, float xc, float yc, float maxWidth, float lineOffset)
        //{
        //    int numLines = 0;
        //    while (s.Length > 0)
        //    {
        //        float offset = numLines == 0 ? 0.0f : 10.0f;
        //        float maxWidthThisLine = maxWidth - offset;

        //        int numCharsThisLine = s.Length;
        //        while (page.TextWidth(s.Substring(0, numCharsThisLine)) > maxWidthThisLine && numCharsThisLine > 10)
        //        {
        //            numCharsThisLine--;
        //        }

        //        // Draw the text.
        //        page.BeginText();
        //        page.MoveTextPos(xc + offset, yc);
        //        string section = s.Substring(0, numCharsThisLine);
        //        page.ShowText(section);
        //        page.EndText();

        //        // Remove the printed characters.
        //        if (numCharsThisLine == s.Length)
        //        {
        //            s = "";
        //        }
        //        else
        //        {
        //            s = s.Substring(numCharsThisLine, s.Length - numCharsThisLine);
        //        }

        //        yc -= lineOffset;
        //        numLines++;
        //    }

        //    return numLines;
        //}

        //private static string[] getSections(string s)
        //{
        //    // Break the string into sections.
        //    List<string> sections = new List<string>();
        //    StringBuilder currentSection = new StringBuilder();
        //    for (int i = 0; i < s.Length; i++)
        //    {
        //        // If this is an open character, dump the last section and start a new section.
        //        if (s[i].Equals('<'))
        //        {
        //            if (!currentSection.ToString().Equals(""))
        //            {
        //                sections.Add(currentSection.ToString());
        //            }
        //            currentSection = new StringBuilder();
        //            currentSection.Append(s[i]);
        //        }
        //        else if (s[i].Equals('>'))
        //        {
        //            currentSection.Append(s[i]);
        //            if (!currentSection.ToString().Equals(""))
        //            {
        //                sections.Add(currentSection.ToString());
        //            }
        //            currentSection = new StringBuilder();
        //        }
        //        // Otherwise, append to the current section.
        //        else
        //        {
        //            currentSection.Append(s[i]);
        //        }
        //    }

        //    // If the current section is non-empty, append it.
        //    if (!currentSection.ToString().Equals(""))
        //    {
        //        sections.Add(currentSection.ToString());
        //    }

        //    // Show the sections.
        //    for (int i = 0; i < sections.Count; i++)
        //    {
        //        //Console.WriteLine("section " + i + ": " + sections[i]);
        //    }

        //    // Return the result as a string array.
        //    return sections.ToArray();
        //}

        private static byte[] ConvertImageToByteArray(System.Drawing.Image imageToConvert, ImageFormat formatOfImage) 
        { 
            // From: http://www.eggheadcafe.com/PrintSearchContent.asp?LINKID=799
            byte[] Ret; 

            try 
            { 
                using (MemoryStream ms = new MemoryStream()) 
                { 
                    imageToConvert.Save(ms,formatOfImage); 
                    Ret = ms.ToArray(); 
                } 
            } 
            catch (Exception ex) 
            { 
                throw;
            } 

            return Ret; 
        } 

        //When you are ready to convert the byte array back 
        //to an image, you can include the following code 
        //in your method. 

        //System.Drawing.Image newImage; 


        //using (MemoryStream ms = new MemoryStream(myByteArray,0,myByteArray.Length)) 
        //{ 

        //ms.Write(myByteArray,0,myByteArray.Length); 

        //newImage = Image.FromStream(ms,true); 

        //// work with image here. 

        //// You'll need to keep the MemoryStream open for 
        //// as long as you want to work with your new image. 

        //}

    }
}
