using System;

namespace KerbalSpaceProgram.Api
{
    internal static class AngleUtils
    {
        [PublicAPI]
        public static double ClampDegrees360(double angle)
        {
            angle = angle % 360.0;
            if (angle < 0) return angle + 360.0;
            else return angle;
        }

        [PublicAPI]
        public static double ClampDegrees180(double angle)
        {
            angle = ClampDegrees360(angle);
            if (angle > 180) angle -= 360;
            return angle;
        }

        /// <summary>
        /// Converts an angle (in degrees) to "Degrees° Minutes' Seconds''" notation.
        /// </summary>
        [PublicAPI, NotNull]
        public static string AngleToDMS(double angle)
        {
            int degrees = (int)Math.Floor(Math.Abs(angle));
            int minutes = (int)Math.Floor(60 * (Math.Abs(angle) - degrees));
            int seconds = (int)Math.Floor(3600 * (Math.Abs(angle) - degrees - minutes / 60.0));

            return string.Format("{0:0}° {1:00}' {2:00}\"", degrees, minutes, seconds);
        }
    }
}
