using Microsoft.Practices.Prism.Commands;

namespace Torshify.Client.Modules.Core
{
    public static class CoreCommands
    {
        public static class Player
        {
            public static readonly CompositeCommand PauseCommand = new CompositeCommand();
            public static readonly CompositeCommand PlayCommand = new CompositeCommand();
            public static readonly CompositeCommand NextCommand = new CompositeCommand();
            public static readonly CompositeCommand PreviousCommand = new CompositeCommand();
            public static readonly CompositeCommand SeekCommand = new CompositeCommand();
        }

        public static readonly CompositeCommand PlayTrackCommand = new CompositeCommand();
        public static readonly CompositeCommand QueueTrackCommand = new CompositeCommand();
        public static readonly CompositeCommand NavigateBackCommand = new CompositeCommand();
        public static readonly CompositeCommand NavigateForwardCommand = new CompositeCommand();
        public static readonly CompositeCommand SearchCommand = new CompositeCommand();
    }
}