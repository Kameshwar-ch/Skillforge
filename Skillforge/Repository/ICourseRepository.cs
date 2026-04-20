using System.Threading.Tasks;
using Skillforge.Domain;
namespace Skillforge.Repository
{
    public interface ICourseRepository
    {

        Task<Course> CreateCourseAsync(Course course);
        
        Task<bool> TrainerExistsAsync(int trainerId);
        
        Task<int> SaveAsync();
    }
}