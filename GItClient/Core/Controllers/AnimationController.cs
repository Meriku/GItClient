using GItClient.Core.Models;
using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace GItClient.Core.Controllers
{
    internal class AnimationController
    {
        private const double MinCommandsBarHeight = 30;
        private const double MinCommandsBarWidth = 250;


        private double MaxCommandsBarHeight { get { return Application.Current.MainWindow.ActualHeight / 1.80; } }
        private double MaxCommandsBarWidth { get { return Application.Current.MainWindow.ActualWidth / 1.80; } }

        public bool IsCommandsBarOpen {get; private set;}

        #region CommandBar
        internal AnimationHolder GetCommandsBarAnimation(double currentHeight, double currentWidth, bool resize)
        {
            var IsOpen = currentHeight > MinCommandsBarHeight && currentWidth > MinCommandsBarWidth;

            var AnimationHeight = new DoubleAnimation()
            {
                EasingFunction = new QuadraticEase(),
                From = currentHeight,
                To = GetFinalHeight(IsOpen, resize),
                Duration = GetDuration(currentHeight, currentWidth, true),
            };

            var AnimationWidth = new DoubleAnimation()
            {
                EasingFunction = new QuadraticEase(),
                From = currentWidth,
                To = GetFinalWidth(IsOpen, resize),
                Duration = GetDuration(currentHeight, currentWidth, true)
            };

            if (resize == false) { IsCommandsBarOpen = !IsOpen; };

            return new AnimationHolder(AnimationHeight, AnimationWidth);
        }
        private double GetFinalHeight(bool isOpen, bool resize = false)
        { // readability is important

            if (resize || !isOpen)
            {
                return MaxCommandsBarHeight;
            }
            return MinCommandsBarHeight;
        }
        private double GetFinalWidth(bool isOpen, bool resize = false)
        { // readability is important

            if (resize || !isOpen)
            {
                return MaxCommandsBarWidth;
            }
            return MinCommandsBarWidth;
        }
        private Duration GetDuration(double currentHeight, double currentWidth, bool resize = false)
        {
            double timeD = 0;

            if (resize)
            {
                if (MaxCommandsBarHeight >= currentHeight || MaxCommandsBarWidth > currentWidth)
                {
                    timeD = ((MaxCommandsBarHeight - currentHeight) + (MaxCommandsBarWidth - currentWidth)) / 10.0 + 100;
                }
                else
                {
                    timeD = ((currentHeight - MaxCommandsBarHeight) + (currentWidth - MaxCommandsBarWidth)) / 10.0 + 100;
                }

                return new Duration(new TimeSpan(0, 0, 0, 0, (int)Math.Round(timeD, 0)));
            }
           
            var IsOpen = currentHeight > MinCommandsBarHeight && currentWidth > MinCommandsBarWidth;
            
            if (IsOpen)
            {
                timeD = ((currentHeight - MinCommandsBarHeight) + (currentWidth - MinCommandsBarWidth)) / 10.0 + 100;
            }
            else
            {
                timeD = ((MaxCommandsBarHeight - currentHeight) + (MaxCommandsBarWidth - currentWidth)) / 10.0 + 100;
            }
            
            return new Duration(new TimeSpan(0, 0, 0, 0, (int)Math.Round(timeD, 0)));
        }
        #endregion


    }
}
