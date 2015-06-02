using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using InstantRunoffVoter.Resources;

namespace InstantRunoffVoter.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.Voters = new ObservableCollection<ItemViewModel>();
            this.Candidates = new ObservableCollection<ItemViewModel>();
        }

        /// <summary>
        /// A collection for ItemViewModel objects representing the known voters.
        /// </summary>
        public ObservableCollection<ItemViewModel> Voters { get; private set; }

        /// <summary>
        /// A collection for ItemViewModel objects representing the known candidates.
        /// </summary>
        public ObservableCollection<ItemViewModel> Candidates { get; private set; }

        private string _sampleProperty = "Sample Runtime Property Value";
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        /// <summary>
        /// Sample property that returns a localized string
        /// </summary>
        public string LocalizedSampleProperty
        {
            get
            {
                return AppResources.SampleProperty;
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}