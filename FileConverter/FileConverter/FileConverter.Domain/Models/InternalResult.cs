namespace FileConverter.Domain.Models
{
    using System.Collections.Generic;
    using System;

    public class InternalResult<T>
    {
        private readonly HashSet<string> errors = new HashSet<string>();
        private readonly Dictionary<string, object> parameters = new Dictionary<string, object>();

        public InternalResult()
        {
                
        }
        public InternalResult(T data, int code = 200)
        {
            Data = data;
            IsSuccess = true;
            Code = code;
        }


        public InternalResult(T data, int code, string message, bool isSuccess)
        {
            Message = message;
            Data = data;
            IsSuccess = isSuccess;
            Code = code;
        }

        public InternalResult(string message, int code, string type, string error, string url = null)
            : this(message, code, type)
        {
            if (string.IsNullOrWhiteSpace(error))
            {
                throw new ArgumentNullException($"{nameof(InternalResult<T>)}.{nameof(Errors)}");
            }

            errors.Add(error);
        }

        private InternalResult(string message, int code, string type)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException($"{nameof(InternalResult<T>)}.{nameof(Message)}");
            }

            if (string.IsNullOrWhiteSpace(type))
            {
                throw new ArgumentNullException($"{nameof(InternalResult<T>)}.{nameof(Type)}");
            }

            Message = message;
            Code = code;
            Type = type;
        }

        public T Data { get; }

        public bool IsSuccess { get; }

        public int Code { get; }

        public string Type { get; }

        public string Message { get; }

        public IEnumerable<string> Errors
        { get { return new HashSet<string>(errors); } /*private set { errors = value; }*/ }
    }
}