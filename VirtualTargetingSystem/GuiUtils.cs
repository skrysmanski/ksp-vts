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
