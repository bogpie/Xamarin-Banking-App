using Banking312.DatabaseModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Banking312
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Login_Button_Clicked(object sender, EventArgs e) // async??
        {

            bool isUserEmpty = string.IsNullOrEmpty(userEntry.Text);
            bool isPasswordEmpty = string.IsNullOrEmpty(passEntry.Text);


            if (isUserEmpty || isPasswordEmpty)
            {

                if (isUserEmpty)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert("Alert", "user is empty !", "OK");
                    });
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert("Alert", "Password is empty !", "OK");
                    });
                }

            }
            else
            {

                if (passEntry.Text.Length < 8)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert("Alert", "Invalid password format !", "OK");
                    });
                }
                else
                {
                    Client triedClient = new Client()
                    {
                        User = userEntry.Text,
                        Pass = passEntry.Text
                    };

                    SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation);
                    conn.CreateTable<Client>();

                    bool foundClient = false;

                    int nrPrimaryKeys = conn.Query<Client>("select * from Client").Count;
                    int idPrimaryKey = 1;
                    for (idPrimaryKey = 1; idPrimaryKey <= nrPrimaryKeys; ++idPrimaryKey)
                    {
                        var existingClient = conn.Get<Client>(idPrimaryKey);
                        if (existingClient.User == triedClient.User)
                        {
                            foundClient = true;
                            if (existingClient.Pass == triedClient.Pass)
                            {
                                await DisplayAlert("Success", "Client Logged In", "OK");
                                App.idClientStatic = idPrimaryKey;
                                App.clientStatic = conn.Get<Client>(App.idClientStatic);
                                await Navigation.PushAsync(new TabPage());
                            }
                            else
                            {
                                await DisplayAlert("Wrong password", "Username exists, but password is wrong", "OK");
                            }
                            break;
                        }

                    }
                    if (!foundClient)
                    {
                        bool answer = await DisplayAlert("Sign up", "No account found. Would you like to sign up?", "Yes", "No");
                        if (answer == true)
                        {
                            triedClient.RON = 1000;
                            triedClient.EUR = 1000;
                            triedClient.USD = 1000;
                            int rows = conn.Insert(triedClient);
                            if (rows > 0)
                            {
                                await DisplayAlert("Success", "Client Signed Up", "OK");
                                App.idClientStatic = nrPrimaryKeys + 1;
                                App.clientStatic = conn.Get<Client>(App.idClientStatic);
                                await Navigation.PushAsync(new TabPage());

                            }
                            else
                            {
                                await DisplayAlert("Failure", "Failure", "OK");
                            }
                        }
                    }
                    conn.Close();

                }

            }





        }
    }
}
