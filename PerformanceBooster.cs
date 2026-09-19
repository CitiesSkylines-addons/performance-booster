using System;
using System.Reflection;
using ICities;
using HarmonyLib;
using CitiesHarmony.API;
using UnityEngine;
using ColossalFramework.UI;

namespace PerformanceBooster
{
    public class Mod : IUserMod
    {
        public string Name
        {
            get { return "FPS & Performance Booster"; }
        }

        public string Description
        {
            get { return "Optimizes framerate, unlocks 240Hz high-refresh displays, and reduces UI render overhead."; }
        }

        public void OnEnabled()
        {
            HarmonyHelper.DoOnHarmonyReady(new Action(Patcher.PatchAll));
            ApplyEngineOptimizations();
        }

        public void OnDisabled()
        {
            Patcher.UnpatchAll();
        }

        public static void ApplyEngineOptimizations()
        {
            try
            {
                // Unlock 240Hz refresh rate
                Application.targetFrameRate = 240;
                QualitySettings.vSyncCount = 0;
                QualitySettings.maxQueuedFrames = 2;
                Debug.Log("[PerformanceBooster] Applied 240Hz display optimization!");
            }
            catch (Exception ex)
            {
                Debug.LogError("[PerformanceBooster] Failed to set frame rate: " + ex);
            }
        }
    }

    public static class Patcher
    {
        private const string HarmonyId = "com.antigravity.performancebooster";
        private static bool patched = false;

        public static void PatchAll()
        {
            if (patched) return;
            try
            {
                var harmony = new Harmony(HarmonyId);
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                patched = true;
                Mod.ApplyEngineOptimizations();
                Debug.Log("[PerformanceBooster] Harmony optimizations active!");
            }
            catch (Exception ex)
            {
                Debug.LogError("[PerformanceBooster] Patching failed: " + ex);
            }
        }

        public static void UnpatchAll()
        {
            if (!patched) return;
            try
            {
                var harmony = new Harmony(HarmonyId);
                harmony.UnpatchAll(HarmonyId);
                patched = false;
            }
            catch (Exception ex)
            {
                Debug.LogError("[PerformanceBooster] Unpatching failed: " + ex);
            }
        }
    }

    // Optimization: When loading into level, re-apply high-refresh target
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            Mod.ApplyEngineOptimizations();
        }
    }
}
