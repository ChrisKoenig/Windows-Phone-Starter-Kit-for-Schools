using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

//
//    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
//    Use of this sample source code is subject to the terms of the Microsoft license
//    agreement under which you licensed this sample source code and is provided AS-IS.
//    If you did not accept the terms of the license agreement, you are not authorized
//    to use this sample source code.  For the terms of the license, please see the
//    license agreement between you and Microsoft.
//
//
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace MySchoolApp
{
    public partial class CampusMapPage : PhoneApplicationPage
    {
        private PageOrientation previousOrientation;
        private double initialScale;

        public CampusMapPage()
        {
            this.InitializeComponent();
            this.initialScale = 1.0;
            this.previousOrientation = base.Orientation;
            base.OrientationChanged += FullImage_OrientationChanged;
        }

        private void FullImage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            RotateTransition rotateTransition = new RotateTransition();
            PhoneApplicationPage element = (PhoneApplicationPage)((PhoneApplicationFrame)Application.Current.RootVisual).Content;

            switch (e.Orientation)
            {
                case PageOrientation.LandscapeLeft:
                    switch (this.previousOrientation)
                    {
                        case PageOrientation.PortraitUp:
                            rotateTransition.Mode = RotateTransitionMode.In90Clockwise;
                            break;
                        case PageOrientation.LandscapeRight:
                            rotateTransition.Mode = RotateTransitionMode.In180Clockwise;
                            break;
                    }
                    break;
                case PageOrientation.LandscapeRight:
                    switch (this.previousOrientation)
                    {
                        case PageOrientation.PortraitUp:
                            rotateTransition.Mode = RotateTransitionMode.In90Counterclockwise;
                            break;
                        case PageOrientation.LandscapeLeft:
                            rotateTransition.Mode = RotateTransitionMode.In180Counterclockwise;
                            break;
                    }
                    break;
                case PageOrientation.Portrait:
                    switch (this.previousOrientation)
                    {
                        case PageOrientation.LandscapeLeft:
                            rotateTransition.Mode = RotateTransitionMode.In90Counterclockwise;
                            break;
                        case PageOrientation.LandscapeRight:
                            rotateTransition.Mode = RotateTransitionMode.In90Clockwise;
                            break;
                    }
                    break;
            }

            ITransition transition = rotateTransition.GetTransition(element);
            transition.Completed += delegate
            {
                transition.Stop();
            };
            transition.Begin();
            this.previousOrientation = e.Orientation;
        }

        private void OnGestureListenerDragDelta(object sender, DragDeltaGestureEventArgs args)
        {
            if (this.initialScale > 1.0)
            {
                this.transform.TranslateX = this.ComputeTranslateX(this.transform.TranslateX + args.HorizontalChange, this.initialScale);
                this.transform.TranslateY = this.ComputeTranslateY(this.transform.TranslateY + args.VerticalChange, this.initialScale);
            }
        }

        private void OnGestureListenerPinchStarted(object sender, PinchStartedGestureEventArgs args)
        {
            this.initialScale = this.transform.ScaleX;
            this.transform.CenterX = this.MyImage.ActualWidth / 2.0;
            this.transform.CenterY = this.MyImage.ActualHeight / 2.0;
        }

        private void OnGestureListenerPinchDelta(object sender, PinchGestureEventArgs args)
        {
            this.transform.ScaleY = this.transform.ScaleX = this.initialScale * args.DistanceRatio;
            if (this.transform.ScaleX > 1.0)
            {
                this.transform.TranslateX = this.ComputeTranslateX(this.transform.TranslateX, this.transform.ScaleX);
                this.transform.TranslateY = this.ComputeTranslateY(this.transform.TranslateY, this.transform.ScaleY);
            }
        }

        private void OnGestureListenerPinchCompleted(object sender, PinchGestureEventArgs args)
        {
            if (this.transform.ScaleX < 1.0)
            {
                this.transform.ScaleX = this.transform.ScaleY = 1.0;
                this.transform.TranslateX = this.transform.TranslateY = 0.0;
            }
        }

        private double ComputeTranslateX(double translate, double ratio)
        {
            if (this.MyImage.ActualWidth * ratio > this.pageImage.ActualWidth)
            {
                var amp = this.MyImage.ActualWidth * ratio / 2;
                return (translate > amp) ? amp : (translate < -amp) ? -amp : translate;
            }

            return 0.0;
        }

        private double ComputeTranslateY(double translate, double ratio)
        {
            if (this.MyImage.ActualHeight * ratio > this.pageImage.ActualHeight)
            {
                var amp = this.MyImage.ActualHeight * ratio / 2;
                return (translate > amp) ? amp : (translate < -amp) ? -amp : translate;
            }
            return 0.0;
        }
    }
}