namespace LaptopSystem.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class CommentViewModel
    {
        public int Id { get; set; }

        public string Author { get; set; }

        public string Content { get; set; }
    }
}