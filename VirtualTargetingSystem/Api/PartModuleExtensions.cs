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

using UnityEngine;

namespace KerbalSpaceProgram.Api
{
    internal static class PartModuleExtensions
    {
        /// <summary>
        /// Whether this part module is the primary module, when the same part module
        /// is available multiple times on the same vessel.
        /// </summary>
        [PublicAPI]
        public static bool IsPrimary([NotNull] this PartModule thisPartModule)
        {
            var vessel = thisPartModule.part.vessel;
            if (vessel == null)
            {
                Debug.LogError("Can't check for primary part in the editor.");
                return false;
            }

            foreach (var part in vessel.Parts)
            {
                if (part.Modules.Contains(thisPartModule.ClassID))
                {
                    return part == thisPartModule.part;
                }
            }

            return false;
        }
    }
}
