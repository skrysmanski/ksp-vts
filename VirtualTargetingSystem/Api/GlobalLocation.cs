using UnityEngine;

namespace KerbalSpaceProgram.Api
{
    internal struct GlobalLocation
    {
        /// <summary>
        /// The celestial body of this location.
        /// </summary>
        [PublicAPI]
        public CelestialBody Body { get; private set; }

        /// <summary>
        /// The coordinates of this location.
        /// </summary>
        [PublicAPI]
        public Coordinates Coordinates { get; private set; }

        /// <summary>
        /// The surface altitude at the specified coordinates.
        /// </summary>
        [PublicAPI]
        public double SurfaceAltitude
        {
            get 
            {
                return this.IsOnSurface ? this.Altitude : this.Body.GetTerrainAltitude(this.Coordinates);
            }
        }

        /// <summary>
        /// Returns a unit vector perpendicular to the surface of the body at the given latitude and
        /// longitude (pretending for the moment that the body is a perfect sphere).
        /// </summary>
        [PublicAPI]
        public Vector3d SurfaceUpVector
        {
            get { return this.Body.GetSurfaceNVector(this.Coordinates); }
        }


        /// <summary>
        /// The altitude as specified in the constructor. Is never lower than <see cref="SurfaceAltitude"/>.
        /// </summary>
        [PublicAPI]
        public double Altitude { get; private set; }

        /// <summary>
        /// Wether <see cref="Altitude"/> is on the surface of <see cref="Body"/>.
        /// </summary>
        [PublicAPI]
        public bool IsOnSurface { get; private set; }

        [PublicAPI]
        public Vector3d Position
        {
            get { return this.Body.GetWorldSurfacePosition(this.Coordinates, this.Altitude); }
        }

        public GlobalLocation([NotNull] CelestialBody body, Coordinates coordinates) : this()
        {
            this.Body = body;
            this.Coordinates = coordinates;
            this.Altitude = body.GetTerrainAltitude(coordinates);
            this.IsOnSurface = true;
        }

        public GlobalLocation([NotNull] CelestialBody body, Coordinates coordinates, double altitude) : this()
        {
            this.Body = body;
            this.Coordinates = coordinates;

            double surfaceAltitude = body.GetTerrainAltitude(coordinates);
            if (surfaceAltitude >= altitude)
            {
                this.Altitude = surfaceAltitude;
                this.IsOnSurface = true;
            }
            else
            {
                this.Altitude = altitude;
                this.IsOnSurface = false;
            }
        }
    }
}
