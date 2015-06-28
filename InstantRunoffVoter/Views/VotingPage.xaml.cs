using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using InstantRunoffVoter.ViewModels;
using InstantRunoffVoter.Resources;

namespace InstantRunoffVoter.Views
{
    /// <summary>
    /// Code-behind class for the voting page.
    /// </summary>
    public partial class VotingPage : PhoneApplicationPage
    {
        /// <summary>
        /// The key to use to pass an '&' delimited list of voters in.
        /// </summary>
        public const string VotersQueryStringKey = "voters";

        /// <summary>
        /// The key to use to pass an '&' delimited list of candidates in.
        /// </summary>
        public const string CandidatesQueryStringKey = "candidates";

        /// <summary>
        /// The view model backing this page.
        /// </summary>
        private readonly VotingPageViewModel viewModel;

        /// <summary>
        /// The local reference of the skip button which was added to the application bar during initialization.
        /// </summary>
        private readonly ApplicationBarIconButton buttonSkip;

        /// <summary>
        /// The local reference of the vote button which was added to the application bar during initialization.
        /// </summary>
        private readonly ApplicationBarIconButton buttonSubmit;

        /// <summary>
        /// Initializes the VotingPage class.
        /// </summary>
        public VotingPage()
        {
            this.InitializeComponent();

            this.viewModel = new VotingPageViewModel();
            this.DataContext = this.viewModel;

            this.buttonSkip = new ApplicationBarIconButton(new Uri("/Assets/Icons/Dark/next.png", UriKind.Relative))
            {
                Text = AppResources.AppBarButtonSkipText,
                IsEnabled = false,
            };

            this.ApplicationBar.Buttons.Add(this.buttonSkip);

            this.buttonSubmit = new ApplicationBarIconButton(new Uri("/Assets/Icons/Dark/save.png", UriKind.Relative))
            {
                Text = AppResources.AppBarButtonSubmitVoteText,
                IsEnabled = true,
            };

            this.ApplicationBar.Buttons.Add(this.buttonSubmit);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.buttonSkip.Click += this.ButtonSkip_Click;
            this.buttonSubmit.Click += this.ButtonSubmit_Click;

            if (!this.viewModel.IsDataLoaded)
            {
                string votersList;
                if (!NavigationContext.QueryString.TryGetValue(VotingPage.VotersQueryStringKey, out votersList))
                {
                    throw new ArgumentNullException(TextEntryPage.TextTargetQueryStringKey);
                }

                List<string> voters = this.SplitQueryStringList(votersList);

                string candidatesList;
                if (!NavigationContext.QueryString.TryGetValue(VotingPage.CandidatesQueryStringKey, out candidatesList))
                {
                    throw new ArgumentNullException(TextEntryPage.TextTargetQueryStringKey);
                }

                List<string> candidates = this.SplitQueryStringList(candidatesList);

                this.viewModel.StartNewVote(voters, candidates);
            }

            this.EvaluateButtonEnabling();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            this.buttonSkip.Click -= this.ButtonSkip_Click;
            this.buttonSubmit.Click -= this.ButtonSubmit_Click;
        }

        /// <summary>
        /// Splits the given '&' delimited query string value, decodes the individual entries and returns them.
        /// </summary>
        /// <param name="queryStringValue">The query string value passed to the page.</param>
        /// <returns>The list of entries that were received.</returns>
        private List<string> SplitQueryStringList(string queryStringValue)
        {
            var decodedEntries = new List<string>();
            foreach (string entry in queryStringValue.Split('&'))
            {
                decodedEntries.Add(HttpUtility.UrlDecode(entry));
            }

            return decodedEntries;
        }

        /// <summary>
        /// Called when the skip button is clicked.
        /// </summary>
        private void ButtonSkip_Click(object sender, EventArgs e)
        {
            this.viewModel.SkipCommand.Execute(null);
            this.EvaluateButtonEnabling();
        }

        /// <summary>
        /// Called when the submit button is clicked.
        /// </summary>
        private void ButtonSubmit_Click(object sender, EventArgs e)
        {
            this.viewModel.SubmitVoteCommand.Execute(null);
            this.EvaluateButtonEnabling();
        }

        /// <summary>
        /// Updates the enabled states of the buttons on the page.
        /// </summary>
        private void EvaluateButtonEnabling()
        {
            this.buttonSkip.IsEnabled = this.viewModel.CanSkipVoter();
            this.buttonSubmit.IsEnabled = !this.viewModel.IsElectionDone();
        }
    }
}