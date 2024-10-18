using UnityEngine;
using System;


namespace Animation
{
    public class ParticleEffectsAnimator : BaseViewAnimator
    {
        [SerializeField] private ParticleSystem _particleSystem;

        public override void AnimateShowing(Action onComplete)
        {
            _particleSystem.Play(true);
            onComplete?.Invoke();
        }

        public override void AnimateHiding(Action onComplete)
        {
            _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            onComplete?.Invoke();
        }
    }
}

