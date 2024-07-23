namespace WebAPI;

public static class AppSettings
{
    public static string? DefaultConnection { get; set; }
    public static TimeSpan ConnectionOpenWaitTime { get; set; } = TimeSpan.FromMilliseconds(200);
    public static int CommectionTimeOut { get; set; } = 30;
}
