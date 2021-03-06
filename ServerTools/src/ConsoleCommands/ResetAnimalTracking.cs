﻿using System;
using System.Collections.Generic;


namespace ServerTools
{
    class ResetAnimalTracking : ConsoleCmdAbstract
    {
        public override string GetDescription()
        {
            return "[ServerTools]-Reset a player's animal tracking delay status so they can track another animal.";
        }

        public override string GetHelp()
        {
            return "Usage:\n" +
                   "  1. animaltracking reset <steamId/entityId>\n" +
                   "1. Reset the delay status of a player for the animal tracking command\n";
        }

        public override string[] GetCommands()
        {
            return new string[] { "st-AnimalTracking", "animaltracking", "at" };
        }

        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (_params.Count < 1 || _params.Count > 2)
                {
                    SdtdConsole.Instance.Output(string.Format("Wrong number of arguments, expected 2, found {0}.", _params.Count));
                    return;
                }
                if (_params[0].ToLower().Equals("reset"))
                {
                    if (_params.Count != 2)
                    {
                        SdtdConsole.Instance.Output(string.Format("Wrong number of arguments, expected 2, found {0}.", _params.Count));
                        return;
                    }
                    if (_params[1].Length < 1 || _params[1].Length > 17)
                    {
                        SdtdConsole.Instance.Output(string.Format("Can not reset Id: Invalid Id {0}.", _params[1]));
                        return;
                    }
                    ClientInfo _cInfo = ConsoleHelper.ParseParamIdOrName(_params[1]);
                    Player p = PersistentContainer.Instance.Players[_cInfo.playerId, false];
                    if (p.LastAnimals != null)
                    {
                        PersistentContainer.Instance.Players[_cInfo.playerId, true].LastAnimals = DateTime.Now.AddDays(-2);
                        PersistentContainer.Instance.Save();
                        SdtdConsole.Instance.Output("Animal tracking delay reset.");
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("Player with id {0} does not have a animal tracking delay to reset.", _params[1]));
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in ResetAnimalTracking.Run: {0}.", e));
            }
        }
    }
}
