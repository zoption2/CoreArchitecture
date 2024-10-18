using UnityEngine;
using Cysharp.Threading.Tasks;
using MVC;


namespace UI
{
    public interface IPopup : IView
    {
        bool AllowMultipleInstances { get; }
        UniTask InitPopup(Camera camera, Transform parent, int orderLayer = 0);
    }
}

