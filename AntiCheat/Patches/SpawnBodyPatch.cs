using AntiCheat.Managers;
using HarmonyLib;
using Il2CppFusion;
using Il2CppSG.Airlock;
using Il2CppSG.Airlock.Network;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(SpawnManager), nameof(SpawnManager.RPC_SpawnBodyByPlayerId))]
    public class BodySpawnPatch
    {
        public static bool Prefix(SpawnManager __instance, PlayerRef id, NetworkRigidbody rb)
        {
            if (Settings.IsHost)
            {
                if (AntiCheatManager.GetPlayerStateByID(id).IsAlive)
                {
                    Logging.AntiCheatWarn("Someone in the room is cheating, unable to identify the cheater");
                    return false;
                }
            }
            return true;
        }
    }
}