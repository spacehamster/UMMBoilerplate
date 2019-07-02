using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boilerplate
{
    class FreeRespec
    {
        public static bool patched = false;
        public static void Player_RespecsUsed_Getter_Patch(ref int __result)
        {
            if (!Main.enabled || !Main.settings.freeRespec) return;
            __result = 0;
            Main.logger.Log("Player_RespecsUsed_Getter_Patch called");
        }
        public static bool Player_RespecsUsed_Setter_Patch(ref int value)
        {
            if (!Main.enabled ||!Main.settings.freeRespec) return true;
            Main.logger.Log("Player_RespecsUsed_Setter_Patch called");
            value = 0;
            return true;
        }
    }
}
