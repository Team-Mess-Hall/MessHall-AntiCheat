using AntiCheat;
using AntiCheat.Managers;
using AntiCheat.Patches;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using UnityEngine;
using static AntiCheat.Settings;
using static AntiCheat.References;
using System.Collections;
using UnityEngine.InputSystem.Controls;

[assembly: MelonGame("Schell Games")]
[assembly: MelonInfo(typeof(Core), "AntiCheat", "1.1.0", "TeamMessHall")]
namespace AntiCheat
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            AntiCheatManager.knownModders.Clear();
            AntiCheatManager.knownModders = AntiCheatManager.LoadKnownModders().GetAwaiter().GetResult();

            foreach (Type type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsSubclassOf(typeof(MonoBehaviour)))
                {
                    ClassInjector.RegisterTypeInIl2Cpp(type);
                }
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            InGame = sceneName != "Boot" && sceneName != "Title";

            if (InGame)
            {
                MelonCoroutines.Start(AntiCheatManager.CheckPlayersLoop());
            }
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            InGame = sceneName != "Boot" && sceneName != "Title";

            if (sceneName == "Title")
            {
                ModStampManager.LoadModStamp();
                ReferencesSet = false;
            }
            if (InGame)
            {
                if (!ReferencesSet)
                {
                    ResetReferences();
                }
            }
        }
    }
}