using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CafeBoost.Data
{
    public class Siparis
    {
        public int MasaNo { get; set; }
        public List<SiparisDetay> SiparisDetaylar { get; set; }
        public DateTime? AcilisZamani { get; set; }
        public DateTime? KapanisZamani { get; set; }
        public SiparisDurum Durum { get; set; }
        public string ToplamTutarTL { get { return ToplamTutar().ToString() + " ₺"; } }
        public Siparis()
        {
            SiparisDetaylar = new List<SiparisDetay>();
            AcilisZamani = DateTime.Now;
        }

        private decimal ToplamTutar()
        {
            return SiparisDetaylar.Sum(x => x.Tutar());
        }
    }
}
