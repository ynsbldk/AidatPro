using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AidatPro.Models
{
    public class HesapViewModel
    {
        public DaireAccountModel DaireAcc { get; set; }

        public BlokModel Blok { get; set; }

        public DaireModel Daire { get; set; }

        public ApplicationUser Use { get; set; }
    }
}