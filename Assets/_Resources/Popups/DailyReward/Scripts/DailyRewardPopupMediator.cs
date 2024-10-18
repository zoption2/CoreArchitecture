using Cysharp.Threading.Tasks;
using MVC;
using UI;
using System;
using UnityEngine;
using DailyReward.Addressable;

namespace DailyReward
{
    public interface IDailyRewardPopupMediator : IPopupMediator
    {
        void RemoveCloseButton();
    }


    public class DailyRewardPopupMediator : BasePopupMediator<IDailyRewardPopupView, IDailyRewardPopupProvider>
        , IDailyRewardPopupMediator
    {
        private readonly IDailyRewardPopupController _controller;
        private bool _closeButtonRemoved = false;

        protected override UIBehaviour UIBehaviour => UIBehaviour.CloseOnNew;

        public DailyRewardPopupMediator(IDailyRewardPopupProvider refsHolder
            , IUIManager uiManager
            , IDailyRewardPopupController controller)
            : base(refsHolder, uiManager)
        {
            _controller = controller;
        }


        public override async UniTask InitPopup(Camera camera, Transform parent, int orderLayer = 0)
        {
            await base.InitPopup(camera, parent, orderLayer);
            if (_closeButtonRemoved)
            {
                _view.RemoveCloseButton();
            }

            await _controller.Init(_view, this);
        }

        public override void Show(Action onShow)
        {
            base.Show(onShow);
            _controller.Show(null);
        }

        public void RemoveCloseButton()
        {
            _closeButtonRemoved = true;
        }

        public override void Release()
        {
            DoOnClose();
            _controller.Release();
        }

        public void ClosePopup()
        {
            Close();
        }

        protected override void DoOnClose()
        {
            base.DoOnClose();
            Debug.Log("DailyReward closed");
        }
    }
}

