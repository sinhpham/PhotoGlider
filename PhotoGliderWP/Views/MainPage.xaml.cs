using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Xamarin.Forms;
using PhotoGliderPCL;
using System.Collections.ObjectModel;
using PhotoGliderPCL.Models;
using PhotoGliderPCL.ViewModels;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace PhotoGliderWP.Views
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            Forms.Init();
            AppPCL.MainVM.Images = new ObservableCollection<RedditImage>();

            DataContext = AppPCL.MainVM;

            LoadData();

            var dSize = (Application.Current.Host.Content.ActualWidth - 20) / 4;
            _mainList.GridCellSize = new System.Windows.Size(dSize, dSize);
        }

        public MainVM VM { get { return (MainVM)DataContext; } }

        bool IsLoading { get; set; }
        int _offsetKnob = 7;
        private void LongListSelector_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            var lls = (LongListSelector)sender;
            if (!IsLoading && lls.ItemsSource != null && lls.ItemsSource.Count >= _offsetKnob)
            {
                if (e.ItemKind == LongListSelectorItemKind.Item)
                {
                    if ((e.Container.Content as RedditImage).Equals(lls.ItemsSource[lls.ItemsSource.Count - _offsetKnob]))
                    {
                        Debug.WriteLine("Searching for {0}", VM.RedditNextPath);
                        LoadData();
                    }
                }
            }
        }

        void LoadData()
        {
            IsLoading = true;
            Task.Run(async () =>
            {
                var res = await NetworkManager.LoadRedditImages(30);
                Device.BeginInvokeOnMainThread(() =>
                {
                    VM.RedditNextPath = res.Item2;
                    foreach (var img in res.Item1)
                    {
                        VM.Images.Add(img);
                    }
                    IsLoading = false;
                });
            });
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var tb = (ToggleButton)sender;

            if (tb.IsChecked.Value)
            {
                _slideView.SelectedIndex = 0;
            }
            else
            {
                _slideView.SelectedIndex = 1;
            }
        }

        private void _subList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                Debug.WriteLine(e.AddedItems[0].ToString());
                var newSub = e.AddedItems[0].ToString();
                VM.SubReddit = newSub;

                LoadData();
                _slideView.SelectedIndex = 1;
            }
        }

        private void _mainList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_mainList.SelectedItem == null)
                return;

            VM.SelectedItem = (RedditImage)_mainList.SelectedItem;
            NavigationService.Navigate(new Uri("/Views/ItemPage.xaml", UriKind.Relative));

            // Reset selected item to null (no selection)
            _mainList.SelectedItem = null;
            
        }
    }
}