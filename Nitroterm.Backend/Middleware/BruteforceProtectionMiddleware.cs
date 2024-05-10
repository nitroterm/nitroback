namespace Nitroterm.Backend.Middleware;

public class BruteforceProtectionMiddleware(RequestDelegate next)
{
    private Dictionary<string, RemoteAddressAccess> _accesses = new();

    public async Task Invoke(HttpContext context)
    {
        foreach (string key in _accesses.Keys.Where(k => _accesses[k].LastAccessTimeSince.TotalMinutes >= 1).ToArray())
            _accesses.Remove(key);
        
        string remoteIpAddress = context.Connection.RemoteIpAddress!.ToString();
        if (_accesses.TryGetValue(remoteIpAddress, out RemoteAddressAccess? access))
        {
            access.AccessCount++;
        }
        else
        {
            _accesses.Add(remoteIpAddress, new RemoteAddressAccess(1, DateTime.Now));
        }

        if (access != null && access.AccessCount > 50 && access.LastAccessTimeSince.TotalSeconds < 5f)
        {
            access.LastAccessTime = DateTime.Now;
            _accesses[remoteIpAddress] = access; // to be sure
            
            context.Response.StatusCode = 429;
            
            return;
        }

        if (access != null)
        {
            access.LastAccessTime = DateTime.Now;
            _accesses[remoteIpAddress] = access; // to be sure
        }
        
        await next(context);
    }

    public class RemoteAddressAccess(int accessCount, DateTime lastAccessTime)
    {
        public int AccessCount { get; set; } = accessCount;
        public DateTime LastAccessTime { get; set; } = lastAccessTime;
        
        public TimeSpan LastAccessTimeSince => DateTime.Now - LastAccessTime;
    }
}