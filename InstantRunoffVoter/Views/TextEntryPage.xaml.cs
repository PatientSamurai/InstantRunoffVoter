using InstantRunoffVoter.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Windows.Input;
using System.Windows.Navigation;

namespace InstantRunoffVoter.Views
{
    /// <summary>
    /// Code-behind class for the simple text entry page.
    /// </summary>
    public partial class TextEntryPage : PhoneApplicationPage
    {
        /// <summary>
        /// The key to use in the query string when launching this view to indicate where in the view model the new text field should be saved.
        /// </summary>
        public const string TextTargetQueryStringKey = "TextTarget";

        /// <summary>
        /// The key to use in the query string when launching this view to indicate the page title text that should be used.
        /// </summary>
        public const string PageTitleQueryStringKey = "PageTitle";

        /// <summary>
        /// The local reference of the save button which was added to the application bar during initialization.
        /// </summary>
        private readonly ApplicationBarIconButton buttonSave;

        /// <summary>
        /// The target string that was passed in via query string when the view was loaded.
        /// </summary>
        private string target;

        /// <summary>
        /// Initializes a new instance of the TextEntryPage class.
        /// </summary>
        public TextEntryPage()
        {
            this.InitializeComponent();

            this.buttonSave = new ApplicationBarIconButton(new Uri("/Assets/Icons/Dark/save.png", UriKind.Relative))
            {
                Text = AppResources.AppBarButtonSaveText,
                IsEnabled = false,
            };

            this.ApplicationBar.Buttons.Add(this.buttonSave);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.buttonSave.Click += this.ButtonSave_Click;

            if (!NavigationContext.QueryString.TryGetValue(TextEntryPage.TextTargetQueryStringKey, out this.target))
            {
                throw new ArgumentNullException(TextEntryPage.TextTargetQueryStringKey);
            }

            string pageTitle;
            if (!NavigationContext.QueryString.TryGetValue(TextEntryPage.PageTitleQueryStringKey, out pageTitle))
            {
                throw new ArgumentNullException(TextEntryPage.TextTargetQueryStringKey);
            }
            else
            {
                this.TextBlockPageTitle.Text = pageTitle;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            this.buttonSave.Click -= this.ButtonSave_Click;
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
        private void TextBoxEntry_KeyUp(object sender, KeyEventArgs e)
        {
            this.buttonSave.IsEnabled = !string.IsNullOrEmpty(this.TextBoxEntry.Text);
        }

        /// <summary>
        /// Called when the save button is clicked.
        /// </summary>
        private void ButtonSave_Click(object sender, EventArgs e)
        {
            App.ViewModel.AddEntry(this.target, this.TextBoxEntry.Text);
            NavigationService.GoBack();
        }
    }
}