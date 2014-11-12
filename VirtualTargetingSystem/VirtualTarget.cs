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
using Object = UnityEngine.Object;

namespace VTS
{
    internal class VirtualTarget : ITargetable, IDisposable
    {
        private const double TARGET_LINE_END_ALTITUDE = 60000;

        private const float TARGET_LINE_PIXEL_WIDTH = 2.0f;

        private GameObject m_gameObject = new GameObject();

        private Line m_targetLine;

        public GlobalLocation Location { get; private set; }

        [PublicAPI, NotNull]
        public string Name { get; set; }

        private readonly Vessel m_vessel;

        public VirtualTarget(GlobalLocation location)
        {
            this.Name = "Virtual Target";
            this.Location = location;

            this.m_vessel = new Vessel()
            {
                id = Guid.NewGuid(),
            };
        }

        public void Dispose()
        {
            Object.Destroy(this.m_gameObject);
            this.m_gameObject = null;

            if (this.m_targetLine != null)
            {
                this.m_targetLine.Dispose();
                this.m_targetLine = null;
            }
        }

        public void SetLocation(GlobalLocation location)
        {
            this.Location = location;

            if (location.Body.HasSurface())
            {
                this.m_gameObject.transform.position = location.Position;
            }
        }

        public double GetDistance(Vessel vessel)
        {
            var targetPos = this.Location.Position;
            var vesselPos = vessel.GetWorldPos3D();

            return (targetPos - vesselPos).magnitude;
        }

        public void EnableTargetLine(bool enable, Color color)
        {
            if (enable)
            {
                if (this.m_targetLine == null)
                {
                    Debug.Log("Target line enabled");
                    this.m_targetLine = Line.Instantiate();
                    this.m_targetLine.Material = GuiUtils.Material;
                    this.m_targetLine.LineWidth = TARGET_LINE_PIXEL_WIDTH;
                    this.m_targetLine.Color = color;
                    this.m_targetLine.SetActive(true);
                }

                UpdateTargetLine();
            }
            else
            {
                if (this.m_targetLine != null)
                {
                    Debug.Log("Target line disabled");
                    this.m_targetLine.Dispose();
                    this.m_targetLine = null;
                }
            }
        }

        private void UpdateTargetLine()
        {
            var position = this.Location.Position;
            this.m_targetLine.SetPositions(position, position + this.Location.SurfaceUpVector * TARGET_LINE_END_ALTITUDE);
        }

        #region ITargetable

        public Vector3 GetFwdVector() { return Vector3d.up; }
        public string GetName() { return this.Name; }
        public Vector3 GetObtVelocity() { return Vector3.zero; }
        public Orbit GetOrbit() { return null; }
        public OrbitDriver GetOrbitDriver() { return null; }
        public Vector3 GetSrfVelocity() { return Vector3.zero; }
        public Transform GetTransform() { return this.m_gameObject.transform; }
        public Vessel GetVessel() { return this.m_vessel; }
        public VesselTargetModes GetTargetingMode() { return VesselTargetModes.DirectionAndVelocity; }

        #endregion ITargetable
    }
}
