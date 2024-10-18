using System;
using AssetsLoading;
using Cysharp.Threading.Tasks;
using UI;
using UnityEngine;

namespace MVC
{
    public interface IPopupMediator
    {
        public event Action ON_CLOSE;
        void CreatePopup(Action onComplete = null, int sortingOrder = -1);
        void ClosePopup(Action onComplete = null);
    }


    public abstract class BasePopupMediator<TView, TPopupProvider>
        : IPopupMediator, IPopup
        where TView : IPopupView
        where TPopupProvider : IAddressablePopupProvider
    {
        public event Action ON_CLOSE;

        protected readonly TPopupProvider _popupProvider;
        protected readonly IUIManager _uiManager;
        protected TView _view;

        protected virtual UIBehaviour UIBehaviour => UIBehaviour.StayWithNew;
        public virtual bool AllowMultipleInstances => false;


        public BasePopupMediator(TPopupProvider popupProvider
            , IUIManager uiManager)
        {
            _popupProvider = popupProvider;
            _uiManager = uiManager;
        }


        public virtual async UniTask InitPopup(Camera camera, Transform parent, int orderLayer = 0)
        {
            TView viewGO = await _popupProvider.InstantiatePopupAsync<TView>(parent);
            _view.ON_CLOSE_CLICK += DoOnCloseClick;
            _view.Init(camera, orderLayer);
        }

        public virtual void CreatePopup(Action onComplete = null, int sortingOrder = -1)
        {
            _uiManager.OpenView(this
                , viewBehaviour: UIBehaviour
                , onShow: onComplete
                , manualPriopity: sortingOrder);
        }

        public virtual void ClosePopup(Action onComplete = null)
        {
            _uiManager.CloseView(this, onComplete);
        }

        public virtual void Show(Action onShow)
        {
            _view.Show(onShow);
        }

        public virtual void Hide(Action onHide)
        {
            _view.Hide(onHide);
        }

        public virtual void Close(Action callback = null)
        {
            ClosePopup(() =>
            {
                DoOnClose();
                callback?.Invoke();
            });
        }

        public virtual void Release()
        {
            DoOnClose();
            _view.Release();
            _popupProvider.Clear();
        }

        protected virtual void DoOnClose()
        {
            _view.ON_CLOSE_CLICK -= DoOnCloseClick;
            ON_CLOSE?.Invoke();
        }

        protected void DoOnCloseClick()
        {
            _view.ON_CLOSE_CLICK -= DoOnCloseClick;
            Close();
        }
    }
}

