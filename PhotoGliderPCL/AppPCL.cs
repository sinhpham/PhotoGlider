﻿using PhotoGliderPCL.ViewModels;
using System;
using Xamarin.Forms;

namespace PhotoGliderPCL
{
    public class AppPCL
    {
        public static Page GetMainPage()
        {	
            return new ContentPage
            { 
                Content = new Label
                {
                    Text = "Hello, Forms!",
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                },
            };
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

