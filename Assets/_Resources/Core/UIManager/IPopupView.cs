using System;
using UnityEngine;
using MVC;


namespace UI
{
    public interface IPopupView : IView
    {
        event Action ON_CLOSE_CLICK;
        void Init(Camera camera, int priority);
    }
}

