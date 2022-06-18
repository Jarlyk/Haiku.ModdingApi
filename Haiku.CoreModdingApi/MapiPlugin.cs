using System.Collections.Generic;
using BepInEx;
using UnityEngine;
using BepInEx.Bootstrap;
using UnityEngine.UI;
using Modding;
using System;

namespace Haiku.CoreModdingApi
{
    [BepInPlugin("haiku.mapi", "Haiku Core Modding API", "1.0.1.0")]
    public sealed class MapiPlugin : BaseUnityPlugin
    {
        private GameObject mapiCanvas;

        private static GameObject AchievementsDisabledText;
        private static bool achievementsDisabled = true;

        private List<string> pluginsLoaded = new List<string>();


        void Start()
        {
            // Remove Steam Name Display
            GameObject SteamNameDisplay = GameObject.Find("WelcomeText");
            Destroy(SteamNameDisplay);

            // Hooks
            On.AchievementManager.SetAchievement += onAchievement;
            On.MainMenuManager.SelectSaveFile += gameStarted;
            On.MainMenuManager.Start += MainMenuActive;

            // MApi settings
            Settings.initSettings(Config);

            #region Loaded Plugin Overlay
            Logger.LogInfo("Trying to find plugins");
            getLoadedPlugins();

            mapiCanvas = CanvasUtil.CreateCanvas(1);
            mapiCanvas.name = "MApi Canvas";
            mapiCanvas.transform.SetParent(gameObject.transform);
            Font font = new Font();

            GameObject testCanvas = new GameObject();
            GameObject testTextPanel = new GameObject();

            displayLoadedPlugins();
            #endregion

            AchievementsDisabledText = CanvasUtil.CreateTextPanel(mapiCanvas, "Achievements are disabled", 9, TextAnchor.LowerLeft,
                new CanvasUtil.RectData(new Vector2(0, 0), new Vector2(5, -1), new Vector2(0, 0), new Vector2(1, 1)), CanvasUtil.GameFont);
        }

        internal static void toggleAchievements()
        {
            achievementsDisabled = !achievementsDisabled;
            AchievementsDisabledText.SetActive(achievementsDisabled);
        }

        private void getLoadedPlugins()
        {
            foreach (KeyValuePair<string, PluginInfo> plugin in Chainloader.PluginInfos)
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
        }

        private void displayLoadedPlugins()
        {
            GameObject MapiPlugins = CanvasUtil.CreateBasePanel(mapiCanvas,
            new CanvasUtil.RectData(new Vector2(0f, 0f), new Vector2(10, -4), new Vector2(0, 0), new Vector2(1, 2)));
            MapiPlugins.name = "Mapi Plugin Panel";
            for (int i = 0; i < pluginsLoaded.Count; i++)
            {
                GameObject textObject = CanvasUtil.CreateTextPanel(MapiPlugins, pluginsLoaded[i], 4, TextAnchor.MiddleLeft,
                    new CanvasUtil.RectData(new Vector2(400f, 10f), new Vector2(0, 0 - i * 5)), CanvasUtil.GameFont);
                textObject.name = $"{pluginsLoaded[i]}";
                textObject.GetComponent<Text>().fontStyle = FontStyle.Bold;
            }
        }

        #region Hooks
        private void MainMenuActive(On.MainMenuManager.orig_Start orig, MainMenuManager self)
        {
            mapiCanvas.SetActive(true);
            Destroy(GameObject.Find("WelcomeText"));
            orig(self);
        }

        private void gameStarted(On.MainMenuManager.orig_SelectSaveFile orig, MainMenuManager self, string saveFilePath)
        {
            orig(self, saveFilePath);
            mapiCanvas.SetActive(false);
        }

        private void onAchievement(On.AchievementManager.orig_SetAchievement orig, AchievementManager self, string achievementName)
        {
            if (!achievementsDisabled)
            {
                orig(self, achievementName);
                return;
            }
            Debug.Log($"Tried to get the Achievement: {achievementName} while Achievements are disabled, passing instead");
        }
        #endregion
    }
}
