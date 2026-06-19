namespace RotaCerta.Domain.ModelsAuthentication;

public class ResponseLogin
{
    public string? Status { get; set; }
    public string? DisplayName { get; set; }
    public string? Message { get; set; }
    public string? Token { get; set; }
    public DateTime Expired { get; set; }
    public Guid? MotoristaId { get; set; }  // ← confirma que existe
}
