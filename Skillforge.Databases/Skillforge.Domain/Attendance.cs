using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Skillforge.Domain;
[Table("Attendance")]
//composite primary key

public class Attendance
{    
    [Key]
    public int AttendanceID { get; set; }

    [ForeignKey("EnrollmentIdNavigation")]
    public int EnrollmentID { get; set; }
    [Required]
    [Column(TypeName = "DATETIME")]
    public DateTime AttendanceDate { get; set; }

    [Required]
    public bool Status { get; set; } 
    public virtual Enrollment EnrollmentIdNavigation { get; set; }
    }






