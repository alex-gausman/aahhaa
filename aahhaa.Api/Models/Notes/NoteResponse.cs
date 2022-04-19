using aahhaa.Core.Models;

namespace aahhaa.Api.Models.Notes
{
    public class NoteResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public Guid CreatorId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }

        public static NoteResponse From(Note note)
        {
            return new NoteResponse()
            {
                Id = note.Id,
                Title = note.Title,
                Text = note.Text,
                CreatorId = note.CreatorId,
                CreatedOn = note.CreatedOn,
                ModifiedOn = note.ModifiedOn
            };
        }
    }
}
