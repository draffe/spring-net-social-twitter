﻿using System;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Collections.Generic;

using Spring.Social.OAuth1;
using Spring.Social.Twitter.Api;

namespace Spring.WindowsPhoneQuickStart.ViewModel
{
    public class TwitterViewModel : INotifyPropertyChanged
    {
        private const string OAuthTokenKey = "OAuthToken";
        public const string CallbackUrl = "http://localhost/Twitter/Callback";

        private OAuthToken requestOAuthToken;
        private Uri authenticateUri;
        private IList<Tweet> tweets;

        public IOAuth1ServiceProvider<ITwitter> TwitterServiceProvider { get; set; }

        public bool IsAuthenticated
        {
            get
            {
                return this.OAuthToken != null;
            }
        }

        public OAuthToken OAuthToken
        {
            get
            {
                return this.LoadSetting<OAuthToken>(OAuthTokenKey, null);
            }
            set
            {
                this.SaveSetting(OAuthTokenKey, value);
                NotifyPropertyChanged("IsAuthenticated");
            }
        }

        public Uri AuthenticateUri
        {
            get
            {
                return this.authenticateUri;
            }
            set
            {
                this.authenticateUri = value;
                NotifyPropertyChanged("AuthenticateUri");
            }
        }

        public IList<Tweet> Tweets
        {
            get
            {
                return this.tweets;
            }
            set
            {
                this.tweets = value;
                NotifyPropertyChanged("Tweets");
            }
        }

        public void Authenticate()
        {
            this.TwitterServiceProvider.OAuthOperations.FetchRequestTokenAsync(CallbackUrl, null,
                r =>
                {
                    this.requestOAuthToken = r.Response;
                    this.AuthenticateUri = new Uri(this.TwitterServiceProvider.OAuthOperations.BuildAuthenticateUrl(r.Response.Value, null));
                });
        }

        public void AuthenticateCallback(string verifier)
        {
            AuthorizedRequestToken authorizedRequestToken = new AuthorizedRequestToken(this.requestOAuthToken, verifier);
            this.TwitterServiceProvider.OAuthOperations.ExchangeForAccessTokenAsync(authorizedRequestToken, null,
                r =>
                {
                    this.OAuthToken = r.Response;
                    this.ShowHomeTimeline();
                });
        }

        public void ShowHomeTimeline()
        {
            ITwitter twitterClient = this.TwitterServiceProvider.GetApi(this.OAuthToken.Value, this.OAuthToken.Secret);
            twitterClient.TimelineOperations.GetHomeTimelineAsync(
                r =>
                {
                    this.Tweets = r.Response;
                });
        }

        public void UpdateStatus(string status)
        {
            ITwitter twitterClient = this.TwitterServiceProvider.GetApi(this.OAuthToken.Value, this.OAuthToken.Secret);
            twitterClient.TimelineOperations.UpdateStatusAsync(status, r => this.ShowHomeTimeline());
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        private void SaveSetting(string key, object value)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains(key))
            {
                settings[key] = value;
            }
            else
            {
                settings.Add(key, value);
            }
        }

        private T LoadSetting<T>(string key, T defaultValue)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (!settings.Contains(key))
            {
                settings.Add(key, defaultValue);
            }
            return (T)settings[key];
        }
    }
}
