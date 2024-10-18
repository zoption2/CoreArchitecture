using UnityEngine;
using System;
using DG.Tweening;

namespace Animation
{
    public class MoveYViewAnimator : BaseViewAnimator
    {
        [SerializeField] private float _showingYPosition;
        [SerializeField] private float _hidedYPosition;
        [SerializeField] private float _appearTime = 0.5f;
        [SerializeField] private float _hideTime = 0.5f;
        [SerializeField] private Ease appearEase = Ease.OutBack;
        [SerializeField] private Ease hidingEase = Ease.OutBack;
        [SerializeField] private RectTransform _animated;
        [SerializeField] private float _showingDelay;
        [SerializeField] private float _hidingDelay;


        public override void AnimateShowing(Action onComplete)
        {
            _animated
                .DOAnchorPosY(_showingYPosition, _appearTime)
                .SetDelay(_showingDelay)
                .SetEase(appearEase)
                .SetId(this)
                .OnComplete(() =>
                {
                    onComplete?.Invoke();
                });
        }

        public override void AnimateHiding(Action onComplete)
        {
            _animated
                .DOAnchorPosY(_hidedYPosition, _hideTime)
                .SetDelay(_hidingDelay)
                .SetEase(hidingEase)
                .SetId(this)
                .OnComplete(() =>
                {
                    onComplete?.Invoke();
                });
        }

        public override void ShowImmediately()
        {
            base.ShowImmediately();
            var position = _animated.anchoredPosition;
            position.y = _showingYPosition;
            _animated.anchoredPosition = position;
        }

        public override void HideImmediately()
        {
            base.HideImmediately();
            var position = _animated.anchoredPosition;
            position.y = _hidedYPosition;
            _animated.anchoredPosition = position;
        }
    }
}

