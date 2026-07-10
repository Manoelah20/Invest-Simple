namespace InvestSimples.Api.Configuration;

public class BrapiOptions
{
    public const string SectionName = "Brapi";
    
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://brapi.dev/api";
}