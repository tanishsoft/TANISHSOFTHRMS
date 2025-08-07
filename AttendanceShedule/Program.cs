namespace AttendanceSchedule
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                UpdateAttendance model = new UpdateAttendance();
                model.UpdateAttendanceAndRoaster();
            }
            catch { }

        }
    }
}
