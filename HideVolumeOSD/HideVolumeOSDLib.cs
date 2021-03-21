using HideVolumeOSD.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace HideVolumeOSD
{
	public class HideVolumeOSDLib
	{
		[DllImport("user32.dll")]
		static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

		[DllImport("user32.dll", SetLastError = true)]
		static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

		[DllImport("user32.dll", SetLastError = true)]
		static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern bool IsWindow(IntPtr hWnd);

		NotifyIcon ni;

		IntPtr hWndInject = IntPtr.Zero;

		public HideVolumeOSDLib(NotifyIcon ni)
		{
			this.ni = ni;
		}

		public void Init()
		{
			hWndInject = FindOSDWindow(true);

			int count = 1;

			while (hWndInject == IntPtr.Zero && count < 9)
			{
				keybd_event((byte)Keys.VolumeUp, 0, 0, 0);
				keybd_event((byte)Keys.VolumeDown, 0, 0, 0);

				hWndInject = FindOSDWindow(true);

				// Quadratic backoff if the window is not found
				System.Threading.Thread.Sleep(1000*(count^2));
				count++;		
			}

			// final try

			hWndInject = FindOSDWindow(false);

			if (hWndInject == IntPtr.Zero)
			{
				Program.InitFailed = true;
				return;
			}
				
			if (ni != null)
			{
				if (Settings.Default.HideOSD)
					HideOSD();
				else
					ShowOSD();

				Application.ApplicationExit += Application_ApplicationExit;
			}
		}

		private IntPtr FindOSDWindow(bool bSilent)
		{
			IntPtr hwndRet = IntPtr.Zero;
			IntPtr hwndHost = IntPtr.Zero;

			int pairCount = 0;

			// search for all windows with class 'NativeHWNDHost'

			while ((hwndHost = FindWindowEx(IntPtr.Zero, hwndHost, "NativeHWNDHost", "")) != IntPtr.Zero)
			{
				// if this window has a child with class 'DirectUIHWND' it might be the volume OSD

				if (FindWindowEx(hwndHost, IntPtr.Zero, "DirectUIHWND", "") != IntPtr.Zero)
				{
					// if this is the only pair we are sure

					if (pairCount == 0)
					{
						hwndRet = hwndHost;
					}

					pairCount++;

					// if there are more pairs the criteria has failed...

					if (pairCount > 1)
					{
						MessageBox.Show("Severe error: Multiple pairs found!", "HideVolumeOSD");
						return IntPtr.Zero;
					}
				}
			}

			// if no window found yet, there is no OSD window at all

			if (hwndRet == IntPtr.Zero && !bSilent)
			{
				MessageBox.Show("Severe error: OSD window not found!", "HideVolumeOSD");
			}

			return hwndRet;
		}

		private void Application_ApplicationExit(object sender, EventArgs e)
		{
			ShowOSD();
		}

		public void HideOSD()
		{
            if (!IsWindow(hWndInject))
            {
                Init();
            }

			ShowWindow(hWndInject, 6); // SW_MINIMIZE

			if (ni != null)
				ni.Icon = Resources.IconDisabled;
		}

		public void ShowOSD()
		{
            if (!IsWindow(hWndInject))
            {
                Init();
            }

			ShowWindow(hWndInject, 9); // SW_RESTORE

			// show window on the screen

			keybd_event((byte)Keys.VolumeUp, 0, 0, 0);
			keybd_event((byte)Keys.VolumeDown, 0, 0, 0);

			if (ni != null)
				ni.Icon = Resources.Icon;
		}
	}
}
