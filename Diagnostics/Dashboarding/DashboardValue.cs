using System;
using System.Collections.Generic;

namespace Diagnostics.Dashboarding
{
    /// <summary>
    /// Name and corresponding value of a property of a dashboardable object.
    /// </summary>
    public class DashboardValue
    {
        private object _value;

        /// <summary>
        /// Raised when the <see cref="Value"/> changes. Subscribers should post
        /// the new value to the dashboard application.
        /// </summary>
        public event EventHandler<object> ValueChanged;

        /// <summary>
        /// Gets the data type of the property's value.
        /// Note - this may not be needed, not sure yet.
        /// </summary>
        public DashboardValueType ValueType { get; private set; }

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the collection of sub-properties of this object.
        /// </summary>
        public List<DashboardValue> Values { get; private set; } = new List<DashboardValue>();

        /// <summary>
        /// Gets or sets the value of the property.
        /// </summary>
        public object Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (_value != value)
                {
                    _value = value;
                    ValueChanged?.Invoke(this, value);
                }
            }
        }
    }
}
