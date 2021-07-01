using System;

namespace DynamicsCrmIntegration.Models
{
    public class ErrorResponse
    {
        public ErrorResponse()
        {
        }

        public ErrorResponse(Exception e)
        {
            StackTrace = e.StackTrace;
            Message = e.Message;
            Source = e.Source;
        }


        public string StackTrace { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
    }
}
