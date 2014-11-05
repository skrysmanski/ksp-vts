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

namespace VTS
{
    internal class VtsMainWindow : Window
    {
        private const int WINDOW_ID = 653285586;

        [NotNull]
        private readonly VtsPartModule m_module;

        private GlobalLocation TargetLocation
        {
            get { return this.m_module.TargetLocation; }
        }

        private string m_latitudeString;

        private string m_longitudeString;

        private string m_errorMessage;

        public VtsMainWindow([NotNull] VtsPartModule module, Vector2 windowPos = new Vector2()) 
                : base(WINDOW_ID, "Virtual Targeting", windowPos: windowPos)
        {
            this.m_module = module;
            Reset();
        }

        private void Reset()
        {
            this.m_latitudeString = this.TargetLocation.Coordinates.Latitude.ToString("F3");
            this.m_longitudeString = this.TargetLocation.Coordinates.Longitude.ToString("F3");
        }

        protected override void OnWindow()
        {
            if (this.m_module.SystemState != SystemStates.PickingTarget)
            {
                if (Button("Pick Target Location"))
                {
                    this.m_module.StartPickingVirtualTarget();
                }
            }
            else
            {
                if (Button("Stop Target Picking"))
                {
                    this.m_module.StopPickingVirtualTarget(keepTarget: false);
                }
                else
                {
                    // Keep the value in the input fields up-to-date.
                    Reset();
                }
            }

            if (Button("Set As Target") && this.m_module.SystemState == SystemStates.TargetSelected)
            {
                this.m_module.SetAsTarget();
            }

            if (Button("Delete Virtual Target") && this.m_module.SystemState == SystemStates.TargetSelected)
            {
                this.m_module.DeleteVirtualTarget();
                Reset();
            }

            if (this.m_errorMessage != null)
            {
                Label(this.m_errorMessage);
            }

            using (HorizontalLayout)
            {
                Label("Reference Body");
                Label(this.TargetLocation.Body.bodyName);
            }

            using (HorizontalLayout)
            {
                Label("Latitude");
                this.m_latitudeString = TextField(this.m_latitudeString);
            }

            using (HorizontalLayout)
            {
                Label("Longitude");
                this.m_longitudeString = TextField(this.m_longitudeString);
            }

            using (HorizontalLayout)
            {
                if (Button("Apply") && this.m_module.SystemState != SystemStates.PickingTarget)
                {
                    UpdateLocation();
                }

                if (Button("Reset") && this.m_module.SystemState != SystemStates.PickingTarget)
                {
                    Reset();
                }
            }
        }

        private void UpdateLocation()
        {
            double latitude, longitude;

            this.m_errorMessage = null;

            if (!double.TryParse(this.m_latitudeString, out latitude))
            {
                this.m_errorMessage = "Invalid latitude";
                return;
            }

            if (!double.TryParse(this.m_longitudeString, out longitude))
            {
                this.m_errorMessage = "Invalid longitude";
                return;
            }

            var location = new GlobalLocation(this.TargetLocation.Body, 
                                              new Coordinates(latitude: latitude, longitude: longitude));
            this.m_module.SetTargetLocation(location);
            Reset();
        }
    }
}
