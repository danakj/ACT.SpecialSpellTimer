using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACT.SpecialSpellTimer.Utility
{
    class Language
    {
        public String FriendlyName { get; set; }
        public String Value { get; set; }

        public override String ToString()
        {
            return FriendlyName;
        }

        public static Language[] GetLanguageList()
        {
            return new Language[] {
                new Language { FriendlyName = "English", Value = "EN" },
                new Language { FriendlyName = "日本語", Value = "JP" },
            };
        }
    }
}
