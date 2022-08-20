using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaApi.Server;
using Terraria;
using TShockAPI;

namespace DimensionsGlobalizer
{
    public class DimensionsGlobalizer : TerrariaPlugin
    {
        private Config config;

        public DimensionsGlobalizer(Main game) : base(game)
        {
            config = Config.Read();
        }

        public override void Initialize()
        {
            if (TShock.Config.Settings.StorageType.ToLowerInvariant() != "mysql")
                throw new Exception("Globalization is only allowed if config.storagetype is set to mysql.");

            var db = new MySql.Data.MySqlClient.MySqlConnection(config.GlobalDatabaseConnection);

            if (config.EnableUserGlobalization)
                TShock.UserAccounts = new TShockAPI.DB.UserAccountManager(db);
            if (config.EnableBanGlobalization)
                TShock.Bans = new TShockAPI.DB.BanManager(db);
            if (config.EnableGroupGlobalization)
                TShock.Groups = new TShockAPI.DB.GroupManager(db);
            if (config.EnableProjectileBanGlobalization)
                TShock.ProjectileBans = new TShockAPI.DB.ProjectileManagager(db);
            if (config.EnableTileBanGlobalization)
                TShock.TileBans = new TShockAPI.DB.TileManager(db);
            if (config.EnableItemBanGlobalization)
                TShock.ItemBans.DataModel = new TShockAPI.DB.ItemManager(db);

            TShockAPI.Hooks.GeneralHooks.ReloadEvent += GeneralHooks_ReloadEvent;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                TShockAPI.Hooks.GeneralHooks.ReloadEvent -= GeneralHooks_ReloadEvent;
            }

            base.Dispose(disposing);
        }

        private void GeneralHooks_ReloadEvent(TShockAPI.Hooks.ReloadEventArgs e)
        {
            config = Config.Read();
        }
    }
}
