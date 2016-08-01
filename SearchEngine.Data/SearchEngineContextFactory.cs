using System.Data.Common;
using System.Data.Entity.Infrastructure;

namespace SearchEngine.Data
{
    public class SearchEngineContextFactory : IDbContextFactory<SearchEngineEntities>
    {
        public SearchEngineEntities Create()
        {
            var context = new SearchEngineEntities();
            context.Configuration.LazyLoadingEnabled = true;
            return context;
        }

        public SearchEngineEntities Create(DbConnection connection)
        {
            var context = new SearchEngineEntities(connection);
            context.Configuration.LazyLoadingEnabled = true;
            return context;
        }
    }
}