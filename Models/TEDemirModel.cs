using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AidatPro.Models
{
    [Table("TEDemir")]
    public class TEDemirModel
    {
        [Key]
        public int Id { get; set; }
        public DemirbasModel Demirs { get; set; }
        public BlokModel Bloks { get; set; }
        public DaireModel Daires { get; set; }
        public List<DemirbasTeslimModel> DTeslimat { get; set; }
        public int BlokId { get; set; }
        public int DemirbasId { get; set; }
        public int DaireId { get; set; }
        public string TeslimEden { get; set; }
        public string TeslimAlan { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public string Kullanici { get; set; } = HttpContext.Current.User.Identity.GetUserId();
    }
}