using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace UI
{
    public enum UIBehaviour
    {
        StayWithNew = 1,
        HideOnNew = 2,
        CloseOnNew = 3,
    }

    public interface IUIManager
    {
        void OpenView(IPopup view, UIBehaviour viewBehaviour = UIBehaviour.StayWithNew, Action onShow = null, int manualPriopity = -1);
        void CloseView(IPopup view, Action onHide = null, params Type[] closeAlso);
        void Clear();
    }


    public class UIManager : IUIManager
    {
        private const int kPriorityStep = 10;

        private Transform _popupsHolder;
        private List<ViewInfo> _popups;
        private int _idCounter = 0;
        private int _priority = 0;

        protected virtual string _goName { get; } = "UIManager";
        private Camera _camera => Camera.main;
        private Transform Holder
        {
            get
            {
                if (_popupsHolder is null)
                {
                    var go = GameObject.Find(_goName);
                    _popupsHolder = go?.transform;
                }

                if (_popupsHolder == null)
                {
                    var go = new GameObject(_goName);
                    _popupsHolder = go.transform;
                    GameObject.DontDestroyOnLoad(go);
                }
                return _popupsHolder;
            }
        }


        public UIManager()
        {
            _popups = new List<ViewInfo>();
        }


        public async void OpenView(IPopup view
            , UIBehaviour viewBehaviour = UIBehaviour.StayWithNew
            , Action onShow = null
            , int manualPriority = -1)
        {

            var viewInfo = new ViewInfo();
            viewInfo.Popup = view;
            viewInfo.Behaviour = viewBehaviour;
            viewInfo.ID = _idCounter;
            _idCounter++;

            var popups = _popups.Count;
            if (popups > 0)
            {
                var previousViewInfo = _popups[popups - 1];
                var previousViewBehaviour = previousViewInfo.Behaviour;
                var previousView = previousViewInfo.Popup;
                switch (previousViewBehaviour)
                {
                    case UIBehaviour.HideOnNew:
                        previousView.Hide(null);
                        break;

                    case UIBehaviour.CloseOnNew:
                        _popups.Remove(previousViewInfo);
                        _priority -= kPriorityStep;
                        previousView.Hide(() =>
                        {
                            previousView.Release();
                        });
                        break;

                    default:
                        break;
                }
            }

            _popups.Add(viewInfo);
            _priority += kPriorityStep;
            var priority = manualPriority == -1 ? _priority : manualPriority;

            await view.InitPopup(_camera, Holder, priority);
            view.Show(()=>
            {
                OnShow();
                onShow?.Invoke();
            });

            void OnShow()
            {
                if (!view.AllowMultipleInstances)
                {
                    var samePopup = _popups
                        .Where(x => x.Popup.GetType() == view.GetType()
                        && x.ID != viewInfo.ID)
                        .FirstOrDefault();
                    if (samePopup != null)
                    {
                        _popups.Remove(samePopup);
                        samePopup.Popup.Release();
                    }
                }
            }
        }

        public void CloseView(IPopup view, Action onHide = null, params Type[] closeAlso)
        {
            TryToCloseAlso();
            var selectedViewInfo = _popups.FirstOrDefault(x => x.Popup == view);
            var selectedView = selectedViewInfo.Popup;
            selectedView.Hide(() =>
            {
                _popups.Remove(selectedViewInfo);
                _priority -= kPriorityStep;
                selectedView.Release();
                OnHide();
                onHide?.Invoke();
            });

            void OnHide()
            {
                var popups = _popups.Count;
                if (popups > 0)
                {
                    var previousViewInfo = _popups[popups - 1];
                    var previousViewBehaviour = previousViewInfo.Behaviour;
                    var previousView = previousViewInfo.Popup;
                    switch (previousViewBehaviour)
                    {
                        case UIBehaviour.HideOnNew:
                            previousView.Show(null);
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    _priority = 0;
                }
            }

            void TryToCloseAlso()
            {
                for (int i = 0, j = closeAlso.Length; i < j; i++)
                {
                    Type popupToClose = closeAlso[i];
                    var viewInfo = _popups.FirstOrDefault(x => x.Popup.GetType() == popupToClose);
                    if (viewInfo is not null)
                    {
                        var view = viewInfo.Popup;
                        _popups.Remove(viewInfo);
                        _priority -= kPriorityStep;
                        view.Release();
                    }
                }
            }
        }

        public void Clear()
        {
            for (int i = 0, j = _popups.Count; i < j; i++)
            {
                _popups[i].Popup.Release();
            }
            _popups.Clear();
            _priority = 0;
            _idCounter = 0;
        }


        private class ViewInfo
        {
            public int ID { get; set; }
            public IPopup Popup { get; set; }
            public UIBehaviour Behaviour { get; set; }
        }
    }
}

