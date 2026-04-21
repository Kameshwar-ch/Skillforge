using Skillforge.Data; 
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
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

        public async Task<Course> CreateCourseAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
            return course;
        }

        public async Task<bool> TrainerExistsAsync(int trainerId)
        {
            return await _context.Users.AnyAsync(u => u.UserID == trainerId);
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
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
