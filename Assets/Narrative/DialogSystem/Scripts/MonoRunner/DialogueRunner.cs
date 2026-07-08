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
        DialogueGraph dialogueGraph;

        [SerializeField]
        StandDialogView dialogView;

        [SerializeField]
        bool autoStart = true;

        [SerializeField, ReadOnly]
        DialogueNode currentNode;

        DialogueViewModel viewModel;
        DialogueCrossService crossService;

        public DialogueGraph DialogueGraph => dialogueGraph;
        public DialogueViewModel ViewModel => viewModel;
        public DialogueNode CurrentNode => viewModel?.CurrentNode;

        #region 生命周期

        void Awake()
        {
            viewModel = new DialogueViewModel();
            viewModel.Initialize();
            crossService = new DialogueCrossService(viewModel);
            BusinessModuleHub.Instance.RegisterBusinessModule(crossService);

            if (dialogView != null)
                dialogView.Bind(viewModel);
        }

        void Start()
        {
            if (autoStart)
                StartDialog();
        }

        void Update()
        {
            currentNode = viewModel?.CurrentNode;
            if (currentNode == null) return;

            if (Input.GetKeyDown(KeyCode.Space))
                Advance();

            if (!currentNode.IsOptionNode) return;

            var choices = viewModel.RuntimeModel.AvailableChoiceList;
            viewModel.RefreshAvailableChoices();
            for (int i = 0; i < choices.Count && i < 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                    SelectOption(i);
            }
        }

        void OnDestroy()
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
        /// 前进
        /// </summary>
        public void Advance() => viewModel?.Advance();

        /// <summary>
        /// 选择选项
        /// </summary>
        public void SelectOption(int index) => viewModel?.SelectOption(index);

        #endregion
    }
}
