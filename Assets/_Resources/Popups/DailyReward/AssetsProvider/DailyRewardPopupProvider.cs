using AssetsLoading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;


namespace DailyReward.Addressable
{

    public interface IDailyRewardPopupProvider : IAddressablePopupProvider
    {
        UniTask<IRewardView> GetRewardView(RewardType reward, Transform parent);
    }


    ///Provider for all addressable assets for Daily reward popup
    [CreateAssetMenu(fileName = "DailyRewardPopupProvider", menuName = "AddressablePopupProviders/DailyReward")]
    public sealed class DailyRewardPopupProvider : AddressablePopupProvider
        , IDailyRewardPopupProvider
    {
        [SerializeField] private List<RewardMapper> _rewards;

        public async UniTask<IRewardView> GetRewardView(RewardType reward, Transform parent)
        {
            var mapper = _rewards.Find(map => map.Reward == reward);
            GameObject rewardPrefab = await mapper.Loader.LoadAsync();
            GameObject instance = Instantiate(rewardPrefab, parent);
            IRewardView result = instance.GetComponent<IRewardView>();
            return result;
        }

        public override void Clear()
        {
            base.Clear();
            for (int i = 0, j = _rewards.Count; i < j; i++)
            {
                _rewards[i].Loader.Clear();
            }
        }

        [System.Serializable]
        private class RewardMapper
        {
            [field: SerializeField] public RewardType Reward { get; private set; }
            [field: SerializeField] public AddressableGameobjectsLoader Loader { get; private set; }
        }

    }
}
