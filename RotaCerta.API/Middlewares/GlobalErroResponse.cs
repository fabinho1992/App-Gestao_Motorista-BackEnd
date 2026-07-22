// RotaCerta.API/Middlewares/GlobalErroResponse.cs

namespace RotaCerta.API.Middlewares;

public class GlobalErroResponse
{
    public GlobalErroResponse(string message)
    {
        Message = message;
        TraceId = Guid.NewGuid().ToString();
    }

    public bool IsSuccess { get; private set; } = false;
    public string Message { get; private set; }
    public string TraceId { get; private set; }
}