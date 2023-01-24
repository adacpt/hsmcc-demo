using System.Collections.Generic;

namespace Diagnostics.Dashboarding
{
    public interface IDashboardable
    {
        /// <summary>
        /// Gets or sets the name of the object to be displayed in the dashboard.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets the collections of <see cref="DashboardValue"/> properties associated with this object.
        /// </summary>
        List<DashboardValue> Values { get; }


        void Register();
    }
}
