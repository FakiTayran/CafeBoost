﻿using CafeBoost.Data;
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
        private readonly KafeVeri db;  //readonly değiştirilemez...
        private readonly Siparis siparis;
        private readonly BindingList<SiparisDetay> blsiparisDetaylar;
        public SiparisForm(KafeVeri kafeVeri ,Siparis siparis)
        {
            //constructor parametresi olarak gelen bu nesneleri 
            //daha sonra da erişebileceğimiz field'lara aktarıyoruz
            db = kafeVeri;
            this.siparis = siparis;
            InitializeComponent();
            UrunleriListele();
            MasaNoGuncelle();
            blsiparisDetaylar = new BindingList<SiparisDetay>(siparis.SiparisDetaylar);
            dgvSiparisDetaylar.DataSource = blsiparisDetaylar;
            OdemeTutariGuncelle();
            blsiparisDetaylar.ListChanged += BlsiparisDetaylar_ListChanged;
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
            cboUrun.DataSource = db.Urunler;
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
                UrunAd = secilenUrun.UrunAd,
                BirimFiyat = secilenUrun.BirimFiyat,
                Adet = adet
            };
            blsiparisDetaylar.Add(detay);
            
        }

        private void dgvSiparisDetaylar_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DialogResult dr = MessageBox.Show("Seçili detayları silmek istediğinize emin misiniz ?","Silme Onayı",MessageBoxButtons.YesNo,MessageBoxIcon.Warning,MessageBoxDefaultButton.Button2);

            if (dr != DialogResult.Yes)
                e.Cancel = true; 
        }
    }
}
