using System;
namespace Skillforge.Dto
{
    public class ModuleFilterRequestDto
    {
        public int? CourseId { get; set; }
        public bool? Status { get; set; }
        public decimal? MinDuration { get; set; }
        public decimal? MaxDuration { get; set; }
    }
}