[![Another Arctic Vault](arctic-vault-banner.png)](#)
[![.NET](https://img.shields.io/badge/.NET-8.0%2C%209.0%2C%2010.0-512BD4)](#)
[![language](https://img.shields.io/badge/language-C%23-239120)](https://learn.microsoft.com/ru-ru/dotnet/csharp/tour-of-csharp/overview)
[![OS](https://img.shields.io/badge/OS-linux%2C%20windows%2C%20macOS-0078D4)](#)
[![CPU](https://img.shields.io/badge/CPU-x86%2C%20x64%2C%20ARM%2C%20ARM64-FF8C00)](#)
[![GitHub release](https://img.shields.io/github/v/release/Prasad-Khandekar/altarctic-vault?style=flat)](#)
[![GitHub release date](https://img.shields.io/github/release-date/Prasad-Khandekar/altarctic-vault?style=flat)](#)
[![GitHub last commit](https://img.shields.io/github/last-commit/Prasad-Khandekar/altarctic-vault?style=flat)](#)
[![License](https://img.shields.io/github/license/Prasad-Khandekar/altarctic-vault?color=blue)](LICENSE.)
[![Free](https://img.shields.io/badge/free_for_commercial_use-brightgreen)](#-license)

⭐ Star us on GitHub — your support motivates us a lot! 🙏😊

[![Share](https://img.shields.io/badge/share-000000?logo=x&logoColor=white)](https://x.com/intent/tweet?text=Check%20out%20this%20project%20on%20GitHub:%20https://github.com/Prasad-Khandekar/altarctic-vault)
[![Share](https://img.shields.io/badge/share-1877F2?logo=facebook&logoColor=white)](https://www.facebook.com/sharer/sharer.php?u=https://github.com/Prasad-Khandekar/altarctic-vault)
[![Share](https://img.shields.io/badge/share-0A66C2?logo=linkedin&logoColor=white)](https://www.linkedin.com/sharing/share-offsite/?url=https://github.com/Prasad-Khandekar/altarctic-vault)
[![Share](https://img.shields.io/badge/share-FF4500?logo=reddit&logoColor=white)](https://www.reddit.com/submit?title=Check%20out%20this%20project%20on%20GitHub:%20https://github.com/Prasad-Khandekar/altarctic-vault)
[![Share](https://img.shields.io/badge/share-0088CC?logo=telegram&logoColor=white)](https://t.me/share/url?url=https://github.com/Prasad-Khandekar/altarctic-vault&text=Check%20out%20this%20project%20on%20GitHub)

# Arctic Vault Alternative
The DigiVault<sup>&reg;</sup> is a OpenSource APL 2.0 licensed application for preserving source code in PDF for long archiving purpose. This application is inspired from GitHub Arctic Vault project. However since 2D Boxing barcode used in Arctic Vault is proprietary IP protected mechanism this application uses simple Version 40 QR Code with Binary Data and Lower error recovery settings to accommodate large bytes in a single QR. The application uses all open source libraries such as PdfPig, ZXing.Net and SkiaSharp to accomplish this. The application also supports restoration mode in which it recovers the original binary artifact from the PDF file. The application has only been tested on Windows<sup>&reg;</sup> but should be able to run on Linux OS as well. The application has been developed using DotNet core 9.0. The rest of the document contains the documented source code of this application.

## THE Project
Since this is a .NET Core application obviously you need to have .NET Core 9.0 installed on your machine. A Good IDE is also required to quickly navigate across the source code. The application also depends on couple of third party Open Source libraries so its important you add those packages. The libraries used and command to add them in project is as shown below.


| Library | Version | Command |
| ------- | ------- | ------- |
| ZXing.Net | 0.16.11 | <code>dotnet add package ZXing.Net --verson 0.11.11</code> |
| ZXing.Net.Bindings.SkiaSharp | 0.16.22 | <code>dotnet add package ZXing.Net.Bindings.SkiaSharp --verson 0.16.22</code> |
| SkiaSharp | 3.119.2 | <code>dotnet add package SkiaSharp --version 3.119.2</code> |
| PdfPig | 0.1.14 | <code>dotnet add package PdfPig --version 0.1.14</code> |
| System.CommandLine | 2.0.7 | <code>dotnet add package System.CommandLine --version 2.0.7</code> |

The source code organization of the application is as outlined below

```mermaid
treeView-beta
"altarctic-vault"
    "bin"
    "src"
        "Globals.cs"
        "BinaryToQrPdf.cs"
        "Program.cs"
        "QrCodeItem.cs"
        "QRPdfToBinary.cs"
    "DigiVault.ico"
    "DigiVault.sln"
    "qrpdf.csproj"
    "README.md"
    "LICENSE"
```

## Usage
The tool offers two modes

 1. **Digitization** mode - In this mode the tool reads the supplied binary file QRTizes it and puts the resulting PDF in the specified folder. The name of the PDF is same as the name of the file being degitized. The command takes two arguments
     - The first argument is a full path and name of the binary file to be QRTized
     - The second argument is a full path of the folder in which the QRTized PDF is to be placed.

**Sample Command**
```
DigiVault digitize <Full_path_to_Binary_Source> <Full_Path_of_Output_Folder>
```

 2. **Restoration** mode - In this mode tools reads the supplied QRTized PDF and recreates the original binary and finally puts the restored binary file in the spcified folder. The command takes three arguments.
    - The first argument is a full path and name of the QRTized PDF file
    - The second argument is a full path of the folder in which the restored binary is to be placed.
    - The last argument is a optional argument which identifies the extension of the restored binary file.
**Sample Command**
```
DigiVault restore <Full_path_to_QRTized_PDF> <Full_Path_of_Output_Folder> <Extension>
```   
    
