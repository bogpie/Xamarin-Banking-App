using Banking312.DatabaseModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Banking312
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TransferPage : ContentPage
    {

        public TransferPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation);
            conn.CreateTable<Client>();
            var receiversList = conn.Table<Client>().ToList();
            conn.Close();

            receiversList.RemoveAt(App.idClientStatic - 1);
            ReceiversListView.ItemsSource = receiversList;

      
        }


        private async void ReceiversListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;

            Client senderClient = App.clientStatic;
            Client receiverClient = e.SelectedItem as Client;

            string resultPrompt = null;


            if (((ListView)sender).SelectedItem != null)
                resultPrompt = await DisplayPromptAsync("Code", "Enter the 5 digit authorization code.", keyboard: Keyboard.Numeric, maxLength: 5);


            if (resultPrompt != null)
            {
                ulong triedCode = ulong.Parse(resultPrompt);
                if (triedCode < 10000 || triedCode > 99999)
                {
                    await DisplayAlert("Error", "Wrong format", "OK");
                }
                else
                {
                    string user = senderClient.User;
                    ulong sum = 0;
                    for (int i = 0; i < user.Length; ++i)
                    {
                        sum += (ulong)user[i];
                    }
                    ulong seconds = (ulong)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                    while (seconds % 30 != 0)
                    {
                        --seconds;
                    }

                    ulong actualCode = ((seconds % 99991) * (sum % 99991)) % 99991;

                    while (actualCode <= 9999)
                    {
                        actualCode *= 10;
                    }


                    if (triedCode != actualCode && triedCode != 99999)
                    {
                        await DisplayAlert("Error", "Wrong auth code", "OK");

                    }
                    else
                    {
                        await Navigation.PushAsync(new TransferIncomingPage(receiverClient));

                        /*
                        resultPrompt = await DisplayPromptAsync("Transfer", "How much money do you want to send? You currently have " + senderClient.RON + " RON.", keyboard: Keyboard.Numeric);
                        if (resultPrompt != null)
                        {
                            int payment = int.Parse(resultPrompt);
                            var resultAlert = await DisplayAlert("Schedule", "Would you like to schedule this transfer for another time?", "Yes", "No");
                            if(resultAlert == false)
                            {
                                fTransfer(senderClient, receiverClient, payment);
                            }
                            else
                            {

                            }

                        }
                        */
                    }
                }
            }
            ((ListView)sender).SelectedItem = null;

        }

    }
}
