using System.Text.Json;
using MyFirstApp.UI;
using MainMenu = MyFirstApp.UI.MainMenu;

namespace MyFirstApp.FileWorker
{
    public class JSONWorker(List<IEmployee>? emps = null)
    {
        private List<IEmployee> employees = emps ?? [];

        const string DBname = "workersDB.json";

        private readonly JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public List<IEmployee>? ReadFromFileJSON(string path)
        {
            var temp = path + DBname;
            if (!File.Exists(temp))
            {
                MainMenu.PrintColored("Файл не найден, создать новый? Y/N (Y по умолчанию)", ConsoleColor.Green, true);
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.N:
                        {
                            return null;
                        }
                    default:
                        {
                            File.Create(path + DBname);
                            break;
                        }
                }
                employees = new List<IEmployee>();
                return employees;
            }
            string jsonString = File.ReadAllText(path + DBname);

            var deserializedList = JsonSerializer.Deserialize<List<RawJsonContainer>>(jsonString);
            var empResult = RawJsonContainer.ConvertToIEmployee(deserializedList);

            return empResult;
        }

        public bool SaveToFileJson(string path)
        {
            string fullPath = path + DBname;
            try
            {
                string jsonString = JsonSerializer.Serialize(employees, options);
                File.WriteAllText(fullPath, jsonString);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

    }
}
