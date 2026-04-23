using System;
using Skillforge.Domain;

namespace Skillforge.Dto;

public class AttendanceRecordDto
{
    public int EnrollmentID { get; set; }
    public AttendanceStatus Status { get; set; }
}
public class MarkAttendanceDto
{
    public int CourseID { get; set; }
    public DateTime AttendanceDate { get; set; }
    public List<AttendanceRecordDto> Records { get; set; } = new();
}

public class AttendanceRecordResultDto
{
    public int  EnrollmentID { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string Status       { get; set; } = string.Empty;
}

public class AttendanceResponseDto
{
    public int CourseID  { get; set; }
    public DateTime AttendanceDate { get; set; }
    public int TotalMarked { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public List<AttendanceRecordResultDto> Records { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}

// GET Course Attendance 
public class CourseAttendanceDto
{
    public int EnrollmentID { get; set; }
    public string EmployeeName { get; set; }
    public string  CourseStatus { get; set; }   
    public DateTime LoginDate { get; set; }   
}
public class GetCourseAttendanceResponseDto
{
    public int CourseID { get; set; }
    public DateTime AttendanceDate { get; set; }
    public List<CourseAttendanceDto> Records { get; set; }
}