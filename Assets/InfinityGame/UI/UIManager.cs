using System;
using System.Collections.Generic;
using InfinityGame.DesignPattern.ServiceLocator;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        private Dictionary<Type, BaseView> _views = new();
        private BaseView _lastActiveView;

#if UNITY_EDITOR
        private void OnValidate()
        {
            AutoCollectViews();
        }
#endif

        protected void Awake()
        {
            ServiceLocator.Global.Register(this);
            AutoCollectViews();
        }

        private void AutoCollectViews()
        {
            _views.Clear();

            BaseView[] allViews = GetComponentsInChildren<BaseView>(true);
            foreach (var view in allViews)
            {
                var type = view.GetType();
                if (!_views.ContainsKey(type))
                {
                    _views.Add(type, view);
                }
            }
        }

        public void ShowView<T>() where T : BaseView
        {
            if (_views.TryGetValue(typeof(T), out var view))
            {
                view.Show();
                _lastActiveView = view;
            }
            else
            {
                Debug.LogError($"The View Of Type {typeof(T)} is Not Exist");
            }
        }

        public void HideView<T>() where T : BaseView
        {
            if (_views.TryGetValue(typeof(T), out var view))
            {
                if (view.IsVisible())
                    view.Hide();
            }
        }

        public void HideActiveView()
        {
            if (_lastActiveView != null)
            {
                _lastActiveView.Hide();
                _lastActiveView = null;
            }
        }

        public BaseView GetView(Type viewType)
        {
            _views.TryGetValue(viewType, out var view);
            return view;
        }

        public T GetView<T>() where T : BaseView
        {
            if (_views.TryGetValue(typeof(T), out var view))
            {
                return (T)view;
            }

            return null;
        }
    }
}