//�������������� ����� ��������� WebApplicationBuilder ������ � �������������� ������������ ����������� �� ���������.
using Application.Server.Middlewares;
using Application.Server.Model;

var builder = WebApplication.CreateBuilder(args);

//��������� ������ ��� ������������ � ��������� IServiceCollection.
//���� ����� �� ������������ ������, ������������ ��� ������������� ��� �������.
builder.Services.AddControllers();

var app = builder.Build();

//Middleware ���������� ��������� ������
app.UseMiddleware<ExceptionHandlingMiddleware>();

//��������� �������� ����� ��� �������� ����������� � IEndpointRouteBuilder �� �������� ��������.
app.MapControllers();

app.Run();
