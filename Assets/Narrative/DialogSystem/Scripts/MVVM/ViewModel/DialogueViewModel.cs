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
        private readonly DialogueRuntimeModel runtimeModel = new();

        /// <summary> 节点变更事件 </summary>
        public event Action<DialogueNodeData> NodeChanged;

        /// <summary> 选项刷新事件 </summary>
        public event Action<IReadOnlyList<DialogueTransLineData>> OptionsChanged;

        /// <summary> 对话结束事件 给 View 收起 UI </summary>
        public event Action DialogEnded;

        public DialogueRuntimeModel RuntimeModel => runtimeModel;
        public DialogueGraph Graph => runtimeModel.Graph;
        public DialogueNodeData CurrentNode => runtimeModel.CurrentNode;

        #region 生命周期

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

        #endregion

        #region 对话流程

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
        /// 继续对话
        /// </summary>
        public void GoNext()
        {
            var currentNode = runtimeModel.CurrentNode;
            if (currentNode == null) return;

            if (currentNode.IsOptionNode)
                return;

            var graph = runtimeModel.Graph;
            var transition = currentNode.NextTransition;
            // 获取下一个节点
            var nextNode = transition.ResolveToNode(graph);
            if (nextNode == null)
            {
                EndDialog();
                return;
            }

            if (!transition.CanPass(runtimeModel.Variables))
                return;

            // 真正切换到下一个节点
            GoTo(nextNode);
        }

        /// <summary>
        /// 跳转到节点
        /// </summary>
        private void GoTo(DialogueNodeData node)
        {
            if (node == null)
            {
                EndDialog();
                return;
            }

            // 设置当前节点
            runtimeModel.SetCurrentNode(node);
            UnityEngine.Debug.Log($"[{node.ConfigId}] {node.SpeakerName}: {node.DialogText}");
            // 通知节点变更
            NodeChanged?.Invoke(node);

            // 选项节点重建可用选项并通知 UI
            if (node.IsOptionNode)
                RebuildAvailableChoiceList(true);
        }

        /// <summary>
        /// 选择选项
        /// </summary>
        /// <param name="index">选项索引</param>
        public void SelectOption(int index)
        {
            // 点选之前刷一遍选项列表 不通知UI
            RebuildAvailableChoiceList(false);

            // 被刷新后的选项列表
            var choiceList = runtimeModel.AvailableChoiceList;
            if (index < 0 || index >= choiceList.Count) return;

            var choice = choiceList[index];

            // 选项带信号则发布到事件总线
            if (!string.IsNullOrEmpty(choice.eventKey))
                NarrativeEventBus.NarrytiveBus.Publish(NarrativeEventKeys.DialogueTriggered, runtimeModel.Graph, choice.eventKey);

            // 跳转到选项对应的节点
            GoTo(choice.ResolveToNode(runtimeModel.Graph));
        }

        /// <summary>
        /// 结束对话
        /// </summary>
        private void EndDialog()
        {
            var graph = runtimeModel.Graph;
            runtimeModel.SetCurrentNode(null);
            DialogEnded?.Invoke();
            if (graph != null)
                NarrativeEventBus.NarrytiveBus.Publish(NarrativeEventKeys.DialogueTriggered, graph, NarrativeEventKeys.DialogueGraphFinishedKey);
        }

        #endregion

        #region 选项

        /// <summary>
        /// 重建可用选项列表
        /// 本质是在改runtimeModel.AvailableChoiceList;
        /// </summary>
        /// <param name="notify">是否抛 OptionsChanged 通知 UI</param>
        private void RebuildAvailableChoiceList(bool notify)
        {
            var choiceList = runtimeModel.AvailableChoiceList;
            choiceList.Clear();

            var currentNode = runtimeModel.CurrentNode;
            if (currentNode?.ChoiceList == null)
            {
                if (notify)
                    OptionsChanged?.Invoke(choiceList);
                return;
            }

            var graph = runtimeModel.Graph;
            // 遍历当前节点的选项
            foreach (var choice in currentNode.ChoiceList)
            {
                if (choice == null || choice.toNodeId == 0) continue;
                // 如果选项对应的节点不存在，则跳过
                if (choice.ResolveToNode(graph) == null) continue;

                // 如果选项可以通过，则添加到可用选项列表
                if (choice.CanPass(runtimeModel.Variables))
                    choiceList.Add(choice);
            }

            // 如果需要通知 UI，则抛事件
            if (notify)
                OptionsChanged?.Invoke(choiceList);
        }

        #endregion
    }
}
