using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using InstantRunoffVoter.Resources;
using InstantRunoffVoter.Views;

namespace InstantRunoffVoter.Views
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        /// <summary>
        /// Called when the add button is clicked on either the voters or candidates pivot.
        /// </summary>
        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            string target = ElementProperties.GetViewModelTarget((UIElement)this.MainPagePivot.SelectedItem);
            string title = ElementProperties.GetTextCollectionString((UIElement)this.MainPagePivot.SelectedItem);

            NavigationService.Navigate(new Uri(string.Format(
                "/Views/TextEntryPage.xaml?{0}={1}&{2}={3}",
                TextEntryPage.TextTargetQueryStringKey,
                target,
                TextEntryPage.PageTitleQueryStringKey,
                HttpUtility.UrlEncode(title)), UriKind.Relative));
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}