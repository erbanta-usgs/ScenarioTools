using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using HPdf;
using ScenarioTools.DataClasses;
using ScenarioTools.PdfWriting;

namespace ScenarioTools.ImageProcessing
{
    public class GraphImageWriter
    {
        private const int TITLE_TOP_BUFFER = 50;

        private const int GRAPH_TOP_BUFFER = 0;
        private const int GRAPH_V_BUFFER = 10;
        private const int GRAPH_LEFT_BUFFER = 15;
        private const float POINT_MARKER_SIZE = 4.5f;
        private const double BUFFER_PIXELS_DOMAIN = 10;
        private const double BUFFER_PIXELS_RANGE = 10;
        private const int TICK_MARK_LENGTH = 5;

        private const int BUFFER_LABELS_X_AXIS = -20;
        private const int SIDE_BUFFER = 100;
        private const int TOP_BUFFER = 50;
        private const int V_BUFFER_TITLE_TO_FIRST_GRAPH = 30;
        private const int V_BUFFER_BETWEEN_GRAPHS = 30;
        private const float X_AXIS_LABEL_TITLE_BUFFER = 4.0f;
        private const float Y_LABEL_GRAPH_BUFFER = 2.0f;
        private const string Y_AXIS_LABEL = "";

        //public static void MakeGraph(ReportElementGraph reportElement)
        {
            // The title is the report element name.
        }
        public static int WriteGraph(System.Drawing.Graphics g, SADocument document, ReportElementGraph reportElement, int pageNumber, int width, int height, string title)
        {
            // Set the title font.
            Font titleFont = new Font(FontFamily.GenericMonospace, 15.0f);
            g.DrawString(title, titleFont, Brushes.Black, new PointF(width / 2.0f, TITLE_TOP_BUFFER / 2.0f));

            // determine the height of the legend.
            int legendHeight = 100;

            // Determine the height available for the graph.
            int graphHeight = height - TITLE_TOP_BUFFER - legendHeight;
            int graphWidth = width - 2 * SIDE_BUFFER;
            int graphTop = TITLE_TOP_BUFFER;
            
            // Draw the graph.
            drawGraph(g, SIDE_BUFFER, graphTop, graphWidth, graphHeight, reportElement);

            // Draw the legend.
            //float legendHeight = drawLegend(pdf, page, page.GetWidth(), top, reportElement);

            // Return the number of pages.
            return 1;
        }
        private static float drawLegend(HPdfDoc pdf, HPdfPage page, float pageWidth, float top, IReportElement reportElement)
        {
            const float BUFFER_V_INTERNAL = 5.0f;
            const float BUFFER_H_INTERNAL = 4.0f;
            const float BUFFER_V_EDGES = 5.0f;
            const float BUFFER_H_EDGES = 5.0f;
            const float LABEL_HEIGHT = 12.0f;
            const float ICON_WIDTH = 20.0f;

            // Determine the width of the widest label.
            float maxLabelWidth = 0;
            int numEntries = reportElement.NumDataSeries;
            for (int i = 0; i < numEntries; i++)
            {
                string name = reportElement.GetDataSeries(i).Name;
                maxLabelWidth = Math.Max(maxLabelWidth, page.TextWidth(name));
            }

            // Calculate the table width.
            float tableWidth = BUFFER_H_EDGES * 2 + BUFFER_H_INTERNAL + ICON_WIDTH + maxLabelWidth;

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
                DrawString(page, reportElement.GetDataSeries(i).Name, xc, yc);
            }

            // Draw the box.
            page.SetLineWidth(1.0f);
            page.Rectangle(tableLeft, tableBottom, tableWidth, tableHeight);
            page.Stroke();

            // Return the legend height.
            return tableHeight;
        }
        private static void drawGraph(System.Drawing.Graphics g, int left, int top, int width, int height, ReportElementGraph reportElement)
        {
            /*
            // This is the space that will be occupied by the y-axis labels.
            int yLabelsSpace = 0;

            // Get the domain and range of the data series.
            DateTime startDate = reportElement.DateRangeStart;
            DateTime endDate = reportElement.DateRangeEnd;
            float rangeMin = reportElement.ValueRangeMin;
            float rangeMax = reportElement.ValueRangeMax;
            float bufferedMin = rangeMin - (rangeMax - rangeMin) / 20.0f;
            float bufferedMax = rangeMax + (rangeMax - rangeMin) / 20.0f;

            // Draw the labels on the x-axis.
            //float xAxisLabelHeight = drawLabelsX(page, graphLeft, graphWidth, graphBottom, startDate, endDate, startDate, endDate);
            //graphBottom += xAxisLabelHeight;
            //graphHeight -= xAxisLabelHeight;

            // Draw the data series.
            for (int i = 0; i < reportElement.NumDataSeries; i++)
            {
                // Get the data series from the report element.
                DataSeries dataSeries = reportElement.GetDataSeries(i);

                // If specified, draw a line series.
                if (!ColorUtil.ColorEquals(dataSeries.LineSeriesColor, Color.Transparent))
                {
                    // Get the dataset synchronously.
                    object dataset = dataSeries.GetDataSynchronous();

                    // If the dataset is not null, draw it appropriately.
                    if (dataset != null)
                    {
                        // This is the case for a scalar value.
                        if (dataset is float)
                        {

                        }

                        // This is the case for a time series.
                        else if (dataset is TimeSeries)
                        {
                            drawLineSeries(page, (TimeSeries)dataset, startDate, endDate, bufferedMin, bufferedMax, graphLeft, graphBottom, graphWidth,
                                graphHeight, dataSeries.LineSeriesColor);
                        }
                    }
                }

                // If specified, draw a point series.
                if (!ColorUtil.ColorEquals(dataSeries.PointSeriesColor, Color.Transparent))
                {
                    // Get the dataset synchronously.
                    object dataset = dataSeries.GetDataSynchronous();

                    // If the dataset is not null, draw it appropriately.
                    if (dataset != null)
                    {
                        // This is the case for a scalar value.
                        if (dataset is float)
                        {

                        }

                        // This is the case for a time series.
                        else if (dataset is TimeSeries)
                        {
                            drawPointSeries(page, (TimeSeries)dataset, startDate, endDate, bufferedMin, bufferedMax, graphLeft, graphBottom,
                                graphWidth, graphHeight, dataSeries.PointSeriesColor);
                        }
                    }

                }

            }

            // Set the stroke back to black.
            page.SetRGBStroke(0.0f, 0.0f, 0.0f);

            // Draw the tick marks on the x-axis.
            drawTickMarksX(page, graphLeft, graphWidth, graphBottom, TICK_MARK_LENGTH, startDate, endDate, startDate, endDate);
            drawTickMarksX(page, graphLeft, graphWidth, graphBottom + graphHeight - TICK_MARK_LENGTH, TICK_MARK_LENGTH, startDate, endDate, startDate,
                endDate);

            // Draw the tick marks on the y-axis.
            drawTickMarksY(page, graphBottom, graphHeight, graphLeft, TICK_MARK_LENGTH, bufferedMin, bufferedMax, rangeMin, rangeMax);
            drawTickMarksY(page, graphBottom, graphHeight, graphLeft + graphWidth - TICK_MARK_LENGTH, TICK_MARK_LENGTH, bufferedMin, bufferedMax,
                rangeMin, rangeMax);

            // Draw the labels to the left of the y-axis.
            drawLabelsY(page, graphBottom, graphHeight, graphLeft, bufferedMin, bufferedMax, rangeMin, rangeMax);

            // Draw the plot outline.
            page.SetLineWidth(1);
            page.Rectangle(graphLeft, graphBottom, graphWidth, graphHeight);
            page.Stroke();
             * */
        }
        private static void drawLabelsY(HPdfPage page, float graphBottom, float graphHeight, float graphLeft, float rangeMin, float rangeMax,
            float firstTick, float lastTick)
        {
            /*
            // Get the y tick values.
            double[] yTickValues = getTickValues(firstTick, lastTick);

            // Set the font.
            float labelHeight = 10.0f;
            page.SetFontAndSize(fontHelvetica, labelHeight);

            // Draw the value labels.
            float maxLabelWidth = 0.0f;
            int[] labelIndices = { 0, (int)Math.Floor(yTickValues.Length / 2.0), yTickValues.Length - 1 };
            for (int i = 0; i < labelIndices.Length; i++)
            {
                // Get the string representation of the number.
                double tickValue = yTickValues[labelIndices[i]];
                string labelString = tickValue.ToString("0.00");

                // Get the width of the number and determine the coordinates.
                float labelWidth = page.TextWidth(labelString);
                maxLabelWidth = Math.Max(labelWidth, maxLabelWidth);
                float relativeY = (float)getRelativeDoubleValue(tickValue, rangeMin, rangeMax);
                float xc = graphLeft - labelWidth - Y_LABEL_GRAPH_BUFFER;
                float yc = graphBottom + relativeY * graphHeight - labelHeight / 2.0f;

                // Draw the text.
                page.BeginText();
                page.MoveTextPos(xc, yc);
                page.ShowText(labelString);
                page.EndText();
            }

            // Draw the axis title.
            float labelTitleHeight = 10.0f;
            float angle = (float)(90.0f / 180.0f * Math.PI);
            page.SetFontAndSize(fontHelvetica, labelTitleHeight);
            float labelTitleWidth = page.TextWidth(Y_AXIS_LABEL);
            page.BeginText();
            float xcTitle = graphLeft - maxLabelWidth - labelTitleHeight;
            float ycTitle = graphBottom + (graphHeight - labelTitleWidth) / 2.0f;
            page.SetTextMatrix((float)Math.Cos(angle), (float)Math.Sin(angle), -(float)Math.Sin(angle), (float)Math.Cos(angle), xcTitle, ycTitle);
            page.ShowText(Y_AXIS_LABEL);
            page.EndText();
             * */
        }
        private static float drawLabelsX(HPdfPage page, float graphLeft, float graphWidth, float tickBottom, DateTime startDate, DateTime endDate,
            DateTime firstTick, DateTime lastTick)
        {

            return 1.0f;
            /*
            // Draw "date," centered.
            string dateTitleString = "date";

            // Determine the width and height of the text.
            float dateTitleTextHeight = 10.0f;
            page.SetFontAndSize(fontHelvetica, dateTitleTextHeight);
            float dateTitleTextWidth = page.TextWidth(dateTitleString);

            // Determine the x and y coordinates.
            float xcTitle = graphWidth / 2.0f + graphLeft - dateTitleTextWidth / 2.0f;
            float ycTitle = tickBottom - 4;

            page.BeginText();
            page.MoveTextPos(xcTitle, ycTitle);
            page.ShowText(dateTitleString);
            page.EndText();

            // Get the array of tick dates.
            DateTime[] tickDates = getTickDates(firstTick, lastTick);

            // Draw the first, middle, and last dates.
            int[] indices = { 0, (int)Math.Floor(tickDates.Length / 2.0), tickDates.Length - 1 };
            float dateTextHeight = 8;
            for (int i = 0; i < indices.Length; i++)
            {
                // Get the date string.
                string dateString = DateUtil.dateToStringMMDDCCYYFormat(tickDates[indices[i]], "/");

                // Determine the width and height of the text.
                page.SetFontAndSize(fontHelvetica, dateTextHeight);
                float dateTextWidth = page.TextWidth(dateString);

                // Determine the x and y coordinates.
                float relativeX = getRelativeDateValue(tickDates[indices[i]], startDate, endDate);
                float xc = relativeX * graphWidth + graphLeft - dateTextWidth / 2.0f;
                float yc = tickBottom - BUFFER_LABELS_X_AXIS - dateTitleTextHeight;

                // Draw the text.
                page.BeginText();
                page.MoveTextPos(xc, yc);
                page.ShowText(dateString);
                page.EndText();
            }

            return dateTitleTextHeight + X_AXIS_LABEL_TITLE_BUFFER + dateTextHeight;
             * */
        }
        private static void drawTickMarksX(HPdfPage page, float graphLeft, float graphWidth, float tickBottom, float tickLength, DateTime firstTick,
            DateTime lastTick, DateTime startDate, DateTime endDate)
        {
            // Set the line width and cap.
            page.SetLineWidth(0.4f);
            page.SetLineCap(HPdfLineCap.HPDF_BUTT_END);

            // Make the array of tick dates.
            DateTime[] tickDates = getTickDates(firstTick, lastTick);

            // Draw the ticks.
            for (int i = 0; i < tickDates.Length; i++)
            {
                double relativeX = getRelativeDateValue(tickDates[i], startDate, endDate);
                float xc = (float)(relativeX * graphWidth + graphLeft);
                page.MoveTo(xc, tickBottom);
                page.LineTo(xc, tickBottom + tickLength);
                page.Stroke();
            }
        }
        private static void drawTickMarksY(HPdfPage page, float graphTop, float graphHeight, float tickLeft, float tickLength, float rangeMin,
            float rangeMax, float firstTick, float lastTick)
        {
            // Set the line width and cap.
            page.SetLineWidth(0.4f);
            page.SetLineCap(HPdfLineCap.HPDF_BUTT_END);

            // Make the array of tick values.
            double[] tickDates = getTickValues(firstTick, lastTick);

            // Draw the ticks.
            for (int i = 0; i < tickDates.Length; i++)
            {
                double relativeY = getRelativeDoubleValue(tickDates[i], rangeMin, rangeMax);
                float yc = (float)((1.0 - relativeY) * graphHeight + graphTop);
                page.MoveTo(tickLeft, yc);
                page.LineTo(tickLeft + tickLength, yc);
                page.Stroke();
            }
        }
        private static double[] getTickValues(double firstTick, double lastTick)
        {
            // Make the array for the tick values.
            double[] tickValues = new double[11];

            double spanSize = (lastTick - firstTick) / 10.0;
            for (int i = 0; i < 11; i++)
            {
                tickValues[i] = firstTick + spanSize * i;
            }

            // Return the result.
            return tickValues;
        }
        private static DateTime[] getTickDates(DateTime firstTick, DateTime lastTick)
        {
            // Make the list for the tick dates.
            List<DateTime> tickDates = new List<DateTime>();

            // If the range is less than a year, add daily dates.
            long spanSize = (lastTick.Ticks - firstTick.Ticks) / 10;
            for (int i = 0; i < 11; i++)
            {
                tickDates.Add(new DateTime(firstTick.Ticks + spanSize * i));
            }

            // If the range is less than 5 years, add monthly dates.

            // Otherwise, add yearly dates.

            // Return the result as an array.
            return tickDates.ToArray();
        }
        private static void drawLineSeries(HPdfPage page, TimeSeries series, DateTime startDate, DateTime endDate, float rangeMin, float rangeMax,
            float graphLeft, float graphTop, float graphWidth, float graphHeight, Color c)
        {
            // Set the stroke.
            float lineWidth = 0.1f;
            page.SetRGBStroke(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f);
            page.SetLineWidth(lineWidth);
            page.SetLineCap(HPdfLineCap.HPDF_ROUND_END);
            page.SetLineJoin(HPdfLineJoin.HPDF_ROUND_JOIN);

            // Iterate along the series and plot all points that are in the specified domain and
            // range.
            for (int i = 1; i < series.Length; i++)
            {
                // Get the sample from the series.
                TimeSeriesSample sample0 = series[i - 1];
                TimeSeriesSample sample1 = series[i];

                if (sample0.Date >= startDate && sample1.Date <= endDate)
                {
                    // Get the relative x and y coordinate of the sample.
                    double relativeX0 = getRelativeDateValue(sample0.Date, startDate, endDate);
                    double relativeY0 = getRelativeDoubleValue(sample0.Value, rangeMin, rangeMax);
                    double relativeX1 = getRelativeDateValue(sample1.Date, startDate, endDate);
                    double relativeY1 = getRelativeDoubleValue(sample1.Value, rangeMin, rangeMax);

                    // Only plot the values if the relative values are within the graph.
                    float xc0 = (float)(relativeX0 * graphWidth + graphLeft);
                    float yc0 = (float)(relativeY0 * graphHeight + graphTop);
                    float xc1 = (float)(relativeX1 * graphWidth + graphLeft);
                    float yc1 = (float)(relativeY1 * graphHeight + graphTop);

                    page.MoveTo(xc0, yc0);
                    page.LineTo(xc1, yc1);
                    page.Stroke();
                }
            }
        }
        private static void drawPointSeries(HPdfPage page, TimeSeries series, DateTime startDate, DateTime endDate, float rangeMin, float rangeMax,
            float graphLeft, float graphTop, float graphWidth, float graphHeight, Color c)
        {
            // Set the stroke.
            page.SetLineWidth(0.05f);
            page.SetRGBStroke(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f);

            // Determine the marker size.
            float markerRadius = POINT_MARKER_SIZE / 2.0f;

            // Iterate along the series and plot all points that are in the specified domain and
            // range.
            for (int i = 0; i < series.Length; i++)
            {
                // Get the sample from the series.
                TimeSeriesSample sample = series[i];

                if (sample.Date >= startDate && sample.Date <= endDate)
                {

                    // Get the relative x and y coordinate of the sample.
                    float relativeX = getRelativeDateValue(sample.Date, startDate, endDate);
                    float relativeY = getRelativeDoubleValue(sample.Value, rangeMin, rangeMax);

                    // Only plot the values if the relative values are within the graph.
                    if (relativeX >= 0.0 && relativeX <= 1.0 && relativeY >= 0.0 && relativeY >= 0.0)
                    {
                        float xC = relativeX * graphWidth + graphLeft;
                        float yC = relativeY * graphHeight + graphTop;

                        page.Circle(xC, yC, markerRadius);
                        page.Stroke();
                    }
                }
            }
        }
        private static float getRelativeDateValue(DateTime date, DateTime startDate, DateTime endDate)
        {
            // This method returns a value from zero to one(*), scaled across the range.
            float scaledValue = (float)(date.Ticks - startDate.Ticks) /
            (float)(endDate.Ticks - startDate.Ticks);

            return scaledValue;
        }
        private static float getRelativeDoubleValue(double value, float rangeMin, float rangeMax)
        {
            // This method returns a value from zero to one(*), scaled across the range. An
            // output value of 0 indicates that the provided value is at the minimum of the 
            // range, and an output of 1 indicates that the provided value is at the maximum
            // of the range.
            float scaledValue = (float)((value - rangeMin) / (rangeMax - rangeMin));

            return scaledValue;
        }
        private void drawTable(HPdfPage page, string[] labels, string[] values, float graphRight, float graphBottom, float graphHeight)
        {
            /*
            const float BUFFER_V_INTERNAL = 5.0f;
            const float BUFFER_H_INTERNAL = 4.0f;
            const float BUFFER_V_EDGES = 5.0f;
            const float BUFFER_H_EDGES = 5.0f;
            const float BUFFER_GRAPH_TABLE = 10.0f;
            const float labelHeight = 8.0f;
            //page.SetFontAndSize(fontCustom, labelHeight);
            page.SetFontAndSize(fontHelvetica, labelHeight);

            // Determine the height of the table.
            float tableHeight = labels.Length * labelHeight + (labels.Length - 1) * BUFFER_V_INTERNAL + 2 * BUFFER_V_EDGES;

            // Draw the labels.
            float tableLeft = graphRight + BUFFER_GRAPH_TABLE;
            float tableBottom = graphBottom + (graphHeight - tableHeight) / 2.0f;
            float xc = tableLeft + BUFFER_H_EDGES;
            float maxLabelWidth = 0.0f;
            for (int i = 0; i < labels.Length; i++)
            {
                float yc = tableBottom + (labels.Length - i - 1) * (labelHeight + BUFFER_V_INTERNAL) + BUFFER_V_EDGES;
                DrawString(page, labels[i], xc, yc);
                maxLabelWidth = Math.Max(maxLabelWidth, getLabelWidth(page, labels[i]));
            }

            // Draw the entries.
            xc += maxLabelWidth + BUFFER_H_INTERNAL;
            float maxValueWidth = 0.0f;
            for (int i = 0; i < values.Length; i++)
            {
                float yc = tableBottom + (labels.Length - i - 1) * (labelHeight + BUFFER_V_INTERNAL) + BUFFER_V_EDGES;
                maxValueWidth = Math.Max(maxValueWidth, page.TextWidth(values[i]));
                page.BeginText();
                page.MoveTextPos(xc, yc);
                page.ShowText(values[i]);
                page.EndText();
            }

            // Draw the box.
            page.SetLineWidth(1.0f);
            float tableWidth = maxLabelWidth + maxValueWidth + BUFFER_H_EDGES * 2 + BUFFER_H_INTERNAL;
            page.Rectangle(tableLeft, tableBottom, tableWidth, tableHeight);
            page.Stroke();
             * */
        }

        public static void DrawString(HPdfPage page, string s, float xc, float yc)
        {
            /*
            // Get the sections of the string.
            string[] sections = getSections(s);

            // Make a stack for symbols and super/subscripts.
            List<bool> symbols = new List<bool>();
            List<bool> scripts = new List<bool>();  // true means superscript, false means subscript

            // Iterate through the elements. Change the writer state according to the tags, and write the text elements.
            for (int i = 0; i < sections.Length; i++)
            {
                string section = sections[i];

                // If this is a tag, process it and update the settings.
                if (section.StartsWith("<"))
                {
                    // If this is a close tag, pop from the appropriate stack and update the settings.
                    if (section.StartsWith("</"))
                    {
                        section = section.ToLower();
                        if (section.Equals("</symbol>"))
                        {
                            symbols.RemoveAt(symbols.Count - 1);
                        }
                        else
                        {
                            scripts.RemoveAt(scripts.Count - 1);
                        }
                    }

                    // If this is an open tag, push it on the appropriate stack and update the settings.
                    else
                    {
                        // Push the tag to the appropriate 
                        section = section.ToLower();
                        if (section.Equals("<symbol>"))
                        {
                            symbols.Add(true);
                        }
                        else
                        {
                            scripts.Add(section.Equals("<super>"));
                        }
                    }

                    // Update the settings.
                    //HPdfFont font = symbols.Count > 0 ? fontCustom : fontHelvetica;
                    HPdfFont font = fontHelvetica;
                    float fontSize = scripts.Count > 0 ? 5.0f : 9.0f;
                    page.SetFontAndSize(font, fontSize);

                    // Here, update the position.
                    float offset = scripts.Count == 0 ? 0.0f : (scripts[0].Equals("<super>") ? 2.0f : -2.0f);
                    page.SetTextRaise(offset);

                }

                // Otherwise, draw the string.
                else
                {
                    Console.WriteLine("wrote: " + section);
                    page.BeginText();
                    page.MoveTextPos(xc, yc);
                    page.ShowText(section);
                    page.EndText();

                    xc += page.TextWidth(section);
                }
            }
             * */
        }
        private static string[] getSections(string s)
        {
            // Break the string into sections.
            List<string> sections = new List<string>();
            StringBuilder currentSection = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                // If this is an open character, dump the last section and start a new section.
                if (s[i].Equals('<'))
                {
                    if (!currentSection.ToString().Equals(""))
                    {
                        sections.Add(currentSection.ToString());
                    }
                    currentSection = new StringBuilder();
                    currentSection.Append(s[i]);
                }
                else if (s[i].Equals('>'))
                {
                    currentSection.Append(s[i]);
                    if (!currentSection.ToString().Equals(""))
                    {
                        sections.Add(currentSection.ToString());
                    }
                    currentSection = new StringBuilder();
                }
                // Otherwise, append to the current section.
                else
                {
                    currentSection.Append(s[i]);
                }
            }

            // If the current section is non-empty, append it.
            if (!currentSection.ToString().Equals(""))
            {
                sections.Add(currentSection.ToString());
            }

            // Show the sections.
            for (int i = 0; i < sections.Count; i++)
            {
                //Console.WriteLine("section " + i + ": " + sections[i]);
            }

            // Return the result as a string array.
            return sections.ToArray();
        }
        private float getLabelWidth(HPdfPage page, string s)
        {
            return 1.0f;
            /*
            // Get the sections of the string.
            string[] sections = getSections(s);

            // Make a stack for symbols and super/subscripts.
            List<bool> symbols = new List<bool>();
            List<bool> scripts = new List<bool>();  // true means superscript, false means subscript

            // this is the width accumulator.
            float width = 0.0f;

            // Iterate through the elements. Change the writer state according to the tags, and get the widths of the text elements.
            for (int i = 0; i < sections.Length; i++)
            {
                string section = sections[i];

                // If this is a tag, process it and update the settings.
                if (section.StartsWith("<"))
                {
                    // If this is a close tag, pop from the appropriate stack and update the settings.
                    if (section.StartsWith("</"))
                    {
                        section = section.ToLower();
                        if (section.Equals("</symbol>"))
                        {
                            symbols.RemoveAt(symbols.Count - 1);
                        }
                        else
                        {
                            scripts.RemoveAt(scripts.Count - 1);
                        }
                    }

                    // If this is an open tag, push it on the appropriate stack and update the settings.
                    else
                    {
                        // Push the tag to the appropriate 
                        section = section.ToLower();
                        if (section.Equals("<symbol>"))
                        {
                            symbols.Add(true);
                        }
                        else
                        {
                            scripts.Add(section.Equals("<super>"));
                        }
                    }

                    // Update the settings.
                    //HPdfFont font = symbols.Count > 0 ? fontCustom : fontHelvetica;
                    HPdfFont font = fontHelvetica;
                    float fontSize = scripts.Count > 0 ? 5.0f : 9.0f;
                    page.SetFontAndSize(font, fontSize);
                }

                // Otherwise, get the width of the string.
                else
                {
                    width += page.TextWidth(section);
                }
            }

            return width;
             * */
        }
    }
}
