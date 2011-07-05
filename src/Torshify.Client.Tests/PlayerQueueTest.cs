using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using Moq;
using NUnit.Framework;
using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Infrastructure.Models;

namespace Torshify.Client.Tests
{
    [TestFixture]
    public class PlayerQueueTest
    {
        private PlayerQueue _queue;

        [SetUp]
        public void Setup()
        {
            _queue = new PlayerQueue(Dispatcher.CurrentDispatcher);
        }

        [Test]
        public void Current_NoSongsAdded_IsNull()
        {
            Assert.IsNull(_queue.Current);
        }

        [Test]
        public void Set_OneTrack_CanGoNext()
        {
            var track = GetTrack();
            _queue.Set(new[] { track });
            Assert.IsTrue(_queue.CanGoNext);
        }

        [Test]
        public void Set_OneTrack_CannotGoPrevious()
        {
            var track = GetTrack();
            _queue.Set(new[] { track });
            Assert.IsFalse(_queue.CanGoPrevious);
        }

        [Test]
        public void Set_PlayedAll_PlaylistEmpty()
        {
            var tracks = GetTracks(5);
            _queue.Set(tracks);
            Assert.AreEqual(tracks.Count(), _queue.Left.Count());

            for (int i = 0; i < tracks.Count(); i++)
            {
                _queue.Next();
                Assert.AreEqual(tracks.Count() - (i + 1), _queue.Left.Count());
            }

            Assert.IsFalse(_queue.CanGoPrevious);
        }

        [Test]
        public void Set_PlayedAllButOne_CanGoPreviousToStart()
        {
            var tracks = GetTracks(5);
            _queue.Set(tracks);
            Assert.AreEqual(tracks.Count(), _queue.Left.Count());

            for (int i = 0; i < tracks.Count() - 1; i++)
            {
                _queue.Next();
                Assert.AreEqual(tracks.Count() - (i + 1), _queue.Left.Count());
            }

            while(_queue.Left.Count() != tracks.Count())
            {
                bool result = _queue.Previous();
                Console.WriteLine(_queue.Left.Count());
                Assert.IsTrue(result);
            }
           
            Assert.IsFalse(_queue.CanGoPrevious);
        }

        [Test]
        public void Set_PlayedAllButOne_CanGoPrevious()
        {
            var tracks = GetTracks(5);
            _queue.Set(tracks);
            Assert.AreEqual(tracks.Count(), _queue.Left.Count());

            for (int i = 0; i < tracks.Count() - 1; i++)
            {
                _queue.Next();
                Assert.AreEqual(tracks.Count() - (i + 1), _queue.Left.Count());
            }

            Assert.IsTrue(_queue.CanGoPrevious);
        }

        [Test]
        public void Enqueue_OneTrack_CanGoNext()
        {
            var track = GetTrack();
            _queue.Enqueue(track);
            Assert.IsTrue(_queue.CanGoNext);
        }

        [Test]
        public void Enqueue_OneTrack_CannotGoPrevious()
        {
            var track = GetTrack();
            _queue.Enqueue(track);
            Assert.IsFalse(_queue.CanGoPrevious);
        }

        [Test]
        public void Enqueue_OneTrack_WillBeAddedAfterCurrent()
        {
            int numberOfTracks = 10;
            IEnumerable<ITrack> tracks = GetTracks(numberOfTracks);

            _queue.Set(tracks);
            Assert.AreEqual(numberOfTracks, _queue.Left.Count());

            for (int i = 0; i < numberOfTracks / 2; i++)
            {
                Assert.AreEqual("Track" + i, _queue.Current.Track.Name);
                bool result = _queue.Next();
                Assert.IsTrue(result);
            }

            _queue.Enqueue(GetTrack("Testing queue"));
            _queue.Next();
            Assert.AreEqual("Testing queue", _queue.Current.Track.Name);
        }

        private ITrack GetTrack(string name = "TrackName")
        {
            Mock<ITrack> trackMock = new Mock<ITrack>();
            trackMock.SetupGet(t => t.Name).Returns(name);
            return trackMock.Object;
        }

        private IEnumerable<ITrack> GetTracks(int count = 10)
        {
            List<ITrack> tracks = new List<ITrack>();
            for (int i = 0; i < count; i++)
            {
                tracks.Add(GetTrack("Track" + i));
            }
            return tracks;
        }
    }
}