namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    /// <summary>
    /// Update Checker
    /// </summary>
    public static class UpdateChecker
    {
        /// <summary>
        /// 最終リリースのURL
        /// </summary>
        public const string LastestReleaseUrl = @"https://github.com/anoyetta/ACT.SpecialSpellTimer/releases/latest";

        /// <summary>
        /// 製品名
        /// </summary>
        public static string ProductName
        {
            get
            {
                var product = (AssemblyProductAttribute)Attribute.GetCustomAttribute(
                    Assembly.GetExecutingAssembly(),
                    typeof(AssemblyProductAttribute));

                return product.Product;
            }
        }

        /// <summary>
        /// アップデートを行う
        /// </summary>
        /// <returns>メッセージ</returns>
        public static string Update()
        {
            var r = string.Empty;

            try
            {
                var html = string.Empty;

                // lastest releaseページを取得する
                using (var wc = new WebClient())
                {
                    using (var stream = wc.OpenRead(LastestReleaseUrl))
                    using (var sr = new StreamReader(stream))
                    {
                        html = sr.ReadToEnd();
                    }
                }

                var lastestReleaseVersion = string.Empty;

                // バージョン情報（lastest releaseのタイトル）を抜き出すパターンを編集する
                var pattern = string.Empty;
                pattern += @"<h1 class=""release-title"".*?>.*?";
                pattern += @"<a href="".*?"".*?>";
                pattern += @"(?<ReleaseTitle>.*?)";
                pattern += @"</a>.*?";
                pattern += @"</h1>";

                // バージョン情報を抜き出す
                var regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                for (Match m = regex.Match(html); m.Success; m = m.NextMatch())
                {
                    lastestReleaseVersion = m.Groups["ReleaseTitle"].Value.Trim();
                }

                if (string.IsNullOrWhiteSpace(lastestReleaseVersion))
                {
                    return r;
                }

                var values = lastestReleaseVersion.Replace("v", string.Empty).Split('.');
                var remoteVersion = new Version(
                    values.Length > 0 ? int.Parse(values[0]) : 0,
                    values.Length > 1 ? int.Parse(values[1]) : 0,
                    0,
                    values.Length > 2 ? int.Parse(values[2]) : 0);

                // 現在のバージョンを取得する
                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

                if (remoteVersion <= currentVersion)
                {
                    return r;
                }

                var prompt = string.Empty;
                prompt += String.Format(Utility.Translate.Get("NewVersionReleased"), ProductName) + Environment.NewLine;
                prompt += "now: " + "v" + currentVersion.Major.ToString() + "." + currentVersion.Minor.ToString() + "." + currentVersion.Revision.ToString() + Environment.NewLine;
                prompt += "new: " + lastestReleaseVersion + Environment.NewLine;
                prompt += Environment.NewLine;
                prompt += Utility.Translate.Get("DownloadPrompt");

                if (MessageBox.Show(prompt, ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) !=
                    DialogResult.Yes)
                {
                    return r;
                }

                Process.Start(LastestReleaseUrl);
            }
            catch (Exception ex)
            {
                r = String.Format(Utility.Translate.Get("NewVersionError"), ex.ToString());
            }

            return r;
        }
    }
}
