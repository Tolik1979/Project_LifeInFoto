using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GlobalSite.Models
{
    public class Photo
    {
        public int Id { get; set; }
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Display(Name = "Изображение")]
        public byte[] Image { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Display(Name = "Цена")]
        public decimal? Price { get; set; }
        [Required]
        public string AlbumId { get; set; }
        public Album Album { get; set; }
    }
}