using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DailyReward
{
    public interface IRewardView
    {
        void Init(int value, bool isActive);
    }


    public class RewardView : MonoBehaviour, IRewardView
    {
        private const string kGrayscaleProperty = "_EffectAmount";
        [SerializeField] private RewardType _reward;
        [SerializeField] private TMP_Text _count;
        [SerializeField] private string _countFormat = "x{0}";
        [SerializeField] private Image _icon;

        public void Init(int value, bool isActive)
        {
            _count.text = string.Format(_countFormat, value);
            var material = new Material(_icon.material);
            float propertyValue = isActive
                ? 0f
                : 1f;
            material.SetFloat(kGrayscaleProperty, propertyValue);
            _icon.material = material;
        }
    }
}

