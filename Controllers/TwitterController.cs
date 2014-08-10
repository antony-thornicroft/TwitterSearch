using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TwitterSearch.Models;

namespace TwitterSearch.Controllers
{
    public class TwitterController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search()
        {
            var tweet = new TweetSearch();

            return View(tweet);
        }

        public ActionResult HeatMap()
        {
            var tweet = new TweetSearch();

            return View(tweet);
        }
    }
}
