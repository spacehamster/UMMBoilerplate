namespace Boilerplate
{
    class FreeMerc
    {
        public static bool patched = false;
        public static void Player_GetCustomCompanionCost_Patch(ref int __result)
        {
            if (!Main.enabled || Main.settings.freeMercs) return;
            __result = 0;
        }
    }
}
