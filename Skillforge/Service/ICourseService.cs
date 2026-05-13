using Skillforge.Dto;
using System.Threading.Tasks;

namespace Skillforge.Service
{
	public interface ICourseService
	{
		Task<int> CreateModuleAsync(int courseId, CreateModuleDto dto, int? trainerId);
        Task CreateCourseAsync(CourseRequestDto courseRequest);
	    Task<CourseResponseDto> GetCourseByIDAsync(int courseID, int userID);
		Task<List<CourseResponseDto>> GetCoursesAsync(CourseFilterRequestDto request);
		Task<bool> UpdateCourseStatus(int courseId, bool status);
		Task<List<ModuleResponseDto>> GetModulesFilteredAsync(ModuleFilterRequestDto request);		
		Task<ModuleResponseDto> GetModuleByIdAsync(int moduleId);
		Task<bool> UpdateModuleAsync(int moduleId, UpdateModuleDto dto);
		Task<bool> DeleteModuleAsync(int moduleId);

	}
}

