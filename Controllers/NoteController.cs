using System.Collections.Concurrent;
using AuthWithControllersExample.Extensions;
using AuthWithControllersExample.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthWithControllersExample.Controllers;

public class NoteController : BaseController
{
    private static readonly ConcurrentBag<Note> Notes = new();

    [HttpPost]
    public IActionResult AddNote(string title)
    {
        Note note = new()
        {
            Id = Notes.Count,
            UserId = HttpContext.ExtractUserIdFromClaims()!.Value, // Написал класс расширение, который достает Id
                                                                   // пользователя из заголовка с авторизацией,
                                                                   // так сложнее нам не нужно делать специфичных политик
            Title = title,
            IsComplete = false,
        };
        
        Notes.Add(note);

        return Ok();
    }

    [HttpGet]
    public List<Note> GetNotes() 
        => Notes.Where(note => note.UserId == HttpContext.ExtractUserIdFromClaims()!.Value).ToList();

    [HttpPut]
    [Authorize(Policy = "NotesOwner")] // Отдельно указываем название специфичной политики, если хотим ее использовать
    public IActionResult Complete(int noteId, int userId)
    {
        var note = Notes.FirstOrDefault(note => note.Id == noteId && note.UserId == userId);
        if (note is null)
        {
            return NotFound();
        }

        note.IsComplete = true;

        return Ok();
    }
}