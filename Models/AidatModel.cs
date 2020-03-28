using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AidatPro.Models
{
    [Table("Aidat")]
    public class AidatModel
    {
        [Key]
        public int Id { get; set; }
        public string Acc { get; set; } = HttpContext.Current.User.Identity.GetUserId();
        public DateTime EklenmeTarih { get; set; } = DateTime.Now;
        public int DaireId { get; set; }
        public int BlokId { get; set; }
        public decimal? Borc { get; set; }
        public bool Odeme { get; set; }
        public DateTime? TahsilatTarih { get; set; }       
        public int KasaId { get; set; }

    }
}