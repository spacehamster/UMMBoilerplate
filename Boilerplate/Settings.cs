using UnityModManagerNet;

namespace Boilerplate
{
    public class Settings : UnityModManager.ModSettings
    {
        public bool freeMercs = true;
        public bool freeRespec = true;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
}
