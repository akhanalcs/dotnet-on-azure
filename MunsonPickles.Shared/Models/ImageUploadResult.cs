namespace MunsonPickles.Shared.Models;

public class ImageUploadResult
{
    public bool Uploaded { get; set; }
    public string? FileName { get; set; }
    public string? StoredFileName { get; set; }
    public string? StoredFileUrl { get; set; }
    public string? ErrorMessage { get; set; }
}