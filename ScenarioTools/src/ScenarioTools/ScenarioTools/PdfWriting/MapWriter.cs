using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using HPdf;

using ScenarioTools.Numerical;
using ScenarioTools.Reporting;

namespace ScenarioTools.PdfWriting
{
    public class MapWriter
    {
        private const int GRAPH_TOP_BUFFER = 0;
        private const int GRAPH_V_BUFFER = 10;
        private const int GRAPH_LEFT_BUFFER = 15;
        private const float POINT_MARKER_SIZE = 4.5f;
        private const double BUFFER_PIXELS_DOMAIN = 10;
        private const double BUFFER_PIXELS_RANGE = 10;
        private const int TICK_MARK_LENGTH = 5;

        private const int BUFFER_LABELS_X_AXIS = -20;
        private const int SIDE_BUFFER = 100;
        private const int TOP_BUFFER = 80;
        private const int V_BUFFER_TITLE_TO_FIRST_GRAPH = 30;
        private const int V_BUFFER_BETWEEN_GRAPHS = 30;
        private const float X_AXIS_LABEL_TITLE_BUFFER = 4.0f;
        private const float Y_LABEL_GRAPH_BUFFER = 2.0f;
        private const float META_DATA_HEIGHT = 130.0f;

        internal static int WriteSTMap(HPdfDoc pdf, SADocument document, ReportElementSTMap element, 
            int pageNumber, HPdfFont fontHelvetica, HPdfFont fontHelveticaBold)
        {
            // Make a new page.
            HPdfPage page = pdf.AddPage();
            int startPageNumber = pageNumber;

            // Get the width and height of the page.
            float width = page.GetWidth();
            float height = page.GetHeight();

            // Set the title font.
            float fontHeight = 24.0f;
            page.SetFontAndSize(fontHelvetica, fontHeight);

            // Get the image for the item.
            Image image = element.GetImage();

            // Define dimensions
            int numMetadata = element.NumDataSeries;
            float pageTopMargin = 30.0f;
            float pageLeftMargin = 50.0f;
            float pageRightMargin = 15.0f;
            float pageBottomMargin = 50.0f;
            float elementVerticalSpace = 20.0f;
            float pageWidth = page.GetWidth();
            float pageHeight = page.GetHeight();
            float metadataHeight = META_DATA_HEIGHT;
            // Allow for a maximum of 4 metadata entries on this page
            if (numMetadata > 2)
            {
                metadataHeight = metadataHeight * 2.0f + elementVerticalSpace;
            }
            float availablePageWidth = pageWidth - pageLeftMargin - pageRightMargin;
            float availablePageHeight = pageHeight - pageTopMargin - fontHeight 
                                        - elementVerticalSpace * 2.0f - metadataHeight 
                                        - pageBottomMargin;
            float imageWidth = image.Width * 0.5f;
            float imageHeight = image.Height * 0.5f;
            float imageLeft = pageLeftMargin;
            if (imageWidth > availablePageWidth)
            {
                float ratio = availablePageWidth / imageWidth;
                imageWidth *= ratio;
                imageHeight *= ratio;
            }
            if (imageHeight > availablePageHeight)
            {
                float ratio = availablePageHeight / imageHeight;
                imageWidth *= ratio;
                imageHeight *= ratio;
                float imageCenter = (pageLeftMargin + pageWidth - pageRightMargin) / 2.0f;
                imageLeft = Math.Max(pageLeftMargin, imageCenter - imageWidth / 2.0f);
            }

            // Draw the title.
            string graphTitle = element.Name;
            float tw = page.TextWidth(graphTitle);
            page.BeginText();
            page.MoveTextPos((page.GetWidth() - tw) / 2, page.GetHeight() - pageTopMargin 
                             - fontHeight);
            page.ShowText(graphTitle);
            page.EndText();

            // Write the image to the page.
            string imageFileName = RandGen.NextUID() + ".png";
            image.Save(imageFileName, ImageFormat.Png);
            HPdfImage pdfImage = pdf.LoadPngImageFromFile(imageFileName);
            float imageBottom = pageHeight - pageTopMargin - fontHeight - elementVerticalSpace 
                                - imageHeight;
            page.DrawImage(pdfImage, imageLeft, imageBottom, imageWidth, imageHeight);
            float currentY = pageHeight - imageHeight - fontHeight;

            // Try to delete the image file.
            try
            {
                File.Delete(imageFileName);
            }
            catch { }

            // Draw the metadata.
            int top; 
            top = Convert.ToInt32(pageHeight - pageTopMargin - fontHeight - imageHeight 
                                  - elementVerticalSpace * 2.0f);
            PdfHelper.DrawMetadata(pdf, ref page, top, element, fontHelvetica, fontHelveticaBold, document, ref pageNumber);

            // Write the footer.
            PdfHelper.WriteFooter(page, document, pageNumber, fontHelveticaBold);

            // Return the number of pages that were written.
            return 1 + pageNumber - startPageNumber;
        }
    }
}
