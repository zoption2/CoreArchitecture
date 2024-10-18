using Data;


namespace DailyReward.Data
{
    public interface IDailyRewardDataProvider : IDataProvider<DailyRewardData>
    {

    }


    public class DailyRewardDataProvider
        : BaseJsonDataProvider<DailyRewardData>
        , IDailyRewardDataProvider
    {
        protected override string JsonFileName => "DailyReward";
    }
}

