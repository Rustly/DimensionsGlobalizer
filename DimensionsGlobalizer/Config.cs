using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionsGlobalizer
{
    public class Config
    {
        private static readonly string _path = Path.Combine(TShockAPI.TShock.SavePath, "realmsclient.json");

        public static Config Read()
        {
            try
            {
                var res = new Config();
                if (File.Exists(_path))
                    res = JsonConvert.DeserializeObject<Config>(File.ReadAllText(_path));
                File.WriteAllText(_path, JsonConvert.SerializeObject(res, Formatting.Indented));
                return res;
            }
            catch (Exception ex)
            {
                TShockAPI.TShock.Log.Error(ex.ToString());
            }

            return null;
        }

        public bool EnableItemBanGlobalization { get; set; } = true;

        public bool EnableProjectileBanGlobalization { get; set; } = true;

        public bool EnableTileBanGlobalization { get; set; } = true;

        public bool EnableGroupGlobalization { get; set; } = true;

        public bool EnableBanGlobalization { get; set; } = true;

        public bool EnableUserGlobalization { get; set; } = true;

        public string GlobalDatabaseConnection { get; set; } = "Server=localhost; Port=3306; Database=global; Uid=username; Pwd=assword;";
    }
}
