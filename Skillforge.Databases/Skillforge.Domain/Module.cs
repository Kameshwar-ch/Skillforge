using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skillforge.Domain;

[Table("Module")]
public class Module
{
    [Key]
    [Column(TypeName = "INT")]
    public int ModuleID { get; set; }

    [Column(TypeName = "char(5)")]
    public int CourseID { get; set; }
    [ForeignKey("CourseID")]
    public virtual Course CourseIDNavigation { get; set; }

    [Column(TypeName = "VARCHAR(20)")]
    public string Title { get; set; }
    [Column(TypeName = "VARCHAR(50)")]
    public string ContentURI { get; set; }
    public decimal Duration { get; set; }
    public bool Status { get; set; }

}
