﻿using System.Collections.Generic;
using System;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using Rocket.API;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Teyhota.CustomKits.Commands
{
    public class Command_ClearInventory : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "clearinventory";

        public string Help => "Clear somebody's inventory";

        public string Syntax => "/ci [player]";

        public List<string> Aliases => new List<string> { "ci" };

        public List<string> Permissions => new List<string> { "ck.clearinventory" };

        public const string OTHER_PERM = "ck.clearinventory.other";


        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                if (caller is ConsolePlayer)
                {
                    Logger.LogError($"Incorrect command usage! Try: {Syntax}");
                    return;
                }

                UnturnedPlayer callr = (UnturnedPlayer)caller;

                InventoryManager.Clear(callr, true);

                UnturnedChat.Say(caller, Plugin.CustomKitsPlugin.Instance.Translate("inventory_cleared"));
            }

            if (command.Length == 1)
            {
                UnturnedPlayer toPlayer = UnturnedPlayer.FromName(command[0]);
                
                if (toPlayer.IsAdmin || toPlayer.HasPermission("ck.admin"))
                {
                    UnturnedChat.Say(caller, Plugin.CustomKitsPlugin.Instance.Translate("ci_bypass", toPlayer.CharacterName), Color.red);
                    return;
                }

                if (caller is ConsolePlayer)
                {
                    if (toPlayer == null)
                    {
                        Logger.Log(Plugin.CustomKitsPlugin.Instance.Translate("player_doesn't_exist", command[0]), ConsoleColor.Red);
                        return;
                    }

                    InventoryManager.Clear(toPlayer, true);

                    Logger.Log(Plugin.CustomKitsPlugin.Instance.Translate("inventory_cleared_other", toPlayer.CharacterName), ConsoleColor.Cyan);
                    return;
                }

                if (caller.HasPermission(OTHER_PERM))
                {
                    if (toPlayer == null)
                    {
                        UnturnedChat.Say(caller, Plugin.CustomKitsPlugin.Instance.Translate("player_doesn't_exist", command[0]), Color.red);
                        return;
                    }

                    InventoryManager.Clear(toPlayer, true);

                    UnturnedChat.Say(toPlayer, Plugin.CustomKitsPlugin.Instance.Translate("inventory_cleared"));
                    UnturnedChat.Say(caller, Plugin.CustomKitsPlugin.Instance.Translate("inventory_cleared_other", toPlayer.CharacterName));
                }
                else
                {
                    UnturnedChat.Say(caller, "You do not have permissions to execute this command.", Color.red);
                }
            }
        }
    }
}