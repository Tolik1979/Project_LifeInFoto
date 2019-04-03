using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GlobalSite.Models
{
    public class Album
    {
        [Display(Name = "Пин код")]
        public string Id { get; set; }
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public virtual ICollection<Photo> Photos { get; set; }
        public Album()
        {
            Photos = new List<Photo>();
        }
    }
}