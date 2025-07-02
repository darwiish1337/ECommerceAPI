namespace Accounts.Infrastructure.Configuration;

public class EmailSettings
{
    public string SmtpServer { get; set; } = default!;
    public int Port { get; set; }
    public bool UseSsl { get; set; }
    public string SenderEmail { get; set; } = default!;
    public string Password { get; set; } = default!;
}
