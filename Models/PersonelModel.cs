using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AidatPro.Models
{
    [Table("Personels")]
    public class PersonelModel
    {
        [Key]
        public int Id { get; set; }
        public string Acc { get; set; } = HttpContext.Current.User.Identity.GetUserId();
        public DateTime EklenmeTarih { get; set; } = DateTime.Now;
        public string PersonelAd { get; set; }
        public string PersonelMail { get; set; }
        public string Telefon { get; set; }       
        public DateTime IsBaslangicTarih { get; set; }        
        public string PersonelGorev { get; set; }
        public string SorumluBlok { get; set; }
        public int SorumluBlokId { get; set; }

        public int SorumluKasaId { get; set; }

    }
}