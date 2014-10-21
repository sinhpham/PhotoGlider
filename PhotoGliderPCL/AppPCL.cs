using PhotoGliderPCL.ViewModels;
using SimpleInjector;
using System;
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

        public static Page GetMainPage()
        {	
            return new MainPage();
        }

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
}

