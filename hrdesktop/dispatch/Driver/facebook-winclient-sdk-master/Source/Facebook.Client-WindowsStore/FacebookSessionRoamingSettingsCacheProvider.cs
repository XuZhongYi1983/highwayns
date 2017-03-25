﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;

namespace Facebook.Client
{

    public class FacebookSessionRoamingSettingsCacheProvider : FacebookSessionCacheProvider
    {
        private const string key = "FACEBOOK_SESSION";

        // TODO: Should we handle the data changing? 
        //public FacebookSessionLocalSettingsCacheProvider()
        //{
        //    Windows.Storage.ApplicationData.Current.DataChanged += Current_DataChanged;
        //}

        //void Current_DataChanged(ApplicationData sender, object args)
        //{
            
        //}

        //public event Windows.Foundation.TypedEventHandler<FacebookSession, object> SessionChanged;

        // NOTE: This provider must handle changes in the app data over different versions of a Store App. 
        // If we update the SDK and the data format changes, we should automatically migrate to prevent
        // app crashes and poor user experience (i.e. forced to login after app updates).

        public override FacebookSession GetSessionData()
        {
            var settings = ApplicationData.Current.RoamingSettings;
            var composite = (ApplicationDataCompositeValue)settings.Values[key];
            
            if (composite == null)
            {
                return null;
            }

            var session = new FacebookSession
            {
                AccessToken = (string)composite["AccessToken"],
                FacebookId = (string)composite["FacebookId"],
            };

            var expires = (string)composite["Expires"];
            if (!string.IsNullOrEmpty(expires))
            {
                DateTime date;
                if (DateTime.TryParse(expires, out date))
                {
                    session.Expires = date;
                }
            }

            var perms = (string)composite["CurrentPermissions"];
            if (!string.IsNullOrEmpty(perms))
            {
                var p = perms.Split(',');
                session.CurrentPermissions.AddRange(p);
            }

            return session;
        }

        public override void SaveSessionData(FacebookSession data)
        {
            var settings = ApplicationData.Current.RoamingSettings;
            var composite = new ApplicationDataCompositeValue();
            composite["AccessToken"] = data.AccessToken;
            composite["CurrentPermissions"] = string.Join(",", data.CurrentPermissions);
            composite["Expires"] = data.Expires.ToString();
            composite["FacebookId"] = data.FacebookId;
            settings.Values[key] = composite;
        }

        public override void DeleteSessionData()
        {
            var settings = ApplicationData.Current.RoamingSettings;
            if (settings.Values.ContainsKey(key))
            {
                settings.Values.Remove(key);
            }
        }
    }
}
