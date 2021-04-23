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

        [Display(Name = "Student Name")]
        public string StudentName { get; set; }

        [ForeignKey("Teacher")]
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }
    }
}
