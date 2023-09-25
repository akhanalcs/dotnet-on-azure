namespace MunsonPickles.Web.Models;

public class AzureStorageConfigOpts
{
    public const string AzureStorageConfig = "AzureStorageConfig";
    
    public string CdnEndpoint { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
}