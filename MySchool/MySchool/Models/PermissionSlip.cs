using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public string Classroom { get; set; }
        [Display(Name = "Student First Name")]
        public string StudentFirst { get; set; }

        [Display(Name = "Student Last Name")]
        public string StudentLast { get; set; }

        [Display(Name = "Approving Parent")]
        public string? ApprovingParent { get; set; }

        
    }
}
