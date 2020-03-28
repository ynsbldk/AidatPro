using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AidatPro.Models
{
    [Table("Demirbas")]
    public class DemirbasModel
    {
        [Key]
        public int Id { get; set; }

        public string DemirbasAd { get; set; }

        public int Kg { get; set; }

        public int Adet { get; set; }

        public decimal? Fiyat { get; set; }

        public DateTime AlisTarih { get; set; }

        public string Aciklama { get; set; }

        public string Acc { get; set; } = HttpContext.Current.User.Identity.GetUserId();

        public DateTime EklenmeTarih { get; set; } = DateTime.Now;

        public bool TeslimEdildiMi { get; set; }

    }
}