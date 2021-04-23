using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MySchool.Models
{
    public class PermissionSlip
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string Time { get; set; }
        
        [Display(Name = "Student Name")]
        public string? StudentName { get; set; }

        [Display(Name = "Approving Parent")]
        public string? ApprovingParent { get; set; }
    }
}
