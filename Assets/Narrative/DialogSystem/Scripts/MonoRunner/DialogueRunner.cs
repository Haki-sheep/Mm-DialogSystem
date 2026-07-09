using MiMieMVVM;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Miemie.DialogSystem
{
    /// <summary>
    /// 对话模块入口
    /// 组装 ViewModel View 并注册跨模块服务
    /// </summary>
    public class DialogueRunner : SerializedMonoBehaviour
    {
        [SerializeField]
        private DialogueGraph dialogueGraph;

        [SerializeField]
        private StandDialogView dialogView;

        [SerializeField]
        private bool autoStart = true;

        [SerializeField, ReadOnly]
        private DialogueNodeData currentNode;

        private DialogueViewModel viewModel;
        private DialogueCrossService crossService;

        public DialogueGraph DialogueGraph => dialogueGraph;
        public DialogueViewModel ViewModel => viewModel;
        public DialogueNodeData CurrentNode => viewModel?.CurrentNode;

        #region 生命周期

        private void Awake()
        {
            // 创建vm对象
            viewModel = new DialogueViewModel();
            viewModel.Initialize();

            // 创建跨模块服务
            crossService = new DialogueCrossService(viewModel);
            BusinessModuleHub.Instance.RegisterBusinessModule(crossService);

            // View层绑定VM
            if (dialogView != null)
                dialogView.Bind(viewModel);
        }

        private void Start()
        {
            if (autoStart)
                StartDialog();
        }

        private void Update()
        {
            // 仅同步 Inspector 只读显示 输入已交给 View
            currentNode = viewModel?.CurrentNode;
        }

        private void OnDestroy()
        {
            dialogView?.Unbind();
            viewModel?.Shutdown();
            if (crossService != null)
                BusinessModuleHub.Instance.UnregisterBusinessModule<IDialogueCrossService>();
        }

        #endregion

        #region 对话流程

        /// <summary>
        /// 开始对话
        /// </summary>
        public void StartDialog()
        {
            viewModel?.StartDialog(dialogueGraph);
        }

        /// <summary>
        /// 播放指定对话图
        /// </summary>
        public void PlayGraph(DialogueGraph graph)
        {
            dialogueGraph = graph;
            StartDialog();
        }

        /// <summary>
        /// 继续对话
        /// </summary>
        public void GoNext() => viewModel?.GoNext();

        /// <summary>
        /// 选择选项
        /// </summary>
        public void SelectOption(int index) => viewModel?.SelectOption(index);

        #endregion
    }
}
