﻿using PhotoGliderPCL;
using PhotoGliderPCL.Models;
using PhotoGliderPCL.ViewModels;
using PhotoGliderWindowsStore.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace PhotoGliderWindowsStore.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private NavigationHelper navigationHelper;
        public MainVM VM
        {
            get { return AppPCL.MainVM; }
        }

        private ObservableDictionary pageDataContext = new ObservableDictionary();
        public ObservableDictionary PageDataContext
        {
            get { return this.pageDataContext; }
        }

        static readonly string MenuOpenedKey = "MenuOpened";

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public MainPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            VM.PropertyChanged += (s, arg) =>
            {
                if (arg.PropertyName == "SubReddit")
                {
                    var idx = VM.SubReddits.IndexOf(VM.SubReddit);
                    _subList.SelectedIndex = idx;
                }
            };

            _subList.Items.VectorChanged += (s, arg) =>
            {
                _subList.SelectedItem = VM.SubReddit;
            };

            _subList.Loaded += (s, arg) =>
            {
                _subList.SelectedItem = VM.SubReddit;
            };

            AppPCL.SettingVM.PropertyChanged += (s, arg) =>
            {
                if (arg.PropertyName == "FilterNSFW")
                {
                    //VM.RefreshCmd.Execute(null);
                }
            };

            PageDataContext.MapChanged += (s, arg) =>
            {
                if (arg.Key == MenuOpenedKey)
                {
                    if ((bool)PageDataContext[MenuOpenedKey])
                    {
                        _showMenu.Begin();
                    }
                    else
                    {
                        _hideMenu.Begin();
                    }
                }
            };

            PageDataContext[MenuOpenedKey] = AppPCL.SettingVM.OpenMenuOnStart;

            RedditImageParser.RunOnUiThread = act =>
            {
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => act());
            };
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            VM.SelectedItem = (RedditImage)e.ClickedItem;
            this.Frame.Navigate(typeof(ItemDetailPage));
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                Debug.WriteLine(e.AddedItems[0].ToString());
                var newSub = e.AddedItems[0].ToString();
                VM.SubReddit = newSub;
            }
        }

        private void Rectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PageDataContext[MenuOpenedKey] = !(bool)PageDataContext[MenuOpenedKey];
        }

        private void itemGridView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((bool)PageDataContext[MenuOpenedKey])
            {
                PageDataContext[MenuOpenedKey] = false;
            }
        }

        private void TextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var tb = (TextBox)sender;
                tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                //_menuAppName.Focus(Windows.UI.Xaml.FocusState.Programmatic);
            }
        }

        private void itemGridView_Loaded(object sender, RoutedEventArgs e)
        {
            var sv = itemGridView.GetFirstDescendantOfType<ScrollViewer>();

            sv.ViewChanged += (s, arg) =>
            {
                // If this is because of changing sub reddit, do not hide the menu.
                if (sv.HorizontalOffset != 0)
                {
                    if ((bool)PageDataContext[MenuOpenedKey])
                    {
                        PageDataContext[MenuOpenedKey] = false;
                    }
                }
            };
        }
    }

    public class ThumbnailTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NormalTemplate { get; set; }
        public DataTemplate NSFWTemplate { get; set; }
        protected override DataTemplate SelectTemplateCore(object item)
        {
            var ri = (RedditImage)item;
            if (ri.NSFW && AppPCL.SettingVM.FilterNSFW)
            {
                return NSFWTemplate;
            }
            return NormalTemplate;
        }
    }

    public static class VisualTreeHelperExtensions
    {
        public static T GetFirstDescendantOfType<T>(this DependencyObject start) where T : DependencyObject
        {
            return start.GetDescendantsOfType<T>().FirstOrDefault();
        }

        public static IEnumerable<T> GetDescendantsOfType<T>(this DependencyObject start) where T : DependencyObject
        {
            return start.GetDescendants().OfType<T>();
        }

        public static IEnumerable<DependencyObject> GetDescendants(this DependencyObject start)
        {
            var queue = new Queue<DependencyObject>();
            var count = VisualTreeHelper.GetChildrenCount(start);

            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(start, i);
                yield return child;
                queue.Enqueue(child);
            }

            while (queue.Count > 0)
            {
                var parent = queue.Dequeue();
                var count2 = VisualTreeHelper.GetChildrenCount(parent);

                for (int i = 0; i < count2; i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    yield return child;
                    queue.Enqueue(child);
                }
            }
        }

        public static T GetFirstAncestorOfType<T>(this DependencyObject start) where T : DependencyObject
        {
            return start.GetAncestorsOfType<T>().FirstOrDefault();
        }

        public static IEnumerable<T> GetAncestorsOfType<T>(this DependencyObject start) where T : DependencyObject
        {
            return start.GetAncestors().OfType<T>();
        }

        public static IEnumerable<DependencyObject> GetAncestors(this DependencyObject start)
        {
            var parent = VisualTreeHelper.GetParent(start);

            while (parent != null)
            {
                yield return parent;
                parent = VisualTreeHelper.GetParent(parent);
            }
        }

        public static bool IsInVisualTree(this DependencyObject dob)
        {
            return Window.Current.Content != null && dob.GetAncestors().Contains(Window.Current.Content);
        }

        public static Rect GetBoundingRect(this FrameworkElement dob, FrameworkElement relativeTo = null)
        {
            if (relativeTo == null)
            {
                relativeTo = Window.Current.Content as FrameworkElement;
            }

            if (relativeTo == null)
            {
                throw new InvalidOperationException("Element not in visual tree.");
            }

            if (dob == relativeTo)
                return new Rect(0, 0, relativeTo.ActualWidth, relativeTo.ActualHeight);

            var ancestors = dob.GetAncestors().ToArray();

            if (!ancestors.Contains(relativeTo))
            {
                throw new InvalidOperationException("Element not in visual tree.");
            }

            var pos =
                dob
                    .TransformToVisual(relativeTo)
                    .TransformPoint(new Point());
            var pos2 =
                dob
                    .TransformToVisual(relativeTo)
                    .TransformPoint(
                        new Point(
                            dob.ActualWidth,
                            dob.ActualHeight));

            return new Rect(pos, pos2);
        }
    }

    public class PaginatedCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading
    {
        private Func<uint, Task<Tuple<IEnumerable<T>, string>>> load;
        public bool HasMoreItems { get; private set; }

        public PaginatedCollection(Func<uint, Task<Tuple<IEnumerable<T>, string>>> load)
        {
            HasMoreItems = true;
            this.load = load;
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return AsyncInfo.Run(async c =>
            {
                var data = await load(count);
                AppPCL.MainVM.RedditNextPath = data.Item2;

                foreach (var item in data.Item1)
                {
                    Add(item);
                }

                HasMoreItems = data.Item1.Any();

                return new LoadMoreItemsResult()
                {
                    Count = (uint)data.Item1.Count(),
                };
            });
        }
    }
}
