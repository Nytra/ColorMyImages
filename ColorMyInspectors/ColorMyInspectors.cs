using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using FrooxEngine.UIX;
using System;
using BaseX;
using System.Linq;

namespace ColorMyInspectors
{
    public class ColorMyInspectors : NeosMod
    {
        public override string Name => "ColorMyInspectors";
        public override string Author => "Nytra";
        public override string Version => "1.0.0";
        public override string Link => "https://github.com/Nytra/NeosColorMyInspectors";

        public static ModConfiguration Config;

        private const string SEP_STRING = "<size=0></size>";

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool> MOD_ENABLED = new ModConfigurationKey<bool>("MOD_ENABLED", "Mod Enabled:", () => true);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<dummy> DUMMY1 = new ModConfigurationKey<dummy>("DUMMY1", SEP_STRING, () => new dummy());
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<color> LEFT_COLOR = new ModConfigurationKey<color>("LEFT_COLOR", "Left Side Color:", () => color.Gray);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<color> RIGHT_COLOR = new ModConfigurationKey<color>("RIGHT_COLOR", "Right Side Color:", () => color.Gray);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<dummy> DUMMY2 = new ModConfigurationKey<dummy>("DUMMY2", SEP_STRING, () => new dummy());
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool> USE_LEFT_COLOR_FOR_BOTH = new ModConfigurationKey<bool>("USE_LEFT_COLOR_FOR_BOTH", "Use Left Side Color for Both Sides:", () => false);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<dummy> DUMMY3 = new ModConfigurationKey<dummy>("DUMMY3", SEP_STRING, () => new dummy());
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool> USE_RANDOM_COLORS = new ModConfigurationKey<bool>("USE_RANDOM_COLORS", "Use Random Colors:", () => false);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<float> SATURATION = new ModConfigurationKey<float>("SATURATION", "Saturation (If using random colors):", () => 0.25f);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<float> VALUE = new ModConfigurationKey<float>("VALUE", "Value (If using random colors):", () => 0.75f);

        public override void OnEngineInit()
        {
            Config = GetConfiguration();
            Harmony harmony = new Harmony("owo.Nytra.ColorMyInspectors");
            harmony.PatchAll();
        }
        private static Random rngTimeSeeded = new Random();
		
        [HarmonyPatch(typeof(SceneInspector), "OnAttach")]
        class ColorMyInspectorsPatch
        {
            public static void Postfix(SceneInspector __instance)
            {
                if (!Config.GetValue(MOD_ENABLED)) return;
                __instance.RunInUpdates(0, delegate
                {
                    Slot imageSlot = __instance.Slot.FindChild((Slot s) => s.Name == "Image");
                    Slot split1 = imageSlot?.Children.ToList()[0];
                    Image image1 = split1?.GetComponentInChildren<Image>();
                    Slot split2 = imageSlot?.Children.ToList()[1];
                    Image image2 = split2?.GetComponentInChildren<Image>();
                    color left, right;
                    if (image1 != null && image2 != null)
                    {
                        if (Config.GetValue(USE_RANDOM_COLORS))
                        {
                            left = new ColorHSV((float)rngTimeSeeded.NextDouble(), Config.GetValue(SATURATION), Config.GetValue(VALUE), 1f);
                            right = new ColorHSV((float)rngTimeSeeded.NextDouble(), Config.GetValue(SATURATION), Config.GetValue(VALUE), 1f);
                        }
                        else
                        {
                            left = Config.GetValue(LEFT_COLOR);
                            right = Config.GetValue(RIGHT_COLOR);
                        }

                        image1.Tint.Value = left;
                        if (Config.GetValue(USE_LEFT_COLOR_FOR_BOTH))
                        {
                            image2.Tint.Value = left;
                        }
                        else
                        {
                            image2.Tint.Value = right;
                        }
                    }
                    
                });
                return;
            }
        }
    }
}