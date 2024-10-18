using DailyReward.Addressable;
using DailyReward.Data;
using UnityEngine;
using Zenject;

namespace DailyReward
{
    [ContextInstaller]
    public class DailyRewardContextInstaller : MonoInstaller
    {
        [SerializeField] private DailyRewardPopupProvider _assetsProvider;

        public override void InstallBindings()
        {
            Container.Bind<IDailyRewardService>().To<DailyRewardService>().AsSingle();
            Container.Bind<IDailyRewardPopupMediator>().To<DailyRewardPopupMediator>().AsTransient();
            Container.Bind<IDailyRewardPopupController>().To<DailyRewardPopupController>().AsTransient();
            Container.Bind<IDailyRewardDataProvider>().To<DailyRewardDataProvider>().AsSingle();
            Container.Bind<IDailyRewardPopupProvider>().FromInstance(_assetsProvider);
        }
    }
}

