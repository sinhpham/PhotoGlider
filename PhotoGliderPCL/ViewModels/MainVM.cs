﻿using Newtonsoft.Json;
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
        }

        string _redditNextPath;
        public string RedditNextPath {
            get { return _redditNextPath; }
            set { SetProperty(ref _redditNextPath, value); }
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
                        AppPCL.Container.GetInstance<IOSFeatures>().CopyToClipboard(SelectedItem.OriginalUrl);
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
                        AppPCL.Container.GetInstance<IOSFeatures>().CopyToClipboard(SelectedItem.Permalink);
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
                        AppPCL.Container.GetInstance<IOSFeatures>().OpenLink(SelectedItem.OriginalUrl);
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
                        AppPCL.Container.GetInstance<IOSFeatures>().OpenLink(SelectedItem.Permalink);
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
                        if (Images != null)
                        {
                            Images = AppPCL.Container.GetInstance<ObservableCollection<RedditImage>>();
                            RedditNextPath = null;
                        }
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

        public IEnumerable<RedditSortBy> SortByList
        {
            get { return Enum.GetValues(typeof(RedditSortBy)).Cast<RedditSortBy>(); }
        }

        RedditSortBy _sortBy;
        public RedditSortBy SortBy
        {
            get { return _sortBy; }
            set
            {
                if (SetProperty(ref _sortBy, value))
                {
                    ShouldDisplaySortByTime = SortBy == RedditSortBy.Top || SortBy == RedditSortBy.Controversial ? true : false;

                    RefreshCmd.Execute(null);
                }
            }
        }

        bool _shouldDisplaySortByTime;
        public bool ShouldDisplaySortByTime
        {
            get { return _shouldDisplaySortByTime; }
            set { SetProperty(ref _shouldDisplaySortByTime, value); }
        }

        public IEnumerable<RedditSortByTime> SortByTimeList
        {
            get { return Enum.GetValues(typeof(RedditSortByTime)).Cast<RedditSortByTime>(); }
        }

        RedditSortByTime _sortByTime;
        public RedditSortByTime SortByTime
        {
            get { return _sortByTime; }
            set
            {
                if (SetProperty(ref _sortByTime, value))
                {
                    RefreshCmd.Execute(null);
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

        public enum RedditSortBy
        {
            Hot,
            New,
            Top,
            Controversial,
        }

        public enum RedditSortByTime
        {
            hour,
            day,
            week,
            month,
            year,
            all
        }
    }

    public interface IOSFeatures
    {
        void OpenLink(string uriString);
        void CopyToClipboard(string str);
    }
}
