using SearchEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SearchEngine.Data;

namespace SearchEngine.Controllers
{
    public class SearchController : ApiController
    {
        private readonly SearchEngineEntities _dbContext;

        public SearchController(SearchEngineEntities dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public HttpResponseMessage DoSearch(SearchInput searchInput)
        {
            try
            {
                var words = searchInput.SearchString.Split(' ');

                //Get an array of word/url associations grouped by count
                var matchingWords = _dbContext.Words.Where(word => words.Any(searchWord => searchWord.Equals(word.WordString,StringComparison.InvariantCultureIgnoreCase)));
                var matchingWordAssociations = matchingWords.Select(word => word.WordUrlAssociations);
                var groupedAssociations = matchingWordAssociations.OrderByDescending(assoc => assoc.Count);

                //Drill down on the same grouped word/url associations to get the actual url strings
                var groupedUrls = groupedAssociations.Select(group => group.Select(gr => gr.Url.UrlString));

                var urls = new List<string>();

                //Aggregate all the groups of urls (the result being a list of urls ordered by the urls with the most count)
                foreach (var group in groupedUrls)
                {
                    urls.AddRange(group);
                }

                //the resulting urls will be deduplicated using Distinct()
                var result = new SearchResult()
                {
                    Urls = urls.Distinct(),
                    Count = urls.Count
                };

                return Request.CreateResponse(HttpStatusCode.OK, new { SearchResult = result });
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
    }
}