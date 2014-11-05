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
