// ----------------------------------------------------------------------------
// <copyright file="ScriptAssembly.cs" company="Cobos SDK">
//
//      Copyright (c) 2009-2012 Nicholas Davis - nick@cobos.co.uk
//
//      Cobos Software Development Kit
//
//      Permission is hereby granted, free of charge, to any person obtaining
//      a copy of this software and associated documentation files (the
//      "Software"), to deal in the Software without restriction, including
//      without limitation the rights to use, copy, modify, merge, publish,
//      distribute, sublicense, and/or sell copies of the Software, and to
//      permit persons to whom the Software is furnished to do so, subject to
//      the following conditions:
//      
//      The above copyright notice and this permission notice shall be
//      included in all copies or substantial portions of the Software.
//      
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//      EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//      MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//      NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//      LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//      OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//      WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Script
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Represents an assembly containing scripts.
    /// </summary>
    public class ScriptAssembly
    {
        /// <summary>
        /// The path of the assembly.
        /// </summary>
        public readonly string AssemblyPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptAssembly"/> class.
        /// </summary>
        /// <param name="assemblyPath">The assembly path.</param>
        public ScriptAssembly(string assemblyPath)
        {
            if (!Path.IsPathRooted(assemblyPath))
            {
                assemblyPath = Path.Combine(Environment.CurrentDirectory, assemblyPath);
            }

            this.AssemblyPath = assemblyPath;
        }

        /// <summary>
        /// Invoke the named method.
        /// </summary>
        /// <param name="class">The name of the script class.</param>
        /// <param name="method">The name of the method.</param>
        /// <param name="args">The method arguments.</param>
        public void Invoke(string @class, string method, string[] args)
        {
            Type objectType = this.GetScriptClass(@class);

            if (objectType == null)
            {
                if (string.IsNullOrEmpty(@class))
                {
                    throw new ScriptException("Failed to find any script classes in the compiled script: {0}", this.AssemblyPath);
                }
                else
                {
                    throw new ScriptException("Failed to find the script class {0} in the compiled script: {1}", @class, this.AssemblyPath);
                }
            }

            MethodInfo methodInfo = this.GetClassMethod(objectType, method);

            if (methodInfo == null)
            {
                if (string.IsNullOrEmpty(method))
                {
                    throw new ScriptException("Failed to find any script methods in the compiled script: {0}", this.AssemblyPath);
                }
                else
                {
                    throw new ScriptException("Failed to find the script method {0} in the compiled script: {1}", method, this.AssemblyPath);
                }
            }

            object instance = Activator.CreateInstance(objectType);

            if (instance == null)
            {
                throw new ScriptException("Failed to instantiate the scripting object.");
            }

            object[] methodArgs = this.GetMethodArguments(methodInfo, args);

            this.TraceMethodCall(objectType.Name, methodInfo.Name, methodArgs);

            IDisposable dispose = instance as IDisposable;

            if (dispose != null)
            {
                using (dispose)
                {
                    methodInfo.Invoke(instance, methodArgs);
                }
            }
            else
            {
                methodInfo.Invoke(instance, methodArgs);
            }

            ScriptTrace.Instance.TraceInformation("Script finished!");
        }

        /// <summary>
        /// Trace a method call for diagnostic purposes.
        /// </summary>
        /// <param name="className">The name of the class.</param>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="methodArgs">The method arguments.</param>
        private void TraceMethodCall(string className, string methodName, object[] methodArgs)
        {
            StringBuilder trace = new StringBuilder(128);
            trace.AppendFormat("Invoking {0}.{1}(", className, methodName);

            if (methodArgs != null)
            {
                for (int i = 0; i < methodArgs.Length; ++i)
                {
                    object arg = methodArgs[i];

                    if (arg is string)
                    {
                        trace.Append("\"" + arg + "\"");
                    }
                    else
                    {
                        trace.Append(arg);
                    }

                    if (i < methodArgs.Length - 1)
                    {
                        trace.Append(", ");
                    }
                }
            }

            trace.Append(")");

            ScriptTrace.Instance.TraceInformation(trace.ToString());
        }

        /// <summary>
        /// Get the named script class.
        /// </summary>
        /// <param name="name">The name of the class to find.  May be null to return the first script class found.</param>
        /// <returns>The script class if found; otherwise null.</returns>
        private Type GetScriptClass(string name)
        {
            if (!File.Exists(this.AssemblyPath))
            {
                throw new ScriptException("The assembly path does not exist: {0}", this.AssemblyPath);
            }

            Assembly assembly = Assembly.Load(AssemblyName.GetAssemblyName(this.AssemblyPath));

            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                object[] attrs = type.GetCustomAttributes(typeof(ScriptClass), true);

                if (attrs.Length > 0)
                {
                    if (string.IsNullOrEmpty(name) || string.Compare(type.Name, name, true) == 0)
                    {
                        return type;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get the named method from the type.
        /// </summary>
        /// <param name="type">The type to look in.</param>
        /// <param name="method">The method to find.</param>
        /// <returns>The named method if found; otherwise null.</returns>
        private MethodInfo GetClassMethod(Type type, string method)
        {
            foreach (MethodInfo info in type.GetMethods())
            {
                object[] attrs = info.GetCustomAttributes(typeof(ScriptMethod), false);

                if (attrs.Length > 0)
                {
                    if (string.IsNullOrEmpty(method) || string.Compare(info.Name, method, true) == 0)
                    {
                        return info;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get the arguments for a method.
        /// </summary>
        /// <param name="method">The method information.</param>
        /// <param name="args">An array of name=value pairs of arguments</param>
        /// <returns>The arguments.</returns>
        private object[] GetMethodArguments(MethodInfo method, string[] args)
        {
            // Check the parameters match the supplied arguments
            if (args == null || args.Length == 0)
            {
                return null;
            }

            ParameterInfo[] paramInfos = method.GetParameters();

            if (paramInfos == null || paramInfos.Length == 0)
            {
                return null;
            }

            ////if ( paramInfos.Length != args.Length )
            ////{
            ////    throw new ScriptException( "There is a mismatch between the number of arguments for method {0}: Expected {1}; Actual {2}", method.Name, paramInfos.Length, args.Length );
            ////}

            // Tokenise all of the arguments into name value pairs
            Dictionary<string, string> nameValue = new Dictionary<string, string>(paramInfos.Length, StringComparer.CurrentCultureIgnoreCase);

            foreach (string arg in args)
            {
                string[] pair = arg.Split('=');

                if (pair == null || pair.Length != 2)
                {
                    throw new ScriptException("Badly formed method argument found for method {0}: Expected name=value; Actual {1}", method.Name, arg);
                }

                nameValue[pair[0]] = pair[1];
            }

            // Parse all of the arguments into the correct types
            object[] @params = new object[paramInfos.Length];

            for (int i = 0; i < paramInfos.Length; ++i)
            {
                ParameterInfo paramInfo = paramInfos[i];
                Type paramType = paramInfo.ParameterType;

                string value;

                if (nameValue.TryGetValue(paramInfo.Name, out value))
                {
                    try
                    {
                        @params[i] = TypeDescriptor.GetConverter(paramType).ConvertFrom(value);
                    }
                    catch (Exception e)
                    {
                        throw new ScriptException("A type conversion error occured attempting to convert the parameter {0} for method {1}", e, paramInfo.Name, method.Name);
                    }
                }
                else
                {
                    // just assign the default value
                    @params[i] = paramType.IsValueType ? Activator.CreateInstance(paramType) : null;
                }
            }

            return @params;
        }
    }
}
