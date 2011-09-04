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
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace MySchoolApp
{
    public class Settings : ModelBase
    {
        #region Constructor

        public Settings()
        {
            Locations = new ObservableCollection<Location>();
        }

        #endregion Constructor

        #region Properties

        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                if (name == value) return;
                name = value;
                RaisePropertyChanged("Name");
            }
        }

        public string NameToUpper
        {
            get { return name.ToUpper(); }
        }

        public string NewsUrl { get; set; }

        private string bingMapsKey;

        public string BingMapsKey
        {
            get { return bingMapsKey; }
            set
            {
                if (bingMapsKey == value) return;
                bingMapsKey = value;
                RaisePropertyChanged("BingMapsKey");
            }
        }

        public ObservableCollection<Location> Locations { get; set; }

        private SolidColorBrush themeColor1;

        public SolidColorBrush ThemeColor1
        {
            get { return themeColor1; }
            set
            {
                if (themeColor1 == value) return;
                themeColor1 = value;
                RaisePropertyChanged("ThemeColor1");
            }
        }

        private SolidColorBrush themeColor2;

        public SolidColorBrush ThemeColor2
        {
            get { return themeColor2; }
            set
            {
                if (themeColor2 == value) return;
                themeColor2 = value;
                RaisePropertyChanged("ThemeColor2");
            }
        }

        #endregion Properties

        #region Public Methods

        public string GetBingStaticMapUrl()
        {
            if (Locations.Count > 0)
            {
                return string.Format("http://dev.virtualearth.net/REST/v1/Imagery/Map/Road/{0},{1}/15?mapSize=376,376&pp={0},{1};21&mapVersion=v1&key={2}", Locations[0].Latitude, Locations[0].Longitude, BingMapsKey);
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion Public Methods
    }
}