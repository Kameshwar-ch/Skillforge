using Skillforge.Dto;

namespace Skillforge.Service
{
	public interface ICourseService
	{
		Task<int> CreateModuleAsync(int courseId, CreateModuleDto dto, int? trainerId);
	}
}
