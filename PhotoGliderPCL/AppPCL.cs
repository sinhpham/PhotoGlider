using PhotoGliderPCL.Models;
using PhotoGliderPCL.ViewModels;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhotoGliderPCL
{
    public class AppPCL
    {
        static AppPCL()
        {
            Container = new Container();
        }

        public static Container Container { get; set; }

        static MainVM _mainVM;
        public static MainVM MainVM
        {
            get
            {
                if (_mainVM == null)
                {
                    _mainVM = new MainVM();
                }
                return _mainVM;
            }
        }

        static SettingsVM _settingsVM;
        public static SettingsVM SettingVM
        {
            get
            {
                if (_settingsVM == null)
                {
                    _settingsVM = new SettingsVM();
                }
                return _settingsVM;
            }
        }
    }

    public class NetworkManager
    {
        class SubRedditInfor
        {
            public string Name { get; set; }
            public string SortBy { get; set; }
            public string SortByTime { get; set; }
            public string CurrNextPath { get; set; }
        }

        public static async Task<Tuple<IEnumerable<RedditImage>, string>> LoadRedditImages(uint count)
        {
            var infor = new NetworkManager.SubRedditInfor
            {
                Name = AppPCL.MainVM.SubReddit,
                SortBy = AppPCL.MainVM.SortBy.ToString(),
                SortByTime = AppPCL.MainVM.SortByTime.ToString(),
                CurrNextPath = AppPCL.MainVM.RedditNextPath,
            };

            if (AppPCL.MainVM.SortBy == MainVM.RedditSortBy.New || AppPCL.MainVM.SortBy == MainVM.RedditSortBy.Hot)
            {
                infor.SortByTime = null;
            }

            var retList = new List<RedditImage>();
            var newNextPath = "";

            if (string.Equals("null", infor.CurrNextPath, StringComparison.Ordinal))
            {
                return Tuple.Create((IEnumerable<RedditImage>)retList, "null");
            }

            var subRedditLinkPortion = infor.Name.StartsWith("user/") ? infor.Name : string.Format("r/{0}", infor.Name);

            var link = infor.CurrNextPath != null ?
                string.Format("http://www.reddit.com/{0}/{1}.json?after={2}&limit={3}", subRedditLinkPortion, infor.SortBy.ToString().ToLower(), infor.CurrNextPath, count) :
                string.Format("http://www.reddit.com/{0}/{1}.json?limit={2}", subRedditLinkPortion, infor.SortBy.ToString().ToLower(), count);

            if (infor.SortByTime != null)
            {
                link = link + string.Format("&t={0}", infor.SortByTime);
            }

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
                retList = RedditImageParser.ParseFromJson(jsonText, out newNextPath);
            }

            return Tuple.Create((IEnumerable<RedditImage>)retList, newNextPath);
        }
    }
}

