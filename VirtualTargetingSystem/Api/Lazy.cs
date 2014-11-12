using System;

namespace KerbalSpaceProgram.Api
{
    [PublicAPI]
    public sealed class Lazy<T>
    {
        private bool m_valueSet;
        private T m_value;
        private Func<T> m_createFunc;

        public T Value
        {
            get
            {
                if (!this.m_valueSet)
                {
                    lock (this)
                    {
                        if (!this.m_valueSet)
                        {
                            this.m_value = this.m_createFunc();
                            this.m_valueSet = true;
                            this.m_createFunc = null;
                        }
                    }
                }

                return this.m_value;
            }
        }

        public Lazy([NotNull] Func<T> createFunc)
        {
            this.m_createFunc = createFunc;
        }
    }
}
