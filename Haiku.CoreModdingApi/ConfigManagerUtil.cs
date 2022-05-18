using UnityEngine;
using BepInEx.Configuration;
using System;
using System.Diagnostics;

namespace Modding
{
    public class ConfigManagerUtil
    {
        /// <summary>
        /// A custom Drawer to have a button Function
        /// </summary>
        /// <param name="method">The Action the Button executes</param>
        /// <param name="name">Name Displayed on the Button</param>
        /// <param name="description">Description shown when hovering over the Button</param>
        /// <param name="expandWidth">Whether the Button should expand as much as possible</param>
        public static void buttonDrawer(Action method, string name, string description, bool expandWidth = false)
        {
            if (GUILayout.Button(new GUIContent(name, description), GUI.skin.button, GUILayout.ExpandWidth(expandWidth)))
            {
                method();
            }
        }

        /// <summary>
        /// Automatically creates a Button by using a Drawer
        /// </summary>
        /// <param name="config">The Plugin's ConfigFile</param>
        /// <param name="method">The method for the Button</param>
        /// <param name="section">The section for the Config</param>
        /// <param name="btnName">The name displayed on the Button, also serves as the Key for the Config</param>
        /// <param name="description">Description shown when hovering over the Button</param>
        /// <param name="ConfigAttributes">Attributes for the Button. Automatically asigns a Custom Drawer with the method and hides the Default Button</param>
        public static void createButton(ConfigFile config, Action method, string section, string btnName, string description, ConfigurationManagerAttributes ConfigAttributes = null)
        {
            if (ConfigAttributes == null)
            {
                ConfigAttributes = new ConfigurationManagerAttributes();
            }
            ConfigAttributes.ReadOnly = true;
            ConfigAttributes.HideDefaultButton = true;
            ConfigAttributes.CustomDrawer = x => buttonDrawer(method, btnName, description);
            config.Bind(section, btnName, "", new ConfigDescription(description, null,
               ConfigAttributes));
        }
        /// <summary>
        /// Automatically creates a Website Button for your Plugin
        /// </summary>
        /// <param name="config">The Plugin's ConfigFile</param>
        /// <param name="website">The Plugin's Website</param>
        public static void createWebsiteButton(ConfigFile config, string website)
        {
            config.Bind("Website", "URL", "", new ConfigDescription("Open Plugin's Website", null,
               new ConfigurationManagerAttributes { CustomDrawer = x => websiteButtonDrawer(() => Process.Start(website), "URL", "Open Plugin's Website"), ReadOnly = true, HideDefaultButton = true, HideSettingName = true }));
        }
        private static void websiteButtonDrawer(Action method, string name, string description, bool expandWidth = false)
        {
            GUILayout.Label("", GUILayout.Width(275f));
            if (GUILayout.Button(new GUIContent(name, description), GUI.skin.button, GUILayout.ExpandWidth(expandWidth)))
            {
                method();
            }
        }

        public static ConfigDescription setPosition(int pos)
        {
            return new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = pos });
        }
    }
}
