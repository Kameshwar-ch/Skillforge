using Skillforge.Data;
using Skillforge.Domain;
using Microsoft.EntityFrameworkCore;
using Skillforge.Dto;

namespace Skillforge.Repository;

/// <summary>
/// A concrete implementation of IAssessmentRepository that uses Entity Framework Core
/// to handle course lookups and assessment persistence against the SQL Server database.
/// </summary>
public class AssessmentRepository : IAssessmentRepository
{
    private readonly SkillForgeDB _context;

    public AssessmentRepository(SkillForgeDB context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a course entity from the database using the specified courseId.
    /// Returns null if the course does not exist.
    /// </summary>
    /// <param name="courseId">The unique identifier of the course to retrieve.</param>
    /// <returns>The Course entity if found; otherwise null.</returns>
    public async Task<Course?> GetCourseByIdAsync(int courseId)
    {
        return await _context.Courses.FindAsync(courseId);
    }

    /// <summary>
    /// Inserts a new Assessment record into the database and persists the changes.
    /// </summary>
    /// <param name="assessment">The Assessment entity to insert.</param>
    /// <returns>The auto-generated AssessmentID assigned after the save.</returns>
    public async Task<int> CreateAssessmentAsync(Assessment assessment)
    {
        _context.Assessments.Add(assessment);
        await _context.SaveChangesAsync();
        return assessment.AssessmentID;
    }

    public async Task<Assessment?> GetAssessmentByIdAsync(int assessmentId)
    {
        return await _context.Assessments.FindAsync(assessmentId);
    }

    public async Task UpdateAssessmentAsync(Assessment assessment)
    {
         _context.Assessments.Update(assessment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAssessmentAsync(Assessment assessment)
    {
        _context.Assessments.Remove(assessment);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Assessment>> GetAssessmentsAsync(AssessmentFilterDto filter)
    {
    var query = _context.Assessments.AsQueryable();

    if (filter.CourseId.HasValue)
        query = query.Where(a => a.CourseID == filter.CourseId.Value);

    if (filter.Type.HasValue)
        query = query.Where(a => a.Type == filter.Type.Value);

    if (filter.FromDate.HasValue)
        query = query.Where(a => a.Date >= filter.FromDate.Value);

    if (filter.ToDate.HasValue)
        query = query.Where(a => a.Date <= filter.ToDate.Value);

    return await query.ToListAsync();
    }
}
