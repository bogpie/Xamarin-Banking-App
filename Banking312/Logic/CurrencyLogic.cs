using Banking312.DatabaseModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Banking312.Logic
{
    public class CurrencyLogic
    {
        public static async Task<Rates> GetRates(string currencyType)
        {
            Rates rates = new Rates();
            string url = CurrencyRoot.GenerateURL(currencyType);
            using (HttpClient client = new HttpClient())
            {
                //var response = client.GetAsync(url).Result;

                var response = client.GetAsync(url).Result;
                var json = response.Content.ReadAsStringAsync().Result;

                //var json = await response.Content.ReadAsStringAsync();

                CurrencyRoot myDeserializedClass = JsonConvert.DeserializeObject<CurrencyRoot>(json);
                rates = myDeserializedClass.rates as Rates;
               
            }
            
            return rates;
        }

        /* public static double ConvertCurrencies(string source,string destination)
          {
              var ratesTask = GetRates(source);
              Rates rates = ratesTask as Rates;

              if(destination == "USD")
              {
                  return rates.USD;
              }

              else if (destination == "RON")
              {
                  return rates.RON;
              }
              else if (destination == "EUR")
              {
                  return rates.EUR;
              }
          }
            */


        public static double GetRate(string sell,string buy)
          {
              var ratesTask = GetRates(sell);
              var rates = ratesTask.Result;

              Type type = rates.GetType(); //typeof(Rates)
              var info = type.GetProperty(buy);
              var rate = (double)info.GetValue(rates);
                // rates.'buy'

              return rate;

          }



    }
}
