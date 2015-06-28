using GalaSoft.MvvmLight.Command;
using InstantRunoffVoter.Voting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InstantRunoffVoter.ViewModels
{
    /// <summary>
    /// View model to back the voting page.
    /// </summary>
    public class VotingPageViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Event that is raised whenever a property on this object has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The current running election.
        /// </summary>
        private Election election;

        /// <summary>
        /// The command containing the action for up-voting an item.
        /// </summary>
        private RelayCommand<ItemViewModel> upCommand;

        /// <summary>
        /// The command containing the action for down-voting an item.
        /// </summary>
        private RelayCommand<ItemViewModel> downCommand;

        /// <summary>
        /// The command containing the action for skipping the current voter.
        /// </summary>
        private RelayCommand skipCommand;

        /// <summary>
        /// The command containing the action for submiting the current voter's ballot.
        /// </summary>
        private RelayCommand submitVoteCommand;

        /// <summary>
        /// The title of the page.
        /// </summary>
        private string pageTitle;

        /// <summary>
        /// Gets or sets the title of the page.
        /// </summary>
        public string PageTitle
        {
            get
            {
                return this.pageTitle;
            }

            set
            {
                if (this.pageTitle != value)
                {
                    this.pageTitle = value;
                    this.NotifyPropertyChanged("PageTitle");
                }
            }
        }

        /// <summary>
        /// A collection for ItemViewModel objects representing the candidates being voted on.
        /// </summary>
        public ObservableCollection<ItemViewModel> Candidates { get; private set; }

        /// <summary>
        /// Gets the command containing the action for up-voting an item.
        /// </summary>
        public RelayCommand<ItemViewModel> UpCommand
        {
            get
            {
                return this.upCommand ??
                    (this.upCommand = new RelayCommand<ItemViewModel>(
                        item =>
                        {
                            int index = this.Candidates.IndexOf(item);

                            if (index > 0)
                            {
                                this.Candidates.RemoveAt(index);
                                this.Candidates.Insert(index - 1, item);
                            }

                            this.UpCommand.RaiseCanExecuteChanged();
                            this.DownCommand.RaiseCanExecuteChanged();
                        },
                        item =>
                        {
                            int index = this.Candidates.IndexOf(item);

                            return (index > 0);
                        }));
            }
        }

        /// <summary>
        /// Gets the command containing the action for down-voting an item.
        /// </summary>
        public RelayCommand<ItemViewModel> DownCommand
        {
            get
            {
                return this.downCommand ??
                    (this.downCommand = new RelayCommand<ItemViewModel>(
                        item =>
                        {
                            int index = this.Candidates.IndexOf(item);

                            if (index + 1 < this.Candidates.Count)
                            {
                                this.Candidates.RemoveAt(index);
                                this.Candidates.Insert(index + 1, item);
                            }

                            this.UpCommand.RaiseCanExecuteChanged();
                            this.DownCommand.RaiseCanExecuteChanged();
                        },
                        item =>
                        {
                            int index = this.Candidates.IndexOf(item);

                            return (index + 1 < this.Candidates.Count);
                        }));
            }
        }

        /// <summary>
        /// Gets the command containing the action for skipping the current voter.
        /// </summary>
        public RelayCommand SkipCommand
        {
            get
            {
                return this.skipCommand ??
                    (this.skipCommand = new RelayCommand(
                        () =>
                        {
                            this.election.SkipNextVoter();
                            this.UpdatePublicProperties();
                        }));
            }
        }

        /// <summary>
        /// Gets the command containing the action for submiting the current voter's ballot.
        /// </summary>
        public RelayCommand SubmitVoteCommand
        {
            get
            {
                return this.submitVoteCommand ??
                    (this.submitVoteCommand = new RelayCommand(
                        () =>
                        {
                            IEnumerable<string> ballot = this.Candidates.Select(item => { return item.Text; });
                            
                            this.election.AddBallot(ballot);
                            this.UpdatePublicProperties();
                        }));
            }
        }

        /// <summary>
        /// Returns a bool indicating if data has been loaded for this view model already.
        /// </summary>
        public bool IsDataLoaded { get; private set; }

        /// <summary>
        /// Initializes a new instance of the VotingPageViewModel class.
        /// </summary>
        public VotingPageViewModel()
        {
            this.Candidates = new ObservableCollection<ItemViewModel>();
        }

        /// <summary>
        /// Starts a new election with the given voters and candidates.
        /// </summary>
        /// <param name="voters">The voters that are going to be voting.</param>
        /// <param name="candidates">The candidates that are going to be voted on.</param>
        public void StartNewVote(IEnumerable<string> voters, IEnumerable<string> candidates)
        {
            this.election = new Election(voters, candidates);
            this.UpdatePublicProperties();

            this.IsDataLoaded = true;
        }

        /// <summary>
        /// Updates all the public properties based off the current state of the election object.
        /// </summary>
        private void UpdatePublicProperties()
        {
            this.Candidates.Clear();

            // Set up all the bound properties
            if (this.election.IsElectionDone)
            {
                this.PageTitle = this.election.Winner;
            }
            else
            {
                this.PageTitle = this.election.GetNextVoter();
                foreach (string candidate in this.election.Candidates)
                {
                    this.Candidates.Add(new ItemViewModel() { Text = candidate });
                }
            }
        }

        /// <summary>
        /// Returns whether the current voter can be skipped.
        /// </summary>
        /// <returns>If the current voter can be skipped.</returns>
        public bool CanSkipVoter()
        {
            return (!this.election.IsElectionDone && this.election.CountVotersWithoutVotes > 1);
        }

        /// <summary>
        /// Returns if this election is over.
        /// </summary>
        /// <returns>If this election is over.</returns>
        public bool IsElectionDone()
        {
            return this.election.IsElectionDone;
        }

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
