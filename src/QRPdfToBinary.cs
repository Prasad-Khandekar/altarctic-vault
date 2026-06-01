using SkiaSharp;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using ZXing;
using ZXing.Common;
using ZXing.SkiaSharp;

namespace com.freebiz
{
    /// <summary>
    /// The class for restoring the original binary file from the digitized PDF file. In which the original binary file is digitized as
    /// a series of Level 40 QR codes. 
    /// </summary>
    public class QRPdfToBinary
    {
        /// <summary>
        /// The method `Restore` reads the specified PDF and converts the digitized data back to original binary form.
        /// </summary>
        /// <param name="pstrSrc">The full path and name of the PDF file in whcih the binary file to be restored is digitized as a seeries of QR codes.</param>
        /// <param name="pstrOutPath">The full path and name of the folder in which to put the restored binary file. This path must end with a directory separator</param>
        /// <param name="pstrExt">The original extension of the binary file. This can be viewed from the first page of the digitized PDF document</param>
        public void Restore(String pstrSrc, String pstrOutPath, String pstrExt)
        {
            //Create a reader for reading the QRCodes from bitmap image of a PDF page.
            BarcodeReader bcr = new()
            {
                AutoRotate = true,
                Options = new DecodingOptions
                {
                    PossibleFormats = new[] { BarcodeFormat.QR_CODE },
                    TryHarder = true
                }
            };

            Console.Clear();
            string strFile = Path.GetFileNameWithoutExtension(pstrSrc);
            using (FileStream fos = new FileStream(pstrOutPath + "restore/" + strFile + "." + pstrExt, FileMode.CreateNew))
            {
                using (PdfDocument doc = PdfDocument.Open(pstrSrc))
                {
                    foreach (Page page in doc.GetPages())
                    {
                        if (page.Number < 2) continue;

                        Console.SetCursorPosition(1, 1);
                        Console.WriteLine($"Reading page {page.Number, 5:D}");
                        List<QrCodeItem> lst = ReadQRCodes(page, bcr);
                        WriteData(fos, lst, page.Number - 1);
                    }
                }
            }
        }

        private List<QrCodeItem> ReadQRCodes(Page ppage, BarcodeReader pbcr)
        {
            List<QrCodeItem> lstDetected = [];
            QrCodeItem? qci;

            foreach (IPdfImage img in ppage.GetImages())
            {
                SKBitmap? sb = GetBitmap(img);
                if (sb != null)
                {
                    qci = DecodeQR(img, sb, pbcr);
                    if (qci != null)
                        lstDetected.Add(qci);
                }
            }
            return lstDetected;
        }

        private QrCodeItem? DecodeQR(IPdfImage pimg, SKBitmap psb, BarcodeReader pbcr)
        {
            Result? retVal = null;
            QrCodeItem? qciRet = null;

            if (psb != null)
                retVal = pbcr.Decode(psb);

            if (retVal != null)
            {
                using MemoryStream ms = new();
                if (retVal.ResultMetadata.TryGetValue(ResultMetadataType.BYTE_SEGMENTS, out object? segmentsObj))
                {
                    IList<byte[]> segments = (IList<byte[]>)segmentsObj;
                    foreach (byte[] bytes in segments)
                    {
                        ms.Write(bytes);
                    }
                }
                else
                {
                    // Fallback if metadata wrapper is empty
                    ms.Write(retVal.RawBytes);
                }
                qciRet = new QrCodeItem
                {
                    Data = ms.ToArray(),
                    X = pimg.BoundingBox.Left,
                    Y = pimg.BoundingBox.Bottom
                };
            }
            return qciRet;
        }

        private void WriteData(FileStream pfos, List<QrCodeItem> plst, int pintPage)
        {
            int intImg = 0;

            if (plst.Count <= 0)
            {
                Console.WriteLine($"No data found on PDF page {pintPage, 5:D}");
                return;
            }
            // 1. Group and sort rows by Y coordinate (Descending: top-of-page to bottom)
            // Grouping items close to each other on the Y-axis (within 20 points tolerance)
            List<IGrouping<double, QrCodeItem>> rowGroups = plst.GroupBy(q => Math.Round(q.Y / 20.0) * 20.0)
                                                                .OrderByDescending(g => g.Key)
                                                                .ToList();
            if (pintPage > 0)
            {
                intImg = (pintPage - 1) * (Globals.PAGE_COLS * Globals.PAGE_ROWS);
            }

            Console.WriteLine($"Processing {rowGroups.Count, 1:D} rows");
            for (int r = 0; r < rowGroups.Count; r++)
            {
                List<QrCodeItem> lstRow = rowGroups[r].OrderBy(q => q.X).ToList();
                foreach (QrCodeItem qi in lstRow)
                {
                    Console.WriteLine($"Reading QRCode {intImg + 1, 2:D}, Page {pintPage, 5:D}, X: {qi.X, 6:F2}, Y: {qi.Y, 6:F2}");
                    if (qi.Data != null)
                    {
                        pfos.Write(qi.Data, 0, qi.Data.Length);
                    }
                    intImg++;
                }
            }
            Console.WriteLine($"Processed {rowGroups.Count, 1:D} rows");
        }

        private SKBitmap? GetBitmap(IPdfImage pimg)
        {
            byte[]? arrBytes;
            SKBitmap? sbRet = null;

            try
            {
                if (pimg.TryGetPng(out arrBytes))
                {
                    using MemoryStream ms = new(arrBytes);
                    sbRet = SKBitmap.Decode(ms);
                }
                else
                {
                    // Fallback for raw JPEGs or unsupported streams
                    arrBytes = pimg.RawBytes.ToArray();
                    using MemoryStream ms = new(arrBytes);
                    sbRet = SKBitmap.Decode(ms);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Skipped unreadable image: {ex.Message}");
            }
            return sbRet;
        }
    }
}
