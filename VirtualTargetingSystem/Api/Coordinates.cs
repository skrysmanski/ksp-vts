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
