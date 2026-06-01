using System;

namespace com.freebiz
{
    /// <summary>
    /// Ths class defines various constants which primarily are used during digitization of a binary artifact into QR codes 
    /// which are then rendered on PDF pages sequentially. PDF coordinates are measured in points (1 inch = 72 points)
    /// 
    /// The Digitization procss assumes a A4 size page with typical size as 8.27 x 11.68 inches which generally results
    /// into pixel size of 2480 x 3508 pixels at 300 DPI print resolution. The 300 DPI resolution is something which enusres
    /// that 12 QR codes can be printed on a single PDF page and can then be scanned and decoded without any errors. 
    /// 
    /// A4 size is 595.27 x 841.89 points (or 8.27 x 11.69 inches) as per the point measurement of PDF
    /// </summary>
    public class Globals
    {
        //A4 document size is 8.27 x 11.69 inches
        //With 300 DPI the pixel size becomes 2480 x 3508 pixels

        /// <summary>
        /// Maximum number of of columns in a single row of the table on a PDF page so that QR codes rendered in 
        /// each table cell can be scanned without any error when the page is printed at 300 DPI and scanned back 
        /// for restoration.
        /// </summary>
        public const int PAGE_COLS = 3;

        /// <summary>
        /// Maximum number of rows a table can have on a single PDF page so that QR codes rendered in each table cell 
        /// can be scanned without any error when the page is printed at 300 DPI and scanned back for restoration. 
        /// </summary>
        public const int PAGE_ROWS = 5;

        /// <summary>
        /// The page margin to be used while creating the QR filled PDF page
        /// </summary>
        public const double PAGE_MARGIN = 50;

        /// <summary>
        /// The cell padding to be used while drawing a QR in a table cell 
        /// </summary>
        public const int CELL_PADDING = 10;

        /// <summary>
        /// Max size of data encoded in a QR code.
        /// </summary>
        public const int CHUNK_SIZE = 2512;

        /// <summary>
        /// Max capacity for binary data in a Version 40 QR code with Low (L) error correction
        /// </summary>
        public const int MAX_QR_CAPACITY = 2953;

        public const double DOTS_PER_INCH = 300;

        public const double POINTS_PER_INCH = 72;

        public const double QR_SIZE = 600;

        public const double IMG_SPACING = 12;

        public const double A4_PAGE_WIDTH = 8.27;

        public const double A4_PAGE_HEIGHT = 11.69;
    }
}
