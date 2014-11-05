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
    internal static class GuiUtils
    {
        private static Material s_material;

        public static Material Material
        {
            get
            {
                if (s_material == null)
                {
                    s_material = new Material(Shader.Find("Particles/Additive"));
                }

                return s_material;
            }
        }

        public static void DrawMapViewGroundMarker(GlobalLocation location, Color c, double rotation = 0, double radius = 0)
        {
            var body = location.Body;
            var coordinates = location.Coordinates;

            Vector3d up = location.SurfaceUpVector;
            var height = body.pqsController.GetSurfaceHeight(  QuaternionD.AngleAxis(coordinates.Longitude, Vector3d.down)
                                                             * QuaternionD.AngleAxis(coordinates.Latitude, Vector3d.forward)
                                                             * Vector3d.right);
            if (height < body.Radius)
            {
                height = body.Radius;
            }

            Vector3d center = body.position + height * up;

            if (body.IsOccluding(center))
            {
                return;
            }

            Vector3d north = Vector3d.Exclude(up, body.transform.up).normalized;

            if (radius <= 0)
            {
                radius = body.Radius / 15;
            }

            GLTriangleMap(new []{
                center,
                center + radius * (QuaternionD.AngleAxis(rotation - 10, up) * north),
                center + radius * (QuaternionD.AngleAxis(rotation + 10, up) * north)
            }, c);

            GLTriangleMap(new []{
                center,
                center + radius * (QuaternionD.AngleAxis(rotation + 110, up) * north),
                center + radius * (QuaternionD.AngleAxis(rotation + 130, up) * north)
            }, c);

            GLTriangleMap(new []{
                center,
                center + radius * (QuaternionD.AngleAxis(rotation - 110, up) * north),
                center + radius * (QuaternionD.AngleAxis(rotation - 130, up) * north)
            }, c);
        }

        private static void GLTriangleMap(Vector3d[] worldVertices, Color c)
        {
            Draw(GL.TRIANGLES, c,
                WorldToMapViewScreen(worldVertices[0]),
                WorldToMapViewScreen(worldVertices[1]),
                WorldToMapViewScreen(worldVertices[2]));
        }

        private static Vector2 WorldToMapViewScreen(Vector3d worldPosition)
        {
            return PlanetariumCamera.Camera.WorldToScreenPoint(ScaledSpace.LocalToScaledSpace(worldPosition));
        }

        private static void Draw(int glMode, Color color, params Vector2[] vertices)
        {
            GL.PushMatrix();
            Material.SetPass(0);
            GL.LoadOrtho();
            GL.Begin(glMode);
            GL.Color(color);

            foreach (var vertex in vertices)
            {
                GL.Vertex3(vertex.x / Screen.width, vertex.y / Screen.height, 0);
            }

            GL.End();
            GL.PopMatrix();
        }
    }
}
