using Sango.Core.Tools;

namespace Sango.Core
{
    public class JobType : SangoObject
    {
        public string path;
        public int kind;
        public int costAP;
        public int cost;
        public int meritGain;
        public int tpGain;
        public int limit;
        public int[] recommandFeatures;

        public static int GetJobCost(int jobId)
        {
            JobType t = Scenario.Cur.CommonData.JobTypes.Get(jobId);
            if (t != null)
            {
                OverrideData<int> overrideData = GameUtility.IntOverrideData.Set(t.cost);
                GameEvent.OnGetJobCost?.Invoke(t, t.cost, overrideData);
                return overrideData.Value;
            }
            return 0;
        }
        public static int GetJobCostAP(int jobId)
        {

            JobType t = Scenario.Cur.CommonData.JobTypes.Get(jobId);
            if (t != null)
            {
                OverrideData<int> overrideData = GameUtility.IntOverrideData.Set(t.costAP);
                GameEvent.OnGetJobCost?.Invoke(t, t.costAP, overrideData);
                return overrideData.Value;
            }
            return 0;
        }
        public static int GetJobMeritGain(int jobId)
        {
            JobType t = Scenario.Cur.CommonData.JobTypes.Get(jobId);
            if (t != null)
            {
                OverrideData<int> overrideData = GameUtility.IntOverrideData.Set(t.meritGain);
                GameEvent.OnGetJobCost?.Invoke(t, t.costAP, overrideData);
                return overrideData.Value;
            }
            return 0;
        }
        public static int GetJobTPGain(int jobId)
        {
            JobType t = Scenario.Cur.CommonData.JobTypes.Get(jobId);
            if (t != null)
            {
                OverrideData<int> overrideData = GameUtility.IntOverrideData.Set(t.tpGain);
                GameEvent.OnGetJobCost?.Invoke(t, t.costAP, overrideData);
                return overrideData.Value;
            }
            return 0;
        }
        public static int GetJobLimit(int jobId)
        {
            JobType t = Scenario.Cur.CommonData.JobTypes.Get(jobId);
            if (t != null) return t.limit;
            return 0;
        }
    }
}
