﻿//
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

using System.Linq;
using KerbalSpaceProgram.Api;
using UnityEngine;

namespace VTS
{
    [UsedImplicitly]
    public class VtsPartModule : PartModule
    {
        // TODO: Automatically choose color of markers based on the planet
        private static readonly Color PICKING_COLOR = new Color(1.0f, 0.56f, 0.0f);
        private static readonly Color POSITION_COLOR = Color.red;
        private static readonly Color TARGET_LINE_COLOR = Color.magenta;

        internal GlobalLocation TargetLocation
        {
            get { return this.m_virtualTarget.Location; }
        }

        /// <summary>
        /// The selected position marker.
        /// </summary>
        private VirtualTarget m_virtualTarget;

        internal SystemStates SystemState { get; private set; }

        private bool m_isMouseOverPickingBody;

        private ScreenMessage m_pickingScreenMessage;

        private MapObject m_prePickingPlanetariumFocus;


        /// <summary>
        /// The body whose sphere of influence the vessel is currently in.
        /// </summary>
        [NotNull]
        private CelestialBody CurrentBody
        {
            get { return this.vessel.mainBody; }
        }

        public override void OnStart(StartState state)
        {
            switch (state)
            {
                case StartState.Editor:
                case StartState.None:
                    return;
            }

            if (this.m_virtualTarget == null)
            {
                this.m_virtualTarget = new VirtualTarget(new GlobalLocation(this.CurrentBody, new Coordinates()));
                this.SystemState = SystemStates.NoTargetSelected;
            }
        }

        public override void OnUpdate()
        {
            // NOTE: The main module may change when the current main module gets destroyed.
            bool shouldBeMainModule = this.vessel.isActiveVessel && this.IsPrimary();
            if (VtsCore.Instance.MainModule == this && !shouldBeMainModule)
            {
                VtsCore.Instance.MainModule = null;
            }
            else if (shouldBeMainModule)
            {
                VtsCore.Instance.MainModule = this;
            }

            if (this.CurrentBody != this.m_virtualTarget.Location.Body)
            {
                // Reset virtual target whenever we change the SOI.
                StopPickingVirtualTarget(keepTarget: false);
                DeleteVirtualTarget();
            }

            bool showTargetLine = this.SystemState == SystemStates.TargetSelected
                               && this.vessel.isActiveVessel
                               && HighLogic.LoadedSceneIsFlight
                               && !MapView.MapIsEnabled;
            this.m_virtualTarget.EnableTargetLine(showTargetLine, TARGET_LINE_COLOR);

            if (this.SystemState == SystemStates.PickingTarget)
            {
                HandlePositionPicking();
            }
        }

        [UsedImplicitly]
        public void OnDestroy()
        {
            if (this.m_virtualTarget != null)
            {
                StopPickingVirtualTarget(keepTarget: false);

                this.m_virtualTarget.Dispose();
            }

            if (VtsCore.Instance != null && VtsCore.Instance.MainModule == this)
            {
                VtsCore.Instance.MainModule = null;
            }
        }

        private void HandlePositionPicking()
        {
            if (!MapView.MapIsEnabled || !this.vessel.isActiveVessel)
            {
                // No longer in map mode.
                StopPickingVirtualTarget(keepTarget: false);
                return;
            }

            CelestialBody pickingBody = this.m_virtualTarget.Location.Body;
            MapObject planetariumTarget = PlanetariumCamera.fetch.target;
            
            if (planetariumTarget.celestialBody != pickingBody || this.CurrentBody != pickingBody)
            {
                StopPickingVirtualTarget(keepTarget: false);
                return;
            }

            Coordinates? mouseCoords = pickingBody.GetMouseCoordinates();
            if (mouseCoords == null)
            {
                Screen.showCursor = true;
                this.m_isMouseOverPickingBody = false;
                return;
            }

            // Position to pick.
            Screen.showCursor = false;
            this.m_isMouseOverPickingBody = true;

            // ReSharper disable once PossibleNullReferenceException
            this.m_virtualTarget.SetLocation(new GlobalLocation(pickingBody, mouseCoords.Value));

            // IMPORTANT: Don't cancel on right click as the right mouse button is used
            //   to rotate the camera.
            // IMPORTANT: We need to use "ButtonDown" here (instead of "ButtonUp"). Otherwise
            //   "ButtonUp" would immediately be true after start picking (because this is triggered
            //   by "ButtonUp" in the same frame).
            if (Input.GetMouseButtonDown(MouseButtons.Left))
            {
                StopPickingVirtualTarget(keepTarget: true);
            }
        }

        internal void StartPickingVirtualTarget()
        {
            if (this.SystemState == SystemStates.PickingTarget)
            {
                // Already picking a location.
                return;
            }

            this.m_virtualTarget.SetLocation(new GlobalLocation(this.CurrentBody, new Coordinates()));
            this.SystemState = SystemStates.PickingTarget;

            MapView.EnterMapView();

            var targetMapObject = PlanetariumCamera.fetch.targets.SingleOrDefault(target => target.celestialBody == this.CurrentBody);
            if (targetMapObject != null)
            {
                this.m_prePickingPlanetariumFocus = PlanetariumCamera.fetch.target;
                PlanetariumCamera.fetch.SetTarget(targetMapObject);
                this.vessel.orbitTargeter.enabled = false;
            }
            else
            {
                Debug.LogError("Couldn't find " + this.CurrentBody.bodyName + " in the target list of the planetarium camera.");
                // We can't continue because our code relies on the fact that the body is
                // focused when picking a target.
                return;
            }

            this.m_pickingScreenMessage = ScreenMessages.PostScreenMessage(
                "Click to select a target on " + this.CurrentBody.bodyName + "'s surface.\n(Leave map view to cancel.)",
                500,
                ScreenMessageStyle.UPPER_CENTER);
        }

        internal void StopPickingVirtualTarget(bool keepTarget)
        {
            if (this.SystemState != SystemStates.PickingTarget)
            {
                return;
            }

            this.SystemState = keepTarget ? SystemStates.TargetSelected : SystemStates.NoTargetSelected;
            if (keepTarget)
            {
                SetAsTarget();
            }

            if (MapView.MapIsEnabled)
            {
                // Only set focus back if we're still in map view.
                PlanetariumCamera.fetch.SetTarget(this.m_prePickingPlanetariumFocus);
            }
            
            this.m_prePickingPlanetariumFocus = null;
            ScreenMessages.RemoveMessage(this.m_pickingScreenMessage);
            Screen.showCursor = true;
            this.vessel.orbitTargeter.enabled = true;
        }

        internal void SetTargetLocation(GlobalLocation location)
        {
            StopPickingVirtualTarget(keepTarget: false);
            this.m_virtualTarget.SetLocation(location);
            this.SystemState = SystemStates.TargetSelected;
        }

        internal void SetAsTarget()
        {
            FlightGlobals.fetch.SetVesselTarget(this.m_virtualTarget);
        }

        internal void DeleteVirtualTarget()
        {
            this.m_virtualTarget.SetLocation(new GlobalLocation(this.CurrentBody, new Coordinates()));
            this.SystemState = SystemStates.NoTargetSelected;
        }

        internal void DrawMarkers()
        {
            if (this.SystemState == SystemStates.NoTargetSelected)
            {
                return;
            }

            if (MapView.MapIsEnabled)
            {
                GuiUtils.DrawMapViewGroundMarker(this.m_virtualTarget.Location,
                                                 this.SystemState == SystemStates.TargetSelected ? POSITION_COLOR : PICKING_COLOR);

                if (this.SystemState == SystemStates.PickingTarget && this.m_isMouseOverPickingBody)
                {
                    var coords = this.m_virtualTarget.Location.Coordinates;
                    GUI.Label(new Rect(Input.mousePosition.x + 15, Screen.height - Input.mousePosition.y, 200, 50),
                        coords.ToStringDecimal() + "\n"
                        + ScienceUtil.GetExperimentBiome(this.m_virtualTarget.Location.Body, coords.Latitude, coords.Longitude));
                }
            }
            else
            {
                double distance = this.m_virtualTarget.GetDistance(this.vessel);
                double radius;
                if (distance < 1000)
                {
                    radius = 30;
                }
                else if (distance < 10000)
                {
                    radius = 300;
                }
                else if (distance < 100000)
                {
                    radius = 3000;
                }
                else
                {
                    radius = -1;
                }

                if (radius > 0)
                {
                    GuiUtils.DrawMapViewGroundMarker(this.m_virtualTarget.Location, POSITION_COLOR, radius: radius);
                }
            }
        }
    }
}
