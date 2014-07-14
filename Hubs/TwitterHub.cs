using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using TwitterSearch.Models;

namespace TwitterSearch.Hubs
{
    //refernece the single running instance of the twitter class from each instance of this hub class
    public class TwitterHub : Hub
    {
        private readonly Twitter _twitter;

        public TwitterHub() : this(Twitter.Instance)
        {
        }

        public TwitterHub(Twitter twitter)
        {
            _twitter = twitter;
        }

        //this is returning a custom type which is for the restful api calls rather than the stream
        //public IEnumerable<Tweet> StartSearch()
        public void StartSearch(string hastTag, string userName)
        {
            _twitter.StartSearch(Context.ConnectionId, hastTag, userName);
        }

        public void StopSearch()
        {
            _twitter.StopSearch(Context.ConnectionId);
        }

        public override Task OnConnected()
        {
            string trrst = "";
            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            string trrst = "";
            return base.OnDisconnected();
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