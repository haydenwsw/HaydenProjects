using CarSearcher.Models;
using CarSearcher.Scrapers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http;

namespace CarSearcher
{
    /// <summary>
    /// class for storing all the car makes and models
    /// </summary>
    public class CarLookup
    {
        public readonly Dictionary<string, (CarKeys, Dictionary<string, CarKeys>)> MakeModel;

        private readonly CarSearcherConfig CarSearcherConfig;

        public CarLookup(IOptions<CarSearcherConfig> carsearcherconfig, IHttpClientFactory httpclientfactory)
        {
            CarSearcherConfig = carsearcherconfig.Value;

            if (!File.Exists(CarSearcherConfig.GetMakeModelPath))
                // very long function call make sure i don't run it accidentally
                if (CarSearcherConfig.GenerateMakeModelFile)
                    GenerateMakeModel(httpclientfactory.CreateClient());
                else
                    throw new FileNotFoundException("MakeModel.json is missing and GenerateMakeModelFile is set to false");

            string json = File.ReadAllText(CarSearcherConfig.GetMakeModelPath);
            MakeModel = JsonConvert.DeserializeObject<Dictionary<string, (CarKeys, Dictionary<string, CarKeys>)>>(json);
        }

        /// <summary>
        /// scrapes all the makes and models from all 3 sites then combinds them in a mega list
        /// </summary>
        /// <param name="carsales"></param>
        /// <param name="fbmarketplace"></param>
        /// <param name="gumtree"></param>
        /// <returns></returns>
        private void GenerateMakeModel(HttpClient httpClient)
        {
            string json;

            var carsalesMakeModel = new Dictionary<string, (string, List<(string, string)>)>();
            if (File.Exists($"{CarSearcherConfig.FolderPath}/carsale.json"))
            {
                json = File.ReadAllText($"{CarSearcherConfig.FolderPath}/carsale.json");
                carsalesMakeModel = JsonConvert.DeserializeObject<Dictionary<string, (string, List<(string, string)>)>>(json);
            }
            else
            {
                Carsales carsales = new Carsales(httpClient, this, CarSearcherConfig);
                carsales.GetMakeModel(carsalesMakeModel).Wait();
                // i manually removed the "S Type" entrie in "Jaguar" in carsale.json because it conflicted with the "S-Type" entry
                carsalesMakeModel["Jaguar"].Item2.Remove(("Model.S-Type", "S-Type"));
                json = JsonConvert.SerializeObject(carsalesMakeModel, Formatting.Indented);
                File.WriteAllText($"{CarSearcherConfig.FolderPath}/carsale.json", json);
            }

            var fbmarketplaceMakeModel = new Dictionary<string, (string, List<(string, string)>)>();
            if (File.Exists($"{CarSearcherConfig.FolderPath}/fbmarketplace.json"))
            {
                json = File.ReadAllText($"{CarSearcherConfig.FolderPath}/fbmarketplace.json");
                fbmarketplaceMakeModel = JsonConvert.DeserializeObject<Dictionary<string, (string, List<(string, string)>)>>(json);
            }
            else
            {
                FbMarketplace fbmarketplace = new FbMarketplace(httpClient, this, CarSearcherConfig);
                fbmarketplace.GetMakeModel(fbmarketplaceMakeModel).Wait();
                json = JsonConvert.SerializeObject(fbmarketplaceMakeModel, Formatting.Indented);
                File.WriteAllText($"{CarSearcherConfig.FolderPath}/fbmarketplace.json", json);
            }

            var gumtreeMakeModel = new Dictionary<string, (string, List<(string, string)>)>();
            if (File.Exists($"{CarSearcherConfig.FolderPath}/gumtree.json"))
            {
                json = File.ReadAllText($"{CarSearcherConfig.FolderPath}/gumtree.json");
                gumtreeMakeModel = JsonConvert.DeserializeObject<Dictionary<string, (string, List<(string, string)>)>>(json);
            }
            else
            {
                Gumtree gumtree = new Gumtree(httpClient, this, CarSearcherConfig);
                gumtree.GetMakeModel(gumtreeMakeModel).Wait();
                json = JsonConvert.SerializeObject(gumtreeMakeModel, Formatting.Indented);
                File.WriteAllText($"{CarSearcherConfig.FolderPath}/gumtree.json", json);
            }

            var countDictionary = new Dictionary<string, SuperValue>();

            // add all makes
            countDictionary.Add("All Makes", new SuperValue("All Makes", true, 3));
            countDictionary["All Makes"].CarKeys.FbMaketplaceKey = "807311116002614";
            countDictionary["All Makes"].ModelDict.Add("All Models", new SuperValue("All Models", false, 3));

            foreach (var make in carsalesMakeModel)
            {
                string displayMake = MakeEdgeCases(make.Key);
                string key = CleanKey(displayMake);
                if (!countDictionary.TryAdd(key, new SuperValue(displayMake, true)))
                    countDictionary[key].Count++;

                countDictionary[key].CarKeys.CarsalesKey = make.Value.Item1;
                countDictionary[key].ModelDict.TryAdd("All Models", new SuperValue("All Models", false, 3));
                foreach ((string, string) model in make.Value.Item2)
                {
                    string displayModel = ModelEdgeCases(model.Item2);
                    string key2 = CleanKey(displayModel);
                    if (!countDictionary[key].ModelDict.TryAdd(key2, new SuperValue(displayModel, false)))
                        countDictionary[key].ModelDict[key2].Count++;

                    countDictionary[key].ModelDict[key2].CarKeys.CarsalesKey = model.Item1;
                }
            }

            foreach (var make in fbmarketplaceMakeModel)
            {
                string displayMake = MakeEdgeCases(make.Key);
                string key = CleanKey(displayMake);
                if (!countDictionary.TryAdd(key, new SuperValue(displayMake, true)))
                    countDictionary[key].Count++;

                countDictionary[key].CarKeys.FbMaketplaceKey = make.Value.Item1;
                countDictionary[key].ModelDict.TryAdd("All Models", new SuperValue("All Models", false, 3));
                foreach ((string, string) model in make.Value.Item2)
                {
                    string displayModel = ModelEdgeCases(model.Item2);
                    string key2 = CleanKey(displayModel);
                    if (!countDictionary[key].ModelDict.TryAdd(key2, new SuperValue(displayModel, false)))
                        countDictionary[key].ModelDict[key2].Count++;

                    countDictionary[key].ModelDict[key2].CarKeys.FbMaketplaceKey = model.Item1;
                }
            }

            foreach (var make in gumtreeMakeModel)
            {
                string displayMake = MakeEdgeCases(make.Key);
                string key = CleanKey(displayMake);
                if (!countDictionary.TryAdd(key, new SuperValue(displayMake, true)))
                    countDictionary[key].Count++;

                countDictionary[key].CarKeys.GumtreeKey = make.Value.Item1;
                countDictionary[key].ModelDict.TryAdd("All Models", new SuperValue("All Models", false, 3));
                foreach ((string, string) model in make.Value.Item2)
                {
                    string displayModel = ModelEdgeCases(model.Item2);
                    string key2 = CleanKey(displayModel);
                    if (!countDictionary[key].ModelDict.TryAdd(key2, new SuperValue(displayModel, false)))
                        countDictionary[key].ModelDict[key2].Count++;

                    countDictionary[key].ModelDict[key2].CarKeys.GumtreeKey = model.Item1;
                }
            }

            // throw if they're more then 3 makes
            if (countDictionary.Any(x => x.Value.Count > 3))
                throw new IndexOutOfRangeException("They're more than 3 makes!");

            // throw if they're more then 3 models
            if (countDictionary.SelectMany(x => x.Value.ModelDict).Any(y => y.Value.Count > 3))
                throw new IndexOutOfRangeException("They're more than 3 models!");

            // combind into mega dictionary
            var masterDictionary = countDictionary.Where(x => x.Value.Count == 3)
                .ToDictionary(x => x.Value.DisplayName, x => (x.Value.CarKeys, x.Value.ModelDict.Where(y => y.Value.Count == 3)
                .ToDictionary(y => y.Value.DisplayName, y => y.Value.CarKeys)));

            int makemax = masterDictionary.Max(x => x.Key.Length);
            int modelmax = masterDictionary.SelectMany(x => x.Value.Item2).Max(x => x.Key.Length);

            // save to json
            json = JsonConvert.SerializeObject(masterDictionary, Formatting.Indented);
            File.WriteAllText(CarSearcherConfig.GetMakeModelPath, json);
        }

        // manually sort out the edge cases for make
        private string MakeEdgeCases(string key)
        {
            switch (key)
            {
                case "HSV":
                    return "Holden Special Vehicles";
                case "BMW Alpina":
                    return "Alpina";
                case "Alpine-Renault":
                    return "Alpine";
                case "INFINITI":
                    return "Infiniti";
                case "MINI":
                    return "Mini";
                case "smart":
                    return "Smart";
                case "SKODA":
                    return "Skoda";
                default:
                    return key;
            }
        }

        // manually sort out the edge cases for model
        private string ModelEdgeCases(string key)
        {
            switch (key)
            {
                case "124 Spider":
                    return "124";
                case "Consul - Cortina":
                    return "Consul";
                case "Avensis Verso":
                    return "Avensis";
                case "pro_cee'd":
                    return "Pro Ceed";
                case "Pro cee`d":
                    return "Pro Ceed";
                default:
                    return key;
            }
        }

        private string CleanKey(string key)
        {
            return key.ToLower().Replace(" ", "").Replace("-", "");
        }

        private class SuperValue
        {
            public string DisplayName;
            public int Count;
            public CarKeys CarKeys;
            public Dictionary<string, SuperValue> ModelDict;

            public SuperValue (string displayname, bool creatDictionary)
            {
                DisplayName = displayname;
                Count = 1;
                CarKeys = new CarKeys();
                ModelDict = creatDictionary ? new Dictionary<string, SuperValue>() : null;
            }

            public SuperValue(string displayname, bool creatDictionary, int count)
            {
                DisplayName = displayname;
                Count = count;
                CarKeys = new CarKeys();
                ModelDict = creatDictionary ? new Dictionary<string, SuperValue>() : null;
            }
        } 
    }
}
