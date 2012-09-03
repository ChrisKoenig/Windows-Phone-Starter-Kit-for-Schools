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
using System.Windows.Media;

namespace MySchoolApp
{
    public class Settings
    {
        public string Name { get; set; }

        public string NewsUrl { get; set; }

        public string BingMapsKey { get; set; }

        public SolidColorBrush ThemeColor1 { get; set; }

        public SolidColorBrush ThemeColor2 { get; set; }

        public string NameToUpper
        {
            get { return Name.ToUpper(); }
        }

        public string WeatherApiKey { get; set; }
    }
}