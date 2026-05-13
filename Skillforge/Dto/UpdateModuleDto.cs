using System;
namespace Skillforge.Dto
{   
public class UpdateModuleDto
    {
        public string Title { get; set; }
        public string ContentURI { get; set; }
        public decimal Duration { get; set; }
        public bool Status { get; set; }
    }
}

