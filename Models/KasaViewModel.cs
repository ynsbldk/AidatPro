using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AidatPro.Models
{
    public class KasaViewModel
    {
        public List<KasaModel> Kasa { get; set; }

        public List<KasaBakiyeModel> KasaBakiye { get; set; }

        public PersonelModel Personel { get; set; }

        public IPagedList<AidatModel> AidatModel { get; set; }

        public KasaModel KasaTek { get; set; }
    }
}