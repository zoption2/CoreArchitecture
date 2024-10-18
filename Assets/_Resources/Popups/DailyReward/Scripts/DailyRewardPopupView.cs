using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UI;
using Animation;

namespace DailyReward
{
    public interface IDailyRewardPopupView : IPopupView
    {
        Transform RewardHolder { get; }
        IDailyRewardButton[] DayButtons { get; }
        void SetTitle(string title);
        void SetTodayRewardText(string text);
        void RemoveCloseButton();
    }



    public class DailyRewardPopupView : MonoBehaviour, IDailyRewardPopupView
    {
        public event Action ON_CLOSE_CLICK;

        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _todayRewardText;
        [SerializeField] private DailyRewardButton[] _dayButtons;
        [SerializeField] private Transform _rewardHolder;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private BaseViewAnimator _animator;

        public Transform RewardHolder => _rewardHolder;
        public IDailyRewardButton[] DayButtons => _dayButtons;


        public void Init(Camera camera, int orderLayer)
        {
            _canvas.worldCamera = camera;
            _canvas.sortingOrder = orderLayer;
            gameObject.SetActive(false);
        }

        public void Show(Action onShow)
        {
            Debug.Log("DailyRewardPopup show() called");
            gameObject.SetActive(true);
            _animator.AnimateShowing(() =>
            {
                _exitButton.onClick.AddListener(OnCloseButtonClick);
                onShow?.Invoke();
            });
        }

        public void RemoveCloseButton()
        {
            _exitButton.gameObject.SetActive(false);
        }

        public void Hide(Action onHide)
        {
            _exitButton.onClick.RemoveListener(OnCloseButtonClick);
            _animator.AnimateHiding(() =>
            {
                gameObject.SetActive(false);
                onHide?.Invoke();
            });
        }

        public void Release()
        {
            Destroy(gameObject);
        }

        public void SetTitle(string title)
        {
            _titleText.text = title;
        }

        public void SetTodayRewardText(string text)
        {
            _todayRewardText.text = text;
        }

        private void OnCloseButtonClick()
        {
            ON_CLOSE_CLICK?.Invoke();
        }
    }
}

