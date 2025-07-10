public interface IUserActionLogger
{
    Task LogAsync(string action, string pageUrl, string userId, string ipAddress);
}
