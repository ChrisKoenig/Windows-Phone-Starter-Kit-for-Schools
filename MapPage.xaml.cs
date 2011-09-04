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
using System.Device.Location;
using System.Linq;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;

namespace MySchoolApp
{
    public partial class MapPage : PhoneApplicationPage
    {
        public MapPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DataContext = App.ViewModel;

            setupMap();
        }

        private void setupMap()
        {
            if (App.ViewModel.Settings.Locations.Count > 0)
            {
                //set the Bing maps key
                map.CredentialsProvider = new ApplicationIdCredentialsProvider(App.ViewModel.Settings.BingMapsKey);

                // Set the center coordinate and zoom level
                GeoCoordinate mapCenter = new GeoCoordinate(App.ViewModel.Settings.Locations[0].Latitude, App.ViewModel.Settings.Locations[0].Longitude);
                int zoom = 15;

                // create a pushpins for each location
                Pushpin pin = null;
                for (int i = 0; i < App.ViewModel.Settings.Locations.Count; i++)
                {
                    pin = new Pushpin();
                    pin.Location = new GeoCoordinate(App.ViewModel.Settings.Locations[i].Latitude, App.ViewModel.Settings.Locations[i].Longitude);
                    pin.Content = App.ViewModel.Settings.Locations[i].Title;
                    map.Children.Add(pin);
                }

                // Set the map style to Aerial
                map.Mode = new RoadMode();

                // Set the view and put the map on the page
                map.SetView(mapCenter, zoom);
            }
            else
            {
                //notify user
            }
        }
    }
}