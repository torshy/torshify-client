using log4net;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Spotify
{
    public class SpotifyLogging : IInitializable
    {
        #region Fields

        private readonly ISession _session;

        private ILog _log = LogManager.GetLogger("Spotify");

        #endregion Fields

        #region Constructors

        public SpotifyLogging(ISession session)
        {
            _session = session;
        }

        #endregion Constructors

        #region Methods

        public void Initialize()
        {
            _session.PlayTokenLost += OnPlayerTokenLost;
            _session.ConnectionError += OnConnectionError;
            _session.EndOfTrack += OnEndOfTrack;
            _session.Exception += OnException;
            _session.LoginComplete += OnLogInComplete;
            _session.LogMessage += OnLogMessage;
            _session.LogoutComplete += OnLogoutComplete;
            _session.MessageToUser += OnMessageToUser;
            _session.StreamingError += OnStreamingError;
        }

        private void OnConnectionError(object sender, SessionEventArgs e)
        {
            _log.Error("Connection error " + e.Message + "[" + e.Status + "]");
        }

        private void OnEndOfTrack(object sender, SessionEventArgs e)
        {
            _log.Info("End of track");
        }

        private void OnException(object sender, SessionEventArgs e)
        {
            _log.Error("Exception: " + e.Message);
        }

        private void OnLogInComplete(object sender, SessionEventArgs e)
        {
            _log.Info("Logged in - " + e.Status + "[" + e.Status.GetMessage() + "]");
        }

        private void OnLogMessage(object sender, SessionEventArgs e)
        {
            _log.Info(e.Message);
        }

        private void OnLogoutComplete(object sender, SessionEventArgs e)
        {
            _log.Info("Logged out");
        }

        private void OnMessageToUser(object sender, SessionEventArgs e)
        {
            _log.Info(e.Message);
        }

        private void OnPlayerTokenLost(object sender, SessionEventArgs e)
        {
            _log.Info("Play token lost");
        }

        private void OnStreamingError(object sender, SessionEventArgs e)
        {
            _log.Warn(e.Message + "[" + e.Status.GetMessage() + "]");
        }

        #endregion Methods
    }
}