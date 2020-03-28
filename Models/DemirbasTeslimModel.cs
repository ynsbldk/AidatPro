using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AidatPro.Models
{
    [Table("DemirbasTeslim")]
    public class DemirbasTeslimModel
    {
        [Key]
        public int Id { get; set; }

        public IEnumerable<BlokModel> Bloks { get; set; }

        public int BlokId { get; set; }

        public IEnumerable<DaireModel> Daires { get; set; }

        public int DaireId { get; set; }

        public string Acc { get; set; } = HttpContext.Current.User.Identity.GetUserId();

        public DemirbasModel Demirs { get; set; }

        public int DemirbasId { get; set; }

        public string TeslimEden { get; set; }

        public string TeslimAlan { get; set; }

        public DateTime EklenmeTarih { get; set; }
    }
}