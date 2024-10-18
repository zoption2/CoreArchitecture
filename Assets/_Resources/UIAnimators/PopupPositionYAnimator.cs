using UnityEngine;
using System;
using DG.Tweening;

namespace Animation
{
    public class PopupPositionYAnimator : BaseViewAnimator
    {
        [SerializeField] private float appearTime = 0.5f;
        [SerializeField] private float hideTime = 0.5f;
        [SerializeField] private float targetYPosition;
        [SerializeField] private Ease appearEase = Ease.OutBack;
        [SerializeField] private Ease hidingEase = Ease.OutBack;
        [SerializeField] private RectTransform animated;
        private Vector2 _offScreenPosition;


        public override void AnimateShowing(Action onComplete)
        {
            var screenXSize = Screen.width;
            _offScreenPosition = animated.anchoredPosition;

            animated.DOAnchorPosY(targetYPosition, appearTime).SetDelay(_startDelay).SetEase(appearEase).SetId(transform).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }

        public override void AnimateHiding(Action onComplete)
        {
            animated.DOAnchorPosY(_offScreenPosition.x, hideTime).SetEase(hidingEase).SetId(transform).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }
    }
}

