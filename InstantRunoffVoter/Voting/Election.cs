using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstantRunoffVoter.Voting
{
    /// <summary>
    /// Class representing a single election process.
    /// </summary>
    public class Election
    {
        /// <summary>
        /// The collection of voters who are voting in this election.
        /// </summary>
        private readonly ReadOnlyCollection<string> voters;

        /// <summary>
        /// The collection of candidates being voted on in this election.
        /// </summary>
        private readonly ReadOnlyCollection<string> candidates;

        /// <summary>
        /// The ordered list of voters who have not voted yet.
        /// </summary>
        private readonly List<string> votersWithoutVotes;

        /// <summary>
        /// The ballot results for the voters. The key is the voter name.
        /// </summary>
        private readonly Dictionary<string, List<string>> ballots;

        /// <summary>
        /// The number of voters required to vote for a single candidate for it to win.
        /// </summary>
        private readonly int requiredMajority;

        /// <summary>
        /// Random number generator to use.
        /// </summary>
        private Random random;

        /// <summary>
        /// The winner of the election once it is known.
        /// </summary>
        private string winner;

        /// <summary>
        /// Initializes a new instance of the Election class with a given set of voters and candidates.
        /// </summary>
        /// <param name="voters">The set of voters voting in this election.</param>
        /// <param name="candidates">The set of candidates being voted on in this election.</param>
        public Election(IEnumerable<string> voters, IEnumerable<string> candidates)
        {
            List<string> votersTemp = new List<string>(voters.Distinct());
            votersTemp.Sort();
            this.voters = votersTemp.AsReadOnly();

            List<string> candidatesTemp = new List<string>(candidates.Distinct());
            candidatesTemp.Sort();
            this.candidates = candidatesTemp.AsReadOnly();

            if (this.voters.Count < 2 || this.candidates.Count < 2)
            {
                throw new ArgumentException("The voters and/or candidates list do not make a valid election.");
            }

            this.votersWithoutVotes = new List<string>(this.voters);

            this.ballots = new Dictionary<string, List<string>>();

            this.requiredMajority = (int)Math.Floor((this.voters.Count / 2.0) + 1);

            this.random = new Random();
        }

        /// <summary>
        /// Gets a sorted list of voters who have not voted in this election yet.
        /// </summary>
        public IEnumerable<string> VotersWithoutVotes
        {
            get
            {
                List<string> votersCopy = new List<string>(this.votersWithoutVotes);
                votersCopy.Sort();
                return votersCopy;
            }
        }

        /// <summary>
        /// Gets the number of voters in this election who have not voted yet.
        /// </summary>
        public int CountVotersWithoutVotes
        {
            get
            {
                return this.votersWithoutVotes.Count;
            }
        }

        /// <summary>
        /// Gets the winner of this election.
        /// </summary>
        public string Winner
        {
            get
            {
                if (this.winner == null)
                {
                    throw new InvalidOperationException("The winner of this election is not known because not all ballots have been received.");
                }

                return this.winner;
            }
        }

        /// <summary>
        /// Gets or sets the random object to be used.
        /// </summary>
        public Random Random
        {
            get
            {
                return this.random;
            }

            set
            {
                this.random = value;
            }
        }

        /// <summary>
        /// Gets the candidates that are being voted on in this election.
        /// </summary>
        public ReadOnlyCollection<string> Candidates
        {
            get
            {
                return this.candidates;
            }
        }

        /// <summary>
        /// Returns the next voter that still needs to vote.
        /// </summary>
        /// <returns>The next voter that needs to vote.</returns>
        public string GetNextVoter()
        {
            if (this.votersWithoutVotes.Count == 0)
            {
                throw new IndexOutOfRangeException("There are no more voters left who haven't voted.");
            }

            return this.votersWithoutVotes[0];
        }

        /// <summary>
        /// Skips the voter at the front of the list and places them at the back of the list.
        /// </summary>
        /// <returns>The voter at the front of the list after skiping the first voter.</returns>
        public string SkipNextVoter()
        {
            if (this.votersWithoutVotes.Count < 2)
            {
                throw new IndexOutOfRangeException(string.Format("There aren't enough voters left who haven't voted: {0}.", this.votersWithoutVotes.Count));
            }

            this.votersWithoutVotes.Add(this.votersWithoutVotes[0]);
            this.votersWithoutVotes.RemoveAt(0);

            return this.votersWithoutVotes[0];
        }

        /// <summary>
        /// Stores the preferences of the current voter.
        /// </summary>
        /// <param name="voterPreferences">The ordered preference list for the voter.</param>
        public void AddBallot(IEnumerable<string> voterPreferences)
        {
            if (this.votersWithoutVotes.Count == 0)
            {
                throw new IndexOutOfRangeException("There are no voters for which we don't have a ballot for.");
            }

            if (voterPreferences.Count() != this.candidates.Count)
            {
                throw new ArgumentException("This set of preferences does not match.");
            }

            if (!voterPreferences.OrderBy(preference => preference).SequenceEqual(this.candidates))
            {
                throw new ArgumentException("This set of preferences does not match.");
            }

            // Make sure to perform a copy to avoid weirdness
            this.ballots[this.GetNextVoter()] = new List<string>(voterPreferences);
            this.votersWithoutVotes.RemoveAt(0);

            if (this.votersWithoutVotes.Count == 0)
            {
                this.winner = this.ComputerWinner();
            }
        }

        private string ComputerWinner()
        {
            Debug.Assert(this.ballots.Count == this.voters.Count);

            Dictionary<string, List<string>> ballots = this.CopyBallots(this.ballots);
            Dictionary<string, List<int>> preferences = this.GeneratePreferenceDictionary(ballots);

            string winner;
            while ((winner = this.FindWinner(preferences)) == null)
            {
                string loser = this.FindLoser(preferences);
                ballots = this.EliminateLoser(ballots, loser);
                preferences = this.GeneratePreferenceDictionary(ballots);
            }

            return winner;
        }

        /// <summary>
        /// Performs a deep copy on a set of ballots and returns the copy.
        /// </summary>
        /// <param name="ballotsToCopy">The ballots to copy.</param>
        /// <returns>A deep copy.</returns>
        Dictionary<string, List<string>> CopyBallots(Dictionary<string, List<string>> ballotsToCopy)
        {
            var copy = new Dictionary<string, List<string>>();

            foreach (KeyValuePair<string, List<string>> ballot in ballotsToCopy)
            {
                copy[ballot.Key] = new List<string>(ballot.Value);
            }

            return copy;
        }

        /// <summary>
        /// Takes all the voter's ballots and returns a dictionary containing each candidate's preference values.
        /// </summary>
        /// <param name="ballots">The voter's ballots to convert.</param>
        /// <returns>The candidate preference values.</returns>
        private Dictionary<string, List<int>> GeneratePreferenceDictionary(Dictionary<string, List<string>> ballots)
        {
            var preferences = new Dictionary<string, List<int>>();
            foreach (IEnumerable<string> voterBallot in ballots.Values)
            {
                int rank = 1;
                foreach (string candidate in voterBallot)
                {
                    if (!preferences.ContainsKey(candidate))
                    {
                        preferences[candidate] = new List<int>();
                    }

                    preferences[candidate].Add(rank++);
                }
            }

            return preferences;
        }

        /// <summary>
        /// Determines if any of the candidates have won based on the given set of preference values.
        /// </summary>
        /// <param name="preferences">The preferences for the candidates left.</param>
        /// <returns>The name of the winning candidate if it won. null if no candidates won.</returns>
        private string FindWinner(Dictionary<string, List<int>> preferences)
        {
            foreach (KeyValuePair<string, List<int>> candidatePreferences in preferences)
            {
                if (candidatePreferences.Value.Count(preference => preference == 1) >= this.requiredMajority)
                {
                    return candidatePreferences.Key;
                }
            }

            return null;
        }

        /// <summary>
        /// Determines the biggest loser in a given preferences set and returns its name.
        /// </summary>
        /// <param name="preferences">The preferences set to analyze.</param>
        /// <returns>The name of the candidate that should be removed.</returns>
        private string FindLoser(Dictionary<string, List<int>> preferences)
        {
            // First figure out the current lowest value for number of #1 votes any candidate has
            int lowestTopVotes = this.voters.Count;
            foreach (List<int> candidatePreferences in preferences.Values)
            {
                int topVotes = candidatePreferences.Count(preference => preference == 1);
                if (topVotes < lowestTopVotes)
                {
                    lowestTopVotes = topVotes;
                }
            }

            // Now pull out all candidates that have the lowest preference and remember the sum of their preferences as well
            Dictionary<string, int> lowestPreferences = new Dictionary<string,int>();
            foreach (KeyValuePair<string, List<int>> candidatePreferences in preferences)
            {
                if (candidatePreferences.Value.Count(preference => preference == 1) == lowestTopVotes)
                {
                    lowestPreferences[candidatePreferences.Key] = candidatePreferences.Value.Sum();
                }
            }

            // Figure out which of the losers is the biggest loser
            int biggestLoss = lowestPreferences.Values.Max();
            List<string> biggestLosers = new List<string>(lowestPreferences.Where(candidate => candidate.Value == biggestLoss).Select(candidate => candidate.Key));
            string loser;
            if (biggestLosers.Count == 1)
            {
                loser = biggestLosers[0];
            }
            else
            {
                // Looks like something must be eliminated by pure random chance...
                // Sort this list for testing determinism.
                biggestLosers.Sort();
                loser = biggestLosers[this.random.Next(biggestLosers.Count)];
            }

            return loser;
        }

        /// <summary>
        /// Takes a set of ballots, copies them and then returns a new set of ballots with the indicates loser candidate removed.
        /// </summary>
        /// <param name="ballots">The ballots to start with.</param>
        /// <param name="loser">The loser candidate to remove.</param>
        /// <returns>The new ballots to use.</returns>
        private Dictionary<string, List<string>> EliminateLoser(Dictionary<string, List<string>> ballots, string loser)
        {
            var newBallots = this.CopyBallots(ballots);

            foreach (List<string> newBallot in newBallots.Values)
            {
                newBallot.Remove(newBallot.First(candidate => candidate == loser));
            }

            return newBallots;
        }
    }
}
