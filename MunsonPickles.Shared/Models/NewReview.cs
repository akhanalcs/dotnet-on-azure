namespace MunsonPickles.Shared.Models;

public class NewReview
{
    public string ReviewText { get; set; } = string.Empty;
    public int ProductId { get; set; } = 0;
    public List<string> PhotoUrls { get; set; }= new List<string>();
}