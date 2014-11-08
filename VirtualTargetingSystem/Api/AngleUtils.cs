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
using System.Text.RegularExpressions;

namespace KerbalSpaceProgram.Api
{
    internal static class AngleUtils
    {
        private static readonly Regex DMS_REGEX = new Regex(@"^ *(-?)(\d+)° +(\d)+' +(\d+)(?:''|"") *$");

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

        [PublicAPI]
        public static bool TryParseDMS([NotNull] string dms, out double angle)
        {
            var match = DMS_REGEX.Match(dms);
            if (!match.Success)
            {
                angle = 0;
                return false;
            }

            int sign = match.Groups[1].Value == "-" ? -1 : 1;
            int degrees = int.Parse(match.Groups[2].Value);
            int minutes = int.Parse(match.Groups[3].Value);
            int seconds = int.Parse(match.Groups[4].Value);

            angle = sign * degrees + (minutes / 60.0) + (seconds / 3600.0);
            return true;
        }
    }
}
