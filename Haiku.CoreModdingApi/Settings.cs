using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.ComponentModel;

namespace Haiku.CoreModdingApi
{
    internal class Settings
    {
        public static void initSettings(BepInEx.Configuration.ConfigFile config)
        {
            config.Bind<possibleResolutions>("MApi", "Resolution", possibleResolutions.Res1920x1080);
            if (config.TryGetEntry<possibleResolutions>(new BepInEx.Configuration.ConfigDefinition("MApi", "Resolution"), out BepInEx.Configuration.ConfigEntry<possibleResolutions> entry))
            {
                changeResolution(GetRes((possibleResolutions)entry.BoxedValue));
            }
            config.SaveOnConfigSet = true;
            config.SettingChanged += Config_SettingChanged;
        }
        #region ResolutionConfigs
        private enum possibleResolutions
        {
            [Description("256x144")]
            Res256x144,
            [Description("320x180")]
            Res320x180,
            [Description("426x240")]
            Res426x240,
            [Description("640x360")]
            Res640x360,
            [Description("848x480")]
            Res848x480,
            [Description("960x540")]
            Res960x540,
            [Description("1280x720")]
            Res1280x720,
            [Description("1600x900")]
            Res1600x900,
            [Description("1920x1080")]
            Res1920x1080,
            [Description("2560x1440")]
            Res2560x1440,
            [Description("3200x1800")]
            Res3200x1800,
            [Description("3840x2160")]
            Res3840x2160
        }
        private static readonly Vector2[] resolutions = new Vector2[]
        {
            new Vector2(256f,144f),
            new Vector2(320f,180f),
            new Vector2(426f,240f),
            new Vector2(640f,360f),
            new Vector2(848f,480f),
            new Vector2(960f,540f),
            new Vector2(1280f,720f),
            new Vector2(1600f,900f),
            new Vector2(1920f,1080f),
            new Vector2(2560f,1440f),
            new Vector2(3200f,1800f),
            new Vector2(3840f,2160f),
        };

        private static Vector2 GetRes(possibleResolutions res)
        {
            return resolutions[(int)res];
        }

        private static void Config_SettingChanged(object sender, BepInEx.Configuration.SettingChangedEventArgs e)
        {
            if (e.ChangedSetting.Definition == new BepInEx.Configuration.ConfigDefinition("MApi", "Resolution"))
            {
                Vector2 res = GetRes((possibleResolutions)e.ChangedSetting.BoxedValue);
                changeResolution(res);
            }
        }

        private static void changeResolution(Vector2 res)
        {
            Screen.SetResolution((int)res.x, (int)res.y, Screen.fullScreen);
        }
        #endregion
    }
}
