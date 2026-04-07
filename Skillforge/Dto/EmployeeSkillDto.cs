using System.ComponentModel.DataAnnotations;

namespace Skillforge.Dto
{
	public class EmployeeSkillDto
	{
		[Required]
		public string? SkillName { get; set; }
		[Required]
		public string? Level { get; set; }
	}
}
