using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MySchool.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        public string Text { get; set; }

        public string Author { get; set; }

        public List<Comment> ? Comments { get; set; }

        [ForeignKey("Classroom")]
        public int ClassId { get; set; }
        public Classroom Classroom { get; set; }

    }
}
