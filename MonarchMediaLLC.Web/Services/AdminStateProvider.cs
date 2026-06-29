namespace MonarchMediaLLC.Web.Services;

public class AdminStateProvider
{
    public bool IsAuthenticated { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public string Username { get; private set; } = string.Empty;

    public void Login(string username, string token)
    {
        IsAuthenticated = true;
        Token = token;
        Username = username;
    }

    public void Logout()
    {
        IsAuthenticated = false;
        Token = string.Empty;
        Username = string.Empty;
    }
}