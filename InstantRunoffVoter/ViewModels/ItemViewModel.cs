using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace InstantRunoffVoter.ViewModels
{
    /// <summary>
    /// Class representing a simple value that can be displayed in UI.
    /// </summary>
    public class ItemViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The text representing this item.
        /// </summary>
        private string text;

        /// <summary>
        /// Gets or sets the text for this item.
        /// </summary>
        /// <returns></returns>
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                if (value != this.text)
                {
                    this.text = value;
                    NotifyPropertyChanged("Text");
                }
            }
        }

        /// <summary>
        /// Event that is raised whenever a property on this object has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed event for the given property.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed.</param>
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}