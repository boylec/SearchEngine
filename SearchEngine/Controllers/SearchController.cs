using SearchEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml.Linq;
using Castle.Components.DictionaryAdapter;
using Castle.Core.Internal;
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

        [Route("api/Search/GetSuggestion")]
        [HttpPost]
        public HttpResponseMessage GetSuggestion(SearchInput searchInput)
        {
            if (string.IsNullOrWhiteSpace(searchInput?.SearchString))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new string[0]);
            }

            var enteredWords = searchInput.SearchString.Split(',').Select(str => str.Trim());

            var associatedWords = new List<Word>();

            _dbContext.Words.Where(word => enteredWords.Any(searchWord => searchWord.Equals(word.WordString, StringComparison.CurrentCultureIgnoreCase)))    //Grab all the words entered from the db
                            .Select(x => x.WordUrlAssociations)                                                                                         //Look at their associations
                            .ForEach(association =>
                            {
                                association.Select(x => x.Url)                                                                  //Look at all of the URLs
                                .ForEach(ur =>                                                                                  //Find other words associated with those URLs
                                {
                                    associatedWords.AddRange(ur.WordUrlAssociations.Select(x => x.Word));                       //Add those other words to the list
                                });
                            });

            associatedWords.RemoveAll(wr => enteredWords.Any(w => string.Equals(w, wr.WordString, StringComparison.CurrentCultureIgnoreCase)));

            //Return the top 5 words that occur most frequently and occur more than twice
            var result =
                associatedWords
                    .GroupBy(a => a.WordString)
                    //.Where(gr => gr.Any())        //Only looking at the words associated more than 1 time
                    .OrderByDescending(g => g.Count()) //Order by the number of times associated
                    .Take(5) //Grab the top 5
                    .Select(gr => $"{searchInput.SearchString.TrimEnd(',')}, {gr.Key}");              //Grabbing the word string


            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [Route("api/Search/DoSearch")]
        [HttpPost]
        public HttpResponseMessage DoSearch(SearchInput searchInput)
        {
            try
            {
                var enteredWords = searchInput.SearchString.Split(',').Select(str => str.Trim());

                IQueryable<Url> matchingUrls = null;
                if (searchInput.Operator == "and")
                {
                    matchingUrls = _dbContext.Urls.Where(
                        x =>
                            enteredWords.All(
                                enteredWord =>
                                    x.WordUrlAssociations.Any(association => enteredWord == association.Word.WordString)));
                }
                else if (searchInput.Operator == "xor")
                {
                    matchingUrls = _dbContext.Urls.Where(
                        x =>
                            enteredWords.Count(
                                enteredWord =>
                                    x.WordUrlAssociations.Any(association => enteredWord == association.Word.WordString)) == 1);
                }
                else
                {
                    matchingUrls = _dbContext.Urls.Where(
                        x =>
                            enteredWords.Any(
                                enteredWord =>
                                    x.WordUrlAssociations.Any(association => enteredWord == association.Word.WordString)))
                        .OrderByDescending(url => enteredWords.Count(
                            enteredWord =>
                                url.WordUrlAssociations.Any(association => enteredWord == association.Word.WordString)));
                }



                var resultingUrls = matchingUrls.Select(x => x.UrlString).Distinct();
                //the resulting urls will be deduplicated using Distinct()
                var result = new SearchResult()
                {
                    Urls = resultingUrls,
                    Count = resultingUrls.Count()
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