using System.Text.Json;

namespace Entities.ErrorModel
{
    public class ErrorDetails
    {
        public string? Message { get; set; }
        public int StatusCode { get; set; }
        public override string ToString()
        => JsonSerializer.Serialize(this);
    }
}
