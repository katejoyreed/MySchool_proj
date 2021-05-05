﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MySchool.Models
{
    public class Parent
    {
        [Key]
        public int Id { get; set; }
        
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Preferred Title")]
        public string PreferredTitle { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public string Email { get; set; }
        
        public string Address { get; set; }

        public string Classroom { get; set; }

        [Display(Name ="Student First Name")]
        public string StudentFirstName { get; set; }
        [Display(Name = "Student Last Name")]
        public string StudentLastName { get; set; }
        [Display(Name = "Student Date of Birth")]
        public string DOB { get; set; }

        [ForeignKey("IdentityUser")]
        public string IdentityUserId { get; set; }
        public IdentityUser IdentityUser { get; set; }
        
        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public Student Student { get; set; }

       
    }
}
