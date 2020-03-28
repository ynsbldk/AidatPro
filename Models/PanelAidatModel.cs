using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AidatPro.Models
{
    public class PanelAidatModel
    {
        public IEnumerable<AidatModel> Aidats { get; set; }

        public IEnumerable<BlokModel> Bloks { get; set; }
    }
}