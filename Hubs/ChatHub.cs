using Microsoft.AspNet.SignalR;

namespace TwitterSearch.Hubs
{
    //You can create public methods on your hub class and then access those methods by calling them from jQuery scripts in a web page
    //Call a jQuery function on the client (such as the addNewMessageToPage function) to update clients
    public class ChatHub : Hub
    {
        //Chat hub method..
        public void Send(string name, string message)
        {
            string test = Context.User.Identity.Name;

            // Call the addNewMessageToPage method to update clients
            Clients.All.addNewMessageToPage(name, message);
        }
    }
}