namespace LaptopSystem.Web.Models
{
    using LaptopSystem.Model;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;

    public class SubmitCommentModel
    {
        [Required]
        [ShouldNotContainEmail]
        public string Comment { get; set; }

        [Required]
        public int LaptopId { get; set; }
    }
}