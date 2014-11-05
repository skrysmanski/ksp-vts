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

using System.Collections.Generic;
using System.Linq;

namespace KerbalSpaceProgram.Api
{
    internal static class CelestialBodyHelper
    {
        /// <summary>
        /// The list of all existing celestial bodies.
        /// </summary>
        [PublicAPI, NotNull]
        public static IEnumerable<CelestialBody> AllBodies
        {
            get { return FlightGlobals.Bodies; }
        }

        /// <summary>
        /// The homeworld (Kerbin).
        /// </summary>
        [PublicAPI, NotNull]
        public static CelestialBody Homeworld
        {
            get { return AllBodies.Single(body => body.isHomeWorld); }
        }

        [PublicAPI, CanBeNull]
        public static CelestialBody FindBody([NotNull] string name)
        {
            return AllBodies.SingleOrDefault(body => body.bodyName == name);
        }
    }
}
