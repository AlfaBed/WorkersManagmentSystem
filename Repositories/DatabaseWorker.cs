namespace MyFirstApp.Repositories
{
    public class EmployeeDatabase
    {

        public EmployeeDatabase(List<IEmployee> employees)
        {
            Employees = employees;
        }

        public List<IEmployee> Employees { get; private set; }

        public void PrintEmployees()
        {
            Console.WriteLine("Список всех сотрудников предприятия:\n");
            var separator = new string('-', 25);  
            foreach (var employee in Employees)
            {
                Console.WriteLine(employee + "\n" + separator + "\n");
            }
        }

        public List<IEmployee> GetEmpsByIndex(object inID)
        {
            var emps = Employees.Where(e => e.InnerId.ToString() == inID.ToString()).ToList();
            return emps;
        }

        public List<IEmployee>? GetAllEmployees()
        {
            return Employees.OrderByDescending(e => e.BirthDate).ToList();
        }

        public bool IsInDatabase(string name, DateTime birthDate)
        {
            return Employees.Any(e => e.Name == name && e.BirthDate == birthDate);
        }

        public bool IsInDatabase(object inID)
        {
            return Employees.Any(e => e.InnerId.Equals(inID));
        }

        public bool Add(IEmployee empToAdd)
        {
            if (!EmployeeAddValidate(empToAdd)) return false;
            Employees.Add(empToAdd);
            return true;
        }

        public int AddManyTransaction(List<IEmployee> empsToAdd)
        {
            if (empsToAdd.Any(emp => IsInDatabase(emp.InnerId))) return -1;
            if (empsToAdd.Any(emp => IsInDatabase(emp.Name, emp.BirthDate))) return -1;
            if (empsToAdd.Any(emp => emp.Name == "" || emp.WorkSpaceName == "")) return -1;
            Employees.AddRange(empsToAdd);
            return empsToAdd.Count;
        }

        public int AddManyPossible(List<IEmployee> empsToAdd)
        {
            int counter = 0;
            foreach (IEmployee emp in empsToAdd)
            {
                if (Add(emp)) counter++;
            }
            if (counter == 0) return -1;
            return counter;
        }

        public IEmployee? Edit(IEmployee oldEmp, IEmployee newEmp)
        {
            if (!oldEmp.InnerId.Equals(newEmp.InnerId)
                && IsInDatabase(newEmp.InnerId)) return null;

            if (oldEmp.Name != newEmp.Name) oldEmp.Name = newEmp.Name;
            if (oldEmp.BirthDate != newEmp.BirthDate) oldEmp.BirthDate = newEmp.BirthDate;
            if (oldEmp.WorkSpaceName != newEmp.WorkSpaceName) oldEmp.WorkSpaceName = newEmp.WorkSpaceName;
            if (!oldEmp.InnerId.Equals(newEmp.InnerId)) oldEmp.InnerId = newEmp.InnerId;
            if (newEmp.BirthDate != oldEmp.BirthDate) oldEmp.BirthDate = newEmp.BirthDate;

            return oldEmp;
        }

        public bool EmployeeAddValidate(IEmployee empToAdd)
        {
            if (IsInDatabase(empToAdd.InnerId)) return false;
            if (IsInDatabase(empToAdd.Name, empToAdd.BirthDate)) return false;
            if (empToAdd.Name == "" || empToAdd.WorkSpaceName == "") return false;

            return true;
        }

        public void Remove(object idToRemove)
        {
            var foundEmp = Employees.FirstOrDefault(e => e.InnerId.Equals(idToRemove));
            if (foundEmp is not null) Employees.Remove(foundEmp);
        }
    }
}
