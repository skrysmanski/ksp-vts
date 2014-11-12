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

using System;
using KerbalSpaceProgram.Api;
using UnityEngine;

namespace VTS
{
    internal class VtsMainWindow : Window
    {
        private const int WINDOW_ID = 653285586;

        private const float EDITOR_COL1_WIDTH = 60;
        private const float EDITOR_COL2_WIDTH = 100;

        private const string LATITUDE_EDIT_FIELD_NAME = "vts.mainwnd.lat";
        private const string LONGITUDE_EDIT_FIELD_NAME = "vts.mainwnd.lon";

        /// <summary>
        /// The <see cref="VtsPartModule"/> this window belongs to.
        /// </summary>
        [NotNull]
        private readonly VtsPartModule m_module;

        private GlobalLocation TargetLocation
        {
            get { return this.m_module.TargetLocation; }
        }

        private SystemStates SystemState
        {
            get { return this.m_module.SystemState; }
        }

        private readonly GUIStyle m_editorLabelStyle;

        private string m_latitudeString;

        private string m_longitudeString;

        private string m_errorMessage;

        public VtsMainWindow([NotNull] VtsPartModule module, Vector2 windowPos = new Vector2()) 
                : base(WINDOW_ID, "Virtual Targeting", windowPos: windowPos)
        {
            this.m_module = module;
            this.m_editorLabelStyle = new GUIStyle(HighLogic.Skin.label)
            {
                alignment = TextAnchor.MiddleRight,
                fixedWidth = EDITOR_COL1_WIDTH,
            };

            Reset();
        }

        private void Reset()
        {
            this.m_latitudeString = this.TargetLocation.Coordinates.Latitude.ToString("F4");
            this.m_longitudeString = this.TargetLocation.Coordinates.Longitude.ToString("F4");
        }

        protected override void OnWindow()
        {
            if (!this.TargetLocation.Body.HasSurface())
            {
                Label("ERROR: " + this.TargetLocation.Body.bodyName + " has no surface.");
                return;
            }

            if (this.SystemState != SystemStates.PickingTarget)
            {
                if (Button("Pick Target Location"))
                {
                    RemoveFocusFromEditFields();
                    this.m_module.StartPickingVirtualTarget();
                }
            }
            else
            {
                if (Button("Stop Target Picking"))
                {
                    RemoveFocusFromEditFields();
                    this.m_module.StopPickingVirtualTarget(keepTarget: false);
                }
                else
                {
                    // Keep the value in the input fields up-to-date.
                    Reset();
                }
            }

            if (Button("Set as Target") && this.SystemState == SystemStates.TargetSelected)
            {
                RemoveFocusFromEditFields();
                this.m_module.SetAsTarget();
            }

            if (Button("Delete Virtual Target") && this.SystemState == SystemStates.TargetSelected)
            {
                RemoveFocusFromEditFields();
                this.m_module.DeleteVirtualTarget();
                Reset();
            }

#if DEBUG
            if (Button("Show Log Console"))
            {
                HighLogic.fetch.showConsole = !HighLogic.fetch.showConsole;
            }
#endif

            if (this.m_errorMessage != null)
            {
                Label(this.m_errorMessage);
            }

            if (Event.current.Equals(Event.KeyboardEvent("return")))
            {
                RemoveFocusFromEditFields();
            }

            using (HorizontalLayout)
            {
                Label("Planet:", m_editorLabelStyle);
                Label(this.TargetLocation.Body.bodyName, options: GUILayout.Width(EDITOR_COL2_WIDTH));
            }

            using (HorizontalLayout)
            {
                Label("Latitude:", m_editorLabelStyle);
                SetNextControlName(LATITUDE_EDIT_FIELD_NAME);
                TextField(ref this.m_latitudeString, options: GUILayout.Width(EDITOR_COL2_WIDTH));
            }

            using (HorizontalLayout)
            {
                Label("Longitude:", m_editorLabelStyle);
                SetNextControlName(LONGITUDE_EDIT_FIELD_NAME);
                TextField(ref this.m_longitudeString, options: GUILayout.Width(EDITOR_COL2_WIDTH));
            }

            using (HorizontalLayout)
            {
                Label("Biome:", m_editorLabelStyle);

                string value = this.SystemState != SystemStates.NoTargetSelected ? this.TargetLocation.BiomeName : "n/a";

                Label(value, options: GUILayout.Width(EDITOR_COL2_WIDTH));
            }

            using (HorizontalLayout)
            {
                Label("Altitude:", m_editorLabelStyle);

                string value;

                if (this.SystemState != SystemStates.NoTargetSelected)
                {
                    value = "{0:0.} m".With(this.TargetLocation.Altitude);
                }
                else
                {
                    value = "n/a";
                }

                Label(value, options: GUILayout.Width(EDITOR_COL2_WIDTH));
            }

            using (HorizontalLayout)
            {
                Label("Distance:", m_editorLabelStyle);

                string value;

                if (this.SystemState != SystemStates.NoTargetSelected)
                {
                    var targetPos = this.TargetLocation.Position;
                    var vesselPos = this.m_module.vessel.GetWorldPos3D();

                    value = "{0:0.} m".With((targetPos - vesselPos).magnitude);
                }
                else
                {
                    value = "n/a";
                }

                Label(value, options: GUILayout.Width(EDITOR_COL2_WIDTH));
            }

            using (HorizontalLayout)
            {
                if (Button("Apply") && this.SystemState != SystemStates.PickingTarget)
                {
                    RemoveFocusFromEditFields();
                    UpdateLocation();
                }

                if (Button("Reset") && this.SystemState != SystemStates.PickingTarget)
                {
                    RemoveFocusFromEditFields();
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
                if (!AngleUtils.TryParseDMS(this.m_latitudeString, out latitude))
                {
                    this.m_errorMessage = "Invalid latitude";
                    return;
                }
            }

            if (!double.TryParse(this.m_longitudeString, out longitude))
            {
                if (!AngleUtils.TryParseDMS(this.m_longitudeString, out longitude))
                {
                    this.m_errorMessage = "Invalid longitude";
                    return;
                }
            }

            var location = new GlobalLocation(this.TargetLocation.Body, 
                                              new Coordinates(latitude: latitude, longitude: longitude));
            this.m_module.SetTargetLocation(location);
            Reset();
        }

        /// <summary>
        /// Removes the focus from the edit fields, if they actually have the focus.
        /// </summary>
        private void RemoveFocusFromEditFields()
        {
            switch (NameOfFocusedControl)
            {
                case LATITUDE_EDIT_FIELD_NAME:
                case LONGITUDE_EDIT_FIELD_NAME:
                    SetFocus(null);
                    break;
            }
        }
    }
}
