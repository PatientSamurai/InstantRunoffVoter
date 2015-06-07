using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InstantRunoffVoter.Views
{
    /// <summary>
    /// Class to hold all custom view element properties needed for XAML elements.
    /// </summary>
    public static class ElementProperties
    {
        /// <summary>
        /// Property defining a settable XAML property for ViewModelTarget.
        /// </summary>
        public static readonly DependencyProperty ViewModelTargetProperty = DependencyProperty.RegisterAttached(
            "ViewModelTarget",
            typeof(string),
            typeof(ElementProperties),
            new PropertyMetadata(null));

        /// <summary>
        /// Method used to retrieve the ViewModelTarget property from a given UIElement.
        /// </summary>
        /// <param name="element">The UIElement to examine.</param>
        /// <returns>The value of the custom property set on that element.</returns>
        public static string GetViewModelTarget(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (string)element.GetValue(ViewModelTargetProperty);
        }

        /// <summary>
        /// Sets the custom ViewModelTarget property on a UIElement.
        /// </summary>
        /// <param name="element">The element to modify.</param>
        /// <param name="value">The value to set.</param>
        public static void SetViewModelTarget(UIElement element, string value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(ViewModelTargetProperty, value);
        }

        /// <summary>
        /// Property defining a settable XAML property for TextCollectionString.
        /// </summary>
        public static readonly DependencyProperty TextCollectionStringProperty = DependencyProperty.RegisterAttached(
            "TextCollectionString",
            typeof(string),
            typeof(ElementProperties),
            new PropertyMetadata(null));

        /// <summary>
        /// Method used to retrieve the TextCollectionString property from a given UIElement.
        /// </summary>
        /// <param name="element">The UIElement to examine.</param>
        /// <returns>The value of the custom property set on that element.</returns>
        public static string GetTextCollectionString(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (string)element.GetValue(TextCollectionStringProperty);
        }

        /// <summary>
        /// Sets the custom TextCollectionString property on a UIElement.
        /// </summary>
        /// <param name="element">The element to modify.</param>
        /// <param name="value">The value to set.</param>
        public static void SetTextCollectionString(UIElement element, string value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(TextCollectionStringProperty, value);
        }
    }
}
