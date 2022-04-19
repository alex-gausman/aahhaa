using aahhaa.Api.Models.Notes;
using aahhaa.Core.Models;
using aahhaa.Shared.Data.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MinimalApis.Extensions.Results;

namespace aahhaa.Api.Endpoints;

public class NotesEndpoints : IEndpoints
{
    public void Register(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost("v1/notes", CreateNoteAsync);
        routeBuilder.MapGet("v1/notes/{id}", GetByIdAsync);
        routeBuilder.MapGet("v1/notes", GetAllNotesAsync);
        routeBuilder.MapPut("v1/notes/{id}", UpdateNoteAsync);
        routeBuilder.MapDelete("v1/notes/{id}", DeleteNoteAsync);
    }

    public async Task<IResult> CreateNoteAsync(
        [FromHeader(Name = "x-aha-user-id")] Guid creatorId,
        CreateNoteRequest createNoteRequest,
        IValidator<CreateNoteRequest> validator,
        IRepository<Note> noteRepository)
    {
        // Validate the request
        var validationResult = validator.Validate(createNoteRequest);
        if (!validationResult.IsValid)
        {
            return Results.Extensions.BadRequest(validationResult.ToString());
        }

        // Create the note
        var note = createNoteRequest.MapToNote(creatorId);
        await noteRepository.CreateAsync(note);

        return Results.Extensions.Created($"v1/notes/{note.Id}", NoteResponse.From(note));
    }

    public async Task<IResult> UpdateNoteAsync(
        Guid id,
        UpdateNoteRequest updateNoteRequest,
        IValidator<UpdateNoteRequest> validator,
        IRepository<Note> noteRepository)
    {
        // Validate the request
        var validationResult = validator.Validate(updateNoteRequest);
        if (!validationResult.IsValid)
        {
            return Results.Extensions.BadRequest(validationResult.ToString());
        }

        // Retrieve note
        var noteToUpdate = await noteRepository.GetAsync(id);
        if (noteToUpdate == null)
        {
            return Results.Extensions.NotFound();
        }

        // Update the note
        noteToUpdate.Text = updateNoteRequest.Text;
        noteToUpdate.Title = updateNoteRequest.Title;
        noteToUpdate.ModifiedOn = DateTime.UtcNow;

        // Persist to datastore
        await noteRepository.UpdateAsync(noteToUpdate);

        return Results.Extensions.Ok(NoteResponse.From(noteToUpdate));
    }

    public async Task<IResult> GetByIdAsync(Guid id, IRepository<Note> noteRepository)
    {
        return await noteRepository.GetAsync(id) switch
        {
            Note note => Results.Extensions.Ok(NoteResponse.From(note)),
            null => Results.Extensions.NotFound()
        };
    }

    public async Task<IResult> GetAllNotesAsync(
        [FromHeader(Name = "x-aha-user-id")] Guid creatorId,
        IRepository<Note> noteRepository)
    {
        var notes = await noteRepository.FindAsync(n => n.CreatorId == creatorId);

        return Results.Extensions.Ok(notes.Select(n => NoteResponse.From(n)));
    }

    public async Task<IResult> DeleteNoteAsync(Guid id, IRepository<Note> noteRepository)
    {
        await noteRepository.DeleteAsync(id);
        
        return Results.Extensions.NoContent();
    }
}