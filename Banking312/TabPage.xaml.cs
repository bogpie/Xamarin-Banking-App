using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace Banking312
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class TabPage : TabbedPage
    {
        public static int counterPress = 0;

        public TabPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            //string toolbarText = "{0} , {1} RON";
            //toolbarItem.Text = string.Format(toolbarText, App.clientStatic.User, App.clientStatic.RON);

            toolbarItem.Text = App.clientStatic.User;
        }

        private async void toolbarItem_Clicked(object sender, EventArgs e)
        {
            ++counterPress;
            if(counterPress==5)
            {
                await Browser.OpenAsync("https://simmer.io/@AlexK47/~b33e517b-2c10-5917-b2ab-b68b458b42e8", BrowserLaunchMode.SystemPreferred);
                counterPress = 0;
            }

        }
    }
}