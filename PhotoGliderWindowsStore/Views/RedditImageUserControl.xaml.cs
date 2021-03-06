﻿using PhotoGliderPCL;
using PhotoGliderPCL.Models;
using PhotoGliderWindowsStore.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace PhotoGliderWindowsStore.Views
{
    public sealed partial class RedditImageUserControl : UserControl
    {
        public RedditImageUserControl()
        {
            this.InitializeComponent();

            _img.MaxWidth = Window.Current.Bounds.Width;
            _img.MaxHeight = Window.Current.Bounds.Height;

            UserControlDataContext["ImageLoading"] = true;
            UserControlDataContext["ImageFailed"] = false;
            UserControlDataContext["ShowGalleryList"] = false;
        }

        private ObservableDictionary _userControlDataContext = new ObservableDictionary();
        public ObservableDictionary UserControlDataContext
        {
            get { return this._userControlDataContext; }
        }

        private void _outerSv_Loaded(object sender, RoutedEventArgs e)
        {
            _outerSv.ChangeView(null, 60.0f, null);
        }

        bool _isPullRefresh = false;
        private void _outerSv_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var sv = sender as ScrollViewer;


            if (sv.VerticalOffset == 0.0f)
                textBlock1.Opacity = 1;
            else
                textBlock1.Opacity = 0.3f;

            if (sv.VerticalOffset != 0.0f)
                _isPullRefresh = true;

            if (!e.IsIntermediate)
            {
                if (sv.VerticalOffset == 0.0f && _isPullRefresh)
                {
                    Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        _galleryList.ClearValue(ListView.SelectedItemProperty);
                        AppPCL.MainVM.OnNeedToGoBack();
                    });
                }
                _isPullRefresh = false;
                //sv.ChangeView(null, 60.0f, null);
            }
        }

        private void _outerSv_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _sv.Width = e.NewSize.Width;
            _sv.Height = e.NewSize.Height;
            _img.MaxWidth = e.NewSize.Width;
            _img.MaxHeight = e.NewSize.Height;
            _outerSv.ChangeView(null, 60.0f, null);
        }

        private void _img_ImageOpened(object sender, RoutedEventArgs e)
        {
            UserControlDataContext["ImageLoading"] = false;
        }

        private void _img_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            UserControlDataContext["ImageLoading"] = false;
            UserControlDataContext["ImageFailed"] = true;
        }

        private void ucRoot_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            // Reset image specific settings.
            UserControlDataContext["ShowGalleryList"] = false;
            _sv.ChangeView(null, null, 1);
            _galleryList.ClearValue(ListView.SelectedItemProperty);

            var currImg = this.DataContext as RedditImage;
            if (currImg == null)
            {
                throw new InvalidOperationException();
            }

            if (currImg.NSFW && AppPCL.SettingVM.FilterNSFW)
            {
                UserControlDataContext["ShowNSFWWarning"] = true;
            }
            else
            {
                // Set image source binding in code to prevent flashing of NSFW content.
                UserControlDataContext["ShowNSFWWarning"] = false;

                var binding = new Binding
                {
                    Path = new PropertyPath("DisplayingImage.ImageLink"),
                };
                _img.SetBinding(Image.SourceProperty, binding);

                if (currImg.GalleryImages != null)
                {
                    // Need to show gallery list.
                    UserControlDataContext["ShowGalleryList"] = true;
                    var isBinding = new Binding { Path = new PropertyPath("GalleryImages") };
                    _galleryList.SetBinding(ListView.ItemsSourceProperty, isBinding);
                    var currItemBinding = new Binding { Path = new PropertyPath("DisplayingImage"), Mode = BindingMode.TwoWay };
                    _galleryList.SetBinding(ListView.SelectedItemProperty, currItemBinding);

                    _galleryList.SelectionChanged += (s, arg) =>
                    {
                        UserControlDataContext["ImageLoading"] = true;
                        _outerSv.ChangeView(null, 60.0f, null);
                        _sv.ChangeView(null, null, 1);
                    };
                }
            }
        }

        private void _sv_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _outerSv.ChangeView(null, 60.0f, null);
            _sv.ChangeView(null, null, 1);
        }
    }
}
