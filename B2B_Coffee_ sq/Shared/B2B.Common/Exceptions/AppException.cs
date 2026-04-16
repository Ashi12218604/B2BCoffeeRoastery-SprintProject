using System;
using System.Net;

namespace B2B.Common.Exceptions;

public class AppException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public AppException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) 
        : base(message)
    {
        StatusCode = statusCode;
    }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message) 
        : base(message, HttpStatusCode.NotFound) { }
}

public class BadRequestException : AppException
{
    public BadRequestException(string message) 
        : base(message, HttpStatusCode.BadRequest) { }
}

public class UnauthorizedAppException : AppException
{
    public UnauthorizedAppException(string message = "Unauthorized access.") 
        : base(message, HttpStatusCode.Unauthorized) { }
}
