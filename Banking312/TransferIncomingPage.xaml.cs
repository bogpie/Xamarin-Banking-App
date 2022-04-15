using Banking312.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using System.Runtime.Serialization;

namespace Banking312
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TransferIncomingPage : ContentPage
    {
        public static Client receiverClientStatic;
        public TransferIncomingPage(Client receiverClient)
        {
            InitializeComponent();
            receiverClientStatic = receiverClient;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Title = string.Format("Transfer to {0}", receiverClientStatic.User);
            TransferDetailsLabel.Text = string.Format("You own {0} RON", App.clientStatic.RON);

        }

        private async void TransferButton_Clicked(object sender, EventArgs e)
        {
            if (AmountEntry.Text==null || AmountEntry.Text==string.Empty)
            {
                await DisplayAlert("Error", "Invalid format!", "OK");
            }
            else
            {
                App.clientStatic.totalSent += double.Parse(AmountEntry.Text);
                receiverClientStatic.totalReceived += double.Parse(AmountEntry.Text);

                double payment = double.Parse(AmountEntry.Text);

                if (payment == 0)
                {
                    await DisplayAlert("Error", "Sum cannot be 0", "OK");

                }
                else if (payment > App.clientStatic.RON)
                {
                    await DisplayAlert("Failure", "Not enough funds", "OK");
                }
                else
                {
                    App.clientStatic.RON -= payment;
                    receiverClientStatic.RON += payment;
                    SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation);
                    conn.CreateTable<Client>();
                    conn.Update(App.clientStatic);
                    conn.Update(receiverClientStatic);
                    conn.Close();
                    await DisplayAlert("Transfer", "Transfer completed!", "OK");
                    await Navigation.PopAsync();
                }
            }

        }
    }
}