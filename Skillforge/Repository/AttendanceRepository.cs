using Microsoft.EntityFrameworkCore;
using Skillforge.Data;
using Skillforge.Domain;

namespace Skillforge.Repository;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly SkillForgeDB _context;

    /// <summary>
    /// Initializes AttendanceRepository with database context.
    /// </summary>
    /// <param name="context">Database context for EF Core queries.</param>
    public AttendanceRepository(SkillForgeDB context)
    {
        _context = context;
    }

    /// <summary>
    /// Upserts attendance for a given enrollment + date.
    /// Checks if a record already exists for the same EnrollmentID on the same day.
    /// Same day → updates Status | New day → inserts new record.
    /// </summary>
    /// <param name="attendance">Attendance domain entity to upsert.</param>
    /// <returns>Tuple of the attendance record and a bool indicating if it was newly created.</returns>
    public async Task<(Attendance attendance, bool isNew)> UpsertAttendanceAsync(Attendance attendance)
    {
        var dateOnly = attendance.AttendanceDate.Date;

        var existing = await _context.Attendances
            .FirstOrDefaultAsync(a =>
                a.EnrollmentID   == attendance.EnrollmentID &&
                a.AttendanceDate >= dateOnly                &&
                a.AttendanceDate <  dateOnly.AddDays(1));

        if (existing == null)
        {
            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();
            return (attendance, true);    // new record
        }
        else
        {
            existing.Status = attendance.Status;
             existing.AttendanceDate = attendance.AttendanceDate; 
            await _context.SaveChangesAsync();
            return (existing, false);     // already exists, updated
        }
    }
}