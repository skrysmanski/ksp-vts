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
