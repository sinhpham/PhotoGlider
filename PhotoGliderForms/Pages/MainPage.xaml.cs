using System;
using System.Collections.Generic;
using Xamarin.Forms;
using PhotoGliderPCL;
using PhotoGliderPCL.ViewModels;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using PhotoGliderPCL.Models;
using Xamarin.Forms.Labs.Controls;

namespace PhotoGliderForms
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            AppPCL.MainVM.Images = new ObservableCollection<RedditImage>();

            for (var i = 0; i < 30; ++i)
            {
                AppPCL.MainVM.Images.Add(new RedditImage() { Title = i.ToString() });
            }

            BindingContext = AppPCL.MainVM;

//            Task.Run(async () =>
//            {
//                //var res = await NetworkManager.LoadRedditImages(30);
//
//
////
////                Device.BeginInvokeOnMainThread(() => {
////                    foreach (var img in res.Item1)
////                    {
////                        VM.Images.Add(img);
////                    }
////                    var a = 0;
////                });
//
//
//            });
        }

        public MainVM VM { get { return (MainVM)BindingContext; } }
    }
}

