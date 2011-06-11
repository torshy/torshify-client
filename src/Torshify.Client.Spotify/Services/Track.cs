using System;

using Microsoft.Practices.Prism.ViewModel;

using ITorshifyTrack = Torshify.Client.Infrastructure.Interfaces.ITrack;

namespace Torshify.Client.Spotify.Services
{
    public class Track : NotificationObject, ITorshifyTrack
    {
        #region Fields

        private int _id;
        private int _index;
        private string _name;

        #endregion Fields

        #region Constructors

        protected Track(ITrack track)
        {
            InternalTrack = track;
        }

        #endregion Constructors

        #region Properties

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        public string Name
        {
            get { return InternalTrack.Name; }
            set { _name = value; }
        }

        protected ITrack InternalTrack
        {
            get; private set;
        }

        #endregion Properties
    }
}