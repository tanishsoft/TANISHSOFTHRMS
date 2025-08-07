using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace WebApplicationHsApp.Models
{

    [StructLayout(LayoutKind.Sequential)]
    public struct DOCINFO
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pDocName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pOutputFile;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pDataType;
    }


    public class PrintDirect
    {
        [DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = false,
       CallingConvention = CallingConvention.StdCall)]
        public static extern long OpenPrinter(string pPrinterName, ref IntPtr phPrinter, int pDefault);

        [DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = false,
       CallingConvention = CallingConvention.StdCall)]
        public static extern long StartDocPrinter(IntPtr hPrinter, int Level, ref DOCINFO pDocInfo);

        [DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = true,
       CallingConvention = CallingConvention.StdCall)]
        public static extern long StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Ansi, ExactSpelling = true,
       CallingConvention = CallingConvention.StdCall)]
        public static extern long WritePrinter(IntPtr hPrinter, string data, int buf, ref int pcWritten);

        [DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = true,
       CallingConvention = CallingConvention.StdCall)]
        public static extern long EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = true,
       CallingConvention = CallingConvention.StdCall)]
        public static extern long EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = true,
       CallingConvention = CallingConvention.StdCall)]
        public static extern long ClosePrinter(IntPtr hPrinter);
    }
}