namespace Simple.Ecommerce.Domain.Errors.BaseError
{
    public class Error
    {
        public string Type { get; set; }
        public string Message { get; set; }

        public Error() { }

        public Error(string type, string message)
        {
            Type = type;
            Message = message;
        }
    }
}
