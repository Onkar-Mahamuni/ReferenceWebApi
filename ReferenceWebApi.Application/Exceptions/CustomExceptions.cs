namespace ReferenceWebApi.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }

        public NotFoundException(string name, object key)
            : base($"{name} with id '{key}' was not found")
        {
        }
    }

    public class ValidationException : Exception
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException()
            : base("One or more validation failures have occurred")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IDictionary<string, string[]> errors) : this()
        {
            Errors = errors;
        }
    }

    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message)
        {
        }
    }
}
