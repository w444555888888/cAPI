using System;

namespace BookingApi.Utils
{
    public class AppException : Exception 
    {
        public int Status {get;}
        public object Details {get;}

        public AppException(int status, string message, object details= null) : base(message)
        {
            Status = status;
            Details = details;
        }
    }
}