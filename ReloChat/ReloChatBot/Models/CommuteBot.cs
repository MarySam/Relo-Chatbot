﻿using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using ReloChatBot;

namespace ReloChatBot.Models
{
    /// <summary>
    /// CommuteBot Inherent from LuisParser Class
    /// </summary>
    public class CommuteBot : LuisParser
    {
        public CommuteBot(Activity activity, string api_endpoint = "https://api.projectoxford.ai/luis/v1/application?id=2fb62e84-da20-4205-8dad-d6206e533681&subscription-key=a0794768387a459da34bab6f49878c1e&q=") : base(activity, api_endpoint) { }

    }
    /// <summary>
    /// CommuteBot Message Controller to respond message based on the intent of messages about commute questions
    /// </summary>

    public class CommuteMessageController
    {
        public static async Task<string> IntentsController(LuisInfo luisInfo)
        {
            string responseMessage;
            if (luisInfo.intents.Count() > 0)
            {
                //intents[0] is the highest probability of intent.
                switch (luisInfo.intents[0].intent)
                {
                    case "GetCommuteTime":
                        if (luisInfo.entities.Count() > 0)
                        {
                            // set Redmond,WA as default destination if people ask question only include one entity
                            if (luisInfo.entities.Length == 1)
                            {
                                responseMessage = await CommuteUtilities.GetCommuteTime(luisInfo.entities[0].entity, "Redmond,WA");
                            }
                            else
                            {
                                responseMessage = await CommuteUtilities.GetCommuteTime(luisInfo.entities[0].entity, luisInfo.entities[1].entity);
                            }

                        }
                        else
                        {
                            responseMessage = "Sorry, I don't understand the location.";
                        }
                        break;
                    case "GetDistance":
                        if (luisInfo.entities.Count() > 0)
                        {
                            if (luisInfo.entities.Length == 1)
                            {
                                responseMessage = await CommuteUtilities.GetDistance(luisInfo.entities[0].entity, "Redmond,WA");
                            }
                            else
                            {
                                responseMessage = await CommuteUtilities.GetDistance(luisInfo.entities[0].entity, luisInfo.entities[1].entity);
                            }
                        }
                        else
                        {
                            responseMessage = "Sorry, I don't understand the location.";
                        }
                        break;
                    case "GetTransportation":

                        responseMessage = "Check out the King County Metro Online website for public transportation: http://metro.kingcounty.gov/. If you want to get driving information, use this link to Bing maps: http://www.bing.com/mapspreview";
              
                        break;
                    case "GetTraffic":

                        responseMessage = "Check out this website to get estimated commute time: http://www.wsdot.com/traffic/Seattle/TravelTimes/reliability/default.aspx";
                      
                        break;
                    case "GetAddress":

                        responseMessage = "Please check out Bing map: http://www.bing.com/mapspreview";

                        break;
                    default:

                        responseMessage = "Sorry, I don't know how to " + luisInfo.intents[0].intent;

                        break;
                }
            }
            else
            {
                responseMessage = "Sorry, I'm not sure what you want.";
            }

            return responseMessage;
        }

    }
}
