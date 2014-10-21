using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms;
using PhotoGliderPCL;
using Xamarin.Forms.Platform.Android;
using PhotoGliderForms;

namespace PhotoGliderAndroid
{
    [Activity(Label = "PhotoGliderAndroid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : AndroidActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Forms.Init(this, bundle);

            SetPage(AppForms.GetMainPage());
        }
    }
}


