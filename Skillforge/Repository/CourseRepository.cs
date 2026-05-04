using Skillforge.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Skillforge.Domain;
using Skillforge.Dto;

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

        public async Task<Course?> GetByIDAsync(int courseID)
        {
            return await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseID == courseID);
        }

        public async Task<PagedResultDto<CourseResponseDto>> GetCoursesFilteredAsync(CourseFilterRequestDto request)
        {
            IQueryable<Course> query = _context.Courses.AsNoTracking(); 

            // Filters
            if (request.Status.HasValue)
                query = query.Where(c => c.Status == request.Status.Value);

            if (request.TrainerId.HasValue)
                query = query.Where(c => c.TrainerID == request.TrainerId.Value);

            if (request.MinDuration.HasValue)
                query = query.Where(c => c.Duration >= request.MinDuration.Value);

            if (request.MaxDuration.HasValue)
                query = query.Where(c => c.Duration <= request.MaxDuration.Value);

            int totalRecords = await query.CountAsync();

            var courses = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new CourseResponseDto
                {
                    CourseID = c.CourseID,
                    Title = c.Title,
                    Description = c.Description,
                    TrainerID = c.TrainerID,
                    Duration = c.Duration,
                    Status = c.Status
                })
                .ToListAsync();

            return new PagedResultDto<CourseResponseDto>
            {
                Items = courses,
                TotalRecords = totalRecords,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize)
            };
        }
    }
}