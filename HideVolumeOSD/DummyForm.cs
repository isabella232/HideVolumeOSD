using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HideVolumeOSD
{
	public partial class DummyForm : Form
	{
		ContextMenus cm;

		public DummyForm()
		{
			InitializeComponent();
		}

		public DummyForm(ContextMenus cm)
		{
			InitializeComponent();
			this.cm = cm;
		}

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);

			if (m.Msg == 0x0401)
			{
				Application.Exit();
			}
		}
	}
}
