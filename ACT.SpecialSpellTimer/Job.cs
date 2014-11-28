namespace ACT.SpecialSpellTimer
{
    using System.Collections.Generic;

    /// <summary>
    /// ジョブ
    /// </summary>
    public class Job
    {
        /// <summary>
        /// JobId
        /// </summary>
        public int JobId { get; set; }

        /// <summary>
        /// JobName
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// ジョブリストを取得する
        /// </summary>
        /// <returns>
        /// ジョブリスト</returns>
        public static Job[] GetJobList()
        {
            var list = new List<Job>();
            list.Add(new Job() { JobId = 1, JobName = "剣術士" });
            list.Add(new Job() { JobId = 2, JobName = "格闘士" });
            list.Add(new Job() { JobId = 3, JobName = "斧術士" });
            list.Add(new Job() { JobId = 4, JobName = "槍術士" });
            list.Add(new Job() { JobId = 5, JobName = "弓術士" });
            list.Add(new Job() { JobId = 6, JobName = "幻術士" });
            list.Add(new Job() { JobId = 7, JobName = "呪術士" });
            list.Add(new Job() { JobId = 8, JobName = "木工師" });
            list.Add(new Job() { JobId = 9, JobName = "鍛冶師" });
            list.Add(new Job() { JobId = 10, JobName = "甲冑師" });
            list.Add(new Job() { JobId = 11, JobName = "彫金師" });
            list.Add(new Job() { JobId = 12, JobName = "革細工師" });
            list.Add(new Job() { JobId = 13, JobName = "裁縫師" });
            list.Add(new Job() { JobId = 14, JobName = "錬金術師" });
            list.Add(new Job() { JobId = 15, JobName = "調理師" });
            list.Add(new Job() { JobId = 16, JobName = "採掘師" });
            list.Add(new Job() { JobId = 17, JobName = "園芸師" });
            list.Add(new Job() { JobId = 18, JobName = "漁師" });
            list.Add(new Job() { JobId = 19, JobName = "ナイト" });
            list.Add(new Job() { JobId = 20, JobName = "モンク" });
            list.Add(new Job() { JobId = 21, JobName = "戦士" });
            list.Add(new Job() { JobId = 22, JobName = "竜騎士" });
            list.Add(new Job() { JobId = 23, JobName = "吟遊詩人" });
            list.Add(new Job() { JobId = 24, JobName = "白魔道士" });
            list.Add(new Job() { JobId = 25, JobName = "黒魔道士" });
            list.Add(new Job() { JobId = 26, JobName = "巴術士" });
            list.Add(new Job() { JobId = 27, JobName = "召喚士" });
            list.Add(new Job() { JobId = 28, JobName = "学者" });
            list.Add(new Job() { JobId = 29, JobName = "双剣士" });
            list.Add(new Job() { JobId = 30, JobName = "忍者" });

            return list.ToArray();
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            return this.JobName;
        }
    }
}
