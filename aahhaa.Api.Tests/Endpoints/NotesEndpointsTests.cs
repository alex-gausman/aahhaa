using aahhaa.Api.Endpoints;
using aahhaa.Api.Models.Notes;
using aahhaa.Api.Models.Notes.Validators;
using aahhaa.Core.Models;
using aahhaa.Shared.Data.Interfaces;
using FluentAssertions;
using MinimalApis.Extensions.Results;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;

namespace aahhaa.Api.Tests.Endpoints
{
    [TestFixture]
    public class NotesEndpointsTests
    {
        private NotesEndpoints? _notesEndpoints;
        private Mock<IRepository<Note>>? _mockNotesRepository;

        [SetUp]
        public void Setup()
        {
            _notesEndpoints = new NotesEndpoints();
            _mockNotesRepository = new Mock<IRepository<Note>>();
        }

        [Test]
        public async Task CreateNoteAsync_ReturnsCreated()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var createNoteRequest = new CreateNoteRequest { Text = "Unit Test", Title = "Note Title" };
            var validator = new CreateNoteRequestValidator();

            _mockNotesRepository.Setup(r => r.CreateAsync(It.IsAny<Note>()));

            // Act
            var result =  await _notesEndpoints.CreateNoteAsync(userId, createNoteRequest, validator, _mockNotesRepository.Object);

            // Assert
            result.Should().NotBeNull().And.BeOfType<Created<NoteResponse>>();

            var actualNoteResponse = result as Created<NoteResponse>;
            actualNoteResponse.Value.CreatorId.Should().Be(userId);
            actualNoteResponse.Value.Id.Should().NotBeEmpty();
            actualNoteResponse.Value.Text.Should().Be("Unit Test");
            actualNoteResponse.Value.Title.Should().Be("Note Title");
            actualNoteResponse.Value.CreatedOn.Should().BeWithin(TimeSpan.FromSeconds(5));
            actualNoteResponse.Value.ModifiedOn.Should().BeWithin(TimeSpan.FromSeconds(5));
        }

        [Test]
        public async Task CreateNoteAsync_ReturnsBadRequest_WhenModelValidationFails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var createNoteRequest = new CreateNoteRequest { Title = "" };
            var validator = new CreateNoteRequestValidator();

            _mockNotesRepository.Setup(r => r.CreateAsync(It.IsAny<Note>()));

            // Act
            var result = await _notesEndpoints.CreateNoteAsync(userId, createNoteRequest, validator, _mockNotesRepository.Object);

            // Assert
            result.Should().NotBeNull().And.BeOfType<BadRequest>();
            result.As<BadRequest>().ResponseContent.Should().Be("'Title' must not be empty.");
        }

        [Test]
        public async Task GetByIdAsync_ReturnsOk()
        {
            // Arrange
            var noteId = Guid.NewGuid();
            var expectedNote = new Note
            {
                Id = noteId,
                CreatedOn = DateTime.UtcNow.Subtract(TimeSpan.FromDays(5)),
                ModifiedOn = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)),
                CreatorId = Guid.NewGuid(),
                Text = "Unit Test Text",
                Title = "Unit Test Title"
            };

            _mockNotesRepository
                .Setup(r => r.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedNote);

            // Act
            var result = await _notesEndpoints.GetByIdAsync(noteId, _mockNotesRepository.Object);

            // Assert
            result.Should().NotBeNull().And.BeOfType<Ok<NoteResponse>>();

            var actualNoteResponse = result as Ok<NoteResponse>;
            actualNoteResponse.Value.CreatorId.Should().Be(expectedNote.CreatorId);
            actualNoteResponse.Value.Id.Should().Be(expectedNote.Id);
            actualNoteResponse.Value.Text.Should().Be(expectedNote.Text);
            actualNoteResponse.Value.Title.Should().Be(expectedNote.Title);
            actualNoteResponse.Value.CreatedOn.Should().Be(expectedNote.CreatedOn);
            actualNoteResponse.Value.ModifiedOn.Should().Be(expectedNote.ModifiedOn);
        }

        [Test]
        public async Task GetByIdAsync_ReturnsNotFound_WhenRequestedNoteIsMissing()
        {
            // Arrange
            var noteId = Guid.NewGuid();
            Note? expectedNote = null;

            _mockNotesRepository
                .Setup(r => r.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedNote);

            // Act
            var result = await _notesEndpoints.GetByIdAsync(noteId, _mockNotesRepository.Object);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NotFound>();
            result.As<NotFound>().ResponseContent.Should().BeNullOrEmpty();
        }

        [Test]
        public async Task GetAllNotesAsync_ReturnsOk()
        {
            // Arrange
            var creatorId = Guid.NewGuid();
            var expectedNotes = new List<Note>
            {
                new Note
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.UtcNow.Subtract(TimeSpan.FromDays(5)),
                    ModifiedOn = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)),
                    CreatorId = creatorId,
                    Text = "Unit Test Text",
                    Title = "Unit Test Title"
                },
                new Note
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.UtcNow.Subtract(TimeSpan.FromDays(5)),
                    ModifiedOn = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)),
                    CreatorId = creatorId,
                    Text = "Unit Test Text",
                    Title = "Unit Test Title"
                }
            };

            _mockNotesRepository
                .Setup(r => r.FindAsync(It.IsAny<Expression<Func<Note, bool>>>()))
                .ReturnsAsync(expectedNotes);

            // Act
            var result = await _notesEndpoints.GetAllNotesAsync(creatorId, _mockNotesRepository.Object);

            // Assert
            result.Should().NotBeNull().And.BeOfType<Ok<IEnumerable<NoteResponse>>>();

            var actualNoteResponses = (result as Ok<IEnumerable<NoteResponse>>).Value.ToList();
            actualNoteResponses.Should().HaveCount(2);

            for (var i = 0; i < actualNoteResponses.Count; i++)
            {
                actualNoteResponses[i].CreatorId.Should().Be(expectedNotes[i].CreatorId);
                actualNoteResponses[i].Id.Should().Be(expectedNotes[i].Id);
                actualNoteResponses[i].Text.Should().Be(expectedNotes[i].Text);
                actualNoteResponses[i].Title.Should().Be(expectedNotes[i].Title);
                actualNoteResponses[i].CreatedOn.Should().Be(expectedNotes[i].CreatedOn);
                actualNoteResponses[i].ModifiedOn.Should().Be(expectedNotes[i].ModifiedOn);
            }
        }
    }
}
