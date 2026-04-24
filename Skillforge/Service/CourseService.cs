using Microsoft.EntityFrameworkCore;
using Skillforge.Domain;
using Skillforge.Data;
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
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IAuditService     _auditService;
        private readonly SkillForgeDB      _context;

        private const string CourseAccessedAction = "CourseAccessed";

        public CourseService(ICourseRepository courseRepository, IAuditService auditService, SkillForgeDB context)
        {
            _courseRepository = courseRepository;
            _auditService     = auditService;
            _context          = context;
        }

        public async Task<int> CreateModuleAsync(int courseId, CreateModuleDto dto, int? trainerId)
        {
            try
            {
                if (dto.Duration <= 0)
                    throw new ArgumentException(CourseMessages.InvalidDuration);

                var course = await _courseRepository.GetCourseByIdAsync(courseId);

                if (course == null)
                    throw new KeyNotFoundException(CourseMessages.CourseNotFound);

                if (course.Status != false)
                    throw new InvalidOperationException(CourseMessages.NotInDraft);

                var newModule = new Module
                {
                    CourseID   = courseId,
                    Title      = dto.Title,
                    ContentURI = dto.ContentURI,
                    Duration   = dto.Duration,
                    Status     = false
                };

                int moduleId = await _courseRepository.AddModuleAsync(newModule);
                await _auditService.LogAsync(trainerId, "Module Created Successfully", $"Module: {dto.Title} (ID: {moduleId}) for Course: {courseId}");
                return moduleId;
            }
            catch (Exception ex)
            {
                await _auditService.LogAsync(trainerId, "Module Creation Failed", $"CourseID: {courseId}, Error: {ex.Message}");
                throw;
            }
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
                Title       = courseRequest.Title,
                Description = courseRequest.Description,
                TrainerID = courseRequest.TrainerID,
                Duration = courseRequest.Duration,
                Status = false 
                TrainerID   = courseRequest.TrainerID,
                Duration    = courseRequest.Duration,
                Status      = true
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
                CourseID = newCourse.CourseID,
                Title    = newCourse.Title,
                Status   = "Success"
            };
        }

        public async Task<CourseResponseDto> GetCourseByIDAsync(int courseID, int userID)
        {
            if (courseID <= 0)
                throw new InvalidOperationException("Invalid CourseID.");

            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseID == courseID);

            if (course == null)
                throw new KeyNotFoundException($"Course {courseID} not found.");

            // Any enrollment (active or completed) allows viewing
            var isEnrolled = await _context.Enrollments
                .AnyAsync(e =>
                    e.EmployeeID == userID  &&
                    e.CourseID   == courseID);

            if (!isEnrolled)
                throw new UnauthorizedAccessException("You are not enrolled in this course.");

            // Only log for active enrollments (Status == false means Active)
            // Completed employees can view but NOT logged
            var isActive = await _context.Enrollments
                .AnyAsync(e =>
                    e.EmployeeID == userID  &&
                    e.CourseID   == courseID &&
                    e.Status     == false);

            if (isActive)
            {
                _context.AuditLogs.Add(new AuditLog
                {
                    UserID    = userID,
                    Action    = CourseAccessedAction,
                    Resource  = $"Course/{courseID}",
                    Timestamp = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }

            return new CourseResponseDto
            {
                CourseID = course.CourseID,
                Title    = course.Title,
                Status   = course.Status ? "Active" : "Inactive"
            };
        }
    }
}