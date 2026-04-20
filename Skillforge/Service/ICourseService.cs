using Skillforge.Dto; 
using System.Threading.Tasks;

namespace Skillforge.Service
{
    public interface ICourseService
    {
        Task<CourseResponseDto> CreateCourseAsync(CourseRequestDto courseRequest);
    }
}