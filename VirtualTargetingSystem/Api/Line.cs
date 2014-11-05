using System;
using System.Linq;
using UnityEngine;

namespace KerbalSpaceProgram.Api
{
    internal class Line : MonoBehaviour, IDisposable
    {
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

        [UsedImplicitly]
        public void Update()
        {
            // ReSharper disable once LocalVariableHidesMember
            Camera camera = PlanetariumCamera.Camera;

            var startScreenPos = camera.WorldToScreenPoint(ScaledSpace.LocalToScaledSpace(this.m_startWorldPos));
            var endScreenPos = camera.WorldToScreenPoint(ScaledSpace.LocalToScaledSpace(this.m_endWorldPos));

            Vector2 directionOnScreen = endScreenPos - startScreenPos;
            directionOnScreen.Normalize();

            var segment = new Vector3(-directionOnScreen.y, directionOnScreen.x, 0) * (LineWidth / 2);

            if (MapView.MapIsEnabled && !MapView.Draw3DLines)
            {
                var dist = Screen.height / 2.0f + 0.01f;
                startScreenPos.z = startScreenPos.z >= 0.15f ? dist : -dist;
                endScreenPos.z = endScreenPos.z >= 0.15f ? dist : -dist;
            }

            this.m_points2D[0] = (startScreenPos - segment);
            this.m_points2D[1] = (startScreenPos + segment);
            this.m_points2D[2] = (endScreenPos - segment);
            this.m_points2D[3] = (endScreenPos + segment);

            this.m_points3D[0] = camera.ScreenToWorldPoint(this.m_points2D[0]);
            this.m_points3D[1] = camera.ScreenToWorldPoint(this.m_points2D[1]);
            this.m_points3D[2] = camera.ScreenToWorldPoint(this.m_points2D[2]);
            this.m_points3D[3] = camera.ScreenToWorldPoint(this.m_points2D[3]);

            this.m_meshFilter.mesh.vertices = MapView.Draw3DLines ? this.m_points3D : this.m_points2D;
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
            this.gameObject.layer = 31;
            this.gameObject.AddComponent<MeshRenderer>();
            this.renderer.material = new Material("Shader \"Vertex Colors/Alpha\" {Category{Tags {\"Queue\"=\"Transparent\" \"IgnoreProjector\"=\"True\" \"RenderType\"=\"Transparent\"}SubShader {Cull Off ZWrite On Blend SrcAlpha OneMinusSrcAlpha Pass {BindChannels {Bind \"Color\", color Bind \"Vertex\", vertex}}}}}");

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
