using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Tweetinvi;
using Tweetinvi.Core.Enum;
using Tweetinvi.Core.Interfaces.Models;
using Tweetinvi.Core.Interfaces.Streaminvi;
using Tweetinvi.Logic.Model;
using TwitterSearch.Hubs;
using Geo = Tweetinvi.Geo;

namespace TwitterSearch.Models
{
    //only one instance should run on the server - called by multiple instances of the hub class
    public class Twitter
    {
        // Singleton instance
        //private readonly static Lazy<Twitter> _instance = new Lazy<Twitter>(
        //    () => new Twitter(GlobalHost.ConnectionManager.GetHubContext<TwitterHub>().Clients, GlobalHost.ConnectionManager.GetHubContext<TwitterHub>()));

        //public static Twitter Instance
        //{
        //    get
        //    {
        //        return _instance.Value;
        //    }
        //}

        //private readonly ConcurrentDictionary<string, Tweet> _tweets = new ConcurrentDictionary<string, Tweet>();

        //private readonly IHubContext _context;

        //private IHubConnectionContext<dynamic> Clients
        //{
        //    get;set;
        //}

        //private Twitter(IHubConnectionContext<dynamic> clients, IHubContext context)
        //{
        //    Clients = clients;
        //    _context = context;
        //    _tweets.Clear();
        //}

        private Thread _thread;
        private IFilteredStream _filteredStream;

        public void StartSearch(string connectionId, string hastTag, string userName)
        {
            try
            {
                string accessToken = System.Configuration.ConfigurationManager.AppSettings["AccessToken"];
                string accessTokenSecret = System.Configuration.ConfigurationManager.AppSettings["AccessTokenSecret"];
                string consumerKey = System.Configuration.ConfigurationManager.AppSettings["ConsumerKey"];
                string consumerSecret = System.Configuration.ConfigurationManager.AppSettings["ConsumerSecret"];

                TwitterCredentials.SetCredentials(accessToken, accessTokenSecret, consumerKey, consumerSecret);

                if (!string.IsNullOrEmpty(hastTag))
                {
                    
                }
                if (!string.IsNullOrEmpty(userName))
                {

                }

                //GetTweetsByUser();
                //GetTweetsBySearchParameters();
                GetLiveFilteredStream(connectionId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void StopSearch(string connectionId)
        {
            //how do we abort the correct thread?
            if (_filteredStream != null)
            {
                _filteredStream.StopStream();
            }

            if (_thread != null && _thread.IsAlive)
            {
                _thread.Abort();
            }

            //_context.Clients.Client(connectionId).addTweetsToPage(connectionId, "stoptweetText", "stoptweetCreator", "stoptweetLocation");
            //Clients.All.addTweetsToPage("stoptweetId", "stoptweetText", "stoptweetCreator", "stoptweetLocation");
        }

        private void GetTweetImages()
        {
            //var tweet = Tweet.GetTweet(454952190915137536);
            //var imageUrl = tweet.Entities.Medias.First().MediaURL;
            //new WebClient().DownloadFile(imageUrl, @"c:\image.jpg");
        }

        private void GetTweetsByUser()
        {
            //var tweetsFromUser = Search.SearchTweets("from:ajt_84");
            //foreach (var tweet in tweetsFromUser)
            //{
            //    Console.WriteLine(tweet.Creator.Name + " says: ");
            //    Console.Write(tweet.Text);
            //    Console.Write("\r\n");
            //    Console.Write("\r\n");
            //}
        }

        private void GetTweetsBySearchParameters()
        {
            //var searchParameter = Search.GenerateSearchTweetParameter("#WorldCup");

            //searchParameter.Lang = Language.English;
            //searchParameter.SearchType = SearchResultType.Popular;
            //searchParameter.MaximumNumberOfResults = 200;
            //searchParameter.Since = new DateTime(2013, 12, 1);

            //var tweets = Search.SearchTweets(searchParameter);
            //tweets.ForEach(t => Clients.All.addTweetsToPage(t.Text));
        }

        private void GetLiveFilteredStream(string connectionId)
        {
            //Get tweets by town (w/radius)

            //bethy green - switch the google maps co-ordinates
            var coordinates1 = new Coordinates(-0.126390, 51.476198);
            var coordinates2 = new Coordinates(0.004374, 51.553188);
            var location = Geo.GenerateLocation(coordinates1, coordinates2);

            _filteredStream = Stream.CreateFilteredStream();
            _filteredStream.AddLocation(location);

            _filteredStream.MatchingTweetAndLocationReceived += (sender, args) =>
            {
                var tweet = args.Tweet;
                var tweetLocation = string.Empty;

                var tweetId = string.Format("TweetId: {0} ", tweet.Id);
                var tweetText = string.Format("Text: {0} ", tweet.Text);
                var tweetCreator = string.Format("Creator: {0} ", tweet.Creator);

                IEnumerable<ILocation> matchingLocations = args.MatchedLocations;

                foreach (var matchingLocation in matchingLocations)
                {
                    tweetLocation += string.Format("{0}, {1}", matchingLocation.Coordinate1.Latitude, matchingLocation.Coordinate1.Longitude);
                    tweetLocation += string.Format("{0}, {1}", matchingLocation.Coordinate2.Latitude, matchingLocation.Coordinate2.Longitude);
                }
                //_context.Clients.Client(connectionId).addTweetsToPage(tweetId, tweetText, tweetCreator, tweetLocation);
                //Clients.All.addTweetsToPage(tweetId, tweetText, tweetCreator, tweetLocation);
            };
            _thread = new Thread(_filteredStream.StartStreamMatchingAllConditions);
            _thread.Start();
        }
    }
}