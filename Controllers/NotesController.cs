using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SecureNoteTakingApi.Data;
using SecureNoteTakingApi.DTOs;
using SecureNoteTakingApi.Models;

namespace SecureNoteTakingApi.Controllers
{
    [ApiController]
    [Route("api/notes")]
    [Authorize] //All endpoints require valid JWT token
    public class NotesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotesController(AppDbContext context)
        {
            _context = context;
        }

        //Ensures notes are always scoped to current user
        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(claim!.Value);
        }

        //Add a new note

        [HttpPost]
        public async Task<IActionResult> AddNote([FromBody] NoteRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId = GetCurrentUserId();

            //Associate note with the currently logged-in user
            var note = new Note
            {
                Title = dto.Title,
                Content = dto.Content,
                UserId = userId
            };

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            // Return success message with the new note's Id
            return Ok(new
            {
                message = "Note added successfully.",
                noteId = note.Id
            });
        }


        //Retrieve all notes of the logged-in user
        [HttpGet]
        public async Task<IActionResult> GetNotes()
        {
            int userId = GetCurrentUserId();

            //Only return notes that belong to the current user
            var notes = await _context.Notes
                .Where(n => n.UserId == userId)
                .Select(n => new
                {
                    n.Id,
                    n.Title,
                    n.Content
                })
                .ToListAsync();

            return Ok(notes);
        }

        //Update an existing note by Id
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNote(int id, [FromBody] NoteRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId = GetCurrentUserId();

            //Find note by Id AND UserId - prevents users editing others' notes
            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note == null)
                return NotFound(new { message = "Note not found." });

            note.Title = dto.Title;
            note.Content = dto.Content;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Note updated successfully." });
        }

        //Delete a note by Id

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            int userId = GetCurrentUserId();

            //Find note by Id AND UserId - prevents users deleting others' notes
            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note == null)
                return NotFound(new { message = "Note not found." });

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Note deleted successfully." });
        }
    }
}