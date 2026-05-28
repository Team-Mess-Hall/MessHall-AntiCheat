using AntiCheat.Managers;
using HarmonyLib;
using Il2CppSG.Airlock;
using Il2CppSG.Airlock.Network;
using Il2CppSG.Airlock.Roles;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(NetworkedLocomotionPlayer), nameof(NetworkedLocomotionPlayer.RPC_EnterVent))]
    public class EnterVentPatch
    {
        public static bool Prefix(NetworkedLocomotionPlayer __instance)
        {
            if (Settings.IsHost)
            {
                bool cheating = false;
                PlayerState venter = __instance.PState;
                GameRole role = AntiCheatManager.GetRole(venter);

                switch (References.GameState.GameModeStateValue.GameMode)
                {
                    case GameModes.Gameplay:
                        if (venter.IsAlive && (role == GameRole.Impostor || role == GameRole.Engineer)) return true;
                        cheating = true;
                        break;

                    case GameModes.Infection:
                        if (venter.IsAlive && venter.ActivePowerUps == PowerUps.CanVent && role != GameRole.Infected) return true;
                        cheating = true;
                        break;
                }

                if (cheating)
                {
                    AntiCheatManager.KickPlayerForCheating(venter.NetworkName.Value, "Invalid Vent Data", venter.PlayerId);
                    return false;
                }
            }
            return true;
        }
    }
}