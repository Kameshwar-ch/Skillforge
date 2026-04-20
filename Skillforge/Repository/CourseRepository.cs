using Microsoft.EntityFrameworkCore;
using Skillforge.Data;
using Skillforge.Domain;

namespace Skillforge.Repository
{
	public class CourseRepository : ICourseRepository
	{
		private readonly SkillForgeDB _context;
		public CourseRepository(SkillForgeDB context)
		{
			_context = context;
		}
		public async Task<Course?> GetCourseByIdAsync(int courseId)
		{
			return await _context.Courses.FindAsync(courseId);
		}

		public async Task<int> AddModuleAsync(Module module)
		{
			_context.Modules.Add(module);
			await _context.SaveChangesAsync();
			return module.ModuleID;
		}

	}
}
