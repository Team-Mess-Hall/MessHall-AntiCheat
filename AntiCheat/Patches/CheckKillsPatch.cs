using AntiCheat.Managers;
using HarmonyLib;
using Il2CppFusion;
using Il2CppSG.Airlock;
using Il2CppSG.Airlock.Network;
using Il2CppSG.Airlock.Roles;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(NetworkedKillBehaviour), nameof(NetworkedKillBehaviour.RPC_TargetedAction))]
    public class CheckKillsPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(NetworkedKillBehaviour __instance, PlayerRef targetedPlayer, PlayerRef perpetrator, int action)
        {
            if (!Settings.IsHost) return true;

            string name = AntiCheatManager.GetPlayerNameViaPlayerState(perpetrator);
            PlayerState killer = AntiCheatManager.GetPlayerStateByID(perpetrator);
            GameRole role = AntiCheatManager.GetRole(killer);

            if (killer.ActionCooldownRemaining < 7)
            {
                AntiCheatManager.KickPlayerForCheating(name, "Has 0 Second Cooldown", perpetrator);
                return false;
            }

            if (role != GameRole.Impostor && role != GameRole.Revenger && role != GameRole.Vigilante)
            {
                AntiCheatManager.KickPlayerForCheating(name, "Invalid Kill", perpetrator);
                return false;
            }

            if (action == (int)ProximityTargetedAction.Infect && References.GameState.GameModeStateValue.GameMode != GameModes.Infection)
            {
                AntiCheatManager.KickPlayerForCheating(name, "Invalid Targeted Action Data", perpetrator);
                return false;
            }

            if (action == (int)ProximityTargetedAction.Guard &&
                ((role != GameRole.GuardianAngel && killer.ActivePowerUps != PowerUps.Guard) || !References.GameState.InTaskState()))
            {
                AntiCheatManager.KickPlayerForCheating(name, "Invalid Targeted Action Data", perpetrator);
                return false;
            }

            if (action == (int)ProximityTargetedAction.Vote &&
                (References.GameState.GameModeStateValue.GameMode != GameModes.Gameplay || role != GameRole.Sheriff || !References.GameState.InVotingState()))
            {
                AntiCheatManager.KickPlayerForCheating(name, "Invalid Targeted Action Data", perpetrator);
                return false;
            }

            return true;
        }
    }
}