using UnityEngine;
using System;
using DG.Tweening;


namespace Animation
{
    public class ScaleYViewAnimator : BaseViewAnimator
    {
        [SerializeField] private float _appearTime = 0.5f;
        [SerializeField] private float _hideTime = 0.5f;
        [SerializeField] private Ease _appearEase = Ease.OutBack;
        [SerializeField] private Ease _hidingEase = Ease.OutBack;
        [SerializeField] private RectTransform _animated;
        [SerializeField] private float _showingDelay;
        [SerializeField] private float _hidingDelay;
        private Sequence _sequence = DOTween.Sequence();



        public override void AnimateShowing(Action onComplete)
        {
            _sequence.Append(ScaleUp());
            _sequence.Join(MoveUp());
            _sequence.SetId(transform);
            _sequence.OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }

        public override void AnimateHiding(Action onComplete)
        {
            _sequence.Append(ScaleDown());
            _sequence.Join(MoveDown());
            _sequence.SetId(transform);
            _sequence.OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }

        private Tween ScaleUp()
        {
            return _animated
                .DOScaleY(1, _appearTime)
                .SetDelay(_showingDelay)
                .SetEase(_appearEase);
        }

        private Tween MoveUp()
        {
            return _animated
                .DOAnchorPosY(-102, _appearTime)
                .SetDelay(_showingDelay)
                .SetEase(_appearEase);
        }

        private Tween ScaleDown()
        {
            return _animated
                .DOScaleY(0, _hideTime)
                .SetDelay(_hidingDelay)
                .SetEase(_hidingEase);
        }

        private Tween MoveDown()
        {
            return _animated
                .DOAnchorPosY(0, _hideTime)
                .SetDelay(_hidingDelay)
                .SetEase(_hidingEase);
        }

        [ContextMenu("AnimateShowing")]
        private void Show_()
        {
            AnimateShowing(null);
        }

        [ContextMenu("AnimateHiding")]
        private void Hide_()
        {
            AnimateHiding(null);
        }
    }
}

