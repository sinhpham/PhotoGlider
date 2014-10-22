using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhotoGliderWP.Resources;
using Xamarin.Forms;
using PhotoGliderForms;
using PhotoGliderPCL;
using System.Collections.ObjectModel;
using PhotoGliderPCL.Models;
using System.Threading.Tasks;
using PhotoGliderPCL.ViewModels;
using System.Diagnostics;

namespace PhotoGliderWP
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            Forms.Init();
            AppPCL.MainVM.Images = new ObservableCollection<RedditImage>();

            DataContext = AppPCL.MainVM;

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
                }
            }
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}