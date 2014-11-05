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
