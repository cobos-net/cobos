using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Intergraph.Oz.Utilities.Wrappers
{
    /// <summary>
    /// Provides Singleton&lt;T&gt.Instance
    /// This class is thread-safe
    /// </summary>
    /// <remarks>
    /// A private or protected constructor must be implemented in the T class
    /// </remarks>
    public static class Singleton<T>
           where T : class
    {
        static volatile T m_Instance;
        static object m_Lock = new object();

        static Singleton()
        {
        }

        /// <summary>
        /// Use as Singleton&lt;MyClass&gt;.Instance
        /// </summary>
        /// <exception cref="SingletonException">If the T classes constructor is not private or protected</exception>
        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    lock (m_Lock)
                    {
                        if (m_Instance == null)
                        {
                            ConstructorInfo constructor = null;

                            try
                            {
                                // Binding flags exclude public constructors.
                                constructor = typeof(T).GetConstructor(BindingFlags.Instance |
                                              BindingFlags.NonPublic, null, new Type[0], null);
                            }
                            catch (Exception exception)
                            {
                                throw new SingletonException(exception);
                            }

                            if (constructor == null || constructor.IsAssembly)
                                // Also exclude internal constructors.
                                throw new SingletonException(string.Format("A private or " +
                                      "protected constructor is missing for '{0}'.", typeof(T).Name));

                            m_Instance = (T)constructor.Invoke(null);
                        }
                    }
                }

                return m_Instance;
            }
        }
    }
}
