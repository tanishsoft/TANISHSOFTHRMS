using System;
using System.Globalization;

namespace WebApplicationHsApp.Models
{
    public static class ProjectConvert
    {
        static CultureInfo provider = CultureInfo.InvariantCulture;
        public static DateTime ConverDateStringtoDatetime(string date)
        {
            return DateTime.ParseExact(date, "dd/MM/yyyy", provider);
        }
        public static DateTime ConverDateStringtoDatetimeMobile(string date)
        {
            return DateTime.ParseExact(date, "yyyy-MM-dd", provider);
        }
        public static DateTime ConverDateStringtoDatetime(string date,string format)
        {
            return DateTime.ParseExact(date, format, provider);
        }
        public static string ConverDateTimeToString(DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }
    }
}