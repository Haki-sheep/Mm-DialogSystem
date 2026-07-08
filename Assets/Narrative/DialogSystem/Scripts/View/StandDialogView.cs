using System;
using System.Collections.Generic;
using DG.Tweening;
using MiMieMVVM;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Miemie.DialogSystem
{
    /// <summary>
    /// 立绘对话 View
    /// </summary>
    public class StandDialogView : MonoBehaviour, IView
    {
        [BoxGroup("黑边参数"), SerializeField]
        private Image upPage;
        [BoxGroup("黑边参数"), SerializeField]
        private Image downPage;
        [BoxGroup("黑边参数")] public float UpHiddenY = 80f;
        [BoxGroup("黑边参数")] public float UpShownY = -80f;
        [BoxGroup("黑边参数")] public float DownHiddenY = -80f;
        [BoxGroup("黑边参数")] public float DownShownY = 80f;

        [BoxGroup("主画面"), SerializeField]
        private Image mainImage;
        [BoxGroup("主画面"), SerializeField, LabelText("打字机 字/秒")]
        private float typewriterSpeed = 40f;

        [BoxGroup("选项参数"), SerializeField, LabelText("选项倒计时")]
        private Image opCountDownImage;

        [BoxGroup("选项参数"), SerializeField]
        private Button[] optionButtonList;

        [SerializeField, LabelText("动画时长")]
        private float duration = 0.5f;

        /// <summary> 绑定 ViewModel </summary>
        DialogueViewModel boundViewModel;

        /// <summary> 选项点击回调 </summary>
        TextMeshProUGUI[] textMeshProUGUIList;
        TextMeshProUGUI downPageText;
        Tweener typewriterTween;
        Tweener opCountDownTween;

        public IViewModel ViewModel => boundViewModel;

        RectTransform UpPageRectTransform => upPage.rectTransform;
        RectTransform DownPageRectTransform => downPage.rectTransform;

        void Awake()
        {
            if (downPageText == null && downPage != null)
                downPageText = downPage.GetComponentInChildren<TextMeshProUGUI>();

            if (optionButtonList is not null)
            {
                textMeshProUGUIList = new TextMeshProUGUI[optionButtonList.Length];
                for (int i = 0; i < optionButtonList.Length; i++)
                {
                    int captured = i;
                    textMeshProUGUIList[i] = optionButtonList[i].GetComponentInChildren<TextMeshProUGUI>();
                    optionButtonList[i].onClick.AddListener(() => OnOptionClicked(captured));
                }
            }
        }

        void Start()
        {
            HidePage();
            MainImageHide();
        }

        void OnDestroy()
        {
            Unbind();
            StopTypewriter();
            KillTweens();
        }

        /// <summary>
        /// 绑定 ViewModel
        /// </summary>
        public void Bind(IViewModel viewModel)
        {
            Unbind();
            boundViewModel = viewModel as DialogueViewModel;
            if (boundViewModel == null)
                return;

            boundViewModel.NodeChanged += OnNodeChanged;
            boundViewModel.OptionsChanged += OnOptionsChanged;
            boundViewModel.DialogEnded += OnDialogEnded;
        }

        /// <summary>
        /// 解绑 ViewModel
        /// </summary>
        public void Unbind()
        {
            if (boundViewModel == null)
                return;

            boundViewModel.NodeChanged -= OnNodeChanged;
            boundViewModel.OptionsChanged -= OnOptionsChanged;
            boundViewModel.DialogEnded -= OnDialogEnded;
            boundViewModel = null;
        }

        void OnNodeChanged(DialogueNode node)
        {
            ShowPage();
            HideAllOptions();
            StopOpCountDown();
            SetTypewriterText($"{node.SpeakerName}\n{node.DialogText}");
        }

        void OnOptionsChanged(IReadOnlyList<DialogueTransition> choices)
        {
            HideAllOptions();
            if (choices == null || choices.Count == 0)
                return;

            for (int i = 0; i < choices.Count && i < optionButtonList.Length; i++)
            {
                SetOptionText(i, choices[i].labelText);
                optionButtonList[i].gameObject.SetActive(true);
            }
        }

        void OnDialogEnded()
        {
            HideAllOptions();
            StopOpCountDown();
            HidePage();
        }

        void OnOptionClicked(int index) => boundViewModel?.SelectOption(index);

        void HideAllOptions()
        {
            if (optionButtonList == null)
                return;

            for (int i = 0; i < optionButtonList.Length; i++)
                optionButtonList[i].gameObject.SetActive(false);
        }

        /// <summary>
        /// 设置选项文本
        /// </summary>
        public void SetOptionText(int index, string text)
        {
            if (textMeshProUGUIList is null) return;
            if (index < 0 || index >= textMeshProUGUIList.Length) return;
            textMeshProUGUIList[index].text = text;
        }

        /// <summary>
        /// 设置并立即显示整段文本
        /// </summary>
        public void SetImmediateText(string text)
        {
            StopTypewriter();
            if (downPageText == null)
                return;
            downPageText.text = text;
        }

        /// <summary>
        /// 设置并以打字机效果显示文本
        /// </summary>
        public void SetTypewriterText(string text, Action onComplete = null)
        {
            if (downPageText == null)
                return;

            StopTypewriter();
            downPageText.text = string.Empty;

            if (string.IsNullOrEmpty(text))
            {
                onComplete?.Invoke();
                return;
            }

            // 计算打字机时长
            float typeDuration = text.Length / Mathf.Max(typewriterSpeed, 1f);
            typewriterTween = downPageText
                .DOText(text, typeDuration, true)
                .SetEase(Ease.Linear)
                .OnComplete(() => onComplete?.Invoke());
        }

        /// <summary>
        /// 立刻显示打字机剩余全文
        /// </summary>
        public void CompleteTypewriter() => typewriterTween?.Complete();

        void StopTypewriter()
        {
            downPageText?.DOKill();
            typewriterTween = null;
        }

        /// <summary>
        /// 设置选项倒计时
        /// </summary>
        public void SetOpCountDown(float countDownTotalTime, Action onComplete = null)
        {
            StopOpCountDown();
            opCountDownImage.gameObject.SetActive(true);
            opCountDownImage.fillAmount = 1;

            opCountDownTween = opCountDownImage
                .DOFillAmount(0, countDownTotalTime)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    opCountDownImage.gameObject.SetActive(false);
                    opCountDownTween = null;
                    onComplete?.Invoke();
                });
        }

        /// <summary>
        /// 停止选项倒计时
        /// </summary>
        public void StopOpCountDown()
        {
            opCountDownImage.DOKill();
            opCountDownTween?.Kill();
            opCountDownTween = null;
            opCountDownImage.gameObject.SetActive(false);
        }

        /// <summary>
        /// 显示主画面
        /// </summary>
        public void MainImageShow()
        {
            float width = Screen.width;
            float height = Screen.height - upPage.rectTransform.rect.height - downPage.rectTransform.rect.height;
            mainImage.rectTransform.sizeDelta = new Vector2(width, height);
            mainImage.DOFade(1, duration).SetEase(Ease.OutCubic);
        }

        /// <summary>
        /// 隐藏主画面
        /// </summary>
        public void MainImageHide()
        {
            mainImage.DOFade(0, duration).SetEase(Ease.InCubic).OnComplete(() =>
            {
                mainImage.rectTransform.sizeDelta = new Vector2(0, 0);
            });
        }

        /// <summary>
        /// 滑入显示
        /// </summary>
        public void ShowPage(bool isShowMainImage = true)
        {
            if (isShowMainImage)
                MainImageShow();
            UpPageRectTransform.DOAnchorPos(new Vector2(UpPageRectTransform.anchoredPosition.x, UpShownY), duration).SetEase(Ease.OutCubic);
            DownPageRectTransform.DOAnchorPos(new Vector2(DownPageRectTransform.anchoredPosition.x, DownShownY), duration).SetEase(Ease.OutCubic);
        }

        /// <summary>
        /// 滑出隐藏
        /// </summary>
        public void HidePage(bool isHideMainImage = true)
        {
            if (isHideMainImage)
                MainImageHide();
            UpPageRectTransform.DOAnchorPos(new Vector2(UpPageRectTransform.anchoredPosition.x, UpHiddenY), duration).SetEase(Ease.InCubic);
            DownPageRectTransform.DOAnchorPos(new Vector2(DownPageRectTransform.anchoredPosition.x, DownHiddenY), duration).SetEase(Ease.InCubic);
        }

        void KillTweens()
        {
            UpPageRectTransform.DOKill();
            DownPageRectTransform.DOKill();
            mainImage.DOKill();
            StopOpCountDown();
        }
    }
}
