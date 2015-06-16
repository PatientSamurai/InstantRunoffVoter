using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InstantRunoffVoter.Voting;
using System.Collections.Generic;
using Moq;

namespace UnitTests.Voting
{
    /// <summary>
    /// Test class for testing Election class.
    /// </summary>
    [TestClass]
    public class ElectionTests
    {
        /// <summary>
        /// The mock Random object that is given to the Election instance.
        /// </summary>
        private Mock<Random> mockRandom;
        
        /// <summary>
        /// Initializes the class before every test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            this.mockRandom = new Mock<Random>(MockBehavior.Strict);
        }

        /// <summary>
        /// Cleans up the class after every test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            this.mockRandom.VerifyAll();
        }

        /// <summary>
        /// Tests an election with 2 voters and 2 candidates where both voters vote the same and candidate 1 wins.
        /// </summary>
        [TestMethod]
        public void TestDeterministic2By2Where1Wins()
        {
            Election election = this.GenerateElection(2, 2);

            election.AddBallot(this.FillCandidateNames(1, 2));
            election.AddBallot(this.FillCandidateNames(1, 2));

            Assert.AreEqual("Candidate 1", election.Winner);
        }

        /// <summary>
        /// Tests an election with 2 voters and 2 candidates where both voters vote the same and candidate 2 wins.
        /// </summary>
        [TestMethod]
        public void TestDeterministic2By2Where2Wins()
        {
            Election election = this.GenerateElection(2, 2);

            election.AddBallot(this.FillCandidateNames(2, 1));
            election.AddBallot(this.FillCandidateNames(2, 1));

            Assert.AreEqual("Candidate 2", election.Winner);
        }

        /// <summary>
        /// Tests an election with 2 voters and 2 candidates where both voters vote differently and candidate 1 wins.
        /// </summary>
        [TestMethod]
        public void TestTied2By2Where1Wins()
        {
            Election election = this.GenerateElection(2, 2);

            this.mockRandom.Setup(random => random.Next(2)).Returns(1);

            election.AddBallot(this.FillCandidateNames(1, 2));
            election.AddBallot(this.FillCandidateNames(2, 1));

            Assert.AreEqual("Candidate 1", election.Winner);
        }

        /// <summary>
        /// Tests an election with 2 voters and 2 candidates where both voters vote differently and candidate 2 wins.
        /// </summary>
        [TestMethod]
        public void TestTied2By2Where2Wins()
        {
            Election election = this.GenerateElection(2, 2);

            this.mockRandom.Setup(random => random.Next(2)).Returns(0);

            election.AddBallot(this.FillCandidateNames(1, 2));
            election.AddBallot(this.FillCandidateNames(2, 1));

            Assert.AreEqual("Candidate 2", election.Winner);
        }

        /// <summary>
        /// Tests an election with 2 voters and 3 candidates where all voters vote the same and candidate 1 wins.
        /// </summary>
        [TestMethod]
        public void TestDeterministic2By3Where1Wins()
        {
            Election election = this.GenerateElection(2, 3);

            election.AddBallot(this.FillCandidateNames(1, 2, 3));
            election.AddBallot(this.FillCandidateNames(1, 2, 3));

            Assert.AreEqual("Candidate 1", election.Winner);
        }

        /// <summary>
        /// Tests an election with 2 voters and 3 candidates where 1 loses, 2 and 3 tie and candidate 2 wins.
        /// </summary>
        [TestMethod]
        public void TestTied2By3Where2Wins()
        {
            Election election = this.GenerateElection(2, 3);

            this.mockRandom.Setup(random => random.Next(2)).Returns(1);

            election.AddBallot(this.FillCandidateNames(3, 2, 1));
            election.AddBallot(this.FillCandidateNames(2, 3, 1));

            Assert.AreEqual("Candidate 2", election.Winner);
        }

        /// <summary>
        /// Tests an election with 2 voters and 3 candidates where 1 loses, 2 and 3 tie and candidate 3 wins.
        /// </summary>
        [TestMethod]
        public void TestTied2By3Where3Wins()
        {
            Election election = this.GenerateElection(2, 3);

            this.mockRandom.Setup(random => random.Next(2)).Returns(0);

            election.AddBallot(this.FillCandidateNames(3, 2, 1));
            election.AddBallot(this.FillCandidateNames(2, 3, 1));

            Assert.AreEqual("Candidate 3", election.Winner);
        }

        /// <summary>
        /// Tests a complicated vote where candidate 1 starts off with less #1 votes than candidates 3 and 4 but still
        /// ends up winning because when candidate 2 is cut, candidate 1 gains a #1 vote.
        /// 
        /// Preferences step[0]:
        ///      C1 | C2 | C3 | C4
        ///     -------------------
        ///      1  | 2  | 3  | 4
        ///      2  | 1  | 3  | 4
        ///      2  | 4  | 1  | 3
        ///      3  | 4  | 1  | 2
        ///      2  | 4  | 3  | 1
        ///      4  | 2  | 3  | 1
        ///     -------------------
        /// #1s: 1  | 1  | 2  | 2
        /// Sum: 14 | 17 | 14 | 15
        /// 
        /// Preferences step[1]:
        ///      C1 | C3 | C4
        ///     --------------
        ///      1  | 2  | 3
        ///      1  | 2  | 3
        ///      2  | 1  | 3
        ///      3  | 1  | 2
        ///      2  | 3  | 1
        ///      3  | 2  | 1
        ///     --------------
        /// #1s: 2  | 2  | 2
        /// Sum: 12 | 11 | 13
        /// 
        /// Preferences step[2] (Requires random):
        ///      C1 | C3
        ///     ---------
        ///      1  | 2
        ///      1  | 2
        ///      1  | 2
        ///      2  | 1
        ///      2  | 1
        ///      2  | 1
        ///     ---------
        /// #1s: 3  | 3
        /// Sum: 9  | 9 
        /// </summary>
        [TestMethod]
        public void TestComeFromBehind6By4Where1Wins()
        {
            Election election = this.GenerateElection(6, 4);

            this.mockRandom.Setup(random => random.Next(2)).Returns(1);

            election.AddBallot(this.FillCandidateNames(1, 2, 3, 4));
            election.AddBallot(this.FillCandidateNames(2, 1, 3, 4));
            election.AddBallot(this.FillCandidateNames(3, 1, 4, 2));
            election.AddBallot(this.FillCandidateNames(3, 4, 1, 2));
            election.AddBallot(this.FillCandidateNames(4, 1, 3, 2));
            election.AddBallot(this.FillCandidateNames(4, 2, 3, 1));

            Assert.AreEqual("Candidate 1", election.Winner);
        }

        /// <summary>
        /// Tests an election with 3 voters and 3 candidates where all candidates are tied to begin with and
        /// after candidate 3 is chosen to lose at random, candidate 1 wins.
        /// 
        /// Preferences step[0] (Requires random):
        ///      C1 | C2 | C3
        ///     --------------
        ///      1  | 2  | 3
        ///      2  | 3  | 1
        ///      3  | 1  | 2
        ///     --------------
        /// #1s: 1  | 1  | 1
        /// Sum: 6  | 6  | 6
        /// 
        /// Preferences step[0]:
        ///      C1 | C2
        ///     ---------
        ///      1  | 2
        ///      1  | 2
        ///      2  | 1
        ///     ---------
        /// #1s: 2  | 1
        /// Sum: 4  | 5
        /// </summary>
        [TestMethod]
        public void TestTied3By3()
        {
            Election election = this.GenerateElection(3, 3);

            this.mockRandom.Setup(random => random.Next(3)).Returns(2);

            election.AddBallot(this.FillCandidateNames(1, 2, 3));
            election.AddBallot(this.FillCandidateNames(3, 1, 2));
            election.AddBallot(this.FillCandidateNames(2, 3, 1));

            Assert.AreEqual("Candidate 1", election.Winner);
        }

        /// <summary>
        /// Generates a new Election class with the given number of voters and candidates.
        /// </summary>
        /// <param name="voters">The desired number of voters in this test.</param>
        /// <param name="candidates">The desired number of candidates in this test.</param>
        /// <returns>The election class to test.</returns>
        private Election GenerateElection(int voters, int candidates)
        {
            return new Election(
                this.GenerateVoters(voters),
                this.GenerateCandidates(candidates))
                {
                    Random = this.mockRandom.Object,
                };
        }

        /// <summary>
        /// Generates a list of voters.
        /// </summary>
        /// <param name="count">The number of voters desired.</param>
        /// <returns>A list of voter names.</returns>
        private IEnumerable<string> GenerateVoters(int count)
        {
            var voters = new List<string>();
            for (int i = 0; i < count; i++)
            {
                voters.Add("Voter " + (i + 1));
            }

            return voters;
        }

        /// <summary>
        /// Generates a list of candidates.
        /// </summary>
        /// <param name="count">The number of candidates desired.</param>
        /// <returns>A list of candidates names.</returns>
        private IEnumerable<string> GenerateCandidates(int count)
        {
            var candidateNumbers = new List<int>();
            for (int i = 0; i < count; i++)
            {
                candidateNumbers.Add(i + 1);
            }

            return this.FillCandidateNames(candidateNumbers.ToArray());
        }

        /// <summary>
        /// Generates a list of candidates based off a list of candidate numbers.
        /// </summary>
        /// <param name="candidateNumbers">The candidate numbers to use.</param>
        /// <returns>A list of candidate names.</returns>
        private IEnumerable<string> FillCandidateNames(params int[] candidateNumbers)
        {
            var candidateList = new List<string>();
            foreach (int candidate in candidateNumbers)
            {
                candidateList.Add("Candidate " + candidate);
            }

            return candidateList;
        }
    }
}
