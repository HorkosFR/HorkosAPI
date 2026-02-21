namespace HorkosAPI.Database.Services;
public class ConfigurationService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ConfigurationService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public virtual string DatabaseConnexionString { get => _configuration["Application:DatabaseConnectionString"] ?? ""; }

    public virtual string CurrentHost => $"{_httpContextAccessor.HttpContext?.Request.Scheme}://{_httpContextAccessor.HttpContext?.Request.Host.Value}";
}