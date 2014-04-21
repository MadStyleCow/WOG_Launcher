using System.Collections.Generic;
using System.Linq;
using Client.Core.Classes;

namespace Client.Core.Utilities
{
    public static class Converters
    {
        public static List<Addon> ToAddonList(IEnumerable<DSServerAddons> pDsAddonArray)
        {
            return pDsAddonArray.Select(dsAddon => new Addon
            {
                Name = dsAddon.Pbo, Hash = dsAddon.Md5, Size = dsAddon.Size, RelativePath = dsAddon.Path, RelativeUrl = dsAddon.Url, Status = false
            }).ToList();
        }

        public static List<Mod> ToModList(IEnumerable<DSServerMods> pDsModArray)
        {
            return pDsModArray.Select(dsMod => new Mod
            {
                Name = dsMod.Name
            }).ToList();
        }
    }
}
