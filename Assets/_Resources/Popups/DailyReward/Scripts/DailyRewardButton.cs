using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MVC;

namespace DailyReward
{
    public interface IDailyRewardButton : IView
    {
        int Day { get; }
        DailyRewardState State { get; }
        void Init(IDailyRewardInputHandler inputHandler, string title, DailyRewardState state, bool isToday);
        void ChangeState(DailyRewardState state);
        void Select(bool isSelected);
    }


    public class DailyRewardButton : MonoBehaviour, IDailyRewardButton
    {
        private const int kCurrentStateIndex = 0;
        private const int kCompletedStateIndex = 1;
        private const int kMissingStateIndex = 2;
        private const int kInactiveStateIndex = 3;

        [SerializeField] private int _day;
        [SerializeField] private TMP_Text _dayTitleText;
        [SerializeField] private Image _giftImage;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _frame;
        [SerializeField] private Image _checkmark;
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _notification;
        [SerializeField] private ParticleSystem _effect;

        [SerializeField] private Sprite[] _giftStateSprites;
        [SerializeField] private Sprite[] _backgroundStateSprites;
        [SerializeField] private Sprite[] _frameSprites;
        [SerializeField] private Sprite[] _checkmarkSprites;

        private IDailyRewardInputHandler _inputHandler;
        private DailyRewardState _currentState;
        private Vector3 _giftOriginPosition;
        private Vector3 _giftOriginSize;
        private bool _shouldPlayEffect;
        private bool _isToday;


        public int Day => _day;
        public DailyRewardState State => _currentState;


        public void Init(IDailyRewardInputHandler inputHandler, string title, DailyRewardState state, bool isToday)
        {
            _inputHandler = inputHandler;
            _dayTitleText.text = title;
            _giftOriginPosition = _giftImage.rectTransform.localPosition;
            _giftOriginSize = _giftImage.rectTransform.localScale;
            _isToday = isToday;

            _button.onClick.AddListener(OnButtonClick);
            ChangeStateInternal(state);
        }

        public void Show(Action onShow)
        {
            gameObject.SetActive(true);

            if (_shouldPlayEffect)
            {
                AnimateGift();
                _effect.Play(true);
            }
            onShow?.Invoke();
        }

        public void ChangeState(DailyRewardState state)
        {
            ChangeStateInternal(state);
        }

        public void Select(bool isSelected)
        {
            if (_isToday)
            {
                return;
            }
            
            _frame.gameObject.SetActive(isSelected);
        }

        public void Hide(Action onHide)
        {
            gameObject.SetActive(false);
            _button.onClick.RemoveListener(OnButtonClick);
            DOTween.Kill(transform);
            onHide?.Invoke();
        }

        public void Release()
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            _inputHandler.OnClick(this);
        }

        private void ChangeStateInternal(DailyRewardState state)
        {
            _effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _shouldPlayEffect = false;
            _checkmark.gameObject.SetActive(false);
            _frame.gameObject.SetActive(false);
            _notification.SetActive(false);
            _frame.sprite = _isToday
                ? _frameSprites[0] 
                : _frameSprites[1];

            DOTween.Kill(transform);
            _giftImage.rectTransform.localScale = Vector3.one;
            _giftImage.rectTransform.localPosition = Vector3.zero;
            Sprite giftSprite = null;
            Sprite backSprite = null;
            switch (state)
            {
                case DailyRewardState.Current:
                    giftSprite = _giftStateSprites[kCurrentStateIndex];
                    backSprite = _backgroundStateSprites[kCurrentStateIndex];
                    _frame.sprite = _frameSprites[0];
                    _notification.SetActive(true);
                    _shouldPlayEffect = true;
                    break;
                case DailyRewardState.Completed:
                    giftSprite = _giftStateSprites[kCompletedStateIndex];
                    backSprite = _backgroundStateSprites[kCompletedStateIndex];
                    _checkmark.sprite = _checkmarkSprites[0];
                    _checkmark.gameObject.SetActive(true);
                    break;
                case DailyRewardState.Missing:
                    giftSprite = _giftStateSprites[kMissingStateIndex];
                    backSprite = _backgroundStateSprites[kMissingStateIndex];
                    _checkmark.sprite = _checkmarkSprites[1];
                    _checkmark.gameObject.SetActive(true);
                    break;
                case DailyRewardState.Inactive:
                    giftSprite = _giftStateSprites[kInactiveStateIndex];
                    backSprite = _backgroundStateSprites[kInactiveStateIndex];
                    break;
            }
            _giftImage.sprite = giftSprite;
            _backgroundImage.sprite = backSprite;
            _currentState = state;

            if (_isToday && _currentState != DailyRewardState.Current)
            {
                _frame.gameObject.SetActive(true);
            }
        }

        private void AnimateGift()
        {
            var random = new System.Random();
            DoAnimation();

            void DoAnimation()
            {
                int randomValue = random.Next(2);
                Tween randomTween = randomValue == 0
                    ? AnimateGiftJumping()
                    : AnimateGiftShaking();
                randomTween.SetDelay(1f)
                    .OnComplete(() =>
                {
                    DoAnimation();
                });
            }
        }

        private Tween AnimateGiftJumping()
        {
            float jumpHight = _day == 7 ? 100f : 50f;
            return _giftImage.rectTransform.DOCartoonJumpUpAnchored(jumpHight, 1f)
                .SetId(transform);
        }

        private Tween AnimateGiftShaking()
        {
            return _giftImage.rectTransform.DOJellyRotation(15f, 6, 1)
                .SetId(transform);
        }
    }

    public enum DailyRewardState
    {
        Current = 0,
        Completed = 1,
        Missing = 2,
        Inactive = 3,
    }
}

