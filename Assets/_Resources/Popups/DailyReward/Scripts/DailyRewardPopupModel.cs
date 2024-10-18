using System.Collections.Generic;
using MVC;

namespace DailyReward
{
    public class DailyRewardPopupModel : IModel
    {
        public string LocalizedTitle { get; set; }
        public string LocalizedTodayRewardText { get; set; }
        public string LocalizedRewardForDayFormat{ get; set; }
        public List<DaySubmodel> DaysModels { get; set; } = new(7);
    }

    public class DaySubmodel
    {
        public string Title { get; set; }
        public bool IsToday { get; set; }
        public int Day { get; set; }
        public DailyRewardState State { get; set; }
        public Dictionary<RewardType, int> Rewards { get; set; }
    }
}

