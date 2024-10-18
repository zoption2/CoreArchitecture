using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DailyReward.Addressable;
using MVC;
using UnityEngine;

namespace DailyReward
{
    public interface IDailyRewardPopupController: IBaseMediatedController
    {

    }

    public interface IDailyRewardInputHandler
    {
        void OnClick(IDailyRewardButton button);
    }


    public class DailyRewardPopupController
        : BaseMediatedController<IDailyRewardPopupView, DailyRewardPopupModel, IDailyRewardPopupMediator>
        , IDailyRewardPopupController, IDailyRewardInputHandler
    {
        private const string kLocalizationTable = "DailyRewardPopup";
        private const string kTitleKey = "titleKey";
        private const string kTodayRewardKey = "todayRewardKey";
        private const string kRewardForDayKey = "rewardForDayKey";
        private const string kDayKey = "dayKey";
        private const string kDayTitleFormat = "{0} {1}";

        private readonly IDailyRewardService _service;
        private readonly IDailyRewardPopupProvider _assetsProvider;
        private Dictionary<RewardType, IRewardView> _rewardViews = new(3);
        private IDailyRewardButton _selectedButton;
        private bool _canClick = true;

        public DailyRewardPopupController(ILocalizationManager localizationManager
            , IDailyRewardService service
            , IDailyRewardPopupProvider assetsProvider)
            : base(localizationManager)
        {
            _service = service;
            _assetsProvider = assetsProvider;
        }

        protected override UniTask<DailyRewardPopupModel> BuildModel()
        {
            var model = new DailyRewardPopupModel();
            model.LocalizedTitle = LocalizationManager.GetLocalizedString(kLocalizationTable, kTitleKey);
            model.LocalizedTodayRewardText = LocalizationManager.GetLocalizedString(kLocalizationTable, kTodayRewardKey);
            model.LocalizedRewardForDayFormat = LocalizationManager.GetLocalizedString(kLocalizationTable, kRewardForDayKey);
            int currentDay = _service.GetCurrentDay();

            var localizedDay = LocalizationManager.GetLocalizedString(kLocalizationTable, kDayKey);
            var data = _service.GetAllDailyRewardsData();
            for (int i = 0, j = data.Count; i < j; i++)
            {
                var dayModel = new DaySubmodel();
                int day = data[i].Day;
                dayModel.Day = day;
                dayModel.IsToday = day == currentDay;
                dayModel.Title = string.Format(kDayTitleFormat, localizedDay, day);
                dayModel.State = _service.GetRewardState(day);
                dayModel.Rewards = _service.GetRewardByDay(day);
                model.DaysModels.Add(dayModel);
            }

            return UniTask.FromResult(model);
        }

        protected override async UniTask DoOnInit(IDailyRewardPopupView view)
        {
            view.ON_CLOSE_CLICK += OnCloseClick;
            view.SetTitle(_model.LocalizedTitle);
            view.SetTodayRewardText(_model.LocalizedTodayRewardText);

            var todayDayModel = _model.DaysModels
                .FirstOrDefault(data => data.IsToday == true);
            int day = todayDayModel.Day;
            _selectedButton = _view.DayButtons.FirstOrDefault(button => button.Day == day);
            bool hasReward = _service.IsRewardAvailable(day);
            await LoadRewards(day, hasReward);

            var dayButtons = view.DayButtons;
            for (int i = 0, j = dayButtons.Length; i < j; i++)
            {
                var dayView = dayButtons[i];
                var dayModel = _model.DaysModels[i];
                dayView.Init(this, dayModel.Title, dayModel.State, dayModel.IsToday);
            }

            await UniTask.CompletedTask;
        }

        public override void Show(Action onShow = null)
        {
            var dayButtons = _view.DayButtons;
            for (int i = 0, j = dayButtons.Length; i < j; i++)
            {
                var dayView = dayButtons[i];
                dayView.Show(null);
            }
        }

        public override void Release()
        {
            _rewardViews.Clear();
            _service.Update();
            _view.Release();
        }

        public void SetMediator(DailyRewardPopupMediator mediator)
        {
            _mediator = mediator;
        }

        public async void OnClick(IDailyRewardButton button)
        {
            if (!_canClick)
            {
                return;
            }

            _canClick = false;
            var clickedDayModel = _model.DaysModels.FirstOrDefault(data => data.Day == button.Day);

            if (clickedDayModel.IsToday && clickedDayModel.State == DailyRewardState.Current)
            {
                button.ChangeState(DailyRewardState.Completed);
                clickedDayModel.State = DailyRewardState.Completed;
                _service.GetReward();
            }

            if (_selectedButton.Equals(button))
            {
                _canClick = true;
                return;
            }

            _selectedButton?.Select(false);
            _selectedButton = button;
            int day = button.Day;
            button.Select(true);
            string rewardInfoText = "";
            if (clickedDayModel.IsToday)
            {
                rewardInfoText = _model.LocalizedTodayRewardText;
            }
            else
            {
                rewardInfoText = string.Format(_model.LocalizedRewardForDayFormat, day);
            }
            
            _view.SetTodayRewardText(rewardInfoText);

            bool hasReward = _service.IsRewardAvailable(day);
            await LoadRewards(day, hasReward);
            
            _canClick = true;
        }

        private void OnCloseClick()
        {
            _view.ON_CLOSE_CLICK -= OnCloseClick;
            _mediator.ClosePopup();
        }

        private async UniTask LoadRewards(int day, bool hasReward)
        {
            var rewards = _model.DaysModels
                .FirstOrDefault(data => data.Day == day)
                .Rewards;

            var holder = _view.RewardHolder;

            foreach (var reward in rewards)
            {
                RewardType key = reward.Key;
                if (!_rewardViews.ContainsKey(key))
                {
                    var view = await _assetsProvider.GetRewardView(reward.Key, holder);
                    _rewardViews.Add(key, view);
                }

                IRewardView rewardView = _rewardViews[key];
                rewardView.Init(reward.Value, hasReward);
            }
        }
    }
}

