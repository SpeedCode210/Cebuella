using System.ComponentModel.DataAnnotations;

namespace Cebuella.Models;

public class Report
{
    [Key] public long ID { get; set; }
    public string Username { get; set; }
    public DateTime Date { get; set; }
    public string Content { get; set; }
}