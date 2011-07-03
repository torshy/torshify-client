using System;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Mocks
{
    public class SearchProvider : ISearchProvider
    {
        public ISearch Search(string query, int trackOffset, int trackCount, int albumOffset, int albumCount, int artistOffset, int artistCount, object userData)
        {
            return new Search();
        }

        public ISearch Search(int fromYear, int toYear, Genre genre, object userData)
        {
            return new Search();
        }
    }
}