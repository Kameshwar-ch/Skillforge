namespace Skillforge.Dto
{
    public class CourseFilterRequestDto
    {
        public bool? Status { get; set; }          
        public int? TrainerId { get; set; }       
        public decimal? MinDuration { get; set; } 
        public decimal? MaxDuration { get; set; }  

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}