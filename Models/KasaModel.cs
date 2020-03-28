using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AidatPro.Models
{
    [Table("Kasa")]
    public class KasaModel
    {
        [Key]
        public int Id { get; set; }

        public string Acc { get; set; } = HttpContext.Current.User.Identity.GetUserId();

        public DateTime EklenmeTarih { get; set; } = DateTime.Now;

        public string KasaAd { get; set; }

        public KasaBakiyeModel KasaBakiye { get; set; }       

        public string Not { get; set; }

        public int SorumluPersonelId { get; set; }

        public PersonelModel SorumluPersonel { get; set; }
    }
}