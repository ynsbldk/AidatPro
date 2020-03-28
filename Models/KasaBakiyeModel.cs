using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AidatPro.Models
{
    [Table("KasaBakiye")]
    public class KasaBakiyeModel
    {
        [Key]
        public int Id { get; set; }

        public string Acc { get; set; } = HttpContext.Current.User.Identity.GetUserId();

        public DateTime EklenmeTarih { get; set; } = DateTime.Now;

        public decimal? Bakiye { get; set; }     

        public DateTime? Tarih { get; set; }

        public bool GiderGelir { get; set; }

        public int KasaId { get; set; }


    }
}