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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;

namespace Cobos.Script
{
    public class ScriptAssembly
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly string AssemblyPath;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyPath"></param>
        public ScriptAssembly(string assemblyPath)
        {
            if (!Path.IsPathRooted(assemblyPath))
            {
                assemblyPath = Path.Combine(Environment.CurrentDirectory, assemblyPath);
            }

            AssemblyPath = assemblyPath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scriptName"></param>
        /// <param name="args"></param>
        public void Invoke(string @class, string method, string[] args)
        {
            Type objectType = GetScriptClass(@class);

            if (objectType == null)
            {
                if (string.IsNullOrEmpty(@class))
                {
                    throw new ScriptException("Failed to find any script classes in the compiled script: {0}", AssemblyPath);
                }
                else
                {
                    throw new ScriptException("Failed to find the script class {0} in the compiled script: {1}", @class, AssemblyPath);
                }
            }

            MethodInfo methodInfo = GetClassMethod(objectType, method);

            if (methodInfo == null)
            {
                if (string.IsNullOrEmpty(method))
                {
                    throw new ScriptException("Failed to find any script methods in the compiled script: {0}", AssemblyPath);
                }
                else
                {
                    throw new ScriptException("Failed to find the script method {0} in the compiled script: {1}", method, AssemblyPath);
                }
            }

            object instance = Activator.CreateInstance(objectType);

            if (instance == null)
            {
                throw new ScriptException("Failed to instantiate the scripting object.");
            }

            object[] methodArgs = GetMethodArguments(methodInfo, args);

            TraceMethodCall(objectType.Name, methodInfo.Name, methodArgs);

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

            LogSingleton.Instance.TraceInformation("Script finished!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
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

            LogSingleton.Instance.TraceInformation(trace.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
        private Type GetScriptClass(string @object)
        {
            if (!File.Exists(AssemblyPath))
            {
                throw new ScriptException("The assembly path does not exist: {0}", AssemblyPath);
            }

            Assembly assembly = Assembly.Load(AssemblyName.GetAssemblyName(AssemblyPath));

            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                object[] attrs = type.GetCustomAttributes(typeof(ScriptClass), true);

                if (attrs.Length > 0)
                {
                    if (string.IsNullOrEmpty(@object) || String.Compare(type.Name, @object, true) == 0)
                    {
                        return type;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private MethodInfo GetClassMethod(Type type, string method)
        {
            foreach (MethodInfo info in type.GetMethods())
            {
                object[] attrs = info.GetCustomAttributes(typeof(ScriptMethod), false);

                if (attrs.Length > 0)
                {
                    if (string.IsNullOrEmpty(method) || String.Compare(info.Name, method, true) == 0)
                    {
                        return info;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="args">An array of name=value pairs of arugments</param>
        /// <returns></returns>
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

            //if ( paramInfos.Length != args.Length )
            //{
            //    throw new ScriptException( "There is a mismatch between the number of arguments for method {0}: Expected {1}; Actual {2}", method.Name, paramInfos.Length, args.Length );
            //}

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
                    //throw new ScriptException( "The parameter {0} for method {1} was not found in the list of script arguments", paramInfo.Name, method.Name );
                    // just assign the default value
                    @params[i] = paramType.IsValueType ? Activator.CreateInstance(paramType) : null;
                }
            }

            return @params;
        }
    }
}
