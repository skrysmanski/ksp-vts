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

        /// <summary>
        /// The biome at the specified location.
        /// </summary>
        [PublicAPI, NotNull]
        public string BiomeName
        {
            get { return ScienceUtil.GetExperimentBiome(this.Body, this.Coordinates.Latitude, this.Coordinates.Longitude); }
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
