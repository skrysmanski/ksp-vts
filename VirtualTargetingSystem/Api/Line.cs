//
// This file is part of the Kerbal Space Program Community API.
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
using System.Linq;
using UnityEngine;

namespace KerbalSpaceProgram.Api
{
    internal class Line : MonoBehaviour, IDisposable
    {
        private const string LINE_SHADER_DEF = @"Shader ""Vertex Colors/Alpha"" 
                                                 {
                                                    Category
                                                    {
                                                        Tags
                                                        {
                                                            ""Queue""=""Overlay""
                                                            ""IgnoreProjector""=""True""
                                                            ""RenderType""=""Transparent""
                                                        }

                                                        SubShader
                                                        {
                                                            Cull Off
                                                            ZWrite On
                                                            AlphaTest Off
                                                            Blend Add

                                                            Pass
                                                            {
                                                                BindChannels
                                                                {
                                                                    Bind ""Color"", color Bind ""Vertex"", vertex
                                                                }
                                                            }
                                                        }
                                                    }
                                                }";

        private static readonly Lazy<Material> s_lineMaterial =
            new Lazy<Material>(() => new Material(LINE_SHADER_DEF));

        /// <summary>
        /// Width of the line in pixels. Defaults to 1 pixel.
        /// </summary>
        [PublicAPI]
        public float LineWidth { get; set; }

        /// <summary>
        /// The material of this line.
        /// </summary>
        [PublicAPI]
        public Material Material
        {
            get { return this.renderer.material; }
            set { this.renderer.material = value; }
        }

        /// <summary>
        /// The color of this line. Defaults to <see cref="UnityEngine.Color.white"/>.
        /// </summary>
        [PublicAPI]
        public Color Color
        {
            get { return this.m_meshFilter.mesh.colors[0]; }
            set { this.m_meshFilter.mesh.colors = Enumerable.Repeat(value, 4).ToArray(); }
        }

        private Vector3d m_startWorldPos;
        private Vector3d m_endWorldPos;

        private MeshFilter m_meshFilter;
        private readonly Vector3[] m_points2D = new Vector3[4];
        private readonly Vector3[] m_points3D = new Vector3[4];

        [Obsolete("Don't use this constructor. Use 'Instatiate()' instead.")]
        public Line() { }

        /// <summary>
        /// Creates a new line instance. Destroy it via <see cref="Dispose"/> when
        /// you no longer need it.
        /// </summary>
        /// <param name="lineName">the name of the line; defaults to "Line"</param>
        [NotNull]
        public static Line Instantiate(string lineName = "Line")
        {
            return new GameObject(lineName, typeof(Line)).GetComponent<Line>();
        }

        /// <summary>
        /// Sets the line's start and end pos.
        /// </summary>
        [PublicAPI]
        public void SetPositions(Vector3d startWorldPos, Vector3d endWorldPos)
        {
            this.m_startWorldPos = startWorldPos;
            this.m_endWorldPos = endWorldPos;
        }

        // TODO: We need to connect this to the camera somehow so that we can change this to "PreCull()". Without this,
        //   the line's coordinates are always one frame behind (probably because the camera's position is changed after
        //   this method has been called).
        [UsedImplicitly]
        public void LateUpdate()
        {
            Camera theCamera = PlanetariumCamera.Camera;

            var startScreenPos = theCamera.WorldToScreenPoint(ScaledSpace.LocalToScaledSpace(this.m_startWorldPos));
            var endScreenPos = theCamera.WorldToScreenPoint(ScaledSpace.LocalToScaledSpace(this.m_endWorldPos));

            Vector2 directionOnScreen = endScreenPos - startScreenPos;
            directionOnScreen.Normalize();

            var segment = new Vector3(-directionOnScreen.y, directionOnScreen.x, 0) * (LineWidth / 2);

            bool draw3DLines = true;

            if (MapView.MapIsEnabled && !MapView.Draw3DLines)
            {
                var dist = Screen.height / 2.0f + 0.01f;
                startScreenPos.z = startScreenPos.z >= 0.15f ? dist : -dist;
                endScreenPos.z = endScreenPos.z >= 0.15f ? dist : -dist;
                draw3DLines = false;
            }

            this.m_points2D[0] = (startScreenPos - segment);
            this.m_points2D[1] = (startScreenPos + segment);
            this.m_points2D[2] = (endScreenPos - segment);
            this.m_points2D[3] = (endScreenPos + segment);

            this.m_points3D[0] = theCamera.ScreenToWorldPoint(this.m_points2D[0]);
            this.m_points3D[1] = theCamera.ScreenToWorldPoint(this.m_points2D[1]);
            this.m_points3D[2] = theCamera.ScreenToWorldPoint(this.m_points2D[2]);
            this.m_points3D[3] = theCamera.ScreenToWorldPoint(this.m_points2D[3]);

            this.m_meshFilter.mesh.vertices = draw3DLines ? this.m_points3D : this.m_points2D;
            this.m_meshFilter.mesh.RecalculateBounds();
        }

        [PublicAPI]
        public void SetActive(bool activate)
        {
            this.gameObject.SetActive(activate);
        }

        [UsedImplicitly]
        public void Awake()
        {
            SetupMesh();
            this.gameObject.layer = 18;
            this.gameObject.AddComponent<MeshRenderer>();
            this.renderer.material = s_lineMaterial.Value;

            this.LineWidth = 1.0f;
            this.Color = Color.white;
        }

        private void SetupMesh()
        {
            this.m_meshFilter = gameObject.AddComponent<MeshFilter>();
            this.m_meshFilter.mesh = new Mesh();
            this.m_meshFilter.mesh.name = this.name;
            this.m_meshFilter.mesh.vertices = new Vector3[4];
            this.m_meshFilter.mesh.uv = new [] { new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 1), new Vector2(1, 0) };
            this.m_meshFilter.mesh.SetIndices(new [] { 0, 2, 1, 2, 3, 1 }, MeshTopology.Triangles, 0);
            this.m_meshFilter.mesh.MarkDynamic();

            this.SetActive(false);
        }

        [UsedImplicitly]
        public void OnDestroy()
        {
            Destroy(this.m_meshFilter);
        }

        public void Dispose()
        {
            Destroy(this.gameObject);
        }
    }
}
