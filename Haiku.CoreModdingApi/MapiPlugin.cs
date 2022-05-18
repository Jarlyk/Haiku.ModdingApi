using System.Collections.Generic;
using BepInEx;
using UnityEngine;
using BepInEx.Bootstrap;
using UnityEngine.UI;
using Modding;
using System;

namespace Haiku.CoreModdingApi
{
    [BepInPlugin("haiku.mapi", "Haiku Core Modding API", "1.0.0.1")]
    public sealed class MapiPlugin : BaseUnityPlugin
    {
        GameObject MApiCanvas;
        void Start()
        {
            List<string> pluginsLoaded = new List<string>();

            // Remove Steam Name Display and get the font
            GameObject SteamNameDisplay = GameObject.Find("WelcomeText");
            CanvasUtil.gameFont = SteamNameDisplay.GetComponent<Text>().font;
            Destroy(SteamNameDisplay);

            // Hooks
            On.AchievementManager.SetAchievement += onAchievement;
            On.MainMenuManager.SelectSaveFile += gameStarted;
            On.MainMenuManager.Start += MainMenuActive;

            // MApi settings
            Settings.initSettings(Config);

            #region Loaded Plugin Overlay
            Logger.LogInfo("Trying to find plugins");
            foreach (KeyValuePair<string,PluginInfo> plugin in Chainloader.PluginInfos)
            {
                BepInPlugin metadata = plugin.Value.Metadata;
                Logger.LogInfo("Plugin found: " + metadata.Name);
                if (metadata.Name.Equals("Haiku Core Modding API") && pluginsLoaded.Count > 0)
                {
                    // Put MApi on top because why not
                    pluginsLoaded.Add(pluginsLoaded[0]);
                    pluginsLoaded[0] = metadata.Name.ToString() + ": " + metadata.Version.ToString();
                }
                else
                {
                    pluginsLoaded.Add(metadata.Name.ToString() + ": " + metadata.Version.ToString());
                }
            }
            foreach (string errorInformation in Chainloader.DependencyErrors)
            {
                Logger.LogError("Failed to load Plugin: \n" + errorInformation);
            }
            if (pluginsLoaded.Count == 0)
            {
                Logger.LogInfo("Couldn't find plugins");
            }

            MApiCanvas = CanvasUtil.CreateCanvas(1);
            MApiCanvas.name = "MApi Canvas";    
            DontDestroyOnLoad(MApiCanvas);

            GameObject MapiPlugins = CanvasUtil.CreateBasePanel(MApiCanvas, 
                new CanvasUtil.RectData(new Vector2(0f, 0f), new Vector2(10, -4),new Vector2(0,0),new Vector2(1,2)));
            MapiPlugins.name = "Mapi Plugin Panel";
            for (int i = 0; i < pluginsLoaded.Count; i++)
            {
                GameObject textObject = CanvasUtil.CreateTextPanel(MapiPlugins, pluginsLoaded[i],4,TextAnchor.MiddleLeft,
                    new CanvasUtil.RectData(new Vector2(400f, 10f), new Vector2(0, 0 - i * 5)), CanvasUtil.gameFont);
                textObject.name = $"{pluginsLoaded[i]}";
                textObject.GetComponent<Text>().fontStyle = FontStyle.Bold;
            }
            #endregion
        }

        private void MainMenuActive(On.MainMenuManager.orig_Start orig, MainMenuManager self)
        {
            MApiCanvas.SetActive(true);
            Destroy(GameObject.Find("WelcomeText"));
            orig(self);
        }

        private void gameStarted(On.MainMenuManager.orig_SelectSaveFile orig, MainMenuManager self, string saveFilePath)
        {
            orig(self, saveFilePath);
            MApiCanvas.SetActive(false);
        }

        #region Hooks
        private void onAchievement(On.AchievementManager.orig_SetAchievement orig, AchievementManager self, string achievementName)
        {
            Debug.LogWarning("Tried to get an Achievement while MApi is enabled, passing instead");
        }
        #endregion
    }
}
