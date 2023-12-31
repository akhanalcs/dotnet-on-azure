namespace MunsonPickles.Shared.Entities;

public class Review
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string? UserId { get; set; }
    public string Text { get; set; } = string.Empty;
    public Product Product { get; set; } = null!;
    public ICollection<ReviewPhoto>? Photos { get; set; }
}