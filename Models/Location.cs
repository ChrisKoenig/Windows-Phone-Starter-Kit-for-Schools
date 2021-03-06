﻿//
//    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
//    Use of this sample source code is subject to the terms of the Microsoft license
//    agreement under which you licensed this sample source code and is provided AS-IS.
//    If you did not accept the terms of the license agreement, you are not authorized
//    to use this sample source code.  For the terms of the license, please see the
//    license agreement between you and Microsoft.
//
//
using System;
using System.Device.Location;

namespace MySchoolApp
{
    public class Location
    {
        public string Title { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public GeoCoordinate Coordinate
        {
            get
            {
                return new GeoCoordinate(Latitude, Longitude);
            }
        }
    }
}