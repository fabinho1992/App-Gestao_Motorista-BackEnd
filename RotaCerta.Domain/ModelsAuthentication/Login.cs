namespace RotaCerta.Domain.ModelsAuthentication;

public class Login
{
    public Login(string email, string senha)
    {
        Email = email;
        Senha = senha;
    }

    public string Email { get; private set; }
    public string Senha { get; private set; }
}
