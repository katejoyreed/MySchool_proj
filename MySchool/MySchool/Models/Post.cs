using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MySchool.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        public string Text { get; set; }

        public List<string>? Comments { get; set; }
    }
}
