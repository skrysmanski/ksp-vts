//
// This file is part of the Virtual Targeting System for KSP.
// 
// This code is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This code is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this code.  If not, see <http://www.gnu.org/licenses/>. 
//

using KerbalSpaceProgram.Api;
using UnityEngine;
using VTS.ToolbarWrapper;

namespace VTS
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class VtsCore : PluginCore<VtsCore>
    {
        private static readonly Vector2 DEFAULT_WINDOW_POS = new Vector2(10, 80);

        /// <summary>
        /// The main VTS module of the currently active vessel.
        /// </summary>
        public VtsPartModule MainModule
        {
            get { return this.m_mainModule; }
            set
            {
                if (this.m_mainModule == value)
                {
                    return;
                }

                this.m_mainModule = value;
                if (value == null)
                {
                    this.m_mainWindow = null;
                }
                else
                {
                    if (this.m_mainWindow != null && this.m_mainWindow.IsOpen)
                    {
                        this.m_mainWindow = new VtsMainWindow(value, windowPos: DEFAULT_WINDOW_POS);
                    }
                    else
                    {
                        this.m_mainWindow = null;
                    }
                }
            }
        }
        private VtsPartModule m_mainModule;

        private VtsMainWindow m_mainWindow;

        private IButton m_mainWindowButton;

        protected override void OnStart()
        {
            if (ToolbarManager.ToolbarAvailable)
            {
                this.m_mainWindowButton = ToolbarManager.Instance.add("virtualtargetingsystem", "mainwnd");
                this.m_mainWindowButton.TexturePath = "VirtualTargetingSystem/Resources/texMainWindowButton";
                this.m_mainWindowButton.ToolTip = "Open/Close \"Virtual Targeting System\" main window";
                this.m_mainWindowButton.OnClick += OnMainWindowButtonClick;
            }

            RenderingManager.AddToPostDrawQueue(0, OnDraw);
        }

        private void OnMainWindowButtonClick(ClickEvent clickEvent)
        {
            if (this.MainModule == null || clickEvent.MouseButton != MouseButtons.Left)
            {
                return;
            }

            if (this.m_mainWindow == null)
            {
                this.m_mainWindow = new VtsMainWindow(this.MainModule, windowPos: DEFAULT_WINDOW_POS);
            }
            else
            {
                this.m_mainWindow.IsOpen = !this.m_mainWindow.IsOpen;
            }
        }

        private void OnDraw()
        {
            if (this.m_mainModule != null)
            {
                this.m_mainModule.DrawMarkers();
            }

            if (this.m_mainWindow != null)
            {
                this.m_mainWindow.ProcessWindow();
            }
        }

        protected override void OnDispose()
        {
            RenderingManager.RemoveFromPostDrawQueue(0, OnDraw);

            if (this.m_mainWindowButton != null)
            {
                this.m_mainWindowButton.Destroy();
                this.m_mainWindowButton = null;
            }

            this.MainModule = null;
        }

        [UsedImplicitly]
        public void Update()
        {
            if (this.MainModule != null && !this.MainModule.vessel.isActiveVessel)
            {
                this.MainModule = null;
            }

            if (this.m_mainWindowButton == null && this.MainModule != null)
            {
                if (this.m_mainWindow == null)
                {
                    this.m_mainWindow = new VtsMainWindow(this.MainModule, windowPos: DEFAULT_WINDOW_POS);
                }

                // If toolbar plugin isn't available, make sure we show the main
                // window only in map view to reduce clutter during the flight.
                this.m_mainWindow.IsOpen = MapView.MapIsEnabled;
            }
        }
    }
}
