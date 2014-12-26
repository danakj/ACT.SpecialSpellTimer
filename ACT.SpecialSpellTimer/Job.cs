namespace ACT.SpecialSpellTimer
{
    using System.Collections.Generic;
    using System.Linq;

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
        /// ロール
        /// </summary>
        public JobRoles Role { get; set; }

        /// <summary>
        /// ジョブリストを取得する
        /// </summary>
        /// <returns>
        /// ジョブリスト</returns>
        public static Job[] GetJobList()
        {
            var list = new List<Job>();
            list.Add(new Job() { JobId = 1, JobName = "GLD", Role = JobRoles.Tank });
            list.Add(new Job() { JobId = 2, JobName = "PUG", Role = JobRoles.MeleeDPS });
            list.Add(new Job() { JobId = 3, JobName = "MRD", Role = JobRoles.Tank });
            list.Add(new Job() { JobId = 4, JobName = "LNC", Role = JobRoles.MeleeDPS });
            list.Add(new Job() { JobId = 5, JobName = "ARC", Role = JobRoles.RangeDPS });
            list.Add(new Job() { JobId = 6, JobName = "CNJ", Role = JobRoles.Healer });
            list.Add(new Job() { JobId = 7, JobName = "THM", Role = JobRoles.RangeDPS });
            list.Add(new Job() { JobId = 8, JobName = "CRP", Role = JobRoles.Crafter });
            list.Add(new Job() { JobId = 9, JobName = "BSM", Role = JobRoles.Crafter });
            list.Add(new Job() { JobId = 10, JobName = "ARM", Role = JobRoles.Crafter });
            list.Add(new Job() { JobId = 11, JobName = "GSM", Role = JobRoles.Crafter });
            list.Add(new Job() { JobId = 12, JobName = "LTW", Role = JobRoles.Crafter });
            list.Add(new Job() { JobId = 13, JobName = "WVR", Role = JobRoles.Crafter });
            list.Add(new Job() { JobId = 14, JobName = "ALC", Role = JobRoles.Crafter });
            list.Add(new Job() { JobId = 15, JobName = "CUL", Role = JobRoles.Crafter });
            list.Add(new Job() { JobId = 16, JobName = "MIN", Role = JobRoles.Gatherer });
            list.Add(new Job() { JobId = 17, JobName = "BOT", Role = JobRoles.Gatherer });
            list.Add(new Job() { JobId = 18, JobName = "FSH", Role = JobRoles.Gatherer });
            list.Add(new Job() { JobId = 19, JobName = "WAR", Role = JobRoles.Tank });
            list.Add(new Job() { JobId = 20, JobName = "MNK", Role = JobRoles.MeleeDPS });
            list.Add(new Job() { JobId = 21, JobName = "PLD", Role = JobRoles.Tank });
            list.Add(new Job() { JobId = 22, JobName = "DRG", Role = JobRoles.MeleeDPS });
            list.Add(new Job() { JobId = 23, JobName = "BRD", Role = JobRoles.RangeDPS });
            list.Add(new Job() { JobId = 24, JobName = "WHM", Role = JobRoles.Healer });
            list.Add(new Job() { JobId = 25, JobName = "BLM", Role = JobRoles.RangeDPS });
            list.Add(new Job() { JobId = 26, JobName = "ACN", Role = JobRoles.RangeDPS });
            list.Add(new Job() { JobId = 27, JobName = "SMN", Role = JobRoles.RangeDPS });
            list.Add(new Job() { JobId = 28, JobName = "SCH", Role = JobRoles.Healer });
            list.Add(new Job() { JobId = 29, JobName = "ROG", Role = JobRoles.MeleeDPS });
            list.Add(new Job() { JobId = 30, JobName = "NIN", Role = JobRoles.MeleeDPS });

            return list.ToArray();
        }

        /// <summary>
        /// ジョブIDからジョブ名を取得する
        /// </summary>
        /// <param name="jobID">ジョブID</param>
        /// <returns>ジョブ名</returns>
        public static string GetJobName(
            int jobID)
        {
            var jobList = GetJobList();
            return jobList
                .Where(x => x.JobId == jobID)
                .Select(x => x.JobName)
                .FirstOrDefault() ?? string.Empty;
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            return Utility.Translate.Get(this.JobName);
        }
    }

    public enum JobRoles
    {
        Tank = 10,
        Healer = 20,
        DPS = 30,
        MeleeDPS = 31,
        RangeDPS = 32,
        Crafter = 40,
        Gatherer = 50,
    }
}
