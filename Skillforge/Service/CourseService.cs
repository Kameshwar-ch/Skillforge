using Skillforge.Domain;
using Skillforge.Dto;
using Skillforge.Repository;
using Skillforge.Utility;
using System;
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

        public async Task CreateCourseAsync(CourseRequestDto courseRequest)
        {
            bool trainerExists = await _courseRepository.TrainerExistsAsync(courseRequest.TrainerID);
            
            if (!trainerExists)
            {
                throw new KeyNotFoundException($"Trainer with ID {courseRequest.TrainerID} not found.");
            }

            var newCourse = new Course
            {
                Title = courseRequest.Title,
                Description = courseRequest.Description,
                TrainerID = courseRequest.TrainerID,
                Duration = courseRequest.Duration,
                Status = false 
            };

           
            await _courseRepository.CreateCourseAsync(newCourse);
            await _courseRepository.SaveAsync();
            await _auditService.LogAsync(newCourse.TrainerID, "CourseCreated", $"New course '{newCourse.Title}' created with status 0.");
            
        }

        public async Task<int> CreateModuleAsync(int courseId, CreateModuleDto dto, int? trainerId)
        {
            try
            {
                if (dto.Duration <= 0)
                    throw new ArgumentException("Invalid duration.");

                var course = await _courseRepository.GetCourseByIdAsync(courseId);

                if (course == null)
                    throw new KeyNotFoundException("Course not found.");

				if(course.Status != false)
				{
					throw new InvalidOperationException(CourseMessages.NotInDraft);
				}
                var newModule = new Module
                {
                    CourseID = courseId,
                    Title = dto.Title,
                    ContentURI = dto.ContentURI,
                    Duration = dto.Duration,
                    Status = false
                };

                int moduleId = await _courseRepository.AddModuleAsync(newModule);
                await _auditService.LogAsync(trainerId, "Module Created Successfully", $"Module: {dto.Title}");
                return moduleId;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}