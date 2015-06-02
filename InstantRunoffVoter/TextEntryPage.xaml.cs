using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace InstantRunoffVoter
{
    /// <summary>
    /// Code-behind class for the simple text entry page.
    /// </summary>
    public partial class TextEntryPage : PhoneApplicationPage
    {
        /// <summary>
        /// Bool indicating that the application bar has been initialized.
        /// </summary>
        private bool appBarLoaded;

        /// <summary>
        /// The local reference the the save button which was added to the application bar during initialization.
        /// </summary>
        private ApplicationBarIconButton buttonSave;

        public TextEntryPage()
        {
            InitializeComponent();
            InitAppBar();
        }

        private void InitAppBar()
        {
            if (appBarLoaded)
            {
                return;
            }

            appBarLoaded = true;

            buttonSave = new ApplicationBarIconButton(new Uri("/Assets/Icons/Dark/save.png", UriKind.Relative))
            {
                Text = "save",
                IsEnabled = false,
            };

            ApplicationBar.Buttons.Add(buttonSave);
        }

        /// <summary>
        /// Called when the cancel button is clicked.
        /// </summary>
        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        /// <summary>
        /// Called when text entry box changes.
        /// </summary>
        private void TextBoxEntry_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            this.buttonSave.IsEnabled = !string.IsNullOrEmpty(this.TextBoxEntry.Text);
        }
    }
}