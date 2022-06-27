using System.Data.SqlClient;

namespace Application.Server.Model
{
    /// <summary>
    /// Класс подключения к базе данных
    /// </summary>
    public class EmployeeDatabase
    {
        public static string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStr"].ConnectionString;

        public string? CommandString;

    }
}
