using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Diagnostics.Debugging
{
    /// <summary>
    /// Implementations will provide developers the ability to monitor data being sent/received the program
    /// and external services (TCP/IP clients, SSH clients, etc).
    /// </summary>
    public abstract class DebugEnvironment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DebugEnvironment"/> class.
        /// </summary>
        public DebugEnvironment()
        {
            // subscribe to the collection changed event
            DebuggableObjects.CollectionChanged += this.HandleDebuggableObjects_CollectionChanged;
        }

        /// <summary>
        /// Event is raised when someone calls the <see cref="DebugEnvironment.Write"/> method. Child
        /// classes should subscribe to this event and respond accordingly.
        /// </summary>
        protected static event EventHandler<string> WriteToConsole;

        /// <summary>
        /// Gets the collection that stores references to all ISendReceiveDebuggable objects.
        /// </summary>
        public static ObservableCollection<ISendReceiveDebuggable> DebuggableObjects { get; private set; }
            = new ObservableCollection<ISendReceiveDebuggable>();

        /// <summary>
        /// Method allows for printing debug information to the debug environment. Child classes
        /// respond to this method and print to their respective environments (Crestron console,
        /// VC-4 virtual console, etc.).
        /// </summary>
        /// <param name="content">String content to print.</param>
        public static void Write(string content)
        {
            WriteToConsole?.Invoke(null, content);
        }

        /// <summary>
        /// Event handler for when a <see cref="ISendReceiveDebuggable"/> sends data.
        /// </summary>
        /// <param name="sender">The <see cref="ISendReceiveDebuggable"/> object that raised the event.</param>
        /// <param name="e">String representation of the data that was sent.</param>
        protected abstract void HandleDebugDataSent(object sender, string e);

        /// <summary>
        /// Event handler for when a <see cref="ISendReceiveDebuggable"/> receives data.
        /// </summary>
        /// <param name="sender">The <see cref="ISendReceiveDebuggable"/> object that raised the event.</param>
        /// <param name="e">String representation of the data that was received.</param>
        protected abstract void HandleDebugDataReceived(object sender, string e);

        /// <summary>
        /// Method is called whenever a ISendReceiveDebuggable is added (or removed) from the static collection
        /// </summary>
        /// <param name="sender">The collection of objects, <see cref="DebuggableObjects"/>.</param>
        /// <param name="e">EventArgs containing the new or modified debuggable objects.</param>
        protected virtual void HandleDebuggableObjects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // iterate through all the new objects
            foreach (object o in e.NewItems)
            {
                var castedObject = (ISendReceiveDebuggable)o;
                castedObject.DebugDataReceived += this.HandleDebugDataReceived;
                castedObject.DebugDataSent += this.HandleDebugDataSent;
            }
        }
    }
}
