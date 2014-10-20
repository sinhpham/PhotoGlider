using Newtonsoft.Json;
using PhotoGliderPCL.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhotoGliderPCL.ViewModels
{
    public class MainVM : NotifyingClass
    {
        public MainVM()
        {
            var subList = JsonConvert.DeserializeObject<List<string>>(AppPCL.SettingVM.SubList);

            foreach (var sub in subList)
            {
                SubReddits.Add(sub);
            }
            SubReddits.CollectionChanged += (s, arg) =>
            {
                AppPCL.SettingVM.SubList = JsonConvert.SerializeObject(SubReddits.ToList());
            };

            if (SubReddits.Count > 0)
            {
                SubReddit = SubReddits[0];
            }

            LoadRedditImages(Images, 100);
        }

        private ObservableCollection<RedditImage> _images;
        public ObservableCollection<RedditImage> Images
        {
            get { return this._images; }
            set { SetProperty(ref _images, value); }
        }

        RedditImage _selectedImg;
        public RedditImage SelectedItem
        {
            get { return _selectedImg; }
            set { SetProperty(ref _selectedImg, value); }
        }

        Command _copyLinkCmd;
        public Command CopyLinkCmd
        {
            get
            {
                if (_copyLinkCmd == null)
                {
                    _copyLinkCmd = new Command(() =>
                    {
                        //var dp = new Windows.ApplicationModel.DataTransfer.DataPackage();
                        //dp.SetText(SelectedItem.OriginalUrl);
                        //Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dp);
                    });
                }
                return _copyLinkCmd;
            }
        }

        Command _copyRedditLinkCmd;
        public Command CopyRedditLinkCmd
        {
            get
            {
                if (_copyRedditLinkCmd == null)
                {
                    _copyRedditLinkCmd = new Command(() =>
                    {
                        //var dp = new Windows.ApplicationModel.DataTransfer.DataPackage();
                        //dp.SetText(SelectedItem.Permalink);
                        //Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dp);
                    });
                }
                return _copyRedditLinkCmd;
            }
        }

        Command _openLinkCmd;
        public Command OpenLinkCmd
        {
            get
            {
                if (_openLinkCmd == null)
                {
                    _openLinkCmd = new Command(() =>
                    {
                        //Windows.System.Launcher.LaunchUriAsync(new Uri(SelectedItem.OriginalUrl));
                    });
                }
                return _openLinkCmd;
            }
        }

        Command _openRedditLinkCmd;
        public Command OpenRedditLinkCmd
        {
            get
            {
                if (_openRedditLinkCmd == null)
                {
                    _openRedditLinkCmd = new Command(() =>
                    {
                        //Windows.System.Launcher.LaunchUriAsync(new Uri(SelectedItem.Permalink));
                    });
                }
                return _openRedditLinkCmd;
            }
        }

        Command _refreshCmd;
        public Command RefreshCmd
        {
            get
            {
                if (_refreshCmd == null)
                {
                    _refreshCmd = new Command(() =>
                    {
                        Images = new ObservableCollection<RedditImage>();
                    });
                }
                return _refreshCmd;
            }
        }

        public bool IsInFav
        {
            get { return SubReddits.Contains(SubReddit); }
            set
            {
                if (value)
                {
                    // Add to fav.
                    if (!SubReddits.Contains(SubReddit))
                    {
                        SubReddits.Add(SubReddit);
                    }
                }
                else
                {
                    // Remove from fav.
                    SubReddits.Remove(SubReddit);
                }
                OnPropertyChanged(() => IsInFav);
            }
        }

        string _subReddit;
        public string SubReddit
        {
            get { return _subReddit; }
            set
            {
                if (SetProperty(ref _subReddit, value))
                {
                    RefreshCmd.Execute(null);
                    OnPropertyChanged(() => IsInFav);
                }
            }
        }

        ObservableCollection<string> _subReddits = new ObservableCollection<string>();
        public ObservableCollection<string> SubReddits
        {
            get { return _subReddits; }
        }


        public event EventHandler NeedToGoBack;
        public void OnNeedToGoBack()
        {
            var eh = NeedToGoBack;
            if (eh != null)
            {
                eh(this, EventArgs.Empty);
            }
        }

        async Task<IEnumerable<RedditImage>> LoadRedditImages(ObservableCollection<RedditImage> collection, uint count)
        {
            var retList = new List<RedditImage>();
            //if (string.Equals("null", collection.NextPath, StringComparison.Ordinal))
            //{
            //    return retList;
            //}

            var link = //false ?
                //string.Format("http://www.reddit.com/r/{0}/new.json?after={1}&limit={2}", SubReddit, collection.NextPath, count) :
                string.Format("http://www.reddit.com/r/{0}/new.json?limit={1}", SubReddit, count);

            var hc = new HttpClient();
            var jsonText = await hc.GetStringAsync(link);

            string newNextPath;
            retList = RedditImageParser.ParseFromJson(jsonText, out newNextPath);
            //collection.NextPath = newNextPath;

            foreach (var ri in retList)
            {
                collection.Add(ri);
            }

            return retList;
        }
    }

    //public class PaginatedCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading
    //{
    //    private Func<PaginatedCollection<T>, uint, Task<IEnumerable<T>>> load;
    //    public bool HasMoreItems { get; private set; }

    //    public string NextPath { get; set; }

    //    public PaginatedCollection(Func<PaginatedCollection<T>, uint, Task<IEnumerable<T>>> load)
    //    {
    //        HasMoreItems = true;
    //        this.load = load;
    //    }

    //    public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
    //    {
    //        return AsyncInfo.Run(async c =>
    //        {
    //            var data = await load(this, count);

    //            foreach (var item in data)
    //            {
    //                Add(item);
    //            }

    //            HasMoreItems = data.Any();

    //            return new LoadMoreItemsResult()
    //            {
    //                Count = (uint)data.Count(),
    //            };
    //        });
    //    }
    //}
}
