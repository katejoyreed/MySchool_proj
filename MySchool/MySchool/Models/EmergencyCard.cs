using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MySchool.Models
{
    public class EmergencyCard
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Student Name")]
        public string StudentName { get; set; }

        [Display(Name = "Parent 1 Name")]
        public string ParentOneName { get; set; }

        [Display(Name = "Parent 1 Contact Number")]
        public string ParentOneContact { get; set; }

        [Display(Name = "Parent 2 Name")]
        public string ParentTwoName { get; set; }

        [Display(Name = "Parent 2 Contact Number")]
        public string ParentTwoContact { get; set; }

        [Display(Name = "Emergency Contact 1 Name")]
        public string ECOneName { get; set; }

        [Display(Name = "Emergency Contact 1 Phone Number")]
        public string ECOneNumber { get; set; }

        [Display(Name = "Emergency Contact 2 Name")]
        public string ECTwoName { get; set; }

        [Display(Name = "Emergency Contact 2 Phone Number")]
        public string ECTwoNumber { get; set; }

        [Display(Name = "Doctor Name")]
        public string DocName { get; set; }

        public string Allergies { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public Student Student { get; set; }
    }
}
