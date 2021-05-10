using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

public class Category 
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } 
    public HierarchyId HierarchyId { get; set; }
}