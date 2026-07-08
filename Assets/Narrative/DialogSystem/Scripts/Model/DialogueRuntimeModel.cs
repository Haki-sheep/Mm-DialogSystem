using System.Collections.Generic;
using MiMieMVVM;

namespace Miemie.DialogSystem
{
    /// <summary>
    /// 对话运行时状态
    /// </summary>
    public class DialogueRuntimeModel : IModelState
    {
        /// <summary> 当前对话图 </summary>
        public DialogueGraph Graph { get; private set; }

        /// <summary> 图变量 </summary>
        public DialogueVariablesStore Variables { get; } = new();

        /// <summary> 当前节点 </summary>
        public DialogueNode CurrentNode { get; private set; }

        /// <summary> 可用选项缓存 </summary>
        public List<DialogueTransition> AvailableChoiceList { get; } = new();

        /// <summary>
        /// 绑定对话图
        /// </summary>
        public void BindGraph(DialogueGraph graph)
        {
            Graph = graph;
            Variables.ApplyDefaults(graph?.Variables);
            CurrentNode = null;
            AvailableChoiceList.Clear();
        }

        /// <summary>
        /// 设置当前节点
        /// </summary>
        public void SetCurrentNode(DialogueNode node) => CurrentNode = node;

        /// <summary>
        /// 清空运行时
        /// </summary>
        public void Clear()
        {
            Graph = null;
            CurrentNode = null;
            AvailableChoiceList.Clear();
        }
    }
}
