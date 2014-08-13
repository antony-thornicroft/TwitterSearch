using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        private readonly static Lazy<Twitter> instance = new Lazy<Twitter>(() => new Twitter(GlobalHost.ConnectionManager.GetHubContext<TwitterHub>().Clients));

        private readonly ConcurrentDictionary<string, Tweet> _tweets = new ConcurrentDictionary<string, Tweet>();
        private List<Tweet> _currentTweets;

        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(10000);
        private readonly Timer _timer;

        private IHubConnectionContext<dynamic> Clients
        {
            get;
            set;
        }

        public static Twitter Instance
        {
            get
            {
                return instance.Value;
            }
        }

        private Twitter(IHubConnectionContext<dynamic> clients)
        {
            //TODO clear the tweet object down every 000 tweets

            Clients = clients;

            _tweets.Clear();

            var tweets = new List<Tweet>
            {
                new Tweet { TweetId = "Test TweetId from constructor 1", TweetText = "Test TweetText from constructor 1" },
                new Tweet { TweetId = "Test TweetId from constructor 2", TweetText = "Test TweetText from constructor 2" }
            };
            tweets.ForEach(tweet => _tweets.TryAdd(tweet.TweetId, tweet));

            _timer = new Timer(UpdateTweet, null, _updateInterval, _updateInterval);
        }

        private void UpdateTweet(object state)
        {
            if (_currentTweets == null)
                _currentTweets = new List<Tweet>();

            var myTweets = _tweets.Values.Where(tweet => !_currentTweets.Contains(tweet)).ToList();

            foreach (var tweet in _tweets.Values)
                _currentTweets.Add(tweet);

            Clients.All.addTweetCollection(myTweets);
        }
        public IEnumerable<Tweet> GetAllTweets()
        {
            return _tweets.Values;
        }

        public void AddTweetToList(Tweet tweet)
        {
            
        }

        public void StartSearch(string userLoginName, string hastTag, string userNameSearch)
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
                if (!string.IsNullOrEmpty(userNameSearch))
                {
                }

                //_connections.Add(userLoginName, Context.ConnectionId);
                //var identity = Thread.CurrentPrincipal.Identity;

                //GetTweetsByUser();
                //GetTweetsBySearchParameters();
                GetLiveFilteredStream("connectionId");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private Thread _thread;
        private IFilteredStream _filteredStream;

        private void GetLiveFilteredStream(string connectionId)
        {
            //east england
            //top right
            var coordinates1 = new Coordinates(1.2958363, 52.9240094);
            //bottom left
            var coordinates2 = new Coordinates(-2.811781, 50.737100);

            //beth
            //top right
            //var coordinates1 = new Coordinates(-0.028270, 51.546073);
            //bottom left
            //var coordinates2 = new Coordinates(-0.115603, 51.475066);

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

                var newTweet = new Tweet
                    {
                        TweetId = tweetId,
                        TweetCreator = tweetCreator,
                        TweetLatitude = tweet.Coordinates.Latitude.ToString(),
                        TweetLongitude = tweet.Coordinates.Longitude.ToString(),
                        TweetText = tweetText
                    };
                _tweets.TryAdd(tweetId, newTweet);

                //working
                //Clients.All.addTweetsToPage(tweetId, tweetText, tweetCreator, tweetLocation, tweetLat, tweetLon);

                //_context.Clients.Client(connectionId).addTweetsToPage(tweetId, tweetText, tweetCreator, tweetLocation);
                //Clients.Client(connectionId).addTweetsToPage(tweetId, tweetText, tweetCreator, tweetLocation);
            };
            _thread = new Thread(_filteredStream.StartStreamMatchingAllConditions);
            _thread.Start();
        }

        //public void StopSearch(string userLoginName)
        //{
            //remove uiser from he group
            //_connections.Remove(name, Context.ConnectionId);

            //_connections.Remove(userLoginName, Context.ConnectionId);

            //this should be called when no one is in the group
            //_twitter.StopSearch(Context.ConnectionId);
        //}

        public void StopSearch(string connectionId)
        {
            //how do we abort the correct thread?
            //if (_filteredStream != null)
            //{
            //    _filteredStream.StopStream();
            //}

            //if (_thread != null && _thread.IsAlive)
            //{
            //    _thread.Abort();
            //}

            //_context.Clients.Client(connectionId).addTweetsToPage(connectionId, "stoptweetText", "stoptweetCreator", "stoptweetLocation");
            //Clients.All.addTweetsToPage("stoptweetId", "stoptweetText", "stoptweetCreator", "stoptweetLocation");
        }
    }
}