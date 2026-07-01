using MyFirstApp.UI;
using MyFirstApp.Repositories;
using MainMenu = MyFirstApp.UI.MainMenu;
using MyFirstApp.FileWorker;

namespace MyFirstApp
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

            EmployeeDatabase dbData = new(new());
            MainMenu menu = new(dbData);


            menu.PrintMainMenu();
        }
    }
}
