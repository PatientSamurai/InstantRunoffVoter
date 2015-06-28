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
using InstantRunoffVoter.ViewModels;

namespace InstantRunoffVoter.Views
{
    /// <summary>
    /// Code-behind class for the main page.
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        /// <summary>
        /// The view model this page is bound to.
        /// </summary>
        private MainViewModel viewModel;

        /// <summary>
        /// The local reference of the vote button which was added to the application bar during initialization.
        /// </summary>
        private readonly ApplicationBarIconButton buttonStartVote;

        /// <summary>
        /// Constructs a new instance of the MainPage class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            this.viewModel = App.ViewModel;
            this.DataContext = this.viewModel;

            this.buttonStartVote = new ApplicationBarIconButton(new Uri("/Assets/Icons/Dark/check.png", UriKind.Relative))
            {
                Text = AppResources.AppBarButtonVoteText,
                IsEnabled = false,
            };

            this.ApplicationBar.Buttons.Add(this.buttonStartVote);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }

            this.buttonStartVote.Click += this.ButtonStartVote_Click;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            this.buttonStartVote.Click -= this.ButtonStartVote_Click;
        }

        /// <summary>
        /// Called when the vote button is clicked.
        /// </summary>
        private void ButtonStartVote_Click(object sender, EventArgs e)
        {
            string voters = string.Join("&", this.viewModel.SelectedVoters.Select(
                voter =>
                {
                    return HttpUtility.UrlEncode(voter.Text);
                }));

            string candidates = string.Join("&", this.viewModel.SelectedCandidates.Select(
                candidate =>
                {
                    return HttpUtility.UrlEncode(candidate.Text);
                }));

            NavigationService.Navigate(new Uri(string.Format(
                "/Views/VotingPage.xaml?{0}={1}&{2}={3}",
                VotingPage.VotersQueryStringKey,
                HttpUtility.UrlEncode(voters),
                VotingPage.CandidatesQueryStringKey,
                HttpUtility.UrlEncode(candidates)), UriKind.Relative));
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

        /// <summary>
        /// Called when the selection of voters has changed.
        /// </summary>
        private void VotersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.viewModel.AddVoters(e.AddedItems.Cast<ItemViewModel>());
            this.viewModel.RemoveVoters(e.RemovedItems.Cast<ItemViewModel>());

            this.EvaluateVoteButtonState();
        }

        /// <summary>
        /// Called when the selection of candidates has changed.
        /// </summary>
        private void CandidatesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.viewModel.AddCandidates(e.AddedItems.Cast<ItemViewModel>());
            this.viewModel.RemoveCandidates(e.RemovedItems.Cast<ItemViewModel>());

            this.EvaluateVoteButtonState();
        }

        /// <summary>
        /// Evaluates and sets the enabled state of the vote button.
        /// </summary>
        private void EvaluateVoteButtonState()
        {
            this.buttonStartVote.IsEnabled = this.viewModel.CanStartVote();
        }
    }
}