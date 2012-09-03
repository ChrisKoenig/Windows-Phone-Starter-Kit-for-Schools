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
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using System.Xml.Linq;

namespace MySchoolApp.Helpers
{
    public class Utils
    {
        public static int GeoToGoogleCode(double p)
        {
            return (int)(p * 1000000);
        }

        public static String ToLongDayOfWeekName(String name)
        {
            //TODO: there is probably a better way to do this than what we have here...
            switch (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)
            {
                case "fr":
                    switch (name.ToLower())
                    {
                        case "lun.": return "lundi";
                        case "mar.": return "mardi";
                        case "mer.": return "mercredi";
                        case "jeu.": return "jeudi";
                        case "ven.": return "vendredi";
                        case "sam.": return "samedi";
                        case "dim.": return "dimanche";
                        default: return name;
                    }

                default:
                    switch (name.ToLower())
                    {
                        case "mon": return "monday";
                        case "tue": return "tuesday";
                        case "wed": return "wednesday";
                        case "thu": return "thursday";
                        case "fri": return "friday";
                        case "sat": return "saturday";
                        case "sun": return "sunday";
                        default: return name;
                    }
            }
        }

        /// <summary>
        /// returns a list of links from a given RSS2.0 or RDF feed
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public static List<Link> GetLinksFromFeed(string xmlString)
        {
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
                         Url = item.Element(xName + "link").Value,
                     }).ToList<Link>();

            return links;
        }

        /// <summary>
        /// returns a SolidColorBrush for a given argb color value
        /// </summary>
        /// <param name="argb"></param>
        /// <returns></returns>
        public static SolidColorBrush GetColorFromRGB(string argb)
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

                var aValue = Convert.ToByte(argb.Substring(0, 2), 16);
                var rValue = Convert.ToByte(argb.Substring(2, 2), 16);
                var gValue = Convert.ToByte(argb.Substring(4, 2), 16);
                var bValue = Convert.ToByte(argb.Substring(6, 2), 16);
                var color = Color.FromArgb(aValue, rValue, gValue, bValue);
                return new SolidColorBrush(color);
            }
            catch
            {
                //set to red if exception thrown while parsing color string
                return new SolidColorBrush(Colors.Red);
            }
        }

        internal static string GetDayOfWeekFromDate(string p)
        {
            var date = DateTime.Parse(p);
            var dow = date.DayOfWeek;
            return dow.ToString();
        }
    }
}