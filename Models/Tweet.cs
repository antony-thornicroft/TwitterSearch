using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using TwitterSearch.Hubs;

namespace TwitterSearch.Models
{
    public enum SearchMode
    {
        HashTag,
        UserName
    };

    //search object used to pass parameters from client to hub
    public class TweetSearch
    {
        public List<SearchMode> SearchModes { get; set; }

        public TweetSearch()
        {
            SearchModes = new List<SearchMode> { SearchMode.HashTag, SearchMode.UserName };
        }

        public string SearchHashTag { get; set; }
        public string SearchUserName { get; set; }
        public string SearchTownName { get; set; }

        //TODO - add search co-ords lower left - upper rirght etc..
    }

    //TODO - return tweet model for view - both restful and streaming API calls
    public class Tweet
    {
        public string TweetId { get; set; }
        public string TweetText { get; set; }
        public string TweetCreator { get; set; }
        public string TweetLocation { get; set; }
    }
}