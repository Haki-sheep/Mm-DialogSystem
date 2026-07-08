using System;
using System.Collections.Generic;
using MiMieMVVM;

namespace Miemie.DialogSystem
{
    /// <summary>
    /// 对话用例 ViewModel
    /// </summary>
    public class DialogueViewModel : IViewModel
    {
        /// <summary> 运行时状态 </summary>
        readonly DialogueRuntimeModel runtimeModel = new();

        /// <summary> 节点变更 </summary>
        public event Action<DialogueNode> NodeChanged;

        /// <summary> 选项刷新 </summary>
        public event Action<IReadOnlyList<DialogueTransition>> OptionsChanged;

        /// <summary> 对话结束 </summary>
        public event Action DialogEnded;

        public DialogueRuntimeModel RuntimeModel => runtimeModel;
        public DialogueGraph Graph => runtimeModel.Graph;
        public DialogueNode CurrentNode => runtimeModel.CurrentNode;

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Shutdown()
        {
            runtimeModel.Clear();
            NodeChanged = null;
            OptionsChanged = null;
            DialogEnded = null;
        }

        /// <summary>
        /// 开始对话
        /// </summary>
        public void StartDialog(DialogueGraph graph)
        {
            if (graph == null)
            {
                UnityEngine.Debug.LogError("Dialogue graph is null");
                return;
            }

            if (graph.StartNode == null)
            {
                UnityEngine.Debug.LogError("Start node is null");
                return;
            }

            runtimeModel.BindGraph(graph);
            GoTo(graph.StartNode);
        }

        /// <summary>
        /// 跳转到节点
        /// </summary>
        public void GoTo(DialogueNode node)
        {
            if (node == null)
            {
                EndDialog();
                return;
            }

            runtimeModel.SetCurrentNode(node);
            node.PlayNode();
            NodeChanged?.Invoke(node);

            if (node.IsOptionNode)
                RefreshAndNotifyOptions();
        }

        /// <summary>
        /// 前进
        /// </summary>
        public void Advance()
        {
            var currentNode = runtimeModel.CurrentNode;
            if (currentNode == null) return;

            if (currentNode.IsOptionNode)
                return;

            var graph = runtimeModel.Graph;
            var transition = currentNode.NextTransition;
            var nextNode = transition.ResolveToNode(graph);
            if (nextNode == null)
            {
                EndDialog();
                return;
            }

            if (!transition.CanPass(runtimeModel.Variables))
                return;

            GoTo(nextNode);
        }

        /// <summary>
        /// 选择选项
        /// </summary>
        public void SelectOption(int index)
        {
            RefreshAvailableChoices();
            var choiceList = runtimeModel.AvailableChoiceList;
            if (index < 0 || index >= choiceList.Count) return;

            var choice = choiceList[index];
            if (!string.IsNullOrEmpty(choice.eventKey))
                GameNotify.DialogueEvent(runtimeModel.Graph, choice.eventKey);

            GoTo(choice.ResolveToNode(runtimeModel.Graph));
        }

        /// <summary>
        /// 刷新可用选项
        /// </summary>
        public void RefreshAvailableChoices()
        {
            var choiceList = runtimeModel.AvailableChoiceList;
            choiceList.Clear();

            var currentNode = runtimeModel.CurrentNode;
            if (currentNode?.ChoiceList == null) return;

            var graph = runtimeModel.Graph;
            foreach (var c in currentNode.ChoiceList)
            {
                if (c == null || c.toNodeId == 0) continue;
                if (c.ResolveToNode(graph) == null) continue;
                if (c.CanPass(runtimeModel.Variables))
                    choiceList.Add(c);
            }
        }

        void RefreshAndNotifyOptions()
        {
            RefreshAvailableChoices();
            OptionsChanged?.Invoke(runtimeModel.AvailableChoiceList);
        }

        void EndDialog()
        {
            var graph = runtimeModel.Graph;
            runtimeModel.SetCurrentNode(null);
            DialogEnded?.Invoke();
            if (graph != null)
                GameNotify.DialogueEvent(graph, DialogueFinishedEventData.GraphFinishedEventKey);
        }
    }
}
