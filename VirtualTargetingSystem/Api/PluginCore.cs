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
    /// <summary>
    /// Core of a plugin. You may use a child class of this class as central point where
    /// all "global" variables are stored. Your child class should be attributed with
    /// <c>[KSPAddon(..., false)]</c>.
    /// </summary>
    /// <typeparam name="TConcrete">the type of the core as used for <see cref="Instance"/></typeparam>
    public abstract class PluginCore<TConcrete> : MonoBehaviour where TConcrete : PluginCore<TConcrete>
    {
        /// <summary>
        /// Instance of this plugin core.
        /// </summary>
        [PublicAPI]
        public static TConcrete Instance { get; private set; }

        /// <summary>
        /// Called when KSP activates this plugin core.
        /// </summary>
        [UsedImplicitly]
        public void Start()
        {
            Instance = (TConcrete)this;

            OnStart();
        }

        /// <summary>
        /// Called when KSP activates this plugin core and <see cref="Instance"/> has been set.
        /// </summary>
        protected abstract void OnStart();

        /// <summary>
        /// Called when KSP/Unity destroys this core.
        /// </summary>
        [UsedImplicitly]
        public void OnDestroy()
        {
            OnDispose();

            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <summary>
        /// Called when KSP/Unity destroys this core, right before <see cref="Instance"/> is unset.
        /// </summary>
        protected abstract void OnDispose();
    }
}
