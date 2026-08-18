namespace tagr.Exceptions
{
    public abstract class AppException : Exception
    {
        protected AppException(string message) : base(message) { }
    }
    public class NotFoundException : AppException
    {
        public NotFoundException(string message) : base(message) { }

        public NotFoundException(string entityName, object key)
            : base($"{entityName} with id '{key}' was not found.") { }
    }
    public class DuplicateEntityException : AppException
    {
        public string FieldName { get; }

        public DuplicateEntityException(string fieldName, string message) : base(message)
        {
            FieldName = fieldName;
        }
    }
    public class BusinessRuleException : AppException
    {
        public BusinessRuleException(string message) : base(message) { }
    }
}
