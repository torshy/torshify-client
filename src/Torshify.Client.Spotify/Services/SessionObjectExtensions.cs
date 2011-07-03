namespace Torshify.Client.Spotify.Services
{
    public static class SessionObjectExtensions
    {
        public static bool IsValid(this ISessionObject sessionObject)
        {
            // TODO : Workaround of a lacking feature in torshify-lib. Not possible to see if the track is valid or not. 
            // The gethashcode will return 0 for invalid objects now
            return sessionObject.GetHashCode() > 0;
        }
    }
}