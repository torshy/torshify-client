namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface ISearchProvider
    {
        ISearch Search(
            string query, 
            int trackOffset, 
            int trackCount, 
            int albumOffset, 
            int albumCount, 
            int artistOffset,
            int artistCount, 
            object userData = null);

        ISearch Search(
            int fromYear, 
            int toYear, 
            Genre genre, 
            object userData = null);
    }
}