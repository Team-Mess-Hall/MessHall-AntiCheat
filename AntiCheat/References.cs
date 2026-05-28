using AntiCheat.Managers;
using Il2CppSG.Airlock;
using Il2CppSG.Airlock.Localization;
using Il2CppSG.Airlock.Network;
using Il2CppSG.Airlock.Roles;
using Il2CppSG.Airlock.XR;
using static AntiCheat.Settings;
using static UnityEngine.Object;

namespace AntiCheat
{
    public class References
    {
        public static XRRig Client;
        public static AirlockPeer Peer;
        public static GameStateManager GameState;
        public static SpawnManager Spawn;
        public static AirlockNetworkRunner networkRunner;
        public static NetworkedKillBehaviour Killing;
        public static RoleManager Role;

        public static void ResetReferences()
        {
            string trace = "";

            try
            {
                trace = "Setting Nulls";
                ReferencesSet = false;
                Client = null;
                Peer = null;
                GameState = null;
                Spawn = null;
                networkRunner = null;
                Killing = null;
                Role = null;

                trace = "Client";
                Client = FindObjectOfType<XRRig>();
                trace = "Peer";
                Peer = FindObjectOfType<AirlockPeer>();
                trace = "GameState";
                GameState = FindObjectOfType<GameStateManager>();
                trace = "Spawn";
                Spawn = FindObjectOfType<SpawnManager>();
                trace = "networkRunner";
                networkRunner = FindObjectOfType<AirlockNetworkRunner>();
                IsHost = Peer.Runner.LocalPlayer.PlayerId == 9;
                trace = "NetworkedKillBehaviour";
                Killing = FindObjectOfType<NetworkedKillBehaviour>();
                trace = "RoleManager";
                Role = FindObjectOfType<RoleManager>();
            }
            catch
            {
                Logging.AntiCheatLog($"error getting: {trace}");
            }
            finally
            {
                ReferencesSet = true;
            }
        }
    }
}