using UnityEngine;
using System;
using DG.Tweening;

namespace Animation
{
    public interface IViewAnimator
    {
        public void AnimateShowing(Action onComplete);
        public void AnimateHiding(Action onComplete);
        void ShowImmediately();
        void HideImmediately();
    }

    public abstract class BaseViewAnimator : MonoBehaviour, IViewAnimator
    {
        [SerializeField] protected float _startDelay = 0;
        public abstract void AnimateHiding(Action onComplete);
        public abstract void AnimateShowing(Action onComplete);

        public virtual void ShowImmediately()
        { }

        public virtual void HideImmediately()
        { }

        protected virtual void OnDisable()
        {
            DOTween.Kill(this);
        }
    }
}

