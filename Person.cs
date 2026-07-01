namespace MyFirstApp
{
    public class Person
    {
        public static readonly DateTime birthMinDate = new(1970, 1, 1);
        public static readonly DateTime birthMaxDate = new(2008, 1, 1);
        protected DateTime birthDate;
        public string Name { get; set; }
        public virtual DateTime BirthDate 
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

        public Person(string name, DateTime birthDate)
        {
            Name = name;
            BirthDate = birthDate;
        }
    }
}
