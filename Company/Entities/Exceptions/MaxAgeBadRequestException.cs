namespace Entities.Exceptions
{
    public class MaxAgeBadRequestException : BadRequestException
    {
        public MaxAgeBadRequestException() : base("Enter a valid age range")
        {
        }
    }
}
