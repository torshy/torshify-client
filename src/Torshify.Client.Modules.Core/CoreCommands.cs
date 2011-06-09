using Microsoft.Practices.Prism.Commands;

namespace Torshify.Client.Modules.Core
{
    public static class CoreCommands
    {
        public static readonly CompositeCommand PlayTrackCommand = new CompositeCommand();
        public static readonly CompositeCommand QueueTrackCommand = new CompositeCommand();
    }
}