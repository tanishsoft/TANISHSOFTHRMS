using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;

namespace WebApplicationHsApp.Models
{


    public class UpcLabel
    {
        private string strFirstName;
        private string strLastName;
        private string strNoOfCopies;
        private string PickUpTime;
        private string Cold;
        private string NewMember;
        public UpcLabel()
        {
        }
        public UpcLabel(string strFirstName, string strLastName, string strNoOfCopies, string PickUpTime, string Cold, string NewMember)
        {
            if (strFirstName == null || strLastName == null || strNoOfCopies == "" || strNoOfCopies == "0")
            {
                throw new ArgumentNullException("strFirstName");
            }
            this.strFirstName = strFirstName;
            this.strLastName = strLastName;
            this.strNoOfCopies = strNoOfCopies;
            this.PickUpTime = PickUpTime;
            this.Cold = Cold;
            this.NewMember = NewMember;
        }
        public string PrintBarcode(string printerName, string pProductName, string pBarcode, string strNumOfCopies)
        {
            try
            {
                return RawPrinterHelper.SendStringToPrinter(printerName, pBarcode.ToString());
                //return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public void Print(string printerName)
        {
            if (printerName == null)
            {
                throw new ArgumentNullException("printerName");
            }
            StringBuilder sb1 = new StringBuilder();
            //^XA=Indicates Starting of Zpl
            sb1.AppendLine("^XA");
            sb1.AppendLine("^LL350");//^FS
            sb1.AppendLine("^PW930");//^FS
            sb1.AppendLine("^FO10,10");
            sb1.AppendLine("^AQ,80,80");
            sb1.AppendLine(string.Format(CultureInfo.InvariantCulture, "^FD{0}^FS", this.strFirstName));
            sb1.AppendLine("^FO10,68");
            sb1.AppendLine("^AQ,80,80");
            sb1.AppendLine(string.Format(CultureInfo.InvariantCulture, "^FD{0}^FS", this.strLastName));
            sb1.AppendLine("^FO10,150");
            sb1.AppendLine("^AQ,50,50");
            sb1.AppendLine(string.Format(CultureInfo.InvariantCulture, "^FD{0}^FS", this.PickUpTime));
            sb1.AppendLine("^FO240,150");
            sb1.AppendLine("^AQ,50,50");
            sb1.AppendLine(string.Format(CultureInfo.InvariantCulture, "^FD{0}^FS", this.Cold));
            sb1.AppendLine(string.Format(CultureInfo.InvariantCulture, "^PQ{0}", this.strNoOfCopies));
            sb1.AppendLine("^XZ");
            RawPrinterHelper.SendStringToPrinter(printerName, sb1.ToString());
            if (this.NewMember != "")
            {
                StringBuilder strb = new StringBuilder();
                strb.AppendLine("^XA");
                strb.AppendLine("^LL350");//^FS
                strb.AppendLine("^PW930");//^FS
                strb.AppendLine("^FO20,30");
                strb.AppendLine("^AQ,80,80");
                strb.AppendLine(string.Format(CultureInfo.InvariantCulture, "^FD{0}^FS", this.NewMember));
                strb.AppendLine("^FO20,150");
                strb.AppendLine("^AQ,50,50");
                strb.AppendLine(string.Format(CultureInfo.InvariantCulture, "^FD{0}^FS", this.PickUpTime));
                strb.AppendLine("^FO240,150");
                strb.AppendLine("^AQ,50,50");
                strb.AppendLine(string.Format(CultureInfo.InvariantCulture, "^FD{0}^FS", this.Cold));
                //^PQ2= Indicates number of copies to print
                strb.AppendLine(string.Format(CultureInfo.InvariantCulture, "^PQ{0}", "1"));
                //sb1.AppendLine("^PQ2");
                //^XZ=Indicates ending of ZPL page
                strb.AppendLine("^XZ");
                RawPrinterHelper.SendStringToPrinter(printerName, strb.ToString());
            }
        }
    }
    public class RawPrinterHelper
    {
        // Structure and API declarions:
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);
        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);
        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);
        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);
        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);
        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);
        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);
        // SendBytesToPrinter()
        // When the function is given a printer name and an unmanaged array
        // of bytes, the function sends those bytes to the print queue.
        // Returns true on success, false on failure.
        public static string SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            string message = "";
            try
            {
                Int32 dwError = 0, dwWritten = 0;
                IntPtr hPrinter = new IntPtr(0);
                DOCINFOA di = new DOCINFOA();
                bool bSuccess = false; // Assume failure unless you specifically succeed.
                di.pDocName = "Asset Label Print";
                di.pDataType = "RAW";
                // Open the printer.
                if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
                {
                    message = "Printer is avalible";
                    // Start a document.
                    if (StartDocPrinter(hPrinter, 1, di))
                    {
                        message = "Staring the print";
                        // Start a page.
                        if (StartPagePrinter(hPrinter))
                        {
                            // Write your bytes.
                            bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                            EndPagePrinter(hPrinter);
                        }
                        EndDocPrinter(hPrinter);
                    }
                    ClosePrinter(hPrinter);
                }
                else {
                    message = "Printer not avalible";
                }
                // If you did not succeed, GetLastError may give more information
                // about why not.
                if (bSuccess == false)
                {
                    dwError = Marshal.GetLastWin32Error();
                }
            }
            catch (Exception ex) {
                message = ex.Message;
            }
            return message;
        }
        //public static bool SendFileToPrinter(string szPrinterName, string szFileName)
        //{
        //    // Open the file.
        //    FileStream fs = new FileStream(szFileName, FileMode.Open);
        //    // Create a BinaryReader on the file.
        //    BinaryReader br = new BinaryReader(fs);
        //    // Dim an array of bytes big enough to hold the file's contents.
        //    Byte[] bytes = new Byte[fs.Length];
        //    bool bSuccess = false;
        //    // Your unmanaged pointer.
        //    IntPtr pUnmanagedBytes = new IntPtr(0);
        //    int nLength;
        //    nLength = Convert.ToInt32(fs.Length);
        //    // Read the contents of the file into the array.
        //    bytes = br.ReadBytes(nLength);
        //    // Allocate some unmanaged memory for those bytes.
        //    pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
        //    // Copy the managed byte array into the unmanaged array.
        //    Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
        //    // Send the unmanaged bytes to the printer.
        //    var msg = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);
        //    if(msg="")
        //    // Free the unmanaged memory that you allocated earlier.
        //    Marshal.FreeCoTaskMem(pUnmanagedBytes);
        //    return bSuccess;
        //}
        public static string SendStringToPrinter(string szPrinterName, string szString)
        {
            string message = "";
            IntPtr pBytes;
            Int32 dwCount;
            // How many characters are in the string?
            dwCount = (szString.Length + 1) * Marshal.SystemMaxDBCSCharSize;//szString.Length;
                                                                            // Assume that the printer is expecting ANSI text, and then convert
                                                                            // the string to ANSI text.
            pBytes = Marshal.StringToCoTaskMemAnsi(szString);
            // Send the converted ANSI string to the printer.
            message= SendBytesToPrinter(szPrinterName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
            return message;
        }
    }
    //public void PrintLabel(string strFirstName, string strLastName, string strNoOfCopies, string PickUpTime, string Cold, string NewMember = "")
    //{
    //    UpcLabel lbl = new UpcLabel(strFirstName, strLastName, strNoOfCopies, PickUpTime, Cold, NewMember);
    //    //Printer name
    //    lbl.Print("ZDesigner QLn220 (ZPL)");
    //}
    //public void PrintBarcode(string pProductName, string pLocation, string pNoOfCopies)
    //{
    //    UpcLabel upcLabel = new UpcLabel();
    //    //Printer name
    //    upcLabel.PrintBarcode("ZDesigner QLn220 (ZPL)", pProductName, pLocation, pNoOfCopies);
    //}

}