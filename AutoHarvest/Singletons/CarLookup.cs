using AutoHarvest.Models.Json;
using AutoHarvest.Scrapers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Globalization;
using AutoHarvest.Models;

namespace AutoHarvest.Singletons
{
    /// <summary>
    /// class for storing all the car makes and models
    /// </summary>
    public class CarLookup
    {
        public readonly Dictionary<string, (CarKeys, Dictionary<string, CarKeys>)> MakeModel;

        private readonly CarFinder CarFinder;

        public CarLookup(IOptions<CarFinder> carfinder, Carsales carsales, FbMarketplace fbmarketplace, Gumtree gumtree)
        {
            CarFinder = carfinder.Value;

            if (!File.Exists(CarFinder.GetMakeModelPath))
                GenerateMakeModel(carsales, fbmarketplace, gumtree);

            string json = File.ReadAllText(CarFinder.GetMakeModelPath);
            MakeModel = JsonConvert.DeserializeObject<Dictionary<string, (CarKeys, Dictionary<string, CarKeys>)>>(json);
        }

        /// <summary>
        /// scrapes all the makes and models from all 3 sites then combinds them in a mega list
        /// </summary>
        /// <param name="carsales"></param>
        /// <param name="fbmarketplace"></param>
        /// <param name="gumtree"></param>
        /// <returns></returns>
        private void GenerateMakeModel(Carsales carsales, FbMarketplace fbmarketplace, Gumtree gumtree)
        {
            string json;

            var carsalesMakeModel = new Dictionary<string, (string, List<(string, string)>)>();
            if (File.Exists($"./{CarFinder.Folder}/carsale.json"))
            {
                json = File.ReadAllText($"./{CarFinder.Folder}/carsale.json");
                carsalesMakeModel = JsonConvert.DeserializeObject<Dictionary<string, (string, List<(string, string)>)>>(json);
            }
            else
            {
                carsales.GetMakeModel(carsalesMakeModel).Wait();
                // i manually removed the "S Type" entrie in "Jaguar" in carsale.json because it conflicted with the "S-Type" entry
                carsalesMakeModel["Jaguar"].Item2.Remove(("Model.S-Type", "S-Type"));
                json = JsonConvert.SerializeObject(carsalesMakeModel, Formatting.Indented);
                File.WriteAllText($"./{CarFinder.Folder}/carsale.json", json);
            }

            var fbmarketplaceMakeModel = new Dictionary<string, (string, List<(string, string)>)>();
            if (File.Exists($"./{CarFinder.Folder}/fbmarketplace.json"))
            {
                json = File.ReadAllText($"./{CarFinder.Folder}/fbmarketplace.json");
                fbmarketplaceMakeModel = JsonConvert.DeserializeObject<Dictionary<string, (string, List<(string, string)>)>>(json);
            }
            else
            {
                fbmarketplace.GetMakeModel(fbmarketplaceMakeModel).Wait();
                json = JsonConvert.SerializeObject(fbmarketplaceMakeModel, Formatting.Indented);
                File.WriteAllText($"./{CarFinder.Folder}/fbmarketplace.json", json);
            }

            var gumtreeMakeModel = new Dictionary<string, (string, List<(string, string)>)>();
            if (File.Exists($"./{CarFinder.Folder}/gumtree.json"))
            {
                json = File.ReadAllText($"./{CarFinder.Folder}/gumtree.json");
                gumtreeMakeModel = JsonConvert.DeserializeObject<Dictionary<string, (string, List<(string, string)>)>>(json);
            }
            else
            {
                gumtree.GetMakeModel(gumtreeMakeModel).Wait();
                json = JsonConvert.SerializeObject(gumtreeMakeModel, Formatting.Indented);
                File.WriteAllText($"./{CarFinder.Folder}/gumtree.json", json);
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
                throw new Exception("They're more than 3 makes!");

            // throw if they're more then 3 models
            if (countDictionary.SelectMany(x => x.Value.ModelDict).Any(y => y.Value.Count > 3))
                throw new Exception("They're more than 3 models!");

            var masterDictionary = countDictionary.Where(x => x.Value.Count == 3)
                .ToDictionary(x => x.Value.DisplayName, x => (x.Value.CarKeys, x.Value.ModelDict.Where(y => y.Value.Count == 3)
                .ToDictionary(y => y.Value.DisplayName, y => y.Value.CarKeys)));

            int makemax = masterDictionary.Max(x => x.Key.Length);
            int modelmax = masterDictionary.SelectMany(x => x.Value.Item2).Max(x => x.Key.Length);

            json = JsonConvert.SerializeObject(masterDictionary, Formatting.Indented);
            File.WriteAllText(CarFinder.GetMakeModelPath, json);
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
