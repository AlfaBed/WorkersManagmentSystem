namespace MyFirstApp
{
    public class RandNames
    {

        private static List<string> Names = ["Mihail", "Anton", "Evgeni", "Egor"];

        private static List<string> SecondNames = ["Famov", "Arstov", "Nudov"];
        public static string GetRNDName()
        {
            string nameAndSecondName = Names[new Random().Next(0, Names.Count)] + " " +
                SecondNames[new Random().Next(0,SecondNames.Count)];
            return nameAndSecondName;
        }
    }
    public class EmployeeRandomizer
    {
        public static DateTime GetRandomDate(bool isBirthDate) 
        {
            int year  = (isBirthDate == true) ? new Random().Next(1995, 2001) : new Random().Next(2010, 2027);
            int month = new Random().Next(1, 13);
            int maxMonthDay = DateTime.DaysInMonth(year, month);
            int day = new Random().Next(1, maxMonthDay + 1);
            return new DateTime(year, month, day);
        }

        public static string GetRandomJob()
        {
            List<string> jobs = ["Student", "Pilot", "SoftwareDeveloper", "HR", "CEO"];
            return jobs[new Random().Next(0, jobs.Count)];
        }

        public static int GetIDInt()
        {
            return new Random().Next(100000, 999999);
        }
        public static string GetIDString()
        {
            var id = Random.Shared.GetString("abcdefzsl", 6);
            return id;
        }

        public static Employee<int> GenerateEmployeeInt()
        {
            var name = RandNames.GetRNDName();
            DateTime birthDate = GetRandomDate(true);
            string job = GetRandomJob();
            int id = GetIDInt();
            DateTime workDate = GetRandomDate(false);
            return new Employee<int> (name, birthDate, job, id, workDate);
        }
        public static Employee<string> GenerateEmployeeString()
        {
            var name = RandNames.GetRNDName();
            DateTime birthDate = GetRandomDate(true);
            string job = GetRandomJob();
            string id = GetIDString();
            DateTime workDate = GetRandomDate(false);
            return new Employee<string>(name, birthDate, job, id, workDate);
        } 
    }
}
