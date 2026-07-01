using MyFirstApp.FileWorker;
using MyFirstApp.Repositories;
using System.Windows.Forms;

namespace MyFirstApp.UI
{
    public class MainMenu
    {
        private static EmployeeDatabase repository;

        public MainMenu(EmployeeDatabase db)
        {
            repository = db;
        }

        public void PrintMainMenu()
        {
            bool appIsWorking = true;
            var separator = new string('-', 25);
            bool needRedraw = true;

            while (appIsWorking)
            {
                if (needRedraw)
                {
                    Console.Clear();
                    PrintColored(separator + "Main menu" + separator,ConsoleColor.Green,true);
                    PrintColored("Выберите вариант:\n",ConsoleColor.Blue,true);
                    PrintColored("1. Вывести всех сотрудников\n",ConsoleColor.Green,true);
                    PrintColored("2. Вывести сотрудника по ID\n", ConsoleColor.Green, true);
                    PrintColored("3. Добавить сотрудника/сотрудников\n", ConsoleColor.Green, true);
                    PrintColored("4. Изменить сотрудника\n", ConsoleColor.Green, true);
                    PrintColored("5. Удалить сотрудника по ID\n", ConsoleColor.Green, true);
                    PrintColored("6. Загрузить сотрудников из файла БД\n", ConsoleColor.Green, true);
                    PrintColored("7. Сохранить сотрудников в файл БД\n",ConsoleColor.Green, true);
                    PrintColored("8. Закончить работу", ConsoleColor.Red, true);
                    PrintColored(separator +"---------" + separator, ConsoleColor.Green);
                    needRedraw = false;
                }

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.D1:
                        {
                            PrintEmployee(repository.GetAllEmployees());
                            Console.ReadKey(true);
                            needRedraw = true;
                            continue;
                        }
                    case ConsoleKey.D2:
                        {
                            Console.Clear();
                            Console.WriteLine("Введите ID сотрудника которого хотите найти");
                            string idChoose = Console.ReadLine();
                            var empsById = repository.GetEmpsByIndex(idChoose);
                            PrintEmployee(empsById);
                            Console.ReadKey(true);
                            needRedraw = true;
                            continue;
                        }
                    case ConsoleKey.D3:
                        {
                            PrintAddingEmployee();
                            needRedraw = true;
                            continue;
                        }
                    case ConsoleKey.D4:
                        {
                            EditEmployee();
                            needRedraw = true;
                            continue;
                        }
                    case ConsoleKey.D5:
                        {
                            DeleteEmployee();
                            needRedraw = true;
                            continue;
                        }
                    case ConsoleKey.D6:
                        {
                            LoadFromFile();
                            needRedraw = true;
                            continue;
                        }
                    case ConsoleKey.D7:
                        {
                            SaveToFile();
                            needRedraw = true;
                            continue;
                        }
                    case ConsoleKey.D8:
                        {
                            appIsWorking = false;
                            break;
                        }

                    default: continue;
                }
            }

        }

        public void LoadFromFile()
        {

            using FolderBrowserDialog dialog = new();
            dialog.Description = "Веберите папку с файлом БД";
            dialog.ShowNewFolderButton = true;
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK) 
            {
                var path = dialog.SelectedPath;
                JSONWorker worker = new JSONWorker();
                var temp = worker.ReadFromFileJSON(path);
                repository.AddManyPossible(temp);
            }
        }

        public void SaveToFile()
        {
            using FolderBrowserDialog dialog = new();
            dialog.Description = "Веберите папку для сохранения сотрудников";
            dialog.ShowNewFolderButton = true;
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                var path = dialog.SelectedPath;
                JSONWorker worker = new JSONWorker(repository.GetAllEmployees());
                worker.SaveToFileJson(path);
            }
        }

        public static void PrintAddingEmployee()
        {

            bool addingIsWorking = true;
            Console.Clear();
            Console.WriteLine(" 1. Ввести данные нового сотрудника:\n");
            Console.WriteLine(" 2. Вернуться");
            while (addingIsWorking) {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.D1:
                        {
                            string name = string.Empty;
                            bool inputIsValid = false;
                            while (!inputIsValid)
                            {
                                Console.Clear();
                                Console.WriteLine("Введите имя и фамилию: ");
                                name = Console.ReadLine();
                                if (name.Length < 7 || name.Length > 30)
                                {
                                    PrintColored("Фамилия и имя должны быть от 8 до 29 символов",ConsoleColor.Red,true);
                                    Console.ReadKey();
                                    continue;
                                }
                                inputIsValid = true;
                            }
                            inputIsValid = false;

                            DateTime birthDate = new();
                            while (!inputIsValid)
                            {
                                Console.Clear();
                                Console.WriteLine("Введите дату рождения в формате DD.MM.YYYY: ");
                                string birthDateStr = Console.ReadLine();
                                birthDate = ConvertStringToDate(birthDateStr);
                                if (birthDate < Person.birthMinDate || birthDate > Person.birthMaxDate)
                                {
                                    PrintColored($"Дата рождения должна быть в диапазоне" +
                                        $" от {Person.birthMinDate:d} до {Person.birthMaxDate:d}",ConsoleColor.Red,true);
                                    Console.ReadKey();
                                    continue;
                                }
                                inputIsValid = true;
                            }
                            inputIsValid = false;

                            Console.Clear();
                            Console.WriteLine("Введите должность: ");
                            var workName = Console.ReadLine();

                            Console.Clear();
                            Console.WriteLine("Случайный id: ");
                            int randIdType = new Random().Next(1, 3);
                            object id;
                            if (randIdType == 1) id = EmployeeRandomizer.GetIDString();

                            else id = EmployeeRandomizer.GetIDInt();
                            Console.Write(id);

                            DateTime workDate = new();
                            inputIsValid = false;
                            while (!inputIsValid)
                            {
                                Console.Clear();
                                Console.WriteLine("Введите дату приема на работу в формате DD.MM.YYYY: ");
                                var workDateStr = Console.ReadLine();
                                workDate = ConvertStringToDate(workDateStr);

                                if (workDate > DateTime.Now)
                                {
                                    PrintColored("Дата према не может быть больше текущей",ConsoleColor.Red,true);
                                    Console.ReadKey();
                                    continue;
                                }
                                if (workDate < birthDate.AddYears(18))
                                {
                                    PrintColored("Возраст сотрудника меньше 18 лет", ConsoleColor.Red, true);
                                    Console.ReadKey();
                                    continue;
                                }
                                if (workDate > birthDate.AddYears(100))
                                {
                                    PrintColored("Сотрудник умер от старости", ConsoleColor.Red, true);
                                    Console.ReadKey();
                                    continue;
                                }
                                else inputIsValid = true;
                            }

                            try {
                                if (randIdType == 1)
                                {
                                    Employee<string> stringEmployee = new(name, birthDate, workName, (string)id, workDate);
                                    repository.Add(stringEmployee);
                                    addingIsWorking = false;
                                    continue;
                                }
                                else
                                {
                                    Employee<int> intEmployee = new(name, birthDate, workName, (int)id, workDate);
                                    repository.Add(intEmployee);
                                    addingIsWorking = false;
                                    continue;
                                }
                            }
                            catch
                            {
                                PrintColored("Ошибка при создании нового пользователя", ConsoleColor.Red, true);
                                continue;
                            }
                            }
                    case ConsoleKey.D2:
                        {
                            addingIsWorking = false;
                            break;
                        }
                    default: continue;
                }
            }
        }

        public static void PrintEmployee(List<IEmployee>? emps)
        {
            var separator = new string('-', 25);
            Console.Clear();
            Console.WriteLine("Сотрудники предприятия:\n");
            Console.WriteLine(separator);
            foreach (var emp in emps) 
            {
                Console.WriteLine(emp);
                Console.WriteLine(separator);
            };
        }

        public void EditEmployee()
        {
            bool editMenuIsWorking = true;
            while (editMenuIsWorking)
            {
                Console.Clear();
                PrintEmployee(repository.GetAllEmployees());
                Console.WriteLine("Выберите действие:\n1. Изменить сотрудника по ID\n2. Выйти");
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.D1:
                        {
                            Console.WriteLine("Введите ID сотруднка которого хотите изменить");
                            var inputID = Console.ReadLine();
                            Console.Clear();
                            IEmployee? empOld = repository.GetEmpsByIndex(inputID).First();
                            Console.WriteLine($"\nВыбран сотрудник с ID {inputID}: ");
                            Console.WriteLine(empOld);

                            Console.WriteLine($"Выберите что хотите изменить:\n1.Имя\n2.Дату рождения" +
                                $"\n3.Название должности\n4.ID\n5.Дата приема");

                            switch (Console.ReadKey(true).Key)
                            {
                                case ConsoleKey.D1:
                                    {
                                        Console.WriteLine("Введите новое имя: ");
                                        string newName = Console.ReadLine() ?? "";
                                        empOld.Name = newName;
                                        continue;
                                    }
                                case ConsoleKey.D2:
                                    {
                                        bool isBirthDateCorrect = false;
                                        while (!isBirthDateCorrect)
                                        {
                                            Console.WriteLine("Введите дату рождения в формате DD.MM.YYYY: ");
                                            DateTime newBirthDate = ConvertStringToDate(Console.ReadLine());
                                            var temp = empOld.ApplicationDate;
                                            empOld.BirthDate = newBirthDate;

                                            if (empOld.BirthDate != newBirthDate)
                                            {
                                                if (empOld.ApplicationDate.AddYears(-18) < newBirthDate)
                                                {
                                                    PrintColored("Возраст при приеме на работу меньше 18", ConsoleColor.Red, true);
                                                    Console.ReadKey(true);
                                                    continue;
                                                }
                                                else PrintColored($"Возраст должен быть в диапазоне " +
                                                    $"от {Person.birthMinDate:d} до {Person.birthMaxDate:d}", ConsoleColor.Red, true);
                                                Console.WriteLine("1. Да\n2. Нет");
                                                Console.ReadKey();
                                            }
                                            else
                                            {
                                                isBirthDateCorrect = true;
                                                break;
                                            }
                                        }
                                        continue;
                                    }
                                case ConsoleKey.D3:
                                    {
                                        Console.WriteLine("Введите должность: ");
                                        string newWorkName = Console.ReadLine() ?? "";
                                        empOld.WorkSpaceName = newWorkName;
                                        continue;
                                    }
                                case ConsoleKey.D4:
                                    {
                                        Console.WriteLine("Введите ID: ");
                                        string newId = Console.ReadLine() ?? "";
                                        if (repository.IsInDatabase(newId))
                                        {
                                            PrintColored("Сотрудник с таким ID уже существует", ConsoleColor.Red, true);
                                            continue;
                                        }
                                        else empOld.InnerId = newId;
                                        continue;
                                    }
                                case ConsoleKey.D5:
                                    {
                                        Console.WriteLine("Введите дату приема в формате DD.MM.YYYY:");
                                        DateTime newAppDate = ConvertStringToDate(Console.ReadLine());
                                        empOld.ApplicationDate = newAppDate;
                                        continue;
                                    }
                                default: continue;
                            }
                        }
                    case ConsoleKey.D2:
                        {
                            editMenuIsWorking = false;
                            break;
                        }
                }
                
            }

        }

        public void DeleteEmployee()
        {
            Console.Clear();
            PrintEmployee(repository.GetAllEmployees());
            Console.WriteLine("Введите ID сотрудника которого хотите удалить: ");
            var id = Console.ReadLine();
            Console.Clear();
            IEmployee? empOld = repository.GetEmpsByIndex(id).FirstOrDefault();
            if (empOld != null) repository.Remove(empOld.InnerId);

        }
        public static void PrintColored(string text, ConsoleColor color,bool isNewLine = false)
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            if (isNewLine)
            {
                Console.WriteLine(text);
            }
            else Console.Write(text);
            Console.ForegroundColor = defaultColor;
        }
        public static DateTime ConvertStringToDate(string dateStr)
        {
            try
            {
                string[] dateParts = dateStr.Split('.');
                int day = int.Parse(dateParts[0]);
                int month = int.Parse(dateParts[1]);
                int year = int.Parse(dateParts[2]);

                return new DateTime(year, month, day);
            }
            catch (Exception)
            {
                return new();
            }
        }
    }
}
