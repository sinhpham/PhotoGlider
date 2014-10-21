using System;
using System.Collections.Generic;
using Xamarin.Forms;
using PhotoGliderPCL;
using PhotoGliderPCL.ViewModels;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using PhotoGliderPCL.Models;

namespace PhotoGliderForms
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            BindingContext = AppPCL.MainVM;

            Task.Run(async () =>
            {
                var res = await NetworkManager.LoadRedditImages(30);

                VM.Images = new ObservableCollection<RedditImage>();

                foreach (var img in res.Item1)
                {
                    VM.Images.Add(img);
                }
            });
        }

        public MainVM VM { get { return (MainVM)BindingContext; } }
    }
}

