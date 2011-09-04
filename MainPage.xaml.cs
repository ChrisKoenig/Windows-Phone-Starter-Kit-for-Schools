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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace MySchoolApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DataContext = App.ViewModel;
            App.ViewModel.NewsUpdated += ViewModel_NewsUpdated;

            resetListBoxes();

            setMapImage();
        }

        private void ViewModel_NewsUpdated(object sender, NewsUpdatedEventArgs e)
        {
            if (e.Message == "ok")
            {
                VisualStateManager.GoToState(this, "CompletedState", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "ErrorState", true);
            }
        }

        private void resetListBoxes()
        {
            // selected indexes are set back to -1 to support
            // selecting a link that has already been selected
            //
            // this happens when a link is selected, which opens a new page
            // then when the back button is pressed, main page is reloaded in its current state
            // if the index is not reset, a user will not be able to reselect the link just opened
            linksListBox.SelectedIndex = -1;
            athleticsListBox.SelectedIndex = -1;
        }

        private void setMapImage()
        {
            string mapUrl = App.ViewModel.Settings.GetBingStaticMapUrl();

            //check to see if there is at least one location and a BingMapsKey
            if (!string.IsNullOrEmpty(mapUrl) && !string.IsNullOrEmpty(App.ViewModel.Settings.BingMapsKey))
            {
                mapKeyWarning.Visibility = System.Windows.Visibility.Collapsed;
                BingMapImage.Source = new BitmapImage(new Uri(mapUrl));
            }
        }

        private void EmailTextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock)
            {
                string emailAddress = ((TextBlock)sender).Text;

                EmailComposeTask ect = new EmailComposeTask();
                ect.To = emailAddress;
                ect.Show();
            }
        }

        private void PhoneTextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock)
            {
                string phoneNumber = ((TextBlock)sender).Text;

                PhoneCallTask pct = new PhoneCallTask();
                pct.PhoneNumber = phoneNumber;
                pct.Show();
            }
        }

        private void BingMapImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MapPage.xaml", UriKind.Relative));
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                App.ViewModel.SelectedLink = (Link)e.AddedItems[0];

                if (App.ViewModel.SelectedLink.IsRss)
                {
                    NavigationService.Navigate(new Uri("/FeedPage.xaml", UriKind.Relative));
                }
                else
                {
                    WebBrowserTask wbt = new WebBrowserTask();
                    wbt.URL = App.ViewModel.SelectedLink.Url;
                    wbt.Show();
                }
            }
        }
    }
}