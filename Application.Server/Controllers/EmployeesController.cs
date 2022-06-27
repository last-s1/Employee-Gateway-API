using Application.Model;
using Application.Server.Model;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GateWayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private ILogger<EmployeesController> _logger;

        public EmployeesController(ILogger<EmployeesController> logger)
        {
            _logger = logger;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetEmployeesList()
        {
            ResponseMessage response = new ResponseMessage();
            try
            {
                response.StatusCode = 200;
                response.Message = "Список сотрудников успешно получен!";
                response.Content = JsonSerializer.Serialize(await EmployeeService.GetEmployeesList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response.StatusCode = 500;
                response.Message = "Ошибка: не удалось получить список сотрудников!";
            }
            return Content(JsonSerializer.Serialize(response));
        }    

        [HttpGet("list/{filter}")]
        public async Task<IActionResult> GetEmployeesFilteredFioList([FromRoute] string filter)
        {
            ResponseMessage response = new ResponseMessage();
            try
            {
                response.StatusCode = 200;
                response.Message = "Отфильтрованный cписок сотрудников успешно получен!";
                response.Content = JsonSerializer.Serialize(await EmployeeService.GetEmployeesFioFilteredList(filter));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response.StatusCode = 500;
                response.Message = "Ошибка: не удалось получить отфильтрованный список сотрудников!";
            }
            return Content(JsonSerializer.Serialize(response));
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddEmployee(Employee EmployeeJson)
        {
            ResponseMessage response = new ResponseMessage();
            try
            {
                response.IdObject = await EmployeeService.AddEmployee(EmployeeJson);
                response.StatusCode = 201;
                response.Message = "Сотрудник успешно добавлен!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response.StatusCode = 500;
                response.Message = "Ошибка: не удалось выполнить операцию добавления!";
            }
            return Content(JsonSerializer.Serialize(response));
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateEmployee(Employee EmployeeJson)
        {
            ResponseMessage response = new ResponseMessage();
            try
            {
                await EmployeeService.UpdateEmployee(EmployeeJson);
                response.StatusCode = 200;
                response.Message = "Информация о сотруднике успешно обновлена!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response.StatusCode = 500;
                response.Message = "Ошибка: не удалось выполнить операцию обновления!";
            }
            return Content(JsonSerializer.Serialize(response));
        }

        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> DeleteEmployee([FromRoute] int id)
        {
            ResponseMessage response = new ResponseMessage();

            try
            {
                await EmployeeService.DeleteEmployee(id);
                response.StatusCode = 200;
                response.Message = "Сотрудник успешно удален!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response.StatusCode = 500;
                response.Message = "Ошибка: не удалось выполнить операцию удаления!";
            }
            return Content(JsonSerializer.Serialize(response));
        }
    }
}
