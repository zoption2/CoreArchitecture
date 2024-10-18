using UnityEngine;
using System;
using DG.Tweening;


namespace Animation
{
    public class LightStripeAnimator : BaseViewAnimator
    {
        private const string kWideProperty = "_StripeWidth";

        [SerializeField] private float _showingTime = 0.5f;
        [SerializeField] private float _hidingTime = 0.3f;
        [SerializeField] private Material _material;

        public override void AnimateShowing(Action onComplete)
        {
            _material.SetFloat(kWideProperty, 0);
            DOVirtual.Float(0, 1, _showingTime, ChangeStripeWidth)
                .SetId(transform)
                .SetEase(Ease.Linear)
                .OnComplete(()=>
                {
                    onComplete?.Invoke();
                });
        }

        public override void AnimateHiding(Action onComplete)
        {
            DOVirtual.Float(1, 0, _hidingTime, ChangeStripeWidth)
                .SetId(transform)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    onComplete?.Invoke();
                });
        }

        private void ChangeStripeWidth(float value)
        {
            _material.SetFloat(kWideProperty, value);
        }
    }
}

