using Harmony12;
using UnityModManagerNet;
using System;
using UnityEngine;
using Kingmaker;

namespace Boilerplate
{
    public class Main
    {
        public static bool enabled;
        public static Settings settings;
        static UnityModManager.ModEntry.ModLogger logger;
        static string modId;
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
                var harmony = HarmonyInstance.Create(modEntry.Info.Id);
                harmony.Patch(AccessTools.Method(typeof(Player), "GetCustomCompanion"),
                postfix: new HarmonyMethod(typeof(FreeMerc), "Player_GetCustomCompanionCost_Patch"));
            } catch(Exception ex)
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
            return true; // Permit or not.
        }
        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }
        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (!enabled) return;
            try
            {
                settings.freeMercs = GUILayout.Toggle(settings.freeMercs, "Free Mercs");
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                throw ex;
            }
        }
    }
}
