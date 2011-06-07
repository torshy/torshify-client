using System;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Mocks
{
    public class Track : ITrack
    {
        public int ID { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
    }
}