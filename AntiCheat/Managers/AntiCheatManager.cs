using System.Collections;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using Il2CppFusion;
using Il2CppFusion.Photon.Realtime;
using Il2CppInternal.Cryptography;
using Il2CppSG.Airlock;
using Il2CppSG.Airlock.Roles;
using Il2CppSG.Platform;
using Il2CppSteamworks;
using MelonLoader;
using UnityEngine;
using static AntiCheat.References;
using static AntiCheat.Settings;
using static Il2Cpp.Interop;

namespace AntiCheat.Managers
{
    public class AntiCheatManager
    {
        private static readonly HttpClient client = new HttpClient();
        private static Dictionary<PlayerState, Vector3> PrevPosition = new Dictionary<PlayerState, Vector3>();
        private static Dictionary<PlayerState, int> SpeedDetections = new Dictionary<PlayerState, int>();
        private static string CheaterModID = "";
        private static float SpeedLimit = 9.5f;
        private static int MaxViolations = 3;

        internal static List<string> knownModders = new List<string>() { };

        public static async Task<List<string>> LoadKnownModders()
        {
            string? url = "https://jolynesbackend.xyz/messhallapi/anticheat/blacklistedusers";
            string? response = await client.GetStringAsync(url);
            var modders = new List<string>();

            foreach (string? line in response.Split('\n'))
            {
                string? userid = line.Trim();
                if (!string.IsNullOrEmpty(userid))
                    modders.Add(userid);
            }
            return modders;
        }


        public static void KickPlayerForCheating(string playerName, string reason, PlayerRef Cheater)
        {
            if (IsHost)
            {
                try
                {
                    CheaterModID = networkRunner.GetPlayerUserId(Cheater);
                    if (CheaterModID.Contains("Steam"))
                    {
                        if (string.IsNullOrEmpty(playerName) || playerName == "null")
                        {
                            networkRunner.Disconnect(Cheater);
                            Logging.AntiCheatWarn($"Could not get Cheating Player's UserName Kicking anyways for Reason: {reason}");
                        }
                        else
                        {
                            networkRunner.Disconnect(Cheater);
                            Logging.AntiCheatWarn($"Kicked {playerName} for cheating. || Reason: {reason}");
                        }
                    }
                }
                catch
                {
                    MelonLogger.Msg($"{playerName} is cheating: {reason}");
                }
            }
        }

        public static string GetPlayerNameViaPlayerState(PlayerRef RequestedPlayer)
        {
            foreach (PlayerState player in Spawn.ActivePlayerStates)
            {
                if (player == null) continue;

                if (player.PlayerId == RequestedPlayer.PlayerId)
                {
                    return player.NetworkName.Value;
                }
            }
            return "null";
        }

        public static bool CheckAliveState(PlayerRef RequestedPlayer)
        {
            foreach (PlayerState player in Spawn.ActivePlayerStates)
            {
                if (player.PlayerId == RequestedPlayer)
                {
                    return player.IsAlive;
                }
            }
            return true;
        }



        public static GameRole GetRole(PlayerState player)
        {
            if (IsHost)
            {
                foreach (Il2CppSystem.Collections.Generic.KeyValuePair<GameRole, Il2CppSystem.Collections.Generic.List<int>> roleEntry in Role.gameRoleToPlayerIds)
                {
                    foreach (int id in roleEntry.Value)
                    {
                        if (player.IsConnected && id == player.PlayerId)
                        {
                            return roleEntry.Key;
                        }
                    }
                }
            }
            return GameRole.NotSet;
        }


        internal static IEnumerator CheckPlayersLoop()
        {
            yield return new WaitForSeconds(5f);

            while (InGame && Spawn != null)
            {
                foreach (PlayerState player in Spawn.ActivePlayerStates)
                {
                    Vector3 PlayerPosition = player.LocomotionPlayer.NetworkRigidbody.Rigidbody.position;

                    if (PrevPosition.TryGetValue(player, out Vector3 LastPos))
                    {
                        float distance = Vector3.Distance(PlayerPosition, LastPos);
                        float speed = distance / 1f;

                        if (speed > SpeedLimit)
                        {
                            if (!SpeedDetections.ContainsKey(player))
                                SpeedDetections[player] = 0;
                            SpeedDetections[player]++;

                            if (SpeedDetections[player] >= MaxViolations)
                            {
                                KickPlayerForCheating(player.NetworkName.Value, "Speed Hacks", player.PlayerId);
                                SpeedDetections[player] = 0;
                            }
                        }
                        else
                        {
                            SpeedDetections[player] = 0;
                        }
                    }

                    PrevPosition[player] = PlayerPosition;
                    if (player.HatId == 98)
                    {
                        KickPlayerForCheating(player.NetworkName.Value, "DevHat", player.PlayerId);
                    }

                    if (!string.IsNullOrEmpty(player.NetworkName.Value) && player.NetworkName.Value.Any(c => "!@$%^&*(){}<>`~ |[]=+;:?#".Contains(c)))
                    {
                        KickPlayerForCheating(player.NetworkName.Value, "Invalid Name", player.PlayerId);
                    }
                }

                yield return new WaitForSeconds(1f);
            }
        }

        public static PlayerState GetPlayerStateByID(PlayerRef Player)
        {
            foreach (PlayerState player in Spawn.ActivePlayerStates)
            {
                if (player.PlayerId == Player.PlayerId)
                {
                    return player;
                }
            }
            return null!;
        }

        public static string ModerationIDtoSHA256(string input)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hash = sha.ComputeHash(bytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hash)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }
    }
}