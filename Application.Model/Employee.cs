using System;
using System.Collections.ObjectModel;

namespace Application.Model
{
    public class Employee
    {
        public int? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public DateTime BirthDate { get; set; }

        public static ObservableCollection<Employee> EmployeesCollection = new ObservableCollection<Employee>();

        public Employee(string firstName, string lastName, string patronymic, DateTime birthDate, int? id = null) 
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Patronymic = patronymic;
            BirthDate = birthDate;
        }
    }
}
