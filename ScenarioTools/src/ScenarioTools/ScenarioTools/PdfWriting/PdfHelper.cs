using System;
using System.Collections.Generic;
using System.Text;

using HPdf;

using ScenarioTools.Reporting;

namespace ScenarioTools.PdfWriting
{
    public class PdfHelper
    {
        private const float FOOTER_FONT_SIZE = 12.0f;

        // For Scenario Manager Export Report
        // PDF units are 1/72 inch
        public const float PAGE_WIDTH = 612;    // 612 = 8.5 inches
        public const float PAGE_HEIGHT = 792;   // 792 = 11 inches

        public const float MARGIN_TOP = 54;     // 0.75 inch
        public const float MARGIN_BOTTOM = 54;  // 0.75 inch
        public const float MARGIN_LEFT = 72;    // 1.0 inch
        public const float MARGIN_RIGHT = 54;   // 0.75 inch

        private const int TOP_BUFFER = 50;
        private const int V_BUFFER_TITLE_TO_CHART = 20;

        public static int DrawMetadata(HPdfDoc pdf, ref HPdfPage page, float top, IReportElement reportElement,
                                HPdfFont fontHelvetica, HPdfFont fontHelveticaBold, SADocument document,
                                ref int pageNumber)
        {
            // Set the font and other constants.
            const float LABEL_HEIGHT_TITLE = 12.0f;
            const float LABEL_HEIGHT_META = 10.0f;
            const float LEFT_BUFFER = 60.0f;
            const float BUFFER_V = 2.0f;
            const float BOTTOM_TEXT = 150.0f;  // Triggers switch to new column or new page

            bool rightColumn = false;
            float startTop = top;

            float pageWidth = page.GetWidth();
            float pageHeight = page.GetHeight();
            float left;

            // Draw the metadata for each data series.
            for (int i = 0; i < reportElement.NumDataSeries; i++)
            {
                if (top < BOTTOM_TEXT) // was (i == 2)
                {
                    if (rightColumn)
                    {
                        // Write "continued" note
                        page.BeginText();
                        left = page.GetWidth() / 2.0f;
                        page.MoveTextPos(left, top);
                        page.ShowText("(continued on next page)");
                        page.EndText();

                        // Write footer
                        PdfHelper.WriteFooter(page, document, pageNumber, fontHelveticaBold);

                        // Add new page and increment page number
                        page = pdf.AddPage();
                        pageNumber++;

                        // Start at top of new page
                        startTop = pageHeight - (TOP_BUFFER + 24 + V_BUFFER_TITLE_TO_CHART);
                    }
                    top = startTop;
                    rightColumn = !rightColumn;
                }

                // Get the data series.
                DataSeries dataSeries = reportElement.GetDataSeries(i);

                // Draw the data series name.
                if (rightColumn)
                {
                    left = page.GetWidth() / 2.0f;
                }
                else
                {
                    left = LEFT_BUFFER;
                }
                page.SetFontAndSize(fontHelveticaBold, LABEL_HEIGHT_TITLE);
                DrawString(page, dataSeries.Name, left, top, fontHelvetica);

                // Decrement top.
                top -= LABEL_HEIGHT_TITLE + BUFFER_V;

                // Draw the data-provider metadata.
                page.SetFontAndSize(fontHelvetica, LABEL_HEIGHT_META);
                float maxWidth = left < pageWidth / 2.0f ? pageWidth / 2.0f - left - 20.0f : pageWidth - left - 20.0f;
                string[] providerMeta = dataSeries.DataProvider.GetMetadata();
                for (int j = 0; j < providerMeta.Length; j++)
                {
                    int numLines = DrawStringSmart(page, providerMeta[j], left, top, maxWidth, LABEL_HEIGHT_META + BUFFER_V);
                    top -= (LABEL_HEIGHT_META + BUFFER_V) * numLines;
                }
                top -= LABEL_HEIGHT_TITLE + BUFFER_V;
            }

            return 0;
        }
        public static void DrawString(HPdfPage page, string s, float xc, float yc, HPdfFont fontHelvetica)
        {
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
                    //Console.WriteLine("wrote: " + section);
                    page.BeginText();
                    page.MoveTextPos(xc, yc);
                    page.ShowText(section);
                    page.EndText();

                    xc += page.TextWidth(section);
                }
            }
        }
        public static int DrawStringSmart(HPdfPage page, string s, float xc, float yc, float maxWidth, float lineOffset)
        {
            int numLines = 0;
            while (s.Length > 0)
            {
                float offset = numLines == 0 ? 0.0f : 10.0f;
                float maxWidthThisLine = maxWidth - offset;

                int numCharsThisLine = s.Length;
                while (page.TextWidth(s.Substring(0, numCharsThisLine)) > maxWidthThisLine && numCharsThisLine > 10)
                {
                    numCharsThisLine--;
                }

                // Draw the text.
                page.BeginText();
                page.MoveTextPos(xc + offset, yc);
                string section = s.Substring(0, numCharsThisLine);
                page.ShowText(section);
                page.EndText();

                // Remove the printed characters.
                if (numCharsThisLine == s.Length)
                {
                    s = "";
                }
                else
                {
                    s = s.Substring(numCharsThisLine, s.Length - numCharsThisLine);
                }

                yc -= lineOffset;
                numLines++;
            }

            return numLines;
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

        public static void WriteFooter(HPdfPage page, SADocument document, int pageNumber, HPdfFont font)
        {
            // Set the font and size.
            page.SetFontAndSize(font, FOOTER_FONT_SIZE);

            // Determine the text for the footer and calculate the position so that it is at the bottom of the page, right justified.
            string fullString = document.ReportName + "   " + pageNumber;
            float xc = page.GetWidth() - page.TextWidth(fullString) - 20.0f;
            float yc = 35.0f;

            // Place the text on the page.
            page.BeginText();
            page.MoveTextPos(xc, yc);
            page.ShowText(fullString);
            page.EndText();
        }

        public static void WriteFooter(HPdfPage page, string footerText, int pageNumber, HPdfFont font)
        {
            // Set the font and size.
            page.SetFontAndSize(font, FOOTER_FONT_SIZE);

            // Determine the text for the footer and calculate the position so that it is at the bottom of the page, right justified.
            string fullString = footerText + "   " + pageNumber;
            float xc = page.GetWidth() - page.TextWidth(fullString) - 20.0f;
            float yc = 35.0f;

            // Place the text on the page.
            page.BeginText();
            page.MoveTextPos(xc, yc);
            page.ShowText(fullString);
            page.EndText();
        }

        /// <summary>
        /// Write a line of text to a HPdfPage, starting at current text position.  
        /// If text will not fit on one line, or if there is not enough (vertical) 
        /// room on the page, do not write the text and return false.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="leadingFactor">Leading, as a fraction of fontHeight; typically about 0.5</param>
        /// <param name="indent">Indent distance from PdfHelper.MARGIN_LEFT</param>
        /// <param name="line"></param>
        /// <param name="currentTextPos">Current text position, revised after writing line</param>
        /// <param name="font"></param>
        /// <param name="fontHeight"></param>
        /// <returns></returns>
        public static bool WriteLine(HPdfPage page, float spaceAbove, float leadingFactor, float indent, string line, 
                                     ref HPdfPoint currentTextPos, HPdfFont font, float fontHeight)
        {
            // Calculate leading height
            float leading = leadingFactor * fontHeight;

            // Set the specified font and size            
            page.SetFontAndSize(font, fontHeight);

            // See if line will fit in available width
            float maxWidth = page.GetWidth() - MARGIN_LEFT - MARGIN_RIGHT - indent;
            int numChars = (int)page.MeasureText(line, maxWidth, false);
            if (numChars < line.Length)
            {
                return false;
            }

            // Line will fit width-wise; now see if there's room at the
            // bottom of the page, starting at current text position
            page.BeginText();
            float xc = MARGIN_LEFT + indent;
            float yc = currentTextPos.y - leading - fontHeight - spaceAbove;
            if (yc < MARGIN_BOTTOM)
            {
                page.EndText();
                return false;
            }

            // Line can be written, so write it
            page.MoveTextPos(xc, yc);
            page.ShowText(line);
            currentTextPos = page.GetCurrentTextPos();
            page.EndText();

            return true;
        }

        /// <summary>
        /// Write a string as one or more lines of text to a HPdfDoc, starting at 
        /// current text position on current page.  Add pages as needed.
        /// </summary>
        /// <param name="pdf"></param>
        /// <param name="spaceAbove">Vertical space above leading preceding first line of type</param>
        /// <param name="leadingFactor">Leading, as a fraction of fontHeight; typically about 0.5</param>
        /// <param name="indent">Indent distance from PdfHelper.MARGIN_LEFT</param>
        /// <param name="text"></param>
        /// <param name="currentTextPos"></param>
        /// <param name="font"></param>
        /// <param name="fontHeight"></param>
        /// <returns></returns>
        public static void WriteLines(HPdfDoc pdf, float spaceAbove, float leadingFactor, float indent, 
                                      string text, ref HPdfPoint currentTextPos, HPdfFont font, float fontHeight)
        {
            // Define variables that will be used and modified in this method
            string workingText = text.Trim();
            string textRemaining;
            string line;
            float spaceAboveLocal = spaceAbove;

            // Get the current page and set the font and size
            HPdfPage page = pdf.GetCurrentPage();
            if (page == null)
            {
                AddNewPage(pdf, ref currentTextPos, ref spaceAboveLocal, ref page, font, fontHeight);
            }
            else
            {
                page.SetFontAndSize(font, fontHeight);
            }

            // Calculate leading height
            float leading = leadingFactor * fontHeight;

            // How much height is available on current page?
            float availableHeight = currentTextPos.y - spaceAboveLocal - MARGIN_BOTTOM;

            // Define width available for each line
            float availableWidth = page.GetWidth() - MARGIN_LEFT - MARGIN_RIGHT - indent;

            // How many lines will available height accommodate?
            int availableLines = Convert.ToInt32(Math.Floor(availableHeight / (leading + fontHeight)));

            // If fewer than 2 lines can be accomodated and text requires more than 2 lines, 
            // start a new page to avoid widow.
            if (availableLines < 2)
            {
                int line1Length = (int)page.MeasureText(workingText, availableWidth, true);
                if (line1Length < workingText.Length)
                {
                    int startIndex = line1Length;
                    int stringLength = workingText.Length - line1Length;
                    textRemaining = workingText.Substring(startIndex, stringLength).Trim();
                    int line2Length = (int)page.MeasureText(textRemaining, availableWidth, true);
                    if (line2Length < textRemaining.Length)
                    {
                        AddNewPage(pdf, ref currentTextPos, ref spaceAboveLocal, ref page, font, fontHeight);
                    }
                }
            }

            // Assign x position
            currentTextPos.x = MARGIN_LEFT + indent;

            // If current text position is not at the top of a new page, adjust current y by spaceAboveLocal
            if (currentTextPos.y < page.GetHeight() - MARGIN_TOP)
            {
                currentTextPos.y = currentTextPos.y - spaceAboveLocal;
            }

            // Write lines to current page until all of workingText is written, adding pages as needed.
            int lineLength;
            while (workingText.Length > 0)
            {
                // If current page cannot accommodate another line, add a new page
                availableHeight = currentTextPos.y - MARGIN_BOTTOM;
                if (availableHeight < leading + fontHeight)
                {
                    AddNewPage(pdf, ref currentTextPos, ref spaceAboveLocal, ref page, font, fontHeight);

                    // Assign x position
                    currentTextPos.x = MARGIN_LEFT + indent;
                }

                // Define text to be written on next line
                lineLength = (int)page.MeasureText(workingText, availableWidth, true);

                // If a word (e.g. a file pathname) is too long to fit in the available width,
                // lineLength will = 0.  Deal with it by breaking within the word at the
                // end of the available width.
                if (lineLength == 0)
                {
                    lineLength = (int)page.MeasureText(workingText, availableWidth, false);
                }

                line = workingText.Substring(0, lineLength);

                // Adjust the y position
                currentTextPos.y = currentTextPos.y - leading - fontHeight;

                // Write the line
                page.BeginText();
                page.TextOut(currentTextPos.x, currentTextPos.y, line);
                page.EndText();

                // Remove text that was written from workingText
                workingText = workingText.Substring(lineLength, workingText.Length - lineLength).Trim();
            }
        }

        public static void AddNewPage(HPdfDoc pdf, ref HPdfPoint currentTextPos, ref float spaceAboveLocal, 
                                       ref HPdfPage page, HPdfFont font, float fontHeight)
        {
            page = pdf.AddPage();
            SetPageSize(page);
            spaceAboveLocal = 0;
            currentTextPos.x = MARGIN_LEFT;
            currentTextPos.y = page.GetHeight() - MARGIN_TOP;
            page.SetFontAndSize(font, fontHeight);
        }

        /// <summary>
        /// Sets page size as 8.5 x 11
        /// </summary>
        /// <param name="page"></param>
        public static void SetPageSize(HPdfPage page)
        {
            page.SetWidth(PAGE_WIDTH);
            page.SetHeight(PAGE_HEIGHT);
        }
    }
}
