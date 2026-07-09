using MiMieMVVM;

namespace Miemie.DialogSystem
{
    /// <summary>
    /// 对话跨模块服务
    /// 这里提供什么 外部就可以调用什么
    /// </summary>
    public interface IDialogueCrossService : ICrossBusinessModuleService
    {
        /// <summary> 是否正在播放 </summary>
        bool IsPlaying { get; }

        /// <summary> 播放对话图 </summary>
        void PlayGraph(DialogueGraph graph);
    }

    /// <summary>
    /// 对话跨模块服务实现
    /// </summary>
    public class DialogueCrossService : IDialogueCrossService
    {
        /// <summary> 对话 ViewModel </summary>
        private readonly DialogueViewModel viewModel;
        public bool IsPlaying => viewModel.CurrentNode != null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="viewModel">对话 ViewModel</param>
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
