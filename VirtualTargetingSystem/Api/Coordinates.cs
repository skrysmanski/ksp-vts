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

namespace KerbalSpaceProgram.Api
{
    /// <summary>
    /// Represents coordinates in longitude and latitude.
    /// </summary>
    internal struct Coordinates
    {
        public readonly double Latitude;
        public readonly double Longitude;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="latitude">latitude in degrees</param>
        /// <param name="longitude">longitude in degrees</param>
        public Coordinates(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        [PublicAPI, NotNull]
        public string ToStringDecimal(bool newline = false, int precision = 3)
        {
            double clampedLongitude = AngleUtils.ClampDegrees180(this.Longitude);
            double latitudeAbs = Math.Abs(this.Latitude);
            double longitudeAbs = Math.Abs(clampedLongitude);

            return latitudeAbs.ToString("F" + precision) + "° " + (this.Latitude > 0 ? "N" : "S") + (newline ? "\n" : ", ")
                + longitudeAbs.ToString("F" + precision) + "° " + (clampedLongitude > 0 ? "E" : "W");
        }

        [PublicAPI, NotNull]
        public string ToStringDMS(bool newline = false)
        {
            double clampedLongitude = AngleUtils.ClampDegrees180(this.Longitude);

            return AngleUtils.AngleToDMS(this.Latitude) + (this.Latitude > 0 ? " N" : " S") + (newline ? "\n" : ", ")
                 + AngleUtils.AngleToDMS(clampedLongitude) + (clampedLongitude > 0 ? " E" : " W");
        }
    }
}
