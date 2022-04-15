using Banking312.DatabaseModel;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Banking312
{
    public partial class App : Application
    {
        public static string DatabaseLocation = string.Empty;
        public static int idClientStatic = -1;
        public static Client clientStatic;

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());
            //MainPage = new MainPage();
        }

        public App(string databaseLocation)
        {
            InitializeComponent();
            //MainPage = new MainPage();
            DatabaseLocation = databaseLocation;
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
