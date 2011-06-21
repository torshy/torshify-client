using Microsoft.Practices.Prism.ViewModel;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Player
{
    public class PlayerViewModel : NotificationObject
    {
        private readonly IPlayer _player;

        public PlayerViewModel(IPlayer player)
        {
            _player = player;
        }

        public IPlayer Player
        {
            get
            {
                return _player;
            }
        }
    }
}