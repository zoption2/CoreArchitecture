using UnityEngine;
using System;
using DG.Tweening;

namespace Animation
{
    public class ScaleXAnimator : BaseViewAnimator
    {
        [SerializeField] private Transform _objectToScale;
        [SerializeField] private float _startingScaleX = 0;
        [SerializeField] private float _endScaleX = 1;
        [SerializeField] private float _appearTime = 0.5f;
        [SerializeField] private float _hideTime = 0.5f;
        [SerializeField] private Ease _appearEase = Ease.OutBack;
        [SerializeField] private Ease _hideEase = Ease.OutExpo;

        public override void AnimateShowing(Action onComplete)
        {
            var tempScale = _objectToScale.localScale;
            tempScale.x = 0;
            _objectToScale.localScale = tempScale;
            _objectToScale.DOScaleX(_endScaleX, _appearTime).SetEase(_appearEase).SetId(transform).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }

        public override void AnimateHiding(Action onComplete)
        {
            _objectToScale.DOScaleX(_startingScaleX, _hideTime).SetEase(_hideEase).SetId(transform).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }
    }
}

