using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MySchool.Models
{
    public class Classroom
    {
        [Key]
        public int ClassId { get; set; }

        [Display(Name = "Class Name")]
        public string ClassName { get; set; }

        public List<Student> Students { get; set; }
    }
}
