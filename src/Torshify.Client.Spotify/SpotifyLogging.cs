using Microsoft.Practices.Prism.Logging;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Spotify
{
    public class SpotifyLogging : IInitializable
    {
        #region Fields

        private readonly ISession _session;
        private readonly ILoggerFacade _logger;

        #endregion Fields

        #region Constructors

        public SpotifyLogging(ISession session, ILoggerFacade logger)
        {
            _session = session;
            _logger = logger;
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
            if (e.Status != Error.OK)
            {
                _logger.Log("Connection error " + e.Message + "[" + e.Status + "]", Category.Exception, Priority.High);
            }
            else
            {
                _logger.Log("Connected", Category.Info, Priority.Low);
            }
        }

        private void OnEndOfTrack(object sender, SessionEventArgs e)
        {
            _logger.Log("End of track", Category.Info, Priority.Low);
        }

        private void OnException(object sender, SessionEventArgs e)
        {
            _logger.Log(e.Message, Category.Exception, Priority.High);
        }

        private void OnLogInComplete(object sender, SessionEventArgs e)
        {
            _logger.Log("Logged in - " + e.Status + "[" + e.Status.GetMessage() + "]", Category.Info, Priority.Low);
        }

        private void OnLogMessage(object sender, SessionEventArgs e)
        {
            _logger.Log(e.Message, Category.Info, Priority.Medium);
        }

        private void OnLogoutComplete(object sender, SessionEventArgs e)
        {
            _logger.Log("Logged out", Category.Info, Priority.Low);
        }

        private void OnMessageToUser(object sender, SessionEventArgs e)
        {
            _logger.Log(e.Message, Category.Info, Priority.High);
        }

        private void OnPlayerTokenLost(object sender, SessionEventArgs e)
        {
            _logger.Log("Play token lost", Category.Info, Priority.High);
        }

        private void OnStreamingError(object sender, SessionEventArgs e)
        {
            _logger.Log(e.Message + "[" + e.Status.GetMessage() + "]", Category.Warn, Priority.High);
        }

        #endregion Methods
    }
}