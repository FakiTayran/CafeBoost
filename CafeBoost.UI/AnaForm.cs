using CafeBoost.Data;
using CafeBoost.UI.Properties;
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
    public partial class AnaForm : Form
    {
        int masaAdet = 20;
        KafeVeri db = new KafeVeri();

        public AnaForm()
        {
            InitializeComponent();
            OrnekUrunleriYukle();
            MasalariOlustur();
        }

        private void OrnekUrunleriYukle()
        {
            db.Urunler.Add(new Urun
            {
                UrunAd = "Kola",
                BirimFiyat = 6m

            });
            db.Urunler.Add(new Urun
            {
                UrunAd = "Ayran",
                BirimFiyat = 4m

            });
        }

        private void MasalariOlustur()
        {
            #region 2-)İmaj Listesinin Hazırlanması
            ImageList il = new ImageList();
            il.Images.Add("bos", Resources.chair_Bos);
            il.Images.Add("dolu", Resources.chair_Dolu);
            il.ImageSize = new Size(64, 64);
            lvwMasalar.LargeImageList = il;
            #endregion
            #region 1-) Masaların Oluşturulması
            ListViewItem lvi;
            for (int i = 1; i <= db.MasaAdet; i++)
            {
                lvi = new ListViewItem("Masa " + i);
                lvi.ImageKey = "bos";
                lvi.Tag = i;
                lvwMasalar.Items.Add(lvi);
            }
            #endregion
        }

        private void tsmiUrunler_Click(object sender, EventArgs e)
        {
            new UrunlerForm(db).ShowDialog();
        }

        private void tsmiGecmisSiparisler_Click(object sender, EventArgs e)
        {
            new GecmisSiparisler(db).ShowDialog();
        }

        private void lvwMasalar_DoubleClick(object sender, EventArgs e)
        {
            int MasaNo = (int)lvwMasalar.SelectedItems[0].Tag;
            Siparis siparis = AktifSiparisBul(MasaNo);

            //Oturan yoksa (sipariş bulunamadıysa)
            if (siparis == null)
            {
                siparis = new Siparis();
                siparis.MasaNo = MasaNo;
                db.AktifSiparisler.Add(siparis);
                lvwMasalar.SelectedItems[0].ImageKey = "dolu";
            }
            SiparisForm frm = new SiparisForm(db,siparis,this);
            DialogResult dr = frm.ShowDialog();

            //sipariş iptal edildiyse ya da ödme alındıysa
            if (dr == DialogResult.OK)
                lvwMasalar.SelectedItems[0].ImageKey = "bos";
        }

        private Siparis AktifSiparisBul(int masaNo)
        {
            #region Yöntem 1
            //foreach (Siparis item in db.AktifSiparisler)
            //{
            //    if (item.MasaNo == masaNo)
            //    {
            //        return item;
            //    }
            //}
            //return null;
            #endregion
            #region Linq Yöntemi
            return db.AktifSiparisler.FirstOrDefault(x => x.MasaNo == masaNo);
            #endregion
        }

        public void MasaTasi(int kaynak,int hedef)
        {
            foreach (ListViewItem lvi in lvwMasalar.Items)
            {
                if((int)lvi.Tag == kaynak)
                {
                    lvi.ImageKey = "bos";
                }
                if ((int)lvi.Tag == hedef)
                {
                    lvi.ImageKey = "dolu";
                }
            }
        }
    }
}
