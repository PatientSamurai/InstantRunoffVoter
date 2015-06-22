using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using InstantRunoffVoter.Resources;
using System.Collections.Generic;

namespace InstantRunoffVoter.ViewModels
{
    /// <summary>
    /// Class representing the main app view model.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Constant value indicating the the voters collection target.
        /// </summary>
        public const string VotersTargetConstant = "voters";

        /// <summary>
        /// Constant value indicating the the candidates collection target.
        /// </summary>
        public const string CandidatesTargetConstant = "candidates";

        /// <summary>
        /// Property for exposing VotersTargetConstant.
        /// </summary>
        public static string VotersTarget { get { return MainViewModel.VotersTargetConstant; } }

        /// <summary>
        /// Property for exposing CandidatesTargetConstant.
        /// </summary>
        public static string CandidatesTarget { get { return MainViewModel.CandidatesTargetConstant; } }

        /// <summary>
        /// A collection for ItemViewModel objects representing the known voters.
        /// </summary>
        public ObservableCollection<ItemViewModel> Voters { get; private set; }

        /// <summary>
        /// A collection for ItemViewModel objects representing the selected voters.
        /// </summary>
        public ObservableCollection<ItemViewModel> SelectedVoters { get; private set; }

        /// <summary>
        /// A collection for ItemViewModel objects representing the known candidates.
        /// </summary>
        public ObservableCollection<ItemViewModel> Candidates { get; private set; }

        /// <summary>
        /// A collection for ItemViewModel objects representing the selected candidates.
        /// </summary>
        public ObservableCollection<ItemViewModel> SelectedCandidates { get; private set; }

        /// <summary>
        /// Returns a bool indicating if data has been loaded for this view model already.
        /// </summary>
        public bool IsDataLoaded { get; private set; }

        /// <summary>
        /// Event that is raised whenever a property on this object has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            this.Voters = new ObservableCollection<ItemViewModel>();
            this.Candidates = new ObservableCollection<ItemViewModel>();
            this.SelectedVoters = new ObservableCollection<ItemViewModel>();
            this.SelectedCandidates = new ObservableCollection<ItemViewModel>();
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            // Sample data; replace with real data
            this.Voters.Add(new ItemViewModel() { LineOne = "Matt V" });
            this.Voters.Add(new ItemViewModel() { LineOne = "Mallory" });
            this.Voters.Add(new ItemViewModel() { LineOne = "Ryan" });
            this.Voters.Add(new ItemViewModel() { LineOne = "Sarah T" });
            this.Voters.Add(new ItemViewModel() { LineOne = "Andrew" });
            this.Voters.Add(new ItemViewModel() { LineOne = "Mary" });
            this.Voters.Add(new ItemViewModel() { LineOne = "Matt L" });
            this.Voters.Add(new ItemViewModel() { LineOne = "Sarah V" });

            this.Candidates.Add(new ItemViewModel() { LineOne = "Board games" });
            this.Candidates.Add(new ItemViewModel() { LineOne = "Minecraft" });
            this.Candidates.Add(new ItemViewModel() { LineOne = "Starcraft 2" });
            this.Candidates.Add(new ItemViewModel() { LineOne = "Go Karts" });
            this.Candidates.Add(new ItemViewModel() { LineOne = "Super Smash Brothers" });
            this.Candidates.Add(new ItemViewModel() { LineOne = "Mario Kart" });
            this.Candidates.Add(new ItemViewModel() { LineOne = "Spy Party" });

            this.IsDataLoaded = true;
        }

        /// <summary>
        /// Adds the given list of voters to the selected voters list.
        /// </summary>
        /// <param name="newVoters">The voters to add.</param>
        public void AddVoters(IEnumerable<ItemViewModel> newVoters)
        {
            foreach (ItemViewModel voter in newVoters)
            {
                this.SelectedVoters.Add(voter);
            }
        }

        /// <summary>
        /// Removes the given list of voters from the selected voters list.
        /// </summary>
        /// <param name="removedVoters">The voters to remove.</param>
        public void RemoveVoters(IEnumerable<ItemViewModel> removedVoters)
        {
            foreach (ItemViewModel voter in removedVoters)
            {
                this.SelectedVoters.Remove(voter);
            }
        }

        /// <summary>
        /// Adds the given list of candidates to the selected candidates list.
        /// </summary>
        /// <param name="newCandidates">The candidates to add.</param>
        public void AddCandidates(IEnumerable<ItemViewModel> newCandidates)
        {
            foreach (ItemViewModel candidate in newCandidates)
            {
                this.SelectedCandidates.Add(candidate);
            }
        }

        /// <summary>
        /// Removes the given list of candidates from the selected candidates list.
        /// </summary>
        /// <param name="removedCandidates">The candidates to remove.</param>
        public void RemoveCandidates(IEnumerable<ItemViewModel> removedCandidates)
        {
            foreach (ItemViewModel candidate in removedCandidates)
            {
                this.SelectedCandidates.Remove(candidate);
            }
        }

        /// <summary>
        /// Determines if the current view model could support a vote.
        /// </summary>
        /// <returns>Whether a valid vote could be started.</returns>
        public bool CanStartVote()
        {
            if (this.SelectedVoters.Count >= 2 && this.SelectedCandidates.Count >= 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Adds a newly entered item view model.
        /// </summary>
        /// <param name="target">The target to add the new data to.</param>
        /// <param name="value">The value of the new item.</param>
        public void AddEntry(string target, string value)
        {
            var newItem = new ItemViewModel() { LineOne = value };
            switch (target)
            {
                case MainViewModel.VotersTargetConstant:
                    this.Voters.Add(newItem);
                    break;

                case MainViewModel.CandidatesTargetConstant:
                    this.Candidates.Add(newItem);
                    break;

                default:
                    throw new ArgumentException("target");
            }
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