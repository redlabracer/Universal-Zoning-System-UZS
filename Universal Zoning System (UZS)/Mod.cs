using Game;
using Game.Modding;
using Game.SceneFlow;
using Colossal.IO.AssetDatabase;
using UniversalZoningSystem.Systems;

namespace UniversalZoningSystem
{
    public class Mod : IMod
    {
        public static Mod? Instance { get; private set; }
        public Setting? Setting { get; private set; }

        public void OnLoad(UpdateSystem updateSystem)
        {
            UnityEngine.Debug.Log("UZS: Mod has been loaded!");
            Instance = this;
            
            Setting = new Setting(this);
            Setting.RegisterInOptionsUI();
            Game.SceneFlow.GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(Setting));
            
            AssetDatabase.global.LoadSettings("UniversalZoningSystem", Setting, new Setting(this));

            updateSystem.UpdateAt<ZoneMergeSystem>(SystemUpdatePhase.PrefabUpdate);
            
            UnityEngine.Debug.Log("UZS: Systems registered");
        }

        public void OnDispose()
        {
            if (Setting != null)
            {
                Setting.UnregisterInOptionsUI();
                Setting = null;
            }
            Instance = null;
        }
    }
}
