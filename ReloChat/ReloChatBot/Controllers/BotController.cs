﻿using Microsoft.Bot.Connector;
using ReloChatBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ReloChatBot.Controllers
{
    public class BotController
    {
        const string RedirectLodging = "RedirectLodging";
        const string RedirectMisc = "RedirectMisc";
        const string PositiveConfirmation = "PositiveConfirmation";
        const string NegativeConfirmation = "NegativeConfirmation";

        /*Changes Made By CommuteBot*/
        public LuisParser masterbot;

        //private LuisParser masterbot;
        private string userinput;

        private string reply;
        private Activity activity;

        //private bool ManualOverRide = false;
        public BotController(LuisParser masterbot, Activity activity)
        {
            this.masterbot = masterbot;
            this.activity = activity;
            this.userinput = this.activity.Text;

            // If the Intent is a Positive or Negative Confirmation, pass it on to the last bot that was talking.
            //this.ManualOverRide = (masterbot.Intent == PositiveConfirmation || masterbot.Intent == NegativeConfirmation);

            if (masterbot.RedirectRequired)
            {
                if (masterbot.Intent == RedirectLodging)
                {
                    //this.SetLastBotConversation(RedirectLodging);
                    //string test1 = this.GetLastBotConversation();
                    //bool test = this.GetLastBotConversation() == RedirectLodging;
                    this.handle_RedirectLodging();
                }
                else if (masterbot.Intent == RedirectMisc)
                {
                    this.handle_RedirectMiscBot();
                }
                else
                {
                    // this.reply = "I see...";
                    this.reply = masterbot.Reply;
                    // this.reply = "No handle created for \"" + this.masterbot.Intent + "\"!";
                    // throw new Exception("No handle created for \"" + this.masterbot.Intent + "\"!");
                }
            } else
            {
                if (masterbot.Intent == PositiveConfirmation)
                {
                    this.reply = "Hooray!";
                } else if (masterbot.Intent == NegativeConfirmation)
                {
                  this.reply = "I see...";
                } else
                {
                    if (masterbot.Intent == "None")
                    {
                        string query = "https://www.bing.com/search?q=" + Uri.EscapeDataString(masterbot.query);
                        this.reply = "I'm not sure if I understood you, I can [bing your query](" + query + ") if you'd like. Alternatively, you can checkout our [FAQ Page](http://www.industryexplorers.com/faq.html)";
                    } else
                    {
                        this.reply = masterbot.Reply;
                    }
                }

            }
        }

        private string GetLastBotConversation()
        {
            StateClient stateclient = this.activity.GetStateClient();
            BotData userData = stateclient.BotState.GetUserData(this.activity.ChannelId, this.activity.From.Id);
            return userData.GetProperty<string>("LastBotConversation");
        }

        private void SetLastBotConversation(string bot)
        {
            StateClient stateclient = this.activity.GetStateClient();
            BotData userdata = stateclient.BotState.GetUserData(this.activity.ChannelId, this.activity.From.Id);
            userdata.SetProperty<string>("LastBotConversation", bot);
            stateclient.BotState.SetUserData(this.activity.ChannelId, this.activity.From.Id, userdata);
        }

        public string LastBotConversation
        {
            get { return this.GetLastBotConversation(); }
            set { this.SetLastBotConversation(value); }
        }

        private void handle_RedirectLodging()
        {
            this.LastBotConversation = RedirectLodging;
            LodgingBot lobot = new LodgingBot(this.activity);
            lobot.Seed(this.masterbot);
            this.reply = lobot.Reply;
        }

        /*Changes Made By CommuteBot*/
        public async Task<string> handle_RedirectCommute()
        {
            CommuteBot combot = new CommuteBot(this.activity);
            var response = await CommuteMessageController.IntentsController(combot.LuisInfoData);
            return response;
        }

        private void handle_RedirectMiscBot()
        {
            this.LastBotConversation = RedirectMisc;
            MiscBot MBot = new MiscBot(this.activity);
            this.reply = MBot.Reply;

        }

        public string Reply
        {
            get { return this.reply; }
        }

    }
}
