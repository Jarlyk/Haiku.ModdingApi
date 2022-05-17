using System.Collections.Generic;
using BepInEx;
using UnityEngine;
using BepInEx.Bootstrap;
using UnityEngine.UI;

namespace Haiku.CoreModdingApi
{
    [BepInPlugin("haiku.mapi", "Haiku Core Modding API", "1.0.0.0")]
    public sealed class MapiPlugin : BaseUnityPlugin
    {

        void Start()
        {
            List<string> pluginsLoaded = new List<string>();

            // Remove Steam Name Display and get the font
            GameObject SteamNameDisplay = GameObject.Find("WelcomeText");
            Font font = SteamNameDisplay.GetComponent<Text>().font;
            Destroy(SteamNameDisplay);

            // Disabling Achievements
            On.AchievementManager.SetAchievement += onAchievement;

            // MApi settings
            Settings.initSettings(Config);

            // MApi overlay
            Logger.LogInfo("Trying to find plugins");
            foreach (var plugin in Chainloader.PluginInfos)
            {
                var metadata = plugin.Value.Metadata;
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
            if (pluginsLoaded.Count == 0)
            {
                Logger.LogInfo("Couldn't find plugins");
            }

            GameObject MApiHeader = new GameObject();
            MApiHeader.name = "MApi Header";
            Canvas MApiCanvas = MApiHeader.AddComponent<Canvas>();
            MApiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler MApiCanvasScaler = MApiHeader.AddComponent<CanvasScaler>();
            MApiCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            MApiCanvasScaler.referencePixelsPerUnit = 16f;
            MApiCanvasScaler.referenceResolution = new Vector2(384f, 216f);
            MApiCanvas.sortingOrder = 1;


            CanvasAspectScaler MApiCanvasAspectScaler = MApiHeader.AddComponent<CanvasAspectScaler>();
            MApiHeader.AddComponent<GraphicRaycaster>();

            DontDestroyOnLoad(MApiHeader);

            GameObject MapiPlugins = new GameObject();
            MapiPlugins.transform.parent = MApiHeader.transform;
            MapiPlugins.transform.localPosition = new Vector3(10, 103, 0);
            MapiPlugins.name = "MApi Plugins";
            for (int i = 0; i < pluginsLoaded.Count; i++)
            {
                GameObject textObject = new GameObject();
                textObject.transform.parent = MapiPlugins.transform;
                textObject.name = $"{pluginsLoaded[i]}";
                Text text = textObject.AddComponent<Text>();
                //text.font = Resources.GetBuiltinResource(typeof(Font), "Jaldi.ttf") as Font;
                text.font = font;
                text.fontStyle = FontStyle.Bold;
                text.text = pluginsLoaded[i];
                text.fontSize = 4;

                // Text position
                RectTransform rectTransform = text.GetComponent<RectTransform>();
                rectTransform.localPosition = new Vector3(0, 0 - i * 5, 0);
                rectTransform.sizeDelta = new Vector2(400, 10);
            }
        }

        #region Hooks
        private void onAchievement(On.AchievementManager.orig_SetAchievement orig, AchievementManager self, string achievementName)
        {
            Debug.LogWarning("Tried to get an Achievement while MApi is enabled, passing instead");
        }
        #endregion
    }
}
