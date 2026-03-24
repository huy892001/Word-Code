using DG.Tweening;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace UI
{
    public enum AnimationType
    {
        None,
        Fade,
        Scale,
        Position
    }

    [System.Serializable]
    public class AnimationSettings
    {
        [EnumToggleButtons, LabelText("Animation")]
        public AnimationType AnimationType = AnimationType.Fade;

        [ShowIf(nameof(ShowCommon))] [LabelText("Duration")]
        public float Duration = 0.5f;

        [ShowIf(nameof(ShowCommon))] [LabelText("Ease")]
        public Ease Ease = Ease.OutQuad;

        [ShowIf(nameof(IsScale))] [LabelText("Hide scale")]
        public Vector3 HiddenScale = Vector3.zero;

        [ShowIf(nameof(IsScale))] [LabelText("Show scale")]
        public Vector3 ShownScale = Vector3.one;

        [ShowIf(nameof(IsPosition))] [LabelText("Start position")]
        public Vector2 StartPos = Vector2.zero;

        [ShowIf(nameof(IsPosition))] [LabelText("End position")]
        public Vector2 EndPos = Vector2.zero;

        private bool ShowCommon => AnimationType != AnimationType.None;
        private bool IsScale => AnimationType == AnimationType.Scale;
        private bool IsPosition => AnimationType == AnimationType.Position;
    }

    public abstract class BaseView : MonoBehaviour
    {
        [FoldoutGroup("Animation Settings")] [LabelText("Show Animation")] [SerializeField]
        private AnimationSettings _showAnim = new AnimationSettings();

        [FoldoutGroup("Animation Settings")] [LabelText("Hide Animation")] [SerializeField]
        private AnimationSettings _hideAnim = new AnimationSettings();

        [SerializeField] private bool _isShowOnAwake = false;

        private Canvas _canvas;
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;

        [SerializeField, LabelText("View Active")]
        private bool _isActiveView;

        public bool IsActiveView => _isActiveView;

        protected virtual void Awake()
        {
            Init();
            if (_isShowOnAwake) Show();
        }

        private void Init()
        {
            if (_canvas != null) return;

            _canvas = GetComponent<Canvas>();
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();

            if (_canvas == null) _canvas = gameObject.AddComponent<Canvas>();
            if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();

            _rectTransform.anchoredPosition = Vector2.zero;

            _canvas.enabled = false;
            _canvasGroup.alpha = 0f;
            _rectTransform.localScale = _showAnim.ShownScale;
        }


        private void OnValidate()
        {
            if (GetComponent<Canvas>() == null)
            {
                var c = gameObject.AddComponent<Canvas>();
                c.overrideSorting = true;
            }

            if (GetComponent<RectTransform>() == null)
            {
                gameObject.AddComponent<RectTransform>();
            }

            if (GetComponent<CanvasGroup>() == null)
            {
                gameObject.AddComponent<CanvasGroup>();
            }

            var graphic = gameObject.GetComponent<GraphicRaycaster>() ?? gameObject.AddComponent<GraphicRaycaster>();
        }

        public virtual void Show()
        {
            Init();
            gameObject.SetActive(true);
            _canvas.enabled = true;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
            _isActiveView = true;

            PlayAnimation(_showAnim, true, ShowView);
        }

        public virtual void ShowView()
        {
        }

        public virtual void Hide()
        {
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
            PlayAnimation(_hideAnim, false, OnHideComplete);
        }


        private void OnHideComplete()
        {
            _isActiveView = false;
            _canvas.enabled = false;
            gameObject.SetActive(false);
        }

        public bool IsVisible() => _canvas.enabled;

        private void PlayAnimation(AnimationSettings settings, bool isShow, TweenCallback onComplete)
        {
            transform.DOKill();

            switch (settings.AnimationType)
            {
                case AnimationType.None:
                    _canvasGroup.alpha = 1f;
                    onComplete?.Invoke();
                    break;

                case AnimationType.Fade:
                    PlayFade(settings, isShow, onComplete);
                    break;

                case AnimationType.Scale:
                    PlayScale(settings, isShow, onComplete);
                    break;

                case AnimationType.Position:
                    PlayPosition(settings, isShow, onComplete);
                    break;
            }
        }

        private void PlayFade(AnimationSettings s, bool isShow, TweenCallback onComplete)
        {
            _canvasGroup.alpha = isShow ? 0f : 1f;
            _canvasGroup.DOFade(isShow ? 1f : 0f, s.Duration)
                .SetEase(s.Ease)
                .OnComplete(onComplete);
        }

        private void PlayScale(AnimationSettings s, bool isShow, TweenCallback onComplete)
        {
            _rectTransform.localScale = isShow ? s.HiddenScale : s.ShownScale;
            _rectTransform.DOScale(isShow ? s.ShownScale : s.HiddenScale, s.Duration)
                .SetEase(s.Ease)
                .OnComplete(onComplete);
        }

        private void PlayPosition(AnimationSettings s, bool isShow, TweenCallback onComplete)
        {
            _rectTransform.anchoredPosition = isShow ? s.StartPos : s.EndPos;
            _rectTransform.DOAnchorPos(isShow ? s.EndPos : s.StartPos, s.Duration)
                .SetEase(s.Ease)
                .OnComplete(onComplete);
        }
    }
}