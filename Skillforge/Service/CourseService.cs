using Skillforge.Dto;
using Skillforge.Repository;
using Skillforge.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skillforge.Service
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IAuditService _auditService;

        public CourseService(ICourseRepository courseRepository, IAuditService auditService)
        {
            _courseRepository = courseRepository;
            _auditService = auditService;
        }

        public async Task<CourseResponseDto> CreateCourseAsync(CourseRequestDto courseRequest)
        {
            // 1. Verify Trainer exists
            bool trainerExists = await _courseRepository.TrainerExistsAsync(courseRequest.TrainerID);
            
            if (!trainerExists)
            {
                throw new KeyNotFoundException($"Trainer with ID {courseRequest.TrainerID} not found.");
            }

            // 2. Map DTO to Model
            var newCourse = new Course
            {
                Title = courseRequest.Title,
                Description = courseRequest.Description,
                TrainerID = courseRequest.TrainerID,
                Duration = courseRequest.Duration,
                Status = true 
            };

            // 3. Save
            await _courseRepository.CreateCourseAsync(newCourse);
            await _courseRepository.SaveAsync();

            // 4. Audit Log
            await _auditService.LogAsync(newCourse.TrainerID, "CourseCreated", $"New course '{newCourse.Title}' created.");

            return new CourseResponseDto
            {
                CourseID = newCourse.CourseID,
                Title = newCourse.Title,
                Status = "Success"
            };
        }
    }
}