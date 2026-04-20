using System;
using Skillforge.Domain;

namespace Skillforge.Dto;

// Single Attendance 
public class MarkAttendanceDto
{
    public int              EnrollmentID   { get; set; }
    public DateTime         AttendanceDate { get; set; }
    public AttendanceStatus Status         { get; set; }
}

public class AttendanceResponseDto
{
    public int    AttendanceID { get; set; }
    public string Message      { get; set; }
}

// ─── Bulk Attendance ──────────────────────────────────────────────────────────

public class BulkMarkAttendanceDto
{
    public int      CourseID       { get; set; }
    public DateTime AttendanceDate { get; set; }
}

public class BulkAttendanceRecordDto
{
    public int    EnrollmentID { get; set; }
    public int    EmployeeID   { get; set; }
    public string EmployeeName { get; set; }
    public string Status       { get; set; }
}

public class BulkAttendanceResponseDto
{
    public int                           CourseID       { get; set; }
    public DateTime                      AttendanceDate { get; set; }
    public int                           TotalMarked    { get; set; }
    public int                           PresentCount   { get; set; }
    public int                           AbsentCount    { get; set; }
    public List<BulkAttendanceRecordDto> Records        { get; set; }
    public string                        Message        { get; set; }
}

// GET Course Attendance 

public class CourseAttendanceDto
{
    public int      EnrollmentID { get; set; }
    public int      EmployeeID   { get; set; }
    public string   EmployeeName { get; set; }
    public string   CourseStatus { get; set; }   // "Accessed" or "Not Accessed"
    public DateTime LoginDate    { get; set; }   // exact time if accessed, date with 00:00:00 if not
}

public class GetCourseAttendanceResponseDto
{
    public int                       CourseID       { get; set; }
    public DateTime                  AttendanceDate { get; set; }
    public List<CourseAttendanceDto> Records        { get; set; }
}