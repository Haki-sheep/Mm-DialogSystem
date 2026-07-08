using MiMieMVVM;

namespace Miemie.DialogSystem
{
    /// <summary>
    /// 对话跨模块服务
    /// </summary>
    public interface IDialogueCrossService : ICrossBusinessModuleService
    {
        DialogueViewModel ViewModel { get; }
        bool IsPlaying { get; }
        void PlayGraph(DialogueGraph graph);
    }

    /// <summary>
    /// 对话跨模块服务实现
    /// </summary>
    public class DialogueCrossService : IDialogueCrossService
    {
        /// <summary> 对话 ViewModel </summary>
        readonly DialogueViewModel viewModel;

        public DialogueViewModel ViewModel => viewModel;
        public bool IsPlaying => viewModel.CurrentNode != null;

        public DialogueCrossService(DialogueViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        /// <summary>
        /// 播放对话图
        /// </summary>
        public void PlayGraph(DialogueGraph graph) => viewModel.StartDialog(graph);
    }
}
