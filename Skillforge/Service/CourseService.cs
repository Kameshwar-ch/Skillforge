using Skillforge.Domain;
using Skillforge.Dto;
using Skillforge.Repository;
using Skillforge.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skillforge.Service
{
	public class CourseService : ICourseService
	{
		private readonly ICourseRepository _repository;
		private readonly IAuditService _auditService;
		public CourseService(ICourseRepository repository, IAuditService auditService)
		{
			_repository = repository;
			_auditService = auditService;
		}
		public async Task<int> CreateModuleAsync(int courseId, CreateModuleDto dto, int? trainerId)
		{
			try
			{
				if (dto.Duration <= 0)
					throw new ArgumentException(CourseMessages.InvalidDuration);

				var course = await _repository.GetCourseByIdAsync(courseId);

				if (course == null)
					throw new KeyNotFoundException(CourseMessages.CourseNotFound);

				if (course.Status != false)
					throw new InvalidOperationException(CourseMessages.NotInDraft);

				var newModule = new Module
				{
					CourseID = courseId,
					Title = dto.Title,
					ContentURI = dto.ContentURI,
					Duration = dto.Duration,
					Status = false
				};

				int moduleId = await _repository.AddModuleAsync(newModule);
				await _auditService.LogAsync(trainerId, "Module Created Successfully", $"Module: {dto.Title} (ID: {moduleId}) for Course: {courseId}");
				return moduleId;

			}
			catch (Exception ex)
			{
				await _auditService.LogAsync(trainerId, "Module Creation Failed", $"CourseID: {courseId}, Error: {ex.Message}");
				throw;
			}


			
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
