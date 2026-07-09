using System.Collections.Generic;
using MiMieMVVM;

namespace Miemie.DialogSystem
{
    /// <summary>
    /// 对话进行中的运行时状态
    /// </summary>
    public class DialogueRuntimeModel : IModelState
    {
        /// <summary> 当前跑在哪张图上 </summary>
        public DialogueGraph Graph { get; private set; }

        /// <summary> 变量黑板（当前值）</summary>
        public DialogueVariablesBlackBoard Variables { get; } = new();

        /// <summary> 当前节点 </summary>
        public DialogueNodeData CurrentNode { get; private set; }

        /// <summary> 可用选项缓存 </summary>
        public List<DialogueTransLineData> AvailableChoiceList { get; } = new();

        /// <summary>
        /// 绑定对话图
        /// </summary>
        public void BindGraph(DialogueGraph graph)
        {
            this.Graph = graph;
            // graph?.VariableList(黑板变量列表) 在配置阶段已填充
            Variables.InitDefaultVariables(graph?.VariableList);
            CurrentNode = null;
            AvailableChoiceList.Clear();
        }

        /// <summary>
        /// 设置当前节点
        /// </summary>
        public void SetCurrentNode(DialogueNodeData node) => CurrentNode = node;

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
