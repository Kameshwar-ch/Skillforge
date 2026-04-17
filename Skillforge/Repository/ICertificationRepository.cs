using Skillforge.Domain;

namespace Skillforge.Repository;

/// <summary>
/// Defines data access operations required for certification issuance.
/// </summary>
public interface ICertificationRepository
{
    /// <summary>Retrieves a user by ID; null if not found.</summary>
    Task<User?> GetUserByIdAsync(int userId);

    /// <summary>Retrieves a course by ID; null if not found.</summary>
    Task<Course?> GetCourseByIdAsync(int courseId);

    /// <summary>
    /// Returns true if the employee has a Pass result for any assessment
    /// belonging to the specified course.
    /// </summary>
    Task<bool> HasPassedAssessmentForCourseAsync(int employeeId, int courseId);

    /// <summary>
    /// Returns the existing Active certification for the given employee and course,
    /// or null if none exists.
    /// </summary>
    Task<Certification?> GetActiveCertificationAsync(int employeeId, int courseId);

    /// <summary>Persists the certification and returns the generated CertificationID.</summary>
    Task<int> IssueCertificationAsync(Certification certification);
}
