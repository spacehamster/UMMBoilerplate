namespace Boilerplate
{
    class FreeMerc
    {
        public static void Player_GetCustomCompanionCost_Patch(ref int __result)
        {
            if (!Main.enabled) return;
            if (!Main.settings.freeMercs) return;
            __result = 0;
        }
    }
}
