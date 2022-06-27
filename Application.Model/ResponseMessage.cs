
namespace Application.Model
{
    public class ResponseMessage
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public int? IdObject { get; set; }
        public string Content { get; set; }
    }
}
