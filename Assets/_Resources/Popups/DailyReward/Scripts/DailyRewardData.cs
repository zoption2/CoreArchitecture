using System;
using System.Collections.Generic;


namespace DailyReward
{
    [Serializable]
    public class DailyRewardData : ICloneable
    {
        public List<DailyRewardDayData> DaysData = new List<DailyRewardDayData>();

        public object Clone()
        {
            var clone = new DailyRewardData();
            clone.DaysData = new List<DailyRewardDayData>(DaysData);
            return clone;
        }
    }

    [Serializable]
    public class DailyRewardDayData : ICloneable
    {
        public int Day;
        public DateTime RewardDate;
        public bool IsClaimed;

        public object Clone()
        {
            var clone = new DailyRewardDayData();
            clone.Day = Day;
            clone.RewardDate = RewardDate;
            clone.IsClaimed = IsClaimed;
            return clone;
        }
    }
}

