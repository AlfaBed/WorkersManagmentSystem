using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyFirstApp
{
    public class Employee<T> : Person, IEmployee, IEquatable<Employee<T>>
    {
        private string workSpaceName;
        private DateTime appDate;
        private T innId;

        public override DateTime BirthDate
        {
            get
            {
                return birthDate;
            }
            set
            {
                if (value > birthMinDate && value < birthMaxDate) birthDate = value;
            }
        }

        [JsonConverter(typeof(ObjectToPrimitiveConverter))]
        public object InnerId
        {
            get
            {
                return innId;
            }
            set
            {
                if (value is null) return;
                List<char> bannedSymbols = ['!', '?', '*', '%'];
                var temp = value.ToString();
                if (int.TryParse(temp, out int _))
                {
                    if (temp.Length == 6) innId = (T)value;
                }
                else if (temp.All(s => !bannedSymbols.Contains(s))) innId = (T)value;
                else throw new ArgumentException();
            }
        }

        public string WorkSpaceName
        {
            get => workSpaceName;
            set
            {
                if (value.Length > 0) workSpaceName = value;
            }
        }

        public DateTime ApplicationDate {
            get => appDate; 
            set
            {
                if (value <= DateTime.Now.AddMonths(-1)) appDate = value;
                else ApplicationDate = EmployeeRandomizer.GetRandomDate(false);
            }
        }

        public Employee(string name, DateTime birthDate, string workName, T Id, DateTime appDate) : base(name, birthDate)
        {
            WorkSpaceName = workName;
            InnerId = Id;
            ApplicationDate = appDate;
        }

        public override string ToString()
        {
            return $"{WorkSpaceName} {Name}\nДата рождения: {BirthDate.ToShortDateString()}\n" +
                $"Дата приема на работу: {ApplicationDate.ToShortDateString()}\nID: {InnerId}\n{GetExpString()}";
        }

        public string GetExpString()
        {
            string dateFormated = string.Empty;
            int experienceYear = DateTime.Now.Year - ApplicationDate.Year;
            int experienceMonth = DateTime.Now.Month - ApplicationDate.Month;
            int experienceDays = DateTime.Now.Day - ApplicationDate.Day;
            bool isFullYear = true;
            bool isFullMonth = true;
            if (ApplicationDate.Month >= DateTime.Now.Month)
            {
                if (ApplicationDate.Month > DateTime.Now.Month)
                {
                    isFullYear = false;
                }
                else
                {
                    if (ApplicationDate.Day > DateTime.Now.Day)
                    {
                        isFullYear = false;
                        isFullMonth = false;
                    }
                }
            }

            if (!isFullYear) --experienceYear;
            if (!isFullMonth) --experienceMonth;
            experienceYear = Math.Abs(experienceYear);
            experienceMonth = Math.Abs(experienceMonth);
            experienceDays = Math.Abs(experienceDays);
            string yearFormatted = string.Empty;
            string monthFormatted = string.Empty;
            string dayFormatted = string.Empty;

            yearFormatted = (experienceYear % 100, experienceYear % 10) switch
            {
                ( >= 11 and <= 19, _) => "лет",
                (_, 1) => "год",
                (_, >= 2 and <= 4) => "года",
                _ => "лет"
            };
            monthFormatted = experienceMonth switch
            {
                1 => "месяц",
                >= 2 and <= 4 => "месяца",
                _ => "месяцев"
            };
            dayFormatted = experienceDays switch
            {
                1 or 21 or 31 => "день",
                (>= 2 and <= 4) or (>= 22 and <= 24) => "дня",
                _ => "дней"
            };

            if (experienceMonth == 0)
            {
                dateFormated = $"Работает {experienceYear} {yearFormatted}";
                if (experienceDays > 0) dateFormated += $" и {experienceDays} {dayFormatted}";
            }
            else if (experienceYear == 0)
            {
                dateFormated = $"Работает {experienceMonth} {monthFormatted}";
                if (experienceDays > 0) dateFormated += $" и {experienceDays} {dayFormatted}";
            }
            else
            {
                dateFormated = $"Работает {experienceYear} {yearFormatted} {experienceMonth} {monthFormatted} и {experienceDays} {dayFormatted}";
            }
            return dateFormated;
        }
        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            return Equals(obj as Employee<T>);
        }

        public bool Equals(Employee<T>? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<T>.Default.Equals(innId, other.innId);
        }
    }
    public interface IEmployee
    {
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string WorkSpaceName { get; set; }
        public object InnerId { get; set; }
        public DateTime ApplicationDate { get; set; }
    }

    public class ObjectToPrimitiveConverter : JsonConverter<object>
    {
        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.Number => reader.TryGetInt64(out long l) ? l : reader.GetDouble(),
                JsonTokenType.True => true,
                JsonTokenType.False => false,
                JsonTokenType.String => reader.GetString(),
                _ => JsonDocument.ParseValue(ref reader).RootElement.Clone()
            };
        }
        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.GetType(), options);
        }
    }

    public class RawJsonContainer
    {
        public string Name { get; set; }
        public DateTime BirthDate {  get; set; }
        public JsonElement InnerId { get; set; }
        public string WorkSpaceName { get; set; }
        public DateTime ApplicationDate { get; set; }

        public static List<IEmployee> ConvertToIEmployee(List<RawJsonContainer> inputList)
        {
            if (inputList == null || inputList.Count == 0) return [];
            List<IEmployee> output = [];

            foreach (var jsonEmp in inputList)
            {
                output.Add(jsonEmp.InnerId.ValueKind switch
                {
                    JsonValueKind.Number =>
                        new Employee<int>(jsonEmp.Name, jsonEmp.BirthDate,
                        jsonEmp.WorkSpaceName, jsonEmp.InnerId.GetInt32(), jsonEmp.ApplicationDate),
                    _ =>
                        new Employee<string>(jsonEmp.Name, jsonEmp.BirthDate,
                        jsonEmp.WorkSpaceName, jsonEmp.InnerId.GetString() ?? "undefined", jsonEmp.ApplicationDate)
                });
            }
            return output;
        }
    }

    //public static class ContainerFactory
    //{
    //    public static Employee<T> CreateFromJson<T>(string json)
    //    {
    //        var raw = JsonSerializer.Deserialize<RawJsonContainer>(json);
    //        string? name = JsonSerializer.Deserialize<string>(raw.Name);
    //        DateTime birthDate = JsonSerializer.Deserialize<DateTime>(raw.BirthDate);
    //        string? workSpaceName = JsonSerializer.Deserialize<string>(raw.WorkSpaceName);
    //        T typedValue = JsonSerializer.Deserialize<T>(raw.InnerId.GetRawText());
    //        DateTime applicationDate = JsonSerializer.Deserialize<DateTime>(raw.ApplicationDate);
    //        return new Employee<T>(name, birthDate, workSpaceName, typedValue, applicationDate);
    //    }
    //}
}