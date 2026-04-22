using Skillforge.Dto;
using System.Threading.Tasks;

namespace Skillforge.Service
{
    public interface ICourseService
    {
        Task<int> CreateModuleAsync(int courseId, CreateModuleDto dto, int? trainerId);
        Task CreateCourseAsync(CourseRequestDto courseRequest);
    }
}