using aahhaa.Core.Models;

namespace aahhaa.Api.Models.Notes;

public class CreateNoteRequest
{
    public string? Title { get; set; }
    public string? Text { get; set; }

    public Note MapToNote(Guid creatorId)
    {
        return new Note
        {
            Id = Guid.NewGuid(),
            CreatedOn = DateTime.UtcNow,
            ModifiedOn = DateTime.UtcNow,
            CreatorId = creatorId,
            Text = Text,
            Title = Title
        };
    }
}
