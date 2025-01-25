using System.ComponentModel.DataAnnotations;

namespace Cebuella.Models;

public class StudentTask
{
    [Key]
    public int Id { get; set; }
    public string StudentId { get; set; } 
    public string CoachId { get; set; } 
    public string Content { get; set; } 
    public Status Status { get; set; } 
}

public enum Status
{
    NotAttempted,
    Attempting,
    Stuck,
    Completed
}
