using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skillforge.Domain;

public enum UserRole
{
    Employee,
    Trainer,
    Manager,
    HR,
    Admin
};

[Table("User")]
public class User
{
    [Key]
    [Column(TypeName = "INT")]
    public int UserID { get; set; }

    [Column(TypeName = "VARCHAR(20)")]
    [Required]
    public string Name { get; set; }

    [Column(TypeName = "VARCHAR(20)")]
    [Required]
    public UserRole Role { get; set; }

    [Column(TypeName = "VARCHAR(50)")]
    [Required, RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
    ErrorMessage = "Invalid email address format.")]
    public string Email { get; set; }

    [Column(TypeName = "VARCHAR(10)")]
    [Required, RegularExpression(@"^\d{10}$", ErrorMessage = "Phone must be exactly 10 digits.")]
    public string Phone { get; set; }

    [Required]
    public bool Status { get; set; }
    public virtual ICollection<AuditLog> AuditLogs{get; set;}=new List<AuditLog>();
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    public virtual ICollection<Result> Results { get; set; } = new List<Result>();
    public virtual ICollection<Certification> Certifications { get; set; } = new List<Certification>();
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
