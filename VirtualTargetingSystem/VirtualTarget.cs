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
            this.m_gameObject.transform.position = location.Position;
        }

        public void Draw(Color color)
        {
            GuiUtils.DrawMapViewGroundMarker(this.Location, color);
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
