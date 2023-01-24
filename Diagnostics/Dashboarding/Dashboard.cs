using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Diagnostics.Dashboarding
{
    public abstract class Dashboard
    {
        public Dashboard()
        {
            Objects.CollectionChanged += Objects_CollectionChanged;
        }

        /// <summary>
        /// Implementations should iterate through each <see cref="DashboardValue"/> in <see cref="IDashboardable.Values"/>
        /// and subscribe to their <see cref="DashboardValue.ValueChanged"/> events.
        /// </summary>
        /// <param name="sender">This class' Objects collection.</param>
        /// <param name="e">EventArgs containing the new <see cref="IDashboardable"/> objects.</param>
        protected abstract void Objects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e);

        public static ObservableCollection<IDashboardable> Objects { get; private set; } = new ObservableCollection<IDashboardable>();
    }
}
