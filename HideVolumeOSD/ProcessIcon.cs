using System;
using System.Diagnostics;
using System.Windows.Forms;
using HideVolumeOSD.Properties;
using System.Runtime.InteropServices;

namespace HideVolumeOSD
{
	/// <summary>
	/// 
	/// </summary>
	class ProcessIcon : IDisposable
	{
		/// <summary>
		/// The NotifyIcon object.
		/// </summary>
		NotifyIcon ni;
		ContextMenus cm;
		DummyForm dummyForm = new DummyForm();

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessIcon"/> class.
		/// </summary>
		public ProcessIcon()
		{
			// Instantiate the NotifyIcon object.
			ni = new NotifyIcon();
			cm = new ContextMenus(ni);

			dummyForm = new DummyForm(cm);			
			dummyForm.Show();
			dummyForm.Visible = false;
		}

		/// <summary>
		/// Displays the icon in the system tray.
		/// </summary>
		public void Display()
		{
			// Put the icon in the system tray and allow it react to mouse clicks.			
			ni.MouseClick += new MouseEventHandler(ni_MouseClick);
			ni.Icon = Resources.Icon;
			ni.Text = "HideVolumeOSD - Hide or show the windows volume OSD window";
			ni.Visible = true;

			// Attach a context menu.
			ni.ContextMenuStrip = cm.Create();
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		public void Dispose()
		{
			// When the application closes, this will remove the icon from the system tray immediately.
			ni.Dispose();
		}

		/// <summary>
		/// Handles the MouseClick event of the ni control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		void ni_MouseClick(object sender, MouseEventArgs e)
		{
			// Handle mouse button clicks.
			if (e.Button == MouseButtons.Left)
			{
				cm.Switch();
			}
		}
	}
}