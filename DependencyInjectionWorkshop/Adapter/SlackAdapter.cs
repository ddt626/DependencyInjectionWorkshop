﻿using SlackAPI;

namespace DependencyInjectionWorkshop.Adapter
{
    public class SlackAdapter
    {
        public void Notify(string message)
        {
            var slackClient = new SlackClient("my api token");
            slackClient.PostMessage(resp => { }, "my channel", message, "my bot name");
        }
    }
}