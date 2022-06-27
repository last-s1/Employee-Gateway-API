using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Application.WPF.Model;
using Application.WPF.View;
using Application.Model;
using System.Net.Http;
using System.Text.Json;

namespace Application.WPF.ViewModel
{
    internal class ViewModelManager : ViewModel
    {

        #region BINDING OBJECTS

        private string _title = "Справочник сотрудников";
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private ObservableCollection<Employee> _allEmployees = Employee.EmployeesCollection;
        public ObservableCollection<Employee> AllEmployees
        {
            get
            {
                return _allEmployees;
            }
            set
            {
                _allEmployees = value;
                OnPropertyChanged("AllEmployees");
            }
        }

        public static int? EmployeeId { get; set; }
        public static string EmployeeFirstName { get; set; }
        public static string EmployeeLastName { get; set; }
        public static string EmployeePatronymic { get; set; }
        public static DateTime EmployeeBirthDate { get; set; }

        public static void EmployeeClearParams()
        {
            EmployeeId = null;
            EmployeeFirstName = null;
            EmployeeLastName = null;
            EmployeePatronymic = null;
            EmployeeBirthDate = default(DateTime);
        }

        private Employee _selectedEmployee;
        /// <summary>
        /// Выбранный сотрудник в списке сотрудников
        /// </summary>
        public Employee SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged("SelectedEmployee");
            }
        }

        private static string s_filterFIO = "";
        private string _filterFIO;
        public string FilterFIO
        {
            get { return _filterFIO; }
            set
            {
                _filterFIO = value;
                s_filterFIO = value;
                OnPropertyChanged("FilterFIO");
                GetEmployeesFilteredFioView(_filterFIO);
            }
        }
        #endregion

        #region COMMANDS TO OPERATE WINDOW
        private void OpenWindow(Window window)
        {
            window.Owner = System.Windows.Application.Current.MainWindow;
            window.Show();
        }

        private void CloseCurrentWindow()
        {
            foreach (Window item in System.Windows.Application.Current.Windows)
                if (item.DataContext == this)
                    item.Close();
        }

        /// <summary>
        /// Определяет назначение формы сотрудника (Добавление\Изменение)
        /// </summary>
        private static string _modeEmployeeForm;

        private void OpenEmployeeFormMethod()
        {
            EmployeeForm  newWindow = new EmployeeForm();
            OpenWindow(newWindow);
            if (_modeEmployeeForm == "Edit")
            {
                newWindow.Title = "Изменение сотрудника";
                newWindow.txtFirstName.Text = SelectedEmployee.FirstName;
                newWindow.txtLastName.Text = SelectedEmployee.LastName;
                newWindow.txtPatronymic.Text = SelectedEmployee.Patronymic;
                newWindow.datePickerBirth.SelectedDate = SelectedEmployee.BirthDate;
            }

        }

        private RelayCommand _openAddEmployeeFormCommand;
        /// <summary>
        /// Команда открытия формы создания сотрудника
        /// </summary>
        public RelayCommand OpenAddEmployeeFormCommand
        {
            get
            {
                return _openAddEmployeeFormCommand ?? new RelayCommand(act =>
                {
                    _modeEmployeeForm = "Add";
                    EmployeeClearParams();
                    OpenEmployeeFormMethod();
                });
            }
        }

        private RelayCommand _openEditEmployeeFormCommand;
        /// <summary>
        /// Команда открытия формы изменения информации о сотруднике
        /// </summary>
        public RelayCommand OpenEditEmployeeFormCommand
        {
            get
            {
                return _openEditEmployeeFormCommand ?? new RelayCommand(act =>
                {
                    if (SelectedEmployee != null)
                    {
                        _modeEmployeeForm = "Edit";
                        OpenEmployeeFormMethod();

                        EmployeeId = SelectedEmployee.Id;
                        EmployeeFirstName = SelectedEmployee.FirstName;
                        EmployeeLastName = SelectedEmployee.LastName;
                        EmployeePatronymic = SelectedEmployee.Patronymic;
                        EmployeeBirthDate = SelectedEmployee.BirthDate;
                    }

                });
            }
        }
        #endregion

        #region OPERATE EMPLOYEE

        private static string _urlGateWayAPI = System.Configuration.ConfigurationManager.ConnectionStrings["SyntellectServer"].ConnectionString;

        /// <summary>
        /// Выполнить GET запрос на получение списка сотрудников
        /// </summary>
        public static async void GetEmployeesView()
        {
            using (var client = new HttpClient())
            {
                var response = await HttpHelper.RequestGetAsync(client, _urlGateWayAPI + "list");

                string responseString = await response.HttpResponse.Content.ReadAsStringAsync();

                ResponseMessage responseMessage = JsonSerializer.Deserialize<ResponseMessage>(responseString);

                List<Employee> employees = JsonSerializer.Deserialize<List<Employee>>(responseMessage.Content);

                Employee.EmployeesCollection.Clear();

                foreach (var employee in employees)
                    Employee.EmployeesCollection.Add(employee);
            }
        }

        /// <summary>
        /// Выполнить GET запрос на получение отфильтрованного по ФИО списка сотрудников
        /// </summary>
        public static async void GetEmployeesFilteredFioView(string filter)
        {
            if (filter != "")
            {
                using (var client = new HttpClient())
                {
                    var response = await HttpHelper.RequestGetAsync(client, _urlGateWayAPI + "list/" + filter);

                    string responseString = await response.HttpResponse.Content.ReadAsStringAsync();

                    ResponseMessage responseMessage = JsonSerializer.Deserialize<ResponseMessage>(responseString);

                    List<Employee> employees = JsonSerializer.Deserialize<List<Employee>>(responseMessage.Content);

                    Employee.EmployeesCollection.Clear();

                    foreach (var employee in employees)
                        Employee.EmployeesCollection.Add(employee);
                }
            }
            else
                GetEmployeesView();
        }

        private RelayCommand _employeeCommand;
        /// <summary>
        /// Команда добавления/изменения сотрудника
        /// </summary>
        public RelayCommand EmployeeCommand
        {
            get
            {
                return _employeeCommand ?? new RelayCommand(obj =>
                {
                    
                   if (EmployeeFirstName == null || EmployeeFirstName.Replace(" ", "").Length == 0 || EmployeeLastName == null || EmployeeLastName.Replace(" ", "").Length == 0)
                    {
                        MessageBox.Show("Ошибка: Параметры указаны неверно!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    else
                    {
                        Employee employee = new Employee(EmployeeFirstName, EmployeeLastName, EmployeePatronymic, EmployeeBirthDate,EmployeeId);
                        if (_modeEmployeeForm == "Add")
                            ResponseAddEmployee(employee);
                        else
                            ResponseUpdateEmployee(employee);

                    }
                    CloseCurrentWindow();
                });
            }
        }

        private RelayCommand _deleteEmploeesCommand;
        /// <summary>
        /// Выполнить DELETE запрос на удаления выбранного сотрудника
        /// </summary>
        public RelayCommand DeleteEmploeesCommand
        {
            get
            {
                return _deleteEmploeesCommand ?? new RelayCommand(act =>
                {
                    if (SelectedEmployee != null)
                    {
                        MessageBoxResult result = MessageBox.Show("Вы действительно желаете удалить выбранные элемент?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.No) return;
                        else
                            ResponseDeleteEmployee(SelectedEmployee);
                    }
                });
            }
        }

        /// <summary>
        /// Выполнить POST запрос на добавление нового сотрудника
        /// </summary>
        /// <param name="employee"></param>
        public static async void ResponseAddEmployee(Employee employee)
        {
            using (var client = new HttpClient())
            {
                var response = await Model.HttpHelper.RequestPostAsync(client, _urlGateWayAPI + "add", employee);

                string responseString = await response.HttpResponse.Content.ReadAsStringAsync();

                ResponseMessage responseMessage = JsonSerializer.Deserialize<ResponseMessage>(responseString);

                if (responseMessage.StatusCode == 201)
                {
                    employee.Id = responseMessage.IdObject;
                    Employee.EmployeesCollection.Add(employee);
                    MessageBox.Show(responseMessage.Message);
                }
                else
                    MessageBox.Show(responseMessage.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Выполнить PUT запрос на редактировании информации о сотруднике
        /// </summary>
        /// <param name="employee"></param>
        public static async void ResponseUpdateEmployee(Employee employee)
        {
            using (var client = new HttpClient())
            {
                var response = await Model.HttpHelper.RequestPutAsync(client, _urlGateWayAPI + "update", employee);

                string responseString = await response.HttpResponse.Content.ReadAsStringAsync();

                ResponseMessage responseMessage = JsonSerializer.Deserialize<ResponseMessage>(responseString);

                if (responseMessage.StatusCode == 200)
                {
                    //Обновление списка сотрудников
                    GetEmployeesFilteredFioView(s_filterFIO);
                    MessageBox.Show(responseMessage.Message);
                }
                else
                    MessageBox.Show(responseMessage.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Выполнить DELETE запрос на удаление записи сотрудника
        /// </summary>
        /// <param name="employee"></param>
        public static async void ResponseDeleteEmployee(Employee employee)
        {
            using (var client = new HttpClient())
            {
                var response = await Model.HttpHelper.RequestDeleteAsync(client, _urlGateWayAPI + $"delete/{employee.Id}");

                string responseString = await response.HttpResponse.Content.ReadAsStringAsync();

                ResponseMessage responseMessage = JsonSerializer.Deserialize<ResponseMessage>(responseString);

                if (responseMessage.StatusCode == 200)
                {
                    Employee.EmployeesCollection.Remove(Employee.EmployeesCollection.Where(x => x.Id == employee.Id).First());
                    MessageBox.Show(responseMessage.Message);
                }
                else
                    MessageBox.Show(responseMessage.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
