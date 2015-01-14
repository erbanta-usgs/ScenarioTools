using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

using HPdf;
using ScenarioTools.Reporting;

namespace ScenarioTools.PdfWriting
{
    public class PdfWriter
    {
        public static void ExportPDF(SADocument document, string path)
        {
            // Get the images from all of the items and stitch them together into a PDF.

            // Make the PDF and set the compression mode.
            HPdfDoc pdf = new HPdfDoc();
            pdf.SetCompressionMode(HPdfDoc.HPDF_COMP_ALL);

            // Get the fonts.
            HPdfFont fontHelvetica = pdf.GetFont("Helvetica", null);
            HPdfFont fontHelveticaBold = pdf.GetFont("Helvetica-Bold", null);
            HPdfFont footerFont = fontHelveticaBold;

            // Write the title page.
            writeTitlePage(pdf, document, fontHelvetica);

            // Start with the page number at 2.
            int pageNumber = 2;

            // Write the report elements in order.
            for (int i = 0; i < document.NumElements; i++)
            {
                // Get the element from the report.
                IReportElement element = document.GetElement(i);

                if (element is ReportElementChart)
                {
                    ReportElementChart chartElement = (ReportElementChart)element;
                    pageNumber += ChartWriter.WriteChart(pdf, document, chartElement, pageNumber, fontHelvetica, fontHelveticaBold);
                }

                else if (element is ReportElementSTMap)
                {
                    pageNumber += MapWriter.WriteSTMap(pdf, document, (ReportElementSTMap)element, pageNumber, fontHelvetica, fontHelveticaBold);
                }

                // This is the case for tables.
                else
                {
                    pageNumber += TableWriter.WriteTable(pdf, document, (ReportElementTable)element, pageNumber, footerFont);
                }
            }

            // Save the report to a file.
            string reportDirectory = Path.GetDirectoryName(path);
            Directory.CreateDirectory(reportDirectory);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            pdf.SaveToFile(path);
        }
        private static void writeTitlePage(HPdfDoc pdf, SADocument document, HPdfFont font)
        {
            // Make the page.
            HPdfPage page = pdf.AddPage();
            float titleFontHeight = 36.0f;
            float authorFontHeight = 18.0f;
            float dateFontHeight = 12.0f;
            float authorTitleSpacing = 24.0f;
            float authorDateSpacing = 120.0f;
            float titleLineSpacing = 2.0f;
            page.SetFontAndSize(font, titleFontHeight);

            // Split the title into lines.
            List<string> title = new List<string>();
            string[] titleString = document.ReportName.Split(new char[] { ' ' });
            int index = 0;
            float maxWidth = page.GetWidth() * 2.0f / 3.0f;
            while (index < titleString.Length)
            {
                string line = "";

                // While adding another word does not make the title too long, add more words.
                bool tooLong = false;
                while (!tooLong && index < titleString.Length)
                {
                    // Add the next word.
                    line += (line.Equals("") ? "" : " ") + titleString[index];

                    // Check if adding another word will make the title too long. If so, flag.
                    if (index + 1 < titleString.Length)
                    {
                        string nextAdded = line + " " + titleString[index + 1];
                        if (page.TextWidth(nextAdded) > maxWidth)
                        {
                            tooLong = true;
                        }
                    }
                    index++;
                }

                title.Add(line);
            }

            // Determine the total size of the title.
            float titleHeight = title.Count * titleFontHeight + (title.Count) * titleLineSpacing + authorFontHeight + authorTitleSpacing;

            // Draw the title.
            float yc = page.GetHeight() - (page.GetHeight() - titleHeight) * 0.4f;
            for (int i = 0; i < title.Count; i++)
            {
                float xc = (page.GetWidth() - page.TextWidth(title[i])) / 2.0f;
                page.BeginText();
                page.MoveTextPos(xc, yc);
                page.ShowText(title[i]);
                page.EndText();

                yc -= titleFontHeight + titleLineSpacing;
            }

            // Draw the author name.
            yc -= authorTitleSpacing;
            page.SetFontAndSize(font, authorFontHeight);
            float xcA = (page.GetWidth() - page.TextWidth(document.Author)) / 2.0f;
            page.BeginText();
            page.MoveTextPos(xcA, yc);
            page.ShowText(document.Author);
            page.EndText();

            // Draw the current date and time
            DateTime currentDateTime = DateTime.Now;
            string dateString = DateUtil.dateTimeToStringNoDay(currentDateTime);
            page.SetFontAndSize(font, dateFontHeight);
            yc -= authorDateSpacing;
            xcA = (page.GetWidth() - page.TextWidth(dateString)) / 2.0f;
            page.BeginText();
            page.MoveTextPos(xcA, yc);
            page.ShowText(dateString);
            page.EndText();
        }
    }
}
