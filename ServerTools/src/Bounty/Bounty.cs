﻿using System.Collections.Generic;
using System.Linq;

namespace ServerTools
{
    class Bounties
    {
        public static bool IsEnabled = false;
        public static int Bounty = 5;

        public static void BountyList(ClientInfo _cInfo, string _playerName)
        {
            List<ClientInfo> _cInfoList = ConnectionManager.Instance.GetClients();
            for (int i = 0; i < _cInfoList.Count; i++)
            {
                ClientInfo _cInfo1 = _cInfoList[i];
                if (_cInfo1 != null)
                {
                    EntityPlayer _player = GameManager.Instance.World.Players.dict[_cInfo1.entityId];
                    int _cost = _player.Level * Bounty;
                    int _currentbounty = PersistentContainer.Instance.Players[_cInfo.playerId, false].Bounty;
                    string _phrase911;
                    if (!Phrases.Dict.TryGetValue(911, out _phrase911))
                    {
                        _phrase911 = "{PlayerName}, # {EntityId}. Current bounty: {CurrentBounty}. Minimum buy in {Cost} {CoinName}.";
                    }
                    _phrase911 = _phrase911.Replace("{PlayerName}", _cInfo1.playerName);
                    _phrase911 = _phrase911.Replace("{EntityId}", _cInfo1.entityId.ToString());
                    _phrase911 = _phrase911.Replace("{CurrentBounty}", _currentbounty.ToString());
                    _phrase911 = _phrase911.Replace("{Cost}", _cost.ToString());
                    _phrase911 = _phrase911.Replace("{CoinName}", Wallet.Coin_Name);
                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1}[-]", Config.Chat_Response_Color, _phrase911), Config.Server_Response_Name, false, "ServerTools", false));
                }
            }
            string _phrase910;
            if (!Phrases.Dict.TryGetValue(910, out _phrase910))
            {
                _phrase910 = "Type /bounty Id# Value or /bounty Id# for the minimum bounty against this player.";
            }
            _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1}[-]", Config.Chat_Response_Color, _phrase910), Config.Server_Response_Name, false, "ServerTools", false));
        }

        public static void NewBounty(ClientInfo _cInfo, string _message, string _playerName)
        {
            int _id;
            string[] _idAndBounty = { };
            int _cost;
            if (_message.Contains(" "))
            {
                _idAndBounty = _message.Split(' ').ToArray();
                if (int.TryParse(_idAndBounty[0], out _id))
                {
                    if (int.TryParse(_idAndBounty[1], out _cost))
                    {
                        ClientInfo _cInfo1 = ConnectionManager.Instance.GetClientInfoForEntityId(_id);
                        if (_cInfo1 != null)
                        {
                            Player p = PersistentContainer.Instance.Players[_cInfo.playerId, false];
                            if (p != null)
                            {
                                EntityPlayer _player = GameManager.Instance.World.Players.dict[_cInfo1.entityId];
                                int _minimum = _player.Level * Bounty;
                                if (_cost < (_player.Level * Bounty))
                                {
                                    _cost = _minimum;
                                }
                                int spentCoins = p.PlayerSpentCoins;
                                int currentCoins = 0;
                                int gameMode = GameManager.Instance.World.GetGameMode();
                                if (gameMode == 7)
                                {
                                    currentCoins = (_player.KilledZombies * Wallet.Zombie_Kills) + (_player.KilledPlayers * Wallet.Player_Kills) - (XUiM_Player.GetDeaths(_player) * Wallet.Deaths) + p.PlayerSpentCoins;
                                    if (!Wallet.Negative_Wallet)
                                    {
                                        if (currentCoins < 0)
                                        {
                                            currentCoins = 0;
                                        }
                                    }
                                }
                                else
                                {
                                    currentCoins = (_player.KilledZombies * Wallet.Zombie_Kills) - (XUiM_Player.GetDeaths(_player) * Wallet.Deaths) + p.PlayerSpentCoins;
                                    if (!Wallet.Negative_Wallet)
                                    {
                                        if (currentCoins < 0)
                                        {
                                            currentCoins = 0;
                                        }
                                    }
                                }
                                if (currentCoins >= _cost)
                                {
                                    int _newCoins = PersistentContainer.Instance.Players[_cInfo.playerId, true].PlayerSpentCoins - _cost;
                                    int _newBounty = PersistentContainer.Instance.Players[_cInfo1.playerId, true].Bounty + _cost;
                                    PersistentContainer.Instance.Players[_cInfo.playerId, true].PlayerSpentCoins = _newCoins;
                                    PersistentContainer.Instance.Players[_cInfo1.playerId, true].Bounty = _newBounty;
                                    PersistentContainer.Instance.Save();
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1} you have added {2} bounty to {3}.[-]", Config.Chat_Response_Color, _playerName, _cost, _cInfo1.playerName), Config.Server_Response_Name, false, "ServerTools", false));
                                }
                                else
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1} .[-]", Config.Chat_Response_Color, _playerName), Config.Server_Response_Name, false, "ServerTools", false));
                                }
                            }
                        }
                    }
                    else
                    {
                        _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1} to add a custom bounty type /bounty Id# Value, or minimum with /bounty Id#.[-]", Config.Chat_Response_Color, _playerName), Config.Server_Response_Name, false, "ServerTools", false));
                    }
                }
            }
            else
            {
                if (int.TryParse(_message, out _id))
                {
                    ClientInfo _cInfo1 = ConnectionManager.Instance.GetClientInfoForEntityId(_id);
                    if (_cInfo1 != null)
                    {
                        Player p = PersistentContainer.Instance.Players[_cInfo.playerId, false];
                        if (p != null)
                        {
                            EntityPlayer _player = GameManager.Instance.World.Players.dict[_cInfo1.entityId];
                            _cost = _player.Level * Bounty;
                            int spentCoins = p.PlayerSpentCoins;
                            int currentCoins = 0;
                            int gameMode = GameManager.Instance.World.GetGameMode();
                            if (gameMode == 7)
                            {
                                currentCoins = (_player.KilledZombies * Wallet.Zombie_Kills) + (_player.KilledPlayers * Wallet.Player_Kills) - (XUiM_Player.GetDeaths(_player) * Wallet.Deaths) + p.PlayerSpentCoins;
                                if (!Wallet.Negative_Wallet)
                                {
                                    if (currentCoins < 0)
                                    {
                                        currentCoins = 0;
                                    }
                                }
                            }
                            else
                            {
                                currentCoins = (_player.KilledZombies * Wallet.Zombie_Kills) - (XUiM_Player.GetDeaths(_player) * Wallet.Deaths) + p.PlayerSpentCoins;
                                if (!Wallet.Negative_Wallet)
                                {
                                    if (currentCoins < 0)
                                    {
                                        currentCoins = 0;
                                    }
                                }
                            }
                            if (currentCoins >= _cost)
                            {
                                int _newCoins = PersistentContainer.Instance.Players[_cInfo.playerId, true].PlayerSpentCoins - _cost;
                                int _newBounty = PersistentContainer.Instance.Players[_cInfo1.playerId, true].Bounty + _cost;
                                PersistentContainer.Instance.Players[_cInfo.playerId, true].PlayerSpentCoins = _newCoins;
                                PersistentContainer.Instance.Players[_cInfo1.playerId, true].Bounty = _newBounty;
                                PersistentContainer.Instance.Save();
                                _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1} you have added {2} bounty to {3}.[-]", Config.Chat_Response_Color, _playerName, _cost, _cInfo1.playerName), Config.Server_Response_Name, false, "ServerTools", false));
                            }
                            else
                            {
                                _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1} you do not have enough {2} to the bounty.[-]", Config.Chat_Response_Color, _playerName, Wallet.Coin_Name), Config.Server_Response_Name, false, "ServerTools", false));
                            }
                        }
                        else
                        {
                            PersistentContainer.Instance.Players[_cInfo.playerId, true].PlayerSpentCoins = 0;
                            PersistentContainer.Instance.Save();
                            NewBounty(_cInfo, _message, _playerName);
                        }
                    }
                }
            }
        }
    }
}
