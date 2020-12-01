using CafeBoost.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CafeBoost.UI
{
    public partial class SiparisForm : Form
    {
        public event EventHandler<MasaTasimaEventArgs> MasaTasindi;
        private readonly CafeBoostContext db;  //readonly değiştirilemez...
        private readonly Siparis siparis;

        private readonly BindingList<SiparisDetay> blsiparisDetaylar;
        public SiparisForm(CafeBoostContext CafeBoostContext, Siparis siparis)
        {
            //constructor parametresi olarak gelen bu nesneleri 
            //daha sonra da erişebileceğimiz field'lara aktarıyoruz
            db = CafeBoostContext;
            this.siparis = siparis;
            InitializeComponent();
            dgvSiparisDetaylar.AutoGenerateColumns = false;
            MasalariListele();
            UrunleriListele();
            MasaNoGuncelle();
            OdemeTutariGuncelle();
            blsiparisDetaylar = new BindingList<SiparisDetay>(siparis.SiparisDetaylar.ToList());
            dgvSiparisDetaylar.DataSource = blsiparisDetaylar;
            blsiparisDetaylar.ListChanged += BlsiparisDetaylar_ListChanged;

        }

        private void MasalariListele()
        {
            cboMasalar.Items.Clear();
            for (int i = 0; i <= db.MasaAdet; i++)
            {
                if (!db.Siparisler.Any(x => x.MasaNo == i && x.Durum == SiparisDurum.Aktif))
                {
                    cboMasalar.Items.Add(i);
                }
            }
        }

        private void BlsiparisDetaylar_ListChanged(object sender, ListChangedEventArgs e)
        {

            OdemeTutariGuncelle();
        }

        private void OdemeTutariGuncelle()
        {
            lblOdemeTutari.Text = siparis.ToplamTutarTL;
        }

        private void UrunleriListele()
        {
            cboUrun.DataSource = db.Urunler.ToList();
        }

        private void MasaNoGuncelle()
        {
            Text = $"Masa {siparis.MasaNo:00} - Sipariş Detayları ({siparis.AcilisZamani.Value.ToShortTimeString()})";
            lblMasaNo.Text = siparis.MasaNo.ToString("00");
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            Urun secilenUrun = (Urun)cboUrun.SelectedItem;
            int adet = (int)nudAdet.Value;
            SiparisDetay detay = new SiparisDetay()
            {
                SiparisId = siparis.Id,
                UrunId = secilenUrun.Id,
                UrunAd = secilenUrun.UrunAd,
                BirimFiyat = secilenUrun.BirimFiyat,
                Adet = adet
            };
            db.SiparisDetaylar.Add(detay);
            db.SaveChanges();
            SiparisDetaylarYenile();

        }

        private void SiparisDetaylarYenile()
        {
            blsiparisDetaylar.Clear();
            siparis.SiparisDetaylar.ToList().ForEach(x => blsiparisDetaylar.Add(x));
        }

        private void dgvSiparisDetaylar_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DialogResult dr = MessageBox.Show("Seçili detayları silmek istediğinize emin misiniz ?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (dr != DialogResult.Yes)
                e.Cancel = true;

            SiparisDetay sd = (SiparisDetay)e.Row.DataBoundItem;
            db.SiparisDetaylar.Remove(sd);
            db.SaveChanges();
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            DialogResult dr = DialogResult.Cancel;
            Close();
        }

        private void btnSiparisIptal_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Sipariş  iptal dilerek kapatılacaktır. Emin misiniz? ", "İptal Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.Yes)
                SiparisKapat(SiparisDurum.Iptal);
        }

        private void btnOdemeAl_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Ödeme alındı sipariş kapatılacaktır. Emin misiniz? ", "Ödeme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.Yes)
                SiparisKapat(SiparisDurum.Odendi, siparis.ToplamTutar());
        }

        private void SiparisKapat(SiparisDurum siparisDurum, decimal odenenTutar = 0)
        {
            siparis.OdenenTutar = odenenTutar;
            siparis.KapanisZamani = DateTime.Now;
            siparis.Durum = siparisDurum;
            db.SaveChanges();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnMasaTasi_Click(object sender, EventArgs e)
        {
            if (cboMasalar.SelectedIndex < 0) return;
            int hedef = (int)cboMasalar.SelectedItem;
            int kaynak = siparis.MasaNo;
            siparis.MasaNo = hedef;
            db.SaveChanges();
            MasaNoGuncelle();
            MasalariListele();

            MasaTasimaEventArgs args = new MasaTasimaEventArgs()
            {
                EskiMasaNo = kaynak,
                YeniMasaNo = hedef
            };
        }
        protected virtual void MasaTasindiginda(MasaTasimaEventArgs args)
        {
            if (MasaTasindi != null)
            {
                MasaTasindi(this, args);
            }
        }


    }
}
