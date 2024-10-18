using UnityEngine;
using System;
using System.Collections.Generic;

namespace Animation
{
    public class QueueAnimatorController : BaseViewAnimator
    {
        [SerializeField] private List<BaseViewAnimator> _showingAnimators;
        [SerializeField] private List<BaseViewAnimator> _hidingAnimators;

        public override void AnimateShowing(Action onComplete)
        {
            if (_showingAnimators.Contains(this))
            {
                _showingAnimators.Remove(this);
            }

            var animatorsCount = _showingAnimators.Count;
            int counter = 0;
            Animate();

            void Animate()
            {
                _showingAnimators[counter].AnimateShowing(()=>
                {
                    counter++;
                    if (counter < animatorsCount)
                    {
                        Animate();
                    }
                    else
                    {
                        onComplete?.Invoke();
                    }
                });
            }
        }

        public override void AnimateHiding(Action onComplete)
        {
            if (_showingAnimators.Contains(this))
            {
                _showingAnimators.Remove(this);
            }

            var animatorsCount = _hidingAnimators.Count;
            int counter = 0;
            Animate();

            void Animate()
            {
                _hidingAnimators[counter].AnimateShowing(() =>
                {
                    counter++;
                    if (counter < animatorsCount)
                    {
                        Animate();
                    }
                    else
                    {
                        onComplete?.Invoke();
                    }
                });
            }
        }
    }
}

