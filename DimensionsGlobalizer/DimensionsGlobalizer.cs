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
    [ApiVersion(2, 1)]
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

            if (config.ServerSpecificPermissionIdentifier is null && config.EnableServerSpecificPermissions)
                throw new Exception("Please provide a permission identifier for server specific permissions in the config");

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
            TShockAPI.Hooks.PlayerHooks.PlayerPermission += PlayerHooks_PlayerPermission;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                TShockAPI.Hooks.GeneralHooks.ReloadEvent -= GeneralHooks_ReloadEvent;
                TShockAPI.Hooks.PlayerHooks.PlayerPermission -= PlayerHooks_PlayerPermission;
            }

            base.Dispose(disposing);
        }

        private void PlayerHooks_PlayerPermission(TShockAPI.Hooks.PlayerPermissionEventArgs args)
        {
            if (config.EnableServerSpecificPermissions)
            {
                if (args.Player.Group.HasPermission(string.Format("{0}.{1}", config.ServerSpecificPermissionIdentifier.ToLowerInvariant(), args.Permission)))
                    args.Result = TShockAPI.Hooks.PermissionHookResult.Granted;
                else if (args.Player.Group.negatedpermissions.Any(s => s == string.Format("{0}.{1}", config.ServerSpecificPermissionIdentifier.ToLowerInvariant(), args.Permission)))
                    args.Result = TShockAPI.Hooks.PermissionHookResult.Denied;
            }
        }

        private void GeneralHooks_ReloadEvent(TShockAPI.Hooks.ReloadEventArgs e)
        {
            config = Config.Read();
        }
    }
}
