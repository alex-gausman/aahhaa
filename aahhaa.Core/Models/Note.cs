namespace aahhaa.Core.Models;

public class Note
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Text { get; set; }
    public Guid CreatorId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
}
