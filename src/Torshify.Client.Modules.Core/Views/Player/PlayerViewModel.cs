using System;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Player
{
    public class PlayerViewModel : NotificationObject
    {
        #region Fields

        private readonly IPlayerController _player;
        private double _requestSeek;

        #endregion Fields

        #region Constructors

        public PlayerViewModel(IPlayerController player)
        {
            _player = player;
        }

        #endregion Constructors

        #region Properties

        public IPlayerController Player
        {
            get
            {
                return _player;
            }
        }

        public double RequestSeek
        {
            get { return _requestSeek; }
            set
            {
                _requestSeek = value;
                _player.Seek(TimeSpan.FromSeconds(value));
            }
        }

        #endregion Properties
    }
}