using Banking312.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using Banking312;
using Banking312.DatabaseModel;

namespace Banking312
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExchangePage : ContentPage
    {

        public static string sellingString = string.Empty;
        public static string buyingString = string.Empty;
        public static double rate = 0;

        public ExchangePage()
        {
            InitializeComponent();
        }

        private async void SellingPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            //var sellingIndex = picker.SelectedIndex;
            sellingString = picker.SelectedItem.ToString();


            if (sellingString == "Other...")
            {
                var result = await DisplayPromptAsync("Currency", "What would you like to sell?");

                if (result == null)
                {
                    SellingPicker.SelectedIndex = 0;
                }
                else if (result.Length != 3)
                {
                    await DisplayAlert("Invalid", "Invalid currency format!", "OK");
                }
                else
                {
                    sellingString = result;
                    SellingPicker.Items.Insert(SellingPicker.Items.Count - 1, sellingString);
                    SellingPicker.SelectedIndex = SellingPicker.Items.Count - 2;
                }

            }

        }

        private async void BuyingPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            //var sellingIndex = picker.SelectedIndex;
            buyingString = picker.SelectedItem.ToString();
            if (buyingString == "Other...")
            {
                var result = await DisplayPromptAsync("Currency", "What would you like to buy?");
                if (result == null)
                {
                    BuyingPicker.SelectedIndex = 0;
                }
                else if (result.Length != 3)
                {
                    await DisplayAlert("Invalid", "Invalid currency format!", "OK");
                }
                else
                {
                    buyingString = result;
                    BuyingPicker.Items.Insert(BuyingPicker.Items.Count - 1, buyingString);
                    BuyingPicker.SelectedIndex = BuyingPicker.Items.Count - 2;
                }

            }

        }

        private async void CalculateRateButton_Clicked(object sender, EventArgs e)
        {
            if (sellingString.Length == 3 && buyingString.Length == 3)
            {
                rate = CurrencyLogic.GetRate(sellingString, buyingString);
                if(rate>0)
                {
                    string promptText = string.Format("1 {0} for {1} {2}. How much {0} would you like to sell?", sellingString, rate, buyingString);
                    var result = await DisplayPromptAsync("Rate", promptText, "OK");
                    if (result != null)
                    {

                        Type type = App.clientStatic.GetType(); //typeof(Rates)
                        var info = type.GetProperty(sellingString);
                        var oldSelled = (double)info.GetValue(App.clientStatic);
                        var differenceSelled = double.Parse(result);

                        //Type type = App.clientStatic.GetType(); //typeof(Rates)
                        info = type.GetProperty(buyingString);
                        var oldBought = (double)info.GetValue(App.clientStatic);
                        var addBought = rate * differenceSelled;

                        if (oldSelled - differenceSelled < 0)
                        {
                            await DisplayAlert("Alert", "Not enough funds!", "OK");
                        }
                        else
                        {
                            promptText = string.Format("You would be left with {0} {1} and you would buy {2} {3}. Continue?", (oldSelled - differenceSelled).ToString(), sellingString, addBought.ToString(), buyingString);
                            bool answer = await DisplayAlert("Transfer", promptText, "Accept", "Decline");
                            if (answer != false)
                            {
                                type = App.clientStatic.GetType(); //typeof(Rates)
                                info = type.GetProperty(sellingString);
                                info.SetValue(App.clientStatic, oldSelled - differenceSelled);

                                //type = App.clientStatic.GetType(); //typeof(Rates)
                                info = type.GetProperty(buyingString);
                                info.SetValue(App.clientStatic, addBought + oldBought);

                                SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation);
                                conn.CreateTable<Client>();
                                conn.Update(App.clientStatic);
                                conn.Close();
                            }
                        }
                    }
                }


            
            }

        }


    }
}