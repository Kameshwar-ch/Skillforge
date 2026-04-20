using Skillforge.Domain;

namespace Skillforge.Repository
{
	public interface ICourseRepository
	{
		Task<Course?> GetCourseByIdAsync(int courseId);
		Task<int> AddModuleAsync(Module module);
	}
}
