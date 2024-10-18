using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;


namespace MVC
{
    public interface IBaseMediatedController : IView
    {
        UniTask Init(IView view, IPopupMediator mediator);
    }


    public abstract class BaseMediatedController<TView, TModel, TMediator> : IBaseMediatedController
        where TView : IView
        where TModel : IModel
        where TMediator : IPopupMediator
    {
        protected TView _view;
        protected TModel _model;
        protected TMediator _mediator;
        protected ILocalizationManager _localizationManager;
        protected List<IDisposable> _disposable = new();
        protected System.Random _random;

        public BaseMediatedController(ILocalizationManager localizationManager)
        {
            _localizationManager = localizationManager;
            _random = new System.Random();
        }

        public async UniTask Init(IView view, IPopupMediator mediator)
        {
            _view = (TView)view;
            _mediator = (TMediator)mediator;
            _model = await BuildModel();
            await DoOnInit(_view);

            _localizationManager.OnLanguageChanged -= UpdateLocalization;
            _localizationManager.OnLanguageChanged += UpdateLocalization;
        }

        public virtual void Show(Action onShow = null)
        { }

        public virtual void Hide(Action onHide)
        { }

        public virtual void Release()
        {
            _localizationManager.OnLanguageChanged -= UpdateLocalization;
            ReleaseDisposable();
        }

        protected abstract UniTask DoOnInit(TView view);
        protected abstract UniTask<TModel> BuildModel();

        protected virtual void UpdateLocalization()
        { }

        protected void ReleaseDisposable()
        {
            for (int i = 0, j = _disposable.Count; i < j; i++)
            {
                _disposable[i].Dispose();
            }
        }
    }
}

