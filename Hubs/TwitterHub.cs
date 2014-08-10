using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using Tweetinvi;
using Tweetinvi.Core.Enum;
using Tweetinvi.Core.Interfaces.Models;
using Tweetinvi.Core.Interfaces.Streaminvi;
using Tweetinvi.Logic.Model;
using TwitterSearch.Hubs;
using TwitterSearch.Models;
using Geo = Tweetinvi.Geo;

namespace TwitterSearch.Hubs
{
    //refernece the single running instance of the twitter class from each instance of this hub class
    public class TwitterHub : Hub
    {
        public class ConnectionMapping<T>
        {
            private readonly Dictionary<T, HashSet<string>> _connections = new Dictionary<T, HashSet<string>>();

            public int Count
            {
                get
                {
                    return _connections.Count;
                }
            }

            public void Add(T key, string connectionId)
            {
                lock (_connections)
                {
                    HashSet<string> connections;
                    if (!_connections.TryGetValue(key, out connections))
                    {
                        connections = new HashSet<string>();
                        _connections.Add(key, connections);
                    }
                    lock (connections)
                        connections.Add(connectionId);
                }
            }

            public IEnumerable<string> GetConnections(T key)
            {
                HashSet<string> connections;
                if (_connections.TryGetValue(key, out connections))
                    return connections;

                return Enumerable.Empty<string>();
            }

            public void Remove(T key, string connectionId)
            {
                //if the connections is 0 we could pause the signle instance of the stream from calling stopStream()

                lock (_connections)
                {
                    HashSet<string> connections;
                    if (!_connections.TryGetValue(key, out connections))
                        return;
                    lock (connections)
                    {
                        connections.Remove(connectionId);

                        if (connections.Count == 0)
                            _connections.Remove(key);
                    }
                }
            }
        }
        
        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();

        //private readonly Twitter _twitter;

        //public TwitterHub() : this(Twitter.Instance)
        //{
        //}

        //public TwitterHub(Twitter twitter)
        //{
        //    _twitter = twitter;
        //}

        //this is returning a custom type which is for the restful api calls rather than the stream
        //public IEnumerable<Tweet> StartSearch()
        //public void StartSearch(string userLoginName, string hastTag, string userNameSearch)
        //{
            //_connections.Add(userLoginName, Context.ConnectionId);

            //_twitter.StartSearch(Context.ConnectionId, hastTag, userName);
        //}

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

                var identity = Thread.CurrentPrincipal.Identity;

                //GetTweetsByUser();
                //GetTweetsBySearchParameters();
                GetLiveFilteredStream(Context.ConnectionId);
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
            //Get tweets by town (w/radius)

            //bethy green - switch the google maps co-ordinates
            //top right
            var coordinates1 = new Coordinates(1.2958363, 52.9240094);
            //bootom left
            var coordinates2 = new Coordinates(-2.811781, 50.737100);
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

                var tweetLat = tweet.Coordinates.Latitude.ToString();
                string tweetLon = tweet.Coordinates.Longitude.ToString();
                
                foreach (var matchingLocation in matchingLocations)
                {
                    tweetLocation += string.Format("{0}, {1}", matchingLocation.Coordinate1.Latitude, matchingLocation.Coordinate1.Longitude);
                    tweetLocation += string.Format("{0}, {1}", matchingLocation.Coordinate2.Latitude, matchingLocation.Coordinate2.Longitude);
                }
                //_context.Clients.Client(connectionId).addTweetsToPage(tweetId, tweetText, tweetCreator, tweetLocation);
                Clients.All.addTweetsToPage(tweetId, tweetText, tweetCreator, tweetLocation, tweetLat, tweetLon);

                //Clients.Client(connectionId).addTweetsToPage(tweetId, tweetText, tweetCreator, tweetLocation);
            };
            _thread = new Thread(_filteredStream.StartStreamMatchingAllConditions);
            _thread.Start();
        }

        public void StopSearch(string userLoginName)
        {
            //remove uiser from he group
            //_connections.Remove(name, Context.ConnectionId);

            //_connections.Remove(userLoginName, Context.ConnectionId);

            //this should be called when no one is in the group
            //_twitter.StopSearch(Context.ConnectionId);
        }

        //coming in
        public override Task OnConnected()
        {
            string name = Context.User.Identity.Name;

            //_connections.Add(Context.ConnectionId, Context.ConnectionId);

            return base.OnConnected();
        }

        //leaving
        public override Task OnDisconnected()
        {
            //i can call this method to remove the user from the list on the hub for now
            //i can call this method from other evetns as well, such as when an app closes or a browser tab closes..
            UserLeft();

            return base.OnDisconnected();
        }

        //i could move this to the twitter class?!
        public void UserLeft()
        {
            //if (Users.Any(x => x.ConnectionID == Context.ConnectionId))
            //{
            //    User user = Users.First(x => x.ConnectionID == Context.ConnectionId);
            //    Users.Remove(user);
            //    Clients.Others.userLeft(JsonConvert.SerializeObject(user), Context.ConnectionId);
            //}
        }

        public override Task OnReconnected()
        {
            string trrst = "";
            return base.OnReconnected();
        }
    }

    //public interface IClient
    //{
    //    void StartSearch(string hastTag, string userName);
    //    void StopSearch();
    //}
}