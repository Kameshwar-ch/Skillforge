using System.ComponentModel.DataAnnotations;

namespace Skillforge.Dto
{
	public class CompetencyMatrixDto
	{
		public int EmployeeId { get; set; }

		[Required]
		public string? EmployeeName { get; set; }
		public List<EmployeeSkillDto> Skills { get; set; } = new();
	}
}
