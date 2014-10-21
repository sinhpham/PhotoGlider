using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using PhotoGliderPCL.Models;
using PhotoGliderPCL.ViewModels;
using System.Net.Http;
using System.Collections.ObjectModel;

namespace PhotoGliderPCL
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            BindingContext = AppPCL.MainVM;

            Task.Run(async () =>
            {
                var rl = await LoadRedditImages();
                Device.BeginInvokeOnMainThread(() =>
                {
                    VM.Images = new PCollection<RedditImage>();
                    foreach (var img in rl)
                    {
                        VM.Images.Add(img);
                    }
                });
            });
        }

        public MainVM VM { get { return (MainVM)BindingContext; } }

        string nextPath = null;
        int count = 30;

        async Task<IEnumerable<RedditImage>> LoadRedditImages()
        {
            var retList = new List<RedditImage>();
            if (string.Equals("null", nextPath, StringComparison.Ordinal))
            {
                return retList;
            }

            var subRedditLinkPortion = VM.SubReddit.StartsWith("user/") ? VM.SubReddit : string.Format("r/{0}", VM.SubReddit);



            var link = nextPath != null ?
                string.Format("http://www.reddit.com/{0}/{1}.json?after={2}&limit={3}", subRedditLinkPortion, VM.SortBy.ToString().ToLower(), nextPath, count) :
                string.Format("http://www.reddit.com/{0}/{1}.json?limit={2}", subRedditLinkPortion, VM.SortBy.ToString().ToLower(), count);

            var hc = new HttpClient();
            string jsonText = null;
            try
            {
                jsonText = await hc.GetStringAsync(link);
            }
            catch (Exception)
            {

            }

            if (jsonText != null)
            {
                string newNextPath;
                retList = RedditImageParser.ParseFromJson(jsonText, out newNextPath);
                //collection.NextPath = newNextPath;
            }

            return retList;
        }
    }

    public class PCollection<T> : ObservableCollection<T>, IPaginatedCollection<T>
    {

    }
}

