//
//    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
//    Use of this sample source code is subject to the terms of the Microsoft license
//    agreement under which you licensed this sample source code and is provided AS-IS.
//    If you did not accept the terms of the license agreement, you are not authorized
//    to use this sample source code.  For the terms of the license, please see the
//    license agreement between you and Microsoft.
//
//
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media;
using System.Windows.Resources;
using System.Xml.Linq;
using Microsoft.Phone.Net.NetworkInformation;

namespace MySchoolApp
{
    public class MainViewModel : ViewModelBase
    {
        #region Constructor

        public MainViewModel()
        {
            this.NewsLinks = new ObservableCollection<Link>();
            this.Contacts = new ObservableCollection<Contact>();
            this.Links = new ObservableCollection<Link>();
            this.AthleticLinks = new ObservableCollection<Link>();
            this.FeedLinks = new ObservableCollection<Link>();
            this.Forecasts = new ObservableCollection<Forecast>();
        }

        #endregion Constructor

        #region Properties

        public ObservableCollection<Link> NewsLinks { get; private set; }

        public ObservableCollection<Contact> Contacts { get; private set; }

        public ObservableCollection<Link> Links { get; private set; }

        public ObservableCollection<Link> AthleticLinks { get; private set; }

        public ObservableCollection<Link> FeedLinks { get; set; }

        public ObservableCollection<Forecast> Forecasts { get; private set; }

        private Link selectedLink;

        public Link SelectedLink
        {
            get { return selectedLink; }
            set
            {
                if (selectedLink == value) return;
                selectedLink = value;
                RaisePropertyChanged("SelectedLink");
            }
        }

        private Link selectedFeedItem;

        public Link SelectedFeedItem
        {
            get { return selectedFeedItem; }
            set
            {
                if (selectedFeedItem == value) return;
                selectedFeedItem = value;
                RaisePropertyChanged("SelectedFeedItem");
            }
        }

        private Settings settings = new Settings();

        public Settings Settings
        {
            get
            {
                return settings;
            }
            set
            {
                if (value != settings)
                {
                    settings = value;
                    RaisePropertyChanged("Settings");
                }
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        #endregion Properties

        #region Public Methods

        public void LoadData()
        {
            loadSettings();

            loadNews();

            loadContacts();

            loadLinks();

            loadWeather();

            this.IsDataLoaded = true;
        }

        public List<Link> GetLinksFromFeed(string xmlString)
        {
            //
            // this routine supports most RSS 2.0 and RDF feeds
            //
            List<Link> links = null;

            //try to parse result as rss feed
            XDocument doc = XDocument.Parse(xmlString);

            //grab default namespace
            XNamespace xName = doc.Root.GetDefaultNamespace();

            //parse items
            links = (from item in doc.Descendants(xName + "item")
                     select new Link
                     {
                         Title = item.Element(xName + "title").Value,
                         Url = item.Element(xName + "link").Value.ToString()
                     }).ToList<Link>();

            return links;
        }

        #endregion Public Methods

        #region Private Methods

        #region Settings

        private void loadSettings()
        {
            StreamResourceInfo xml = Application.GetResourceStream(new Uri("/MySchoolApp;component/Data/Settings.xml", UriKind.Relative));
            XDocument settingsDoc = XDocument.Load(xml.Stream);

            this.Settings.Name = settingsDoc.Root.Element("name").Value;
            this.Settings.NewsUrl = settingsDoc.Root.Element("newsUrl").Value;
            this.Settings.BingMapsKey = settingsDoc.Root.Element("bingMapsKey").Value;
            this.Settings.ThemeColor1 = getColorFromRGB(settingsDoc.Root.Element("themeColor1").Value);
            this.Settings.ThemeColor2 = getColorFromRGB(settingsDoc.Root.Element("themeColor2").Value);
            loadLocations(settingsDoc.Root.Element("locations").Descendants("location"));
        }

        private void loadLocations(IEnumerable<XElement> locations)
        {
            if (locations != null)
            {
                Location loc = null;
                foreach (XElement item in locations)
                {
                    loc = new Location();
                    loc.Title = item.Attribute("title").Value;
                    loc.Latitude = (double)item.Attribute("latitude");
                    loc.Longitude = (double)item.Attribute("longitude");
                    Settings.Locations.Add(loc);
                }
            }
        }

        private SolidColorBrush getColorFromRGB(string argb)
        {
            try
            {
                if (argb.StartsWith("#"))
                {
                    argb = argb.Substring(1);
                }

                //add alpha value if not set
                if (argb.Length == 6)
                {
                    argb = "FF" + argb;
                }

                //set to red if string length is not correct
                if (argb.Length != 8)
                {
                    return new SolidColorBrush(Colors.Red);
                }

                return new SolidColorBrush(Color.FromArgb(System.Convert.ToByte(argb.Substring(0, 2), 16), System.Convert.ToByte(argb.Substring(2, 2), 16), System.Convert.ToByte(argb.Substring(4, 2), 16), System.Convert.ToByte(argb.Substring(6, 2), 16)));
            }
            catch (Exception ex)
            {
                //set to red if exception thrown while parsing color string
                return new SolidColorBrush(Colors.Red);
            }
        }

        #endregion Settings

        #region News

        private void loadNews()
        {
            //check if network and client are available and newsurl exists
            if (NetworkInterface.GetIsNetworkAvailable() && !string.IsNullOrEmpty(App.ViewModel.Settings.NewsUrl))
            {
                string url = App.ViewModel.Settings.NewsUrl;

                var request = HttpWebRequest.Create(url);
                var result = (IAsyncResult)request.BeginGetResponse(loadNewsCallback, request);
            }
            else
            {
                //notify user
                OnNewsUpdated(new NewsUpdatedEventArgs() { Message = "error" });
            }
        }

        private void loadNewsCallback(IAsyncResult result)
        {
            try
            {
                var request = (HttpWebRequest)result.AsyncState;
                var response = request.EndGetResponse(result);

                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    string contents = reader.ReadToEnd();

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        loadNewsCompleted(contents);
                    });
                }
            }
            catch (Exception ex)
            {
                OnNewsUpdated(new NewsUpdatedEventArgs() { Message = "error" });
            }
        }

        private void loadNewsCompleted(string results)
        {
            try
            {
                //ensure NewsLinks is empty
                App.ViewModel.NewsLinks.Clear();

                List<Link> links = App.ViewModel.GetLinksFromFeed(results);

                links.ForEach(x => App.ViewModel.NewsLinks.Add(x));

                OnNewsUpdated(new NewsUpdatedEventArgs() { Message = "ok" });
            }
            catch (Exception ex)
            {
                OnNewsUpdated(new NewsUpdatedEventArgs() { Message = "error" });
            }
        }

        #endregion News

        #region Contacts

        private void loadContacts()
        {
            StreamResourceInfo xml = Application.GetResourceStream(new Uri("/MySchoolApp;component/Data/Contacts.xml", UriKind.Relative));
            XDocument contactsDoc = XDocument.Load(xml.Stream);

            List<Contact> cItems = (from item in contactsDoc.Descendants("contact")
                                    select new Contact
                                    {
                                        Name = item.Element("name").Value.ToString(),
                                        Email = item.Element("email").Value.ToString(),
                                        PhotoUrl = item.Element("photoUrl").Value.ToString(),
                                        PhoneNumber = item.Element("phoneNumber").Value.ToString()
                                    }).ToList<Contact>();

            cItems.ForEach(x => Contacts.Add(x));
        }

        #endregion Contacts

        #region Links

        private void loadLinks()
        {
            parseLinkFile("/Data/Links.xml").ForEach(x => Links.Add(x));

            parseLinkFile("/Data/Athletics.xml").ForEach(x => AthleticLinks.Add(x));
        }

        private List<Link> parseLinkFile(string resourcePath)
        {
            StreamResourceInfo xml = Application.GetResourceStream(new Uri("/MySchoolApp;component" + resourcePath, UriKind.Relative));
            XDocument linksDoc = XDocument.Load(xml.Stream);

            List<Link> lItems = (from item in linksDoc.Descendants("link")
                                 select new Link
                                 {
                                     Title = item.Element("title").Value.ToString(),
                                     Url = item.Element("url").Value.ToString(),
                                     IsRss = item.Attribute("isRSS") != null
                                 }).ToList<Link>();

            return lItems;
        }

        #endregion Links

        #region Weather

        private void loadWeather()
        {
            //check if network and client are available and newsurl exists
            if (NetworkInterface.GetIsNetworkAvailable() && Settings.Locations.Count > 0)
            {
                string url = string.Format("http://forecast.weather.gov/MapClick.php?lat={0}&lon={1}&FcstType=dwml", Settings.Locations[0].Latitude, Settings.Locations[0].Longitude);

                var request = HttpWebRequest.Create(url);
                var result = (IAsyncResult)request.BeginGetResponse(loadWeatherCallback, request);
            }
        }

        private void loadWeatherCallback(IAsyncResult result)
        {
            try
            {
                var request = (HttpWebRequest)result.AsyncState;
                var response = request.EndGetResponse(result);

                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    string contents = reader.ReadToEnd();

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        loadWeatherCompleted(contents);
                    });
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void loadWeatherCompleted(string results)
        {
            try
            {
                XDocument weatherDoc = XDocument.Parse(results);

                XElement timeLayouts = weatherDoc.Root.Element("data").Descendants("time-layout").Where(t => t.Descendants("start-valid-time").Count() >= 12).First();

                //create new forecasts with name and date
                List<Forecast> fItems = (from item in timeLayouts.Descendants("start-valid-time")
                                         select new Forecast
                                         {
                                             Name = item.Attribute("period-name") != null ? item.Attribute("period-name").Value.ToString() : "",
                                             Date = DateTime.Parse(item.Value.ToString())
                                         }).ToList<Forecast>();

                fItems.ForEach(x => Forecasts.Add(x));

                //set min temperature
                int tempCount = 0;
                IEnumerable<XElement> currentNodes = (from a in weatherDoc.Root.Element("data").Element("parameters").Elements("temperature")
                                                      where a.Attribute("type").Value == "minimum"
                                                      select a).Descendants("value");
                if (currentNodes != null)
                {
                    if (Forecasts[0].Name == "Tonight")
                        tempCount = 0;
                    else
                        tempCount = 1;

                    foreach (XElement item in currentNodes)
                    {
                        Forecasts[tempCount].Temperature = int.Parse(item.Value);
                        tempCount += 2;
                    }
                }

                //set max temperature
                currentNodes = (from a in weatherDoc.Root.Element("data").Element("parameters").Elements("temperature")
                                where a.Attribute("type").Value == "maximum"
                                select a).Descendants("value");
                if (currentNodes != null)
                {
                    if (Forecasts[0].Name == "Tonight")
                        tempCount = 1;
                    else
                        tempCount = 0;

                    foreach (XElement item in currentNodes)
                    {
                        Forecasts[tempCount].Temperature = int.Parse(item.Value);
                        tempCount += 2;
                    }
                }

                //set weather condition
                currentNodes = weatherDoc.Root.Element("data").Element("parameters").Element("weather").Descendants("weather-conditions");
                if (currentNodes != null)
                {
                    for (int i = 0; i < currentNodes.Count(); i++)
                    {
                        Forecasts[i].Conditions = currentNodes.ElementAt(i).Attribute("weather-summary").Value.ToString();
                    }
                }

                //set image url
                //(precipitation percentage is embedded in image)
                currentNodes = weatherDoc.Root.Element("data").Element("parameters").Element("conditions-icon").Descendants("icon-link");
                if (currentNodes != null)
                {
                    for (int i = 0; i < currentNodes.Count(); i++)
                    {
                        Forecasts[i].ImageUrl = currentNodes.ElementAt(i).Value;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        #endregion Weather

        #endregion Private Methods

        #region Event

        public delegate void NewsUpdatedEventHandler(object sender, NewsUpdatedEventArgs e);

        public event NewsUpdatedEventHandler NewsUpdated;

        protected virtual void OnNewsUpdated(NewsUpdatedEventArgs e)
        {
            if (NewsUpdated != null)
                NewsUpdated(this, e);
        }

        #endregion Event
    }

    public class NewsUpdatedEventArgs : EventArgs
    {
        public string Message { get; set; }
    }
}