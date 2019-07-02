using Harmony12;
using UnityModManagerNet;
using System;
using UnityEngine;
using Kingmaker;
using System.Diagnostics;

namespace Boilerplate
{
    public class Main
    {
        public static bool enabled;
        public static Settings settings;
        public static UnityModManager.ModEntry.ModLogger logger;
        static string modId;
        static HarmonyInstance harmony;
        static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                logger = modEntry.Logger;
                modId = modEntry.Info.Id;
                settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
                modEntry.OnToggle = OnToggle;
                modEntry.OnGUI = OnGUI;
                modEntry.OnSaveGUI = OnSaveGUI;
                harmony = HarmonyInstance.Create(modEntry.Info.Id);
                CheckPatches();
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                throw ex;
            }
            return true;
        }
        // Called when the mod is turned to on/off.
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value /* active or inactive */)
        {
            enabled = value;
            CheckPatches();
            return true; // Permit or not.
        }
        static void CheckPatches()
        {
            logger.Log("Checking patches");
            using (new TestCodeTimer("Patching Time"))
            {
                if (!FreeMerc.patched && settings.freeMercs && enabled)
                {
                    logger.Log("Patching GetCustomCompanionCost");
                    harmony.Patch(AccessTools.Method(typeof(Player), "GetCustomCompanionCost"),
                        postfix: new HarmonyMethod(typeof(FreeMerc), "Player_GetCustomCompanionCost_Patch"));
                    FreeMerc.patched = true;
                }
                if (FreeMerc.patched && (!settings.freeMercs || !enabled))
                {
                    logger.Log("Unpatching GetCustomCompanionCost");
                    harmony.Unpatch(AccessTools.Method(typeof(Player), "GetCustomCompanionCost"), HarmonyPatchType.All, modId);
                    FreeMerc.patched = false;
                }
                if (!FreeRespec.patched && settings.freeRespec && enabled)
                {
                    logger.Log("Patching RespecsUsed");
                    harmony.Patch(AccessTools.Property(typeof(Player), "RespecsUsed").GetMethod,
                         postfix: new HarmonyMethod(typeof(FreeRespec), "Player_RespecsUsed_Getter_Patch"));
                    harmony.Patch(AccessTools.Property(typeof(Player), "RespecsUsed").SetMethod,
                        prefix: new HarmonyMethod(typeof(FreeRespec), "Player_RespecsUsed_Setter_Patch"));
                    FreeRespec.patched = true;
                }
                if (FreeRespec.patched && (!settings.freeRespec || !enabled))
                {
                    logger.Log("Unpatching RespecsUsed");
                    harmony.Unpatch(AccessTools.Property(typeof(Player), "RespecsUsed").GetMethod, HarmonyPatchType.All, modId);
                    harmony.Unpatch(AccessTools.Property(typeof(Player), "RespecsUsed").SetMethod, HarmonyPatchType.All, modId);
                    FreeRespec.patched = false;
                }
            }
        }
        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
            CheckPatches();
        }
        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (!enabled) return;
            try
            {
                var toggle = GUILayout.Toggle(settings.freeMercs, "Free Mercs");
                settings.freeRespec = GUILayout.Toggle(settings.freeRespec, "Free Respecs");
                if (Game.Instance.Player != null)
                {
                    GUILayout.Label($"MercCost {Game.Instance.Player.GetCustomCompanionCost()}");
                    GUILayout.Label($"Respecs {Game.Instance.Player.RespecsUsed}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                throw ex;
            }
        }
        public class TestCodeTimer : IDisposable
        {
            private readonly Stopwatch m_Stopwatch;
            private readonly string m_Text;
            public TestCodeTimer(string text)
            {
                this.m_Text = text;
                this.m_Stopwatch = Stopwatch.StartNew();
            }
            public void Dispose()
            {
                this.m_Stopwatch.Stop();
                string message = string.Format("Profiled {0}: {1:0.00}ms", this.m_Text, this.m_Stopwatch.ElapsedMilliseconds);
                logger.Log(message);
            }
        }
    }
}
