using Application.Model;
using System.Data.SqlClient;
using System.Data;

namespace Application.Server.Model
{
    /// <summary>
    /// Класс реализующий логику взаимодействия с сущностью базы данных Employee
    /// </summary>
    public class EmployeeService
    {

        /// <summary>
        /// Получить полный список сотрудников
        /// </summary>
        /// <returns></returns>
        public static async Task<List<Employee>> GetEmployeesList()
        {
            List<Employee> employees = new List<Employee>();

            EmployeeDatabase db = new EmployeeDatabase();
            db.CommandString = "SELECT Id, FirstName, LastName, Patronymic, BirthDate FROM Employee";

            using (SqlConnection connection = new SqlConnection(EmployeeDatabase.ConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(db.CommandString, connection);
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            employees.Add(new Employee(reader["FirstName"].ToString(),
                                                        reader["LastName"].ToString(),
                                                        reader["Patronymic"].ToString(),
                                                        Convert.ToDateTime(reader["BirthDate"]),
                                                        Convert.ToInt32(reader["Id"])));
                        }
                    }
                }

            }

            return employees;
        }

        /// <summary>
        /// Получить список сотрудников отфильтрованных по ФИО
        /// </summary>
        /// <param name="filterFIO"></param>
        /// <returns></returns>
        public static async Task<List<Employee>> GetEmployeesFioFilteredList(string filterFIO)
        {
            List<Employee> employees = new List<Employee>();

            EmployeeDatabase db = new EmployeeDatabase();
            db.CommandString = @"SELECT Id, FirstName, LastName, Patronymic, BirthDate 
                                    FROM Employee 
                                 WHERE UPPER(CONCAT(LastName,FirstName,Patronymic)) LIKE CONCAT('%',UPPER(REPLACE(@Filter,' ','')),'%')";

            using (SqlConnection connection = new SqlConnection(EmployeeDatabase.ConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(db.CommandString, connection);
                command.Parameters.Add("@Filter", SqlDbType.VarChar).Value = filterFIO;
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            employees.Add(new Employee(reader["FirstName"].ToString(),
                                                        reader["LastName"].ToString(),
                                                        reader["Patronymic"].ToString(),
                                                        Convert.ToDateTime(reader["BirthDate"]),
                                                        Convert.ToInt32(reader["Id"])));
                        }
                    }
                }

            }

            return employees;
        }

        public static async Task<int> AddEmployee(Employee employee)
        {
            EmployeeDatabase db = new EmployeeDatabase();
            db.CommandString = @"INSERT INTO Employee (FirstName,LastName,Patronymic,BirthDate) 
                                    VALUES(@FirstName,@LastName,@Patronymic,@BirthDate) 
                                SET @Id = SCOPE_IDENTITY()";

            using (SqlConnection connection = new SqlConnection(EmployeeDatabase.ConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(db.CommandString, connection);
                command.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = employee.FirstName;
                command.Parameters.Add("@LastName", SqlDbType.VarChar).Value = employee.LastName;
                command.Parameters.Add("@Patronymic", SqlDbType.VarChar).Value = employee.Patronymic == null ? DBNull.Value : employee.Patronymic;
                command.Parameters.Add("@BirthDate", SqlDbType.Date).Value = employee.BirthDate;
                command.Parameters.Add("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;

                await command.ExecuteNonQueryAsync();

                return (int)command.Parameters["@Id"].Value;
            }
        }

        public static async Task UpdateEmployee(Employee employee)
        {
            EmployeeDatabase db = new EmployeeDatabase();
            db.CommandString = @"UPDATE Employee SET FirstName = @FirstName, 
                                                     LastName = @LastName, 
                                                     Patronymic = @Patronymic, 
                                                     BirthDate = @BirthDate 
                                            WHERE Id = @Id";

            using (SqlConnection connection = new SqlConnection(EmployeeDatabase.ConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(db.CommandString, connection);
                command.Parameters.Add("@Id", SqlDbType.Int).Value = employee.Id;
                command.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = employee.FirstName;
                command.Parameters.Add("@LastName", SqlDbType.VarChar).Value = employee.LastName;
                command.Parameters.Add("@Patronymic", SqlDbType.VarChar).Value = employee.Patronymic;
                command.Parameters.Add("@BirthDate", SqlDbType.Date).Value = employee.BirthDate;

                await command.ExecuteNonQueryAsync();
            }
        }

        public static async Task DeleteEmployee(int employeeId)
        {
            EmployeeDatabase db = new EmployeeDatabase();
            db.CommandString = "DELETE FROM Employee WHERE Id = @Id";

            using (SqlConnection connection = new SqlConnection(EmployeeDatabase.ConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(db.CommandString, connection);
                command.Parameters.Add("@Id", SqlDbType.Int).Value = employeeId;

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
