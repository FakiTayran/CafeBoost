﻿using System;
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
        public AnaForm()
        {
            InitializeComponent();
        }

        private void tsmiUrunler_Click(object sender, EventArgs e)
        {
            new UrunlerForm().ShowDialog();
        }

        private void tsmiGecmisSiparisler_Click(object sender, EventArgs e)
        {
            new GecmisSiparisler().ShowDialog();
        }
    }
}