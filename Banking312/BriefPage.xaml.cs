using Banking312.DatabaseModel;
using Banking312.Logic;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Banking312
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BriefPage : ContentPage
    {
        public BriefPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation);
            conn.CreateTable<Client>();
            var client = conn.Get<Client>(App.idClientStatic);
            WelcomeLabel.Text = client.User;
            // AmountLabel.Text = "You currently have " + client.RON + " money";
            string AmountString = "{0} RON\nEUR {1}\nUSD {2}";
            AmountLabel.Text = string.Format(AmountString, client.RON, client.EUR, client.USD);
            conn.Close();

            TransferReviewLabel.Text = string.Format("Up until now, you have sent {0} RON and received {1} RON", App.clientStatic.totalSent, App.clientStatic.totalReceived);



            if (App.clientStatic.totalSent > App.clientStatic.totalReceived)
            {
                if (App.clientStatic.totalSent - App.clientStatic.totalReceived < 500)
                {
                    TransferAdviceLabel.Text = "You tend to spend a bit more than you receive. Just try to save a bit everyday and you should be fine";
                }
                else
                {
                    TransferAdviceLabel.Text = "Be careful with how you spend your money! Your financial accounts are no joke!";
                }
            }
            else
            {
                if (App.clientStatic.totalReceived - App.clientStatic.totalSent < 500)
                {
                    TransferAdviceLabel.Text = "You're doing pretty well with your income. Keep saving money!";
                }
                else
                {
                    TransferAdviceLabel.Text = "It's great to be financially stable, isn't it?";
                }
            }


        }

        /* private void TransferButton_Clicked(object sender, EventArgs e)
         {
             Navigation.PushAsync(new TransferPage());
         }
        */

        private async void ExchangeButton_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayPromptAsync("Exchange", "What currency would you like to exchange?", maxLength: 3);

            if (result != null)
            {
                var rates = await CurrencyLogic.GetRates(result);
            }


        }

        private async void AccountDetailsButton_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayPromptAsync("Old password", "Enter old password", "Enter");
            if(result!=App.clientStatic.Pass)
            {
                await DisplayAlert("Error", "Wrong password", "OK");
            }
            else
            {
                result = await DisplayPromptAsync("New password", "Enter new password", "Enter");

                if (result.Length < 8)
                {
                    await DisplayAlert("Alert", "Invalid password format!", "OK");
                }
                else
                {
                    App.clientStatic.Pass = result;
                    SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation);
                    conn.CreateTable<Client>();
                    conn.Update(App.clientStatic);
                    conn.Close();
                    await DisplayAlert("Success", "Password changed!", "OK");

                }

            }
        }
    }
}