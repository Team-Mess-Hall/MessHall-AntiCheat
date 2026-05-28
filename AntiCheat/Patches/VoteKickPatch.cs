using AntiCheat.Managers;
using HarmonyLib;
using Il2CppSG.Airlock.Network;
using static AntiCheat.References;
using static AntiCheat.Settings;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(ModerationManager), nameof(ModerationManager.RPC_KickVote))]
    public class VoteKickPatch
    {
        public static bool Prefix(int kickPlayer, int sourcePlayer)
        {
            if (IsHost)
            {
                if (kickPlayer == sourcePlayer)
                {
                    Logging.AntiCheatWarn($"Someone is Cheating! || Reason: Attempted Force VoteKick || Action: Canceled (Cannot Get Cheater)");
                    return false;
                }
                return true;
            }
            return true;
        }
    }
}