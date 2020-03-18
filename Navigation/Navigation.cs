﻿using System;
using System.Windows.Controls;

namespace XboxGameClipLibrary.Navigation
{
    public static class Navigation
    {
        private static Frame _frame;
        private static object _data = null;

        public static Frame Frame
        {
            get { return _frame; }
            set { _frame = value; }
        }

        public static bool Navigate(Uri sourcePageUri, object extraData)
        {
            if (_frame.CurrentSource != sourcePageUri)
            {
                return _frame.Navigate(sourcePageUri, extraData);
            }
            return true;
        }

        public static bool Navigate(object content)
        {
            if (_frame.NavigationService.Content != content)
            {
                return _frame.Navigate(content);
            }
            return true;
        }
    }
}
