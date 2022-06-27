//Инициализирует новый экземпляр WebApplicationBuilder класса с предварительно настроенными настройками по умолчанию.
using Application.Server.Middlewares;
using Application.Server.Model;

var builder = WebApplication.CreateBuilder(args);

//Добавляет службы для контроллеров в указанный IServiceCollection.
//Этот метод не регистрирует службы, используемые для представлений или страниц.
builder.Services.AddControllers();

var app = builder.Build();

//Middleware глобальной обработки ошибок
app.UseMiddleware<ExceptionHandlingMiddleware>();

//Добавляет конечные точки для действий контроллера в IEndpointRouteBuilder не указывая маршруты.
app.MapControllers();

app.Run();
