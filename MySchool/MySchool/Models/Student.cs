using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MySchool.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Display(Name = "Student First Name")]
        public string StudentFirstName { get; set; }

        [Display(Name = "Student Last Name")]
        public string StudentLastName { get; set; }

        public string DOB { get; set; }

        public string Classroom { get; set; }
    }
}
