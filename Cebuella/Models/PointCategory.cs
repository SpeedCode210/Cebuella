using System.ComponentModel.DataAnnotations;

namespace Cebuella.Models;

public class PointCategory
{
    [Key] public long ID { get; set; }
    public string Name { get; set; }
}