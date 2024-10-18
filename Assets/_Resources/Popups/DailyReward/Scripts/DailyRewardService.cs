using Cysharp.Threading.Tasks;
using DailyReward.Data;
using System;
using System.Collections.Generic;
using System.Linq;


namespace DailyReward
{
    public interface IDailyRewardServiceTesting
    {
        int GetCurrentDay();
        UniTask ResetCurrentDay();
        UniTask ResetProgress();
        UniTask GoNextDay();
    }


    public interface IDailyRewardService
    {
        event Action ON_RESET;
        bool IsInited { get; }
        UniTask Init();
        int GetCurrentDay();
        List<DailyRewardDayData> GetAllDailyRewardsData();
        DailyRewardDayData GetDailyRewardData(int day);
        TimeSpan GetTimeToNextReward();
        Dictionary<RewardType, int> GetTodayRewards();
        Dictionary<RewardType, int> GetRewardByDay(int day);
        DailyRewardState GetRewardState(int day);
        void GetReward();
        bool HasAvailableReward();
        bool IsRewardAvailable(int day);
        void Update();
    }


    public class DailyRewardService : IDailyRewardService, IDailyRewardServiceTesting
    {
        public event Action ON_RESET;

        private readonly IDailyRewardDataProvider _dataProvider;
        private bool _isInited;
        private DailyRewardData _data;
        private DailyRewardDayData _today;

        public bool IsInited => _isInited;

        public DailyRewardService(IDailyRewardDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async UniTask Init()
        {
            if (_isInited)
            {
                return;
            }

            await _dataProvider.InitAsync();

            DailyRewardData dataHolder = _dataProvider.GetData();
            var dailyRewardData = dataHolder.DaysData;
            var lastDayDate = dailyRewardData[dailyRewardData.Count - 1].RewardDate;

            if (lastDayDate < DateTime.Today)
            {
                for ( var i = 0; i < dailyRewardData.Count; i++ )
                {
                    var data = dailyRewardData[i];
                    data.RewardDate = DateTime.Today.AddDays(i);
                    data.IsClaimed = false;
                }
                _dataProvider.Save(dataHolder);
            }
            _data = dataHolder;
            _today = dataHolder.DaysData.FirstOrDefault(x => x.RewardDate == DateTime.Today);
            _isInited = true;

            await UniTask.CompletedTask;
        }

        public DailyRewardDayData GetDailyRewardData(int day)
        {
            var data = _data.DaysData.FirstOrDefault(x => x.Day == day);
            return data;
        }

        public List<DailyRewardDayData> GetAllDailyRewardsData()
        {
            return _data.DaysData;
        }

        public int GetCurrentDay()
        {
            return _today.Day;
        }

        public bool HasAvailableReward()
        {
            var hasReward = !_today.IsClaimed;
            return hasReward;
        }

        public bool IsRewardAvailable(int day)
        {
            var dayData = _data.DaysData.FirstOrDefault(data => data.Day == day);
            bool isClaimed = dayData.IsClaimed;
            bool isPreviousDay = dayData.Day < _today.Day;
            return !isClaimed && !isPreviousDay;
        }

        public TimeSpan GetTimeToNextReward()
        {
            var nextDay = DateTime.Today.AddDays(1);
            var difference = nextDay - DateTime.UtcNow;
            return difference;
        }

        public Dictionary<RewardType, int> GetTodayRewards()
        {
            var day = _today.Day;
            return _allRewards[day];
        }

        public Dictionary<RewardType, int> GetRewardByDay(int day)
        {
            return _allRewards[day];
        }

        public DailyRewardState GetRewardState(int day)
        {
            DailyRewardState state;
            var dayData = _data.DaysData.FirstOrDefault(x => x.Day == day);

            switch (day)
            {
                case int x when (x == _today.Day):
                    state = dayData.IsClaimed ? DailyRewardState.Completed : DailyRewardState.Current;
                    break;
                case int x when (x < _today.Day):
                    state = dayData.IsClaimed ? DailyRewardState.Completed : DailyRewardState.Missing;
                    break;
                default:
                    state = DailyRewardState.Inactive;
                    break;
            }

            return state;
        }

        public void Update()
        {
            DoOnDataReset();
        }

        private async void DoOnDataReset()
        {
            _isInited = false;
            await Init();
            ON_RESET?.Invoke();
        }

        public void GetReward()
        {
            _today.IsClaimed = true;
            var rewards = _allRewards[_today.Day];
            foreach ( var reward in rewards )
            {
                SavePlayerProgress(reward.Key, reward.Value);
            }
            _dataProvider.Save(_data);
        }

        private bool HasMissingRewardInPeriod()
        {
            int todayIndex = _today.Day;
            var daysData = _data.DaysData;
            for (int i = 0; i < todayIndex; i++)
            {
                if (!daysData[i].IsClaimed)
                {
                    return true;
                }
            }
            return false;
        }

        private void SavePlayerProgress(RewardType reward, int count)
        {
            switch (reward)
            {
                case RewardType.Experience:

                    break;

                case RewardType.Stars:

                    break;

                case RewardType.Cards:

                    break;

                default:
                    break;
            }
        }

        private Dictionary<int, Dictionary<RewardType, int>> _allRewards = new()
        {
            {1, new Dictionary<RewardType, int>() 
                {
                    { RewardType.Stars, 5},
                    { RewardType.Experience, 300 },
                    { RewardType.Cards, 1 }
                } 
            },
            {2, new Dictionary<RewardType, int>()
                {
                    { RewardType.Stars, 10},
                    { RewardType.Experience, 300 },
                    { RewardType.Cards, 1 }
                }
            },
            {3, new Dictionary<RewardType, int>()
                {
                    { RewardType.Stars, 15},
                    { RewardType.Experience, 300 },
                    { RewardType.Cards, 1 }
                }
            },
            {4, new Dictionary<RewardType, int>()
                {
                    { RewardType.Stars, 20},
                    { RewardType.Experience, 300 },
                    { RewardType.Cards, 1 }
                }
            },
            {5, new Dictionary<RewardType, int>()
                {
                    { RewardType.Stars, 25},
                    { RewardType.Experience, 300 },
                    { RewardType.Cards, 1 }
                }
            },
            {6, new Dictionary<RewardType, int>()
                {
                    { RewardType.Stars, 30},
                    { RewardType.Experience, 300 },
                    { RewardType.Cards, 1 }
                }
            },
            {7, new Dictionary<RewardType, int>()
                {
                    { RewardType.Stars, 60},
                    { RewardType.Experience, 300 },
                    { RewardType.Cards, 1 }
                }
            }
        };

        #region Testing
        public async UniTask ResetCurrentDay()
        {
            _today.IsClaimed = false;
            _dataProvider.Save(_data);
            DoOnDataReset();
            UnityEngine.Debug.Log("Daily rewards current day progress reseted!");
        }

        public async UniTask ResetProgress()
        {
            DoOnDataReset();
            UnityEngine.Debug.Log("Daily rewards progress reseted!");
        }

        public async UniTask GoNextDay()
        {
            var currentDay = _today.Day;
            var nextDay = currentDay + 1;
            if (nextDay > 7)
            {
                await ResetProgress();
                return;
            }
            var tempData = _data.DaysData;

            for (int i = 0, j = tempData.Count; i < j; i++)
            {
                var dayData = tempData[i];
                var date = dayData.RewardDate;
                dayData.RewardDate = date.AddDays(-1);
            }
            _dataProvider.Save(_data);

            _today = _data.DaysData.FirstOrDefault(x => x.RewardDate == DateTime.Today);
            DoOnDataReset();

            UnityEngine.Debug.Log("Daily rewards progress reseted!");
        }
        #endregion
    }
}

