using System;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace com.freebiz
{
    class Program
    {
        /// <summary>
        /// The program for digitizing a binary file to a PDF containing data encoded as series of QR codes. Each QR Code is a level 40 QR Code with maximum of 2512 bytes. 
        /// </summary>
        /// <param name="digitize">Converts a specified binary file to multi-page PDF containing QR codes</param>
        /// <param name="restore">Reads the multi-page PDF and reading the QR codes restores the original file</param>
        /// <param name="src">The source binary file to be digitized or the PDF file using which to restore the original binary file</param>
        /// <param name="out">The folder where to put the restored binary file or digitized PDF file.</param>
        static void Main(string[] args)
        {
            Console.Clear();

            Argument<String> argSrc = new("The full path and name of the source file.")
            {
                Arity = ArgumentArity.ZeroOrOne
            };

            Argument<String> argOutPath = new("The full patha and name of folder in which to output the result")
            {
                Arity = ArgumentArity.ZeroOrOne
            };

            Command cmdDigitize = new("digitize", "Digitize the source  binary file and save the digitized data as a PDF in specified output folder")
            {
                argSrc,
                argOutPath
            };
            cmdDigitize.SetAction(parseResult =>
            {
                String? strSrcFile = parseResult.GetValue(argSrc);
                String? strOutPath = parseResult.GetValue(argOutPath);

                // 2. Explicit null checks
                if (strSrcFile == null || strOutPath == null)
                {
                    Console.WriteLine("Error: Both source and output paths must be provided.");
                } else
                {
                    BinaryToQrPdf doc = new BinaryToQrPdf();
                    if (strOutPath.EndsWith("/")) {
                        doc.Digitize(strSrcFile, strOutPath);   // + "pdf\\" + Path.GetFileNameWithoutExtension(strSrcFile) + ".pdf");
                    } else {
                        Console.WriteLine("Will digitize supplied document {0:G}", strSrcFile);
                        doc.Digitize(strSrcFile, strOutPath + "/");   // + "\\pdf\\" + Path.GetFileNameWithoutExtension(strSrcFile) + ".pdf");
                    }
                }
            });

            Argument<String> argBinExt = new("Extensions of the resulting binary file")
            {
                DefaultValueFactory = _ => "zip"
            };
            Command cmdRestore = new("restore", "Restores the original binary file from the digitized PDF file and store it in specified output folder")
            {
                argSrc,
                argOutPath,
                argBinExt
            };
            cmdRestore.SetAction(parseResult =>
            {
                String? strSrcFile = parseResult.GetValue(argSrc);
                String? strOutPath = parseResult.GetValue(argOutPath);
                String strExt = parseResult.GetRequiredValue(argBinExt);
                // 2. Explicit null checks
                if (strSrcFile == null || strOutPath == null) {
                    Console.WriteLine("Error: Both source and output paths must be provided.");
                } else {
                    Console.WriteLine("Will restore original binary from supplied document {0:G}", strSrcFile);
                    QRPdfToBinary cnvrtr = new QRPdfToBinary();
                    cnvrtr.Restore(strSrcFile, strOutPath, strExt);
                }
            });

            RootCommand rCmd = new RootCommand("Large data digitizer and restore utility for vault storage");
            rCmd.Subcommands.Add(cmdDigitize);
            rCmd.Subcommands.Add(cmdRestore);
            rCmd.Parse(args).Invoke();
        }
    }
}
