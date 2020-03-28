using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AidatPro.Models
{
    [Table("Daires")]
    public class DaireModel
    {
        [Key]
        public int Id { get; set; }
        public int DaireId { get; set; }

        public string DaireAd { get; set; }

        public string DaireNo { get; set; }

        public string Sahip { get; set; }

        public int HaneUye { get; set; }

        public string Telefon { get; set; }

        public string EMail { get; set; }

        public string EvSahibi { get; set; }

        public string Notlar { get; set; }

        public int BlokId { get; set; }

        public string Acc { get; set; } = HttpContext.Current.User.Identity.GetUserId();

        public DateTime EklenmeTarih { get; set; } = DateTime.Now;

        public IEnumerable<AidatModel> Aidat { get; set; }

    }
}