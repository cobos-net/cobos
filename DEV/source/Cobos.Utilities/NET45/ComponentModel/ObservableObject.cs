// ----------------------------------------------------------------------------
// <copyright file="ObservableObject.cs" company="Cobos SDK">
//
//      Copyright (c) 2009-2014 Nicholas Davis - nick@cobos.co.uk
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

namespace Cobos.Utilities.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Lightweight implementation of an Observable object based on <c>MvvmLight.ObservableObject</c>.
    /// </summary>
    public class ObservableObject : INotifyPropertyChanged, INotifyPropertyChanging
    {
        /// <summary>
        /// Occurs after a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs before a property value changes.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// Gets access to the PropertyChanging event handler to derived classes.
        /// </summary>
        protected PropertyChangingEventHandler PropertyChangingHandler
        {
            get
            {
                return this.PropertyChanging;
            }
        }

        /// <summary>
        /// Gets access to the PropertyChanged event handler to derived classes.
        /// </summary>
        protected PropertyChangedEventHandler PropertyChangedHandler
        {
            get
            {
                return this.PropertyChanged;
            }
        }

        /// <summary>
        /// Verifies that a property name exists in this ViewModel. This method
        /// can be called before the property is used, for instance before
        /// calling RaisePropertyChanged. It avoids errors when a property name
        /// is changed but some places are missed.
        /// </summary>
        /// <remarks>This method is only active in DEBUG mode.</remarks>
        /// <param name="propertyName">The name of the property that will be
        /// checked.</param>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            var myType = GetType();

            if (!string.IsNullOrEmpty(propertyName) && myType.GetProperty(propertyName) == null)
            {
                var descriptor = this as ICustomTypeDescriptor;

                if (descriptor != null)
                {
                    if (descriptor.GetProperties()
                        .Cast<PropertyDescriptor>()
                        .Any(property => property.Name == propertyName))
                    {
                        return;
                    }
                }

                throw new ArgumentException("Property not found", propertyName);
            }
        }

        /// <summary>
        /// Raises the PropertyChanging event if needed.
        /// </summary>
        /// <remarks>If the propertyName parameter
        /// does not correspond to an existing property on the current class, an
        /// exception is thrown in DEBUG configuration only.</remarks>
        /// <param name="propertyName">The name of the property that
        /// changed.</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "This cannot be an event")]
        protected virtual void RaisePropertyChanging(string propertyName)
        {
            this.VerifyPropertyName(propertyName);

            var handler = this.PropertyChanging;
            if (handler != null)
            {
                handler(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event if needed.
        /// </summary>
        /// <remarks>If the propertyName parameter
        /// does not correspond to an existing property on the current class, an
        /// exception is thrown in DEBUG configuration only.</remarks>
        /// <param name="propertyName">The name of the property that
        /// changed.</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "This cannot be an event")]
        protected virtual void RaisePropertyChanged(string propertyName) 
        {
            this.VerifyPropertyName(propertyName);

            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Assigns a new value to the property. Then, raises the
        /// PropertyChanged event if needed. 
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="propertyName">The name of the property that
        /// changed.</param>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change
        /// occurred.</param>
        /// <returns>True if the PropertyChanged event has been raised,
        /// false otherwise. The event is not raised if the old
        /// value is equal to the new value.</returns>
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "This syntax is more convenient than the alternatives.")]
        protected bool Set<T>(string propertyName, ref T field, T newValue)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

            this.RaisePropertyChanging(propertyName);
            field = newValue;
            this.RaisePropertyChanged(propertyName);
            return true;
        }
    }
}
