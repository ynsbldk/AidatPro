using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AidatPro.Models
{
    
    public class DaireMailModel
    {
        public int Id { get; set; }

        public string Konu { get; set; }

        public string Mesaj { get; set; }

        public string Telefon { get; set; }

        public string GonderenAd { get; set; }
    }
}