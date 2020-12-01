﻿using CafeBoost.Data;
using CafeBoost.UI.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CafeBoost.UI
{
    public partial class AnaForm : Form
    {
        int masaAdet = 20;
        CafeBoostContext db = new CafeBoostContext();

        public AnaForm()
        {
            InitializeComponent();
            MasalariOlustur();
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
                lvi.ImageKey = db.Siparisler.Any(x=> x.MasaNo == i && x.Durum ==SiparisDurum.Aktif) ? "dolu":"bos";
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
                db.Siparisler.Add(siparis);
                lvwMasalar.SelectedItems[0].ImageKey = "dolu";
            }
            SiparisForm frm = new SiparisForm(db,siparis);
            frm.MasaTasindi += Frm_MasaTasindi;
            DialogResult dr = frm.ShowDialog();

            //sipariş iptal edildiyse ya da ödme alındıysa
            if (dr == DialogResult.OK)
                lvwMasalar.SelectedItems[0].ImageKey = "bos";
        }

        private void Frm_MasaTasindi(object sender, MasaTasimaEventArgs e)
        {
            MasaTasi(e.EskiMasaNo, e.YeniMasaNo);
        }

        private Siparis AktifSiparisBul(int masaNo)
        {
            return db.Siparisler.FirstOrDefault(x => x.MasaNo == masaNo && x.Durum==SiparisDurum.Aktif);
        }

        private void MasaTasi(int kaynak,int hedef)
        {
            foreach (ListViewItem lvi in lvwMasalar.Items)
            {
                if((int)lvi.Tag == kaynak)
                {
                    lvi.ImageKey = "bos";
                    lvi.Selected = false;
                }
                if ((int)lvi.Tag == hedef)
                {
                    lvi.ImageKey = "dolu";
                    lvi.Selected = true;
                }
            }
        }

      

    

       
    }
}
