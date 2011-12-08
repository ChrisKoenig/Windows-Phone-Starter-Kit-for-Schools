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
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Tasks;

namespace MySchoolApp
{
    public partial class FeedPage : PhoneApplicationPage
    {
        #region Private Variables

        WebClient feedClient;

        #endregion Private Variables

        public FeedPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.New)
            {
                String uri = null;
                NavigationContext.QueryString.TryGetValue("uri", out uri);
                var feedVM = new FeedViewModel() { Uri = uri };
                DataContext = feedVM;
            }
            else
            {
                var feedvm=(FeedViewModel)DataContext;
                if(!feedvm.FeedLinks.Any())
                    feedvm.LoadFeed();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

        }

        private void feedListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var link = (Link)e.AddedItems[0];

                var wbt = new WebBrowserTask();
                wbt.Uri = new Uri(link.Url, UriKind.Absolute);
                wbt.Show();
            }
        }




    }
}