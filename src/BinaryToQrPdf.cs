using System.Text;
using SkiaSharp;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Fonts.Standard14Fonts;
using UglyToad.PdfPig.Writer;
using ZXing;
using ZXing.QrCode;
using ZXing.SkiaSharp.Rendering;
using static UglyToad.PdfPig.Writer.PdfDocumentBuilder;

namespace com.freebiz
{
	public class BinaryToQrPdf
	{
		public void Digitize(String pstrSrcFile, String pstrOutPath)
		{
			int intPart = 1;
			int intPage = 1;
			int intImgCntr = 0;
			int bytesRead = 0;
			int intCol = 0;
			int intRow = 0;
			double dblXPos = 0;
			double dblYPos = 0;
			double dblTopY = 0;
			byte[] buffer = new byte[Globals.CHUNK_SIZE]; // buffer
			FileInfo fileInfo;
			PdfDocumentBuilder pdb;

			fileInfo = new(pstrSrcFile);
			int totParts = (int)Math.Ceiling((double)fileInfo.Length / (double)Globals.CHUNK_SIZE);
			BarcodeWriter<SKBitmap> bw = new()
			{
				Format = BarcodeFormat.QR_CODE,
				Renderer = new SKBitmapRenderer(),
				Options = new QrCodeEncodingOptions
				{
					Height = (int) Globals.QR_SIZE,
					Width = (int) Globals.QR_SIZE,
					Margin = 1,
					CharacterSet = "ISO-8859-1",
					Hints = { { EncodeHintType.QR_VERSION, 40 } },
				}
			};

			double imgSizePt = (Globals.QR_SIZE / Globals.DOTS_PER_INCH) * Globals.POINTS_PER_INCH;             // 144 Points
			double spcPoints = ((Globals.IMG_SPACING * 2) / Globals.DOTS_PER_INCH) * Globals.POINTS_PER_INCH;   // 5.76 Points
			double pgMargin = ((Globals.PAGE_MARGIN * 2) / Globals.DOTS_PER_INCH) * Globals.POINTS_PER_INCH;    // 24.0 Points
			//double pageWidth = Globals.A4_PAGE_WIDTH * Globals.POINTS_PER_INCH;                               // 595.44 Points
			double pageHeight = Globals.A4_PAGE_HEIGHT * Globals.POINTS_PER_INCH;                               // 841.68 Points

			pdb = new PdfDocumentBuilder();
			AddedFont fntTxt = pdb.AddStandard14Font(Standard14Font.Helvetica);
			AddOverview(pdb, fntTxt, pstrSrcFile, fileInfo.Length, totParts);
			PdfPageBuilder page = pdb.AddPage(PageSize.A4);
			Console.WriteLine($"Page Width {page.PageSize.Width} & Height is {page.PageSize.Height}");
			using (FileStream fs = new(pstrSrcFile, FileMode.Open, FileAccess.Read))
			{
				while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
				{
					Console.SetCursorPosition(1, 1);
					Console.WriteLine($"Read {bytesRead:N} bytes for part {intPart:N} of {totParts:N}");

					if (bytesRead < 2512)
					{
						//Last chunk of file.
						Array.Resize(ref buffer, bytesRead);
					}
					string pseudoString = Encoding.GetEncoding("ISO-8859-1").GetString(buffer);
					SKBitmap sbm = bw.Write(pseudoString);
					SKImage img = SKImage.FromBitmap(sbm);
					SKData imgData = img.Encode(SKEncodedImageFormat.Png, 100);

					intCol = intImgCntr % Globals.PAGE_COLS;
					intRow = intImgCntr / Globals.PAGE_COLS;

					dblXPos = pgMargin + (intCol * (imgSizePt + spcPoints));
					dblTopY = pageHeight - pgMargin - (intRow * (imgSizePt + spcPoints));
					dblYPos = dblTopY - imgSizePt;

					Console.WriteLine($"Drawing QR Code of size {imgSizePt:N} points at X: {dblXPos:F}, Y: {dblYPos:F}, In Column {intCol:N} & Row {intRow:N} of page {intPage:N}");
					PdfRectangle pr = new PdfRectangle(dblXPos, dblYPos, dblXPos + imgSizePt, dblYPos + imgSizePt);
					page.AddPng(imgData.ToArray(), pr);

					DisposeResource(imgData);
					DisposeResource(img);
					DisposeResource(sbm);
					intPart++;
					intImgCntr++;
					if (intImgCntr > (Globals.PAGE_COLS * Globals.PAGE_ROWS) - 1)
					{
						DrawFooter(page, fntTxt, intPage);
						Console.WriteLine($"Added page {intPage}");
						page = pdb.AddPage(PageSize.A4);
						intImgCntr = 0;
						intPage++;
					}
				}
			}
			//The pstrOutPath is expected to have a trailing path separator
			File.WriteAllBytes(pstrOutPath + "pdf/" + Path.GetFileNameWithoutExtension(pstrSrcFile) + ".pdf", pdb.Build());
			DisposeResource(pdb);
			Console.WriteLine($"Digitization of {pstrSrcFile} is successful");
		}

		/// <summary>
        /// Helper method to add a page footer
        /// </summary>
        /// <param name="ppb">The PdgPageBuilder refernce</param>
        /// <param name="intPage">The page number</param>
        /// <param name="pgMargin">Page margin</param>
		private void DrawFooter(PdfPageBuilder ppb, AddedFont pfnt, int intPage)
		{
			string strFooter = $"Page {ppb.PageNumber}";
			IReadOnlyList<Letter> ftr = ppb.MeasureText(strFooter, 10, new PdfPoint(0, 0), pfnt);
			double dblWidth = ftr.Sum(l => l.Width);
			ppb.AddText(strFooter, 10, new PdfPoint((ppb.PageSize.Width - Globals.PAGE_MARGIN - dblWidth) / 2, Globals.PAGE_MARGIN - 12), pfnt);
		}

		private void AddOverview(PdfDocumentBuilder pdb, AddedFont pfnt, String pstrFile, long plngSize, long plngParts)
		{
			//PdfPig uses X distance from Left and Y distance from bottom
			PdfDocumentBuilder.AddedFont fntTitle = pdb.AddStandard14Font(Standard14Font.HelveticaBold);

			PdfPageBuilder ppb = pdb.AddPage(PageSize.A4, true);
			PdfPoint pp = new PdfPoint(Globals.PAGE_MARGIN, ppb.PageSize.Top - Globals.PAGE_MARGIN);

			IReadOnlyList<Letter> title = ppb.AddText("Overview", 24, pp, fntTitle);

			double intY = pp.Y - 24 - 4;
			pp = new PdfPoint(Globals.PAGE_MARGIN + 10, intY);
			ppb.AddText($"Source File  : {Path.GetFileName(pstrFile)}", 12, pp, pfnt);

			intY = pp.Y - 12 - 4;
			pp = new PdfPoint(Globals.PAGE_MARGIN + 10, intY);
			ppb.AddText($"Source Size  : {plngSize:N} byte(s)", 12, pp, pfnt);

			intY = pp.Y - 12 - 4;
			pp = new PdfPoint(Globals.PAGE_MARGIN + 10, intY);
			ppb.AddText($"Total Codes  : {plngParts:N}", 12, pp, pfnt);

			intY = pp.Y - 12 - 4;
			double totPages = Math.Ceiling((double) plngParts / (Globals.PAGE_COLS * Globals.PAGE_ROWS));
			pp = new PdfPoint(Globals.PAGE_MARGIN + 10, intY);
			ppb.AddText($"Total Pages  : {totPages:N}", 12, pp, pfnt);
		}

		private double ComputeWidth(PdfPageBuilder ppb, String pstrText, AddedFont pfnt)
		{
			IReadOnlyList<Letter> ftr = ppb.MeasureText(pstrText, 10, new PdfPoint(0, 88), pfnt);
			double dblWidth = ftr.Sum(l => l.Width);
			return dblWidth;
		}

		private void DisposeResource(IDisposable pObj)
        {
			if (null == pObj) return;
			try
			{
				pObj.Dispose();
			}
			catch (Exception ex)
            {
				Console.Write($"Unknown error ({ex.Message}) occurred while disposing resource");
            }
        }
	}
}
