using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Advanced_Combat_Tracker;

namespace ACT.SpecialSpellTimer.Utility
{
    class Translate
    {
        static System.Resources.ResourceManager Language;

        public static System.Resources.ResourceManager GetTranslationsFor(String name)
        {
            switch (name)
            {
                case "EN":
                    return resources.strings.Strings_EN.ResourceManager;
                case "JP":
                    return resources.strings.Strings_JP.ResourceManager;
            }
            ActGlobals.oFormActMain.WriteExceptionLog(
                new Exception(),
                "Unknown language: " + Properties.Settings.Default.Language);
            return resources.strings.Strings_JP.ResourceManager;
        }

        private static void InitializeIfNeeded()
        {
            if (Language != null)
                return;
            Language = GetTranslationsFor(Properties.Settings.Default.Language);
        }

        public static String Get(String name)
        {
            InitializeIfNeeded();

            if (name == "")
                return name;
            if (name.StartsWith("__"))
                return name.Substring(2);
            int a;
            if (int.TryParse(name, out a))
                return name;
            String s = Language.GetString(name);
            if (s == null)
                throw new Exception("Unable to find translation for " + name);
            return s.Replace("\\n", Environment.NewLine);
        }

        public static void TranslateControls(System.Windows.Forms.Control control)
        {
            control.Text = Get(control.Text);
            foreach (System.Windows.Forms.Control c in control.Controls)
                TranslateControls(c);

            // Controls may have a context menu, these are not controls but they do have Text.
            if (control.ContextMenuStrip != null)
            {
                foreach (System.Windows.Forms.ToolStripItem c in control.ContextMenuStrip.Items)
                    c.Text = Get(c.Text);
            }

            // Controls that are tables may have column headers that are not controls but have Text.
            if (control is System.Windows.Forms.ListView) {
                System.Windows.Forms.ListView listview = (System.Windows.Forms.ListView)control;
                foreach (System.Windows.Forms.ColumnHeader c in listview.Columns)
                    c.Text = Get(c.Text);
            }

            // Controls that are combo boxes have items which need translation.
            if (control is System.Windows.Forms.ComboBox)
            {
                System.Windows.Forms.ComboBox combo = (System.Windows.Forms.ComboBox)control;
                for (int i = 0; i < combo.Items.Count; ++i) {
                    if (combo.Items[i] is String)
                        combo.Items[i] = Get((String)combo.Items[i]);
                }
            }
        }
    }
}
