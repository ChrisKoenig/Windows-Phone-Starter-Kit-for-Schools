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

namespace MySchoolApp
{
    public class Forecast : ModelBase
    {
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

        private DateTime date;

        public DateTime Date
        {
            get { return date; }
            set
            {
                if (date == value) return;
                date = value;
                RaisePropertyChanged("Date");
            }
        }

        private int temperature;

        public int Temperature
        {
            get { return temperature; }
            set
            {
                if (temperature == value) return;
                temperature = value;
                RaisePropertyChanged("Temperature");
            }
        }

        private string conditions;

        public string Conditions
        {
            get { return conditions; }
            set
            {
                if (conditions == value) return;
                conditions = value;
                RaisePropertyChanged("Conditions");
            }
        }

        private string imageUrl;

        public string ImageUrl
        {
            get { return imageUrl; }
            set
            {
                if (imageUrl == value) return;
                imageUrl = value;
                RaisePropertyChanged("ImageUrl");
            }
        }

        public string TempAndConditions
        {
            get { return string.Format("{0} and {1}", Temperature, Conditions); }
        }

        public string DateText
        {
            get { return Date.ToString("m"); }
        }
    }
}