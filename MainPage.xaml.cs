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
using System.Net;
using System.Windows;
using System.Windows.Controls;
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
            setMapImage();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            if (ContactSelection.Visibility == Visibility.Visible)
            {
                e.Cancel = true;
                ContactSelection.Visibility = Visibility.Collapsed;
            }
        }

        private void setMapImage()
        {
            string mapUrl = App.ViewModel.BingStaticMapUrl;

            //check to see if there is at least one location and a BingMapsKey
            if (!string.IsNullOrEmpty(mapUrl) && !string.IsNullOrEmpty(App.ViewModel.Settings.BingMapsKey))
            {
                mapKeyWarning.Visibility = System.Windows.Visibility.Collapsed;
                BingMapImage.Source = new BitmapImage(new Uri(mapUrl));
            }
        }

        private void LinkListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            var link = (Link)e.AddedItems[0];
            ((ListBox)sender).SelectedItem = null;

            if (link.IsRss)
            {
                NavigationService.Navigate(new Uri(String.Format("/FeedPage.xaml?uri={0}", HttpUtility.UrlEncode(link.Url)), UriKind.Relative));
            }
            else
            {
                var wbt = new WebBrowserTask();
                wbt.Uri = new Uri(link.Url, UriKind.Absolute);
                wbt.Show();
            }
        }

        private void ClubsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            var club = (Club)e.AddedItems[0];
            ((ListBox)sender).SelectedItem = null;

            var wbt = new WebBrowserTask();
            wbt.Uri = new Uri(club.Url, UriKind.Absolute);
            wbt.Show();
        }

        private void Contacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            var contact = e.AddedItems[0] as Contact;

            if (!String.IsNullOrEmpty(contact.Email))
            {
                if (!String.IsNullOrEmpty(contact.PhoneNumber))
                {
                    ShowContactSelection(contact);
                }
                else
                {
                    EmailComposeTask ect = new EmailComposeTask();
                    ect.To = contact.Email;
                    ect.Show();
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(contact.PhoneNumber))
                {
                    PhoneCallTask pct = new PhoneCallTask();
                    pct.PhoneNumber = contact.PhoneNumber;
                    pct.Show();
                }
            }

            ((ListBox)sender).SelectedItem = null;
        }

        private void ShowContactSelection(Contact contact)
        {
            ContactSelection.DataContext = contact;
            StoryboardShowContactSelection.Begin();
        }

        private void ContactSelectionPhone_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var contact = ((FrameworkElement)sender).DataContext as Contact;
            PhoneCallTask pct = new PhoneCallTask();
            pct.PhoneNumber = contact.PhoneNumber;
            pct.Show();
        }

        private void ContactSelectionEmail_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var contact = ((FrameworkElement)sender).DataContext as Contact;
            EmailComposeTask ect = new EmailComposeTask();
            ect.To = contact.Email;
            ect.Show();
        }

        private void CampusMap_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/CampusMapPage.xaml", UriKind.Relative));
        }

        private void Map_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MapPage.xaml", UriKind.Relative));
        }
    }
}