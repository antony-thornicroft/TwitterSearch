using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
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
        private readonly Twitter _twitterTicker;

        public TwitterHub() : this(Twitter.Instance) { }

        public TwitterHub(Twitter twitterTicker)
        {
            _twitterTicker = twitterTicker;
        }

        public IEnumerable<Models.Tweet> GetAllTweets()        
        {
            return _twitterTicker.GetAllTweets();
        }

        public void StartSearch()
        {
            _twitterTicker.StartSearch("userLoginName", "hastTag", "userNameSearch");
        }

        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            return base.OnDisconnected();
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }

        //public class ConnectionMapping<T>
        //{
        //    private readonly Dictionary<T, HashSet<string>> _connections = new Dictionary<T, HashSet<string>>();

        //    public int Count
        //    {
        //        get
        //        {
        //            return _connections.Count;
        //        }
        //    }

        //    public void Add(T key, string connectionId)
        //    {
        //        lock (_connections)
        //        {
        //            HashSet<string> connections;
        //            if (!_connections.TryGetValue(key, out connections))
        //            {
        //                connections = new HashSet<string>();
        //                _connections.Add(key, connections);
        //            }
        //            lock (connections)
        //                connections.Add(connectionId);
        //        }
        //    }

        //    public IEnumerable<string> GetConnections(T key)
        //    {
        //        HashSet<string> connections;
        //        if (_connections.TryGetValue(key, out connections))
        //            return connections;

        //        return Enumerable.Empty<string>();
        //    }

        //    public void Remove(T key, string connectionId)
        //    {
        //        //if the connections is 0 we could pause the signle instance of the stream from calling stopStream()

        //        lock (_connections)
        //        {
        //            HashSet<string> connections;
        //            if (!_connections.TryGetValue(key, out connections))
        //                return;
        //            lock (connections)
        //            {
        //                connections.Remove(connectionId);

        //                if (connections.Count == 0)
        //                    _connections.Remove(key);
        //            }
        //        }
        //    }
        //}
    }
}