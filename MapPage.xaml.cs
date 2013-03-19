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
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Phone.Maps.Controls;
using System.Windows;
using System.Windows.Controls;

namespace MySchoolApp
{
    public partial class MapPage : PhoneApplicationPage
    {
        private MapLayer locationLayer = null;

        public MapPage()
        {
            InitializeComponent();
            map.Loaded += map_Loaded;
            DataContext = App.ViewModel;
        }

        private void map_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = App.ViewModel.Settings.MapsApplicationId;
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = App.ViewModel.Settings.MapsAuthenticationToken;
            ShowLocations();
        }

        private void ShowLocations()
        {
            bool useThemeColor1 = true;

            foreach (var location in App.ViewModel.Locations)
            {
                // Create a small circle to mark the current location.
                Ellipse myCircle = new Ellipse();

                if (useThemeColor1)
                {
                    myCircle.Fill = App.ViewModel.Settings.ThemeColor1;
                    useThemeColor1 = false;
                }
                else
                {
                    myCircle.Fill = App.ViewModel.Settings.ThemeColor2;
                    useThemeColor1 = true;
                }

                myCircle.Height = 20;
                myCircle.Width = 20;
                myCircle.Opacity = 50;
                myCircle.Tag = location;
                myCircle.Tap += location_Tap;

                // Create a MapOverlay to contain the circle.
                MapOverlay myLocationOverlay = new MapOverlay();
                myLocationOverlay.Content = myCircle;
                myLocationOverlay.PositionOrigin = new Point(0.5, 0.5);
                myLocationOverlay.GeoCoordinate = location.Coordinate;

                // Create a MapLayer to contain the MapOverlay.
                locationLayer = new MapLayer();
                locationLayer.Add(myLocationOverlay);

                // Add the MapLayer to the Map.
                map.Layers.Add(locationLayer);
            }
        }

        private void location_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (sender is Ellipse)
            {
                var ellipse = sender as Ellipse;
                if (ellipse.Tag != null && ellipse.Tag is Location)
                {
                    var location = ellipse.Tag as Location;
                    MessageBox.Show(location.Title);
                }
            }
        }
    }
}