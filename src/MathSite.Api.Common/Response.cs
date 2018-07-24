using System;

namespace MathSite.Api.Common
{
    public class Response<T> where T: class
    {
        public string Status { get; }
        public T Data { get; }

        public Response(string status, T data)
        {
            Status = status;
            Data = data;
        }
    }
}
