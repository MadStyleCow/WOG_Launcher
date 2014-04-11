using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Core.Classes;

namespace Client.Core.Utilities
{
    public static class Converters
    {
        public static List<Addon> ToAddonList(DSServerAddons[] pDSAddonArray, String pBaseURL)
        {
            List<Addon> ServerAddonList = new List<Addon>();

            foreach (DSServerAddons DSAddon in pDSAddonArray)
            {
                ServerAddonList.Add(new Addon()
                    {
                        Name = DSAddon.Pbo,
                        Hash = DSAddon.Md5,
                        Size = DSAddon.Size,
                        RelativePath = DSAddon.Path,
                        AbsoluteURL = String.Format("{0}/{1}", pBaseURL, DSAddon.Url),
                        Status = false
                    }); 
            }
            return ServerAddonList;
        }

        public static List<Mod> ToModList(DSServerMods[] pDSModArray)
        {
            List<Mod> ServerModList = new List<Mod>();

            foreach (DSServerMods DSMod in pDSModArray)
            {
                ServerModList.Add(new Mod()
                    {
                        Name = DSMod.Name
                    });
            }
            return ServerModList;
        }
    }
}
