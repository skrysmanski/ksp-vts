using System;
using UnityEngine;

namespace KerbalSpaceProgram.Api
{
    internal abstract class Window
    {
        private readonly int m_windowId;
        
        [PublicAPI, CanBeNull]
        public GUIStyle WindowStyle { get; set; }

        [PublicAPI, NotNull]
        public string Title { get; set; }

        [PublicAPI]
        public Rect WindowRect { get; set; }

        [PublicAPI]
        public bool IsDragable { get; set; }

        [PublicAPI]
        public bool IsOpen { get; set; }

        protected Window(int windowId, string title, GUIStyle windowStyle = null, Vector2 windowPos = new Vector2())
            : this(windowId, title, windowStyle, new Rect(windowPos.x, windowPos.y, 0, 0))
        {
        }

        protected Window(int windowId, string title, GUIStyle windowStyle = null, Rect windowRect = new Rect())
        {
            this.m_windowId = windowId;
            this.Title = title;
            this.WindowStyle = windowStyle;
            this.WindowRect = windowRect;
            this.IsDragable = true;
            this.IsOpen = true;
        }

        /// <summary>
        /// Processes this window (i.e. displays it and calls <see cref="OnWindow()"/>).
        /// </summary>
        /// <returns>Whether the window will remain open (<c>true</c>) or has been closed (<c>false</c>).</returns>
        [PublicAPI]
        public void ProcessWindow()
        {
            if (this.IsOpen)
            {
                this.WindowRect = GUILayout.Window(this.m_windowId, this.WindowRect, this.OnWindow, this.Title,
                                                   this.WindowStyle ?? HighLogic.Skin.window);
            }
        }

        private void OnWindow(int windowId)
        {
            OnWindow();

            if (this.IsDragable)
            {
                GUI.DragWindow();
            }
        }

        protected abstract void OnWindow();

        [PublicAPI]
        protected static ControlGroup HorizontalLayout
        {
            get { return new ControlGroup(horizontal: true); }
        }

        [PublicAPI]
        protected static ControlGroup VerticalLayout
        {
            get { return new ControlGroup(horizontal: false); }
        }

        [PublicAPI]
        protected static void Label(string text, GUIStyle guiStyle = null)
        {
            GUILayout.Label(text, guiStyle ?? HighLogic.Skin.label);
        }

        [PublicAPI]
        protected static string TextField(string text, int maxLength = -1, GUIStyle guiStyle = null)
        {
            return GUILayout.TextField(text, maxLength, guiStyle ?? HighLogic.Skin.textField);
        }

        [PublicAPI]
        protected static bool Button(string text, GUIStyle guiStyle = null)
        {
            return GUILayout.Button(text, guiStyle ?? HighLogic.Skin.button);
        }

        protected struct ControlGroup : IDisposable
        {
            private readonly bool m_isHorizontal;

            public ControlGroup(bool horizontal)
            {
                this.m_isHorizontal = horizontal;
                if (horizontal)
                {
                    GUILayout.BeginHorizontal();
                }
                else
                {
                    GUILayout.BeginVertical();
                }
            }

            public void Dispose()
            {
                if (this.m_isHorizontal)
                {
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.EndVertical();
                }
            }
        }
    }
}
