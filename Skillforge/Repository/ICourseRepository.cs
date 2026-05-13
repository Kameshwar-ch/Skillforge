using System.Threading.Tasks;
using Skillforge.Domain;
using Skillforge.Dto;
namespace Skillforge.Repository
{
    public interface ICourseRepository
    {

        Task<Course> CreateCourseAsync(Course course);
        
        Task<bool> TrainerExistsAsync(int trainerId);
        
        Task<int> SaveAsync();

        Task<Course?> GetCourseByIdAsync(int courseId);
        
		Task<int> AddModuleAsync(Module module);
        
        Task<Course?> GetByIDAsync(int courseID);

        Task<List<CourseResponseDto>> GetCoursesFilteredAsync(CourseFilterRequestDto request);

        Task<List<ModuleResponseDto>> GetModulesFilteredAsync(ModuleFilterRequestDto request);

        Task<Module> GetModuleByIdAsync(int moduleId);
   
        Task UpdateModuleAsync(Module module);

        Task DeleteModuleAsync(Module module);
 
    }
}
