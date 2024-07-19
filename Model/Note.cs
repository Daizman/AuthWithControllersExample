namespace AuthWithControllersExample.Model;

public class Note
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public string Title { get; set; } = null!;
    public bool IsComplete { get; set; }
}