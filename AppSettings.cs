namespace WebAPI;

public class AppSettings
{
    public string? DefaultConnection { get; set; }
    public TimeSpan ConnectionOpenWaitTime { get; set; } = TimeSpan.FromMilliseconds(200);
    public int CommectionTimeOut { get; set; } = 30;
}
