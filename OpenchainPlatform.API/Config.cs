using OpenchainPlatform.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.API
{
    public class AssetModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public int Precision { get; set; }
    }

    public class Config
    {
        public static List<AssetModel> GetAssets()
        {
            return new List<AssetModel> {
                new AssetModel {
                    Id = "JMD",
                    Name = "Jamaican Dollar",
                    Symbol = "$",
                    Precision = 2
                }
            };
        }
    }
}
