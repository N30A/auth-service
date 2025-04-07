using System.Net;

namespace Api.Endpoints;

public class ApiNoDataResponse
{
    public string Message { get; set; } = string.Empty;
    public HttpStatusCode StatusCode { get; set; }
}
