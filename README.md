# altarctic-vault
An attempt to create a tool similar to GitHub Arctic Vault. It however uses standard V40 QRCodes and generates a PDF for easy storage in DMS. The tool provides both the digitization mechanism and Restoration of the original artifact again. It's best suited to work with binary files suc as exe, msi, zip, tar etc.

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
    
