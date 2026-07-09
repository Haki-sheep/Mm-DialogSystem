#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Miemie.DialogSystem.Editor
{
    /// <summary>
    /// 对话图校验工具
    /// </summary>
    public static class DialogueGraphValidator
    {
        public static void Validate(DialogueGraph graph)
        {
            if (graph == null)
            {
                Debug.LogWarning("未选中任何 DialogueGraph");
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine($"=== 校验 {graph.name} ===");

            if (graph.StartNode == null)
                sb.AppendLine("[错误] startNode 未设置");

            if (graph.NodeList == null || graph.NodeList.Count == 0)
                sb.AppendLine("[警告] nodeList 为空");

            var inGraph = new HashSet<int>();
            if (graph.NodeList != null)
            {
                foreach (var node in graph.NodeList)
                {
                    if (node == null)
                    {
                        sb.AppendLine("[错误] nodeList 中有空引用");
                        continue;
                    }
                    inGraph.Add(node.ConfigId);
                }
            }

            if (graph.StartNode != null && !inGraph.Contains(graph.StartNodeId))
                sb.AppendLine("[警告] startNode 不在 nodeList 中");

            if (graph.NodeList != null)
            {
                foreach (var node in graph.NodeList)
                {
                    if (node == null)
                        continue;
                    ValidateNode(graph, node, inGraph, sb);
                }
            }

            sb.AppendLine("=== 结束 ===");
            Debug.Log(sb.ToString());
        }

        static void ValidateNode(DialogueGraph graph, DialogueNodeData node, HashSet<int> inGraph, StringBuilder sb)
        {
            if (node.IsOptionNode)
            {
                if (node.ChoiceList == null || node.ChoiceList.Count == 0)
                    sb.AppendLine($"[错误] 选项节点 [{node.ConfigId}] choiceList 为空");
                else
                    ValidateChoices(node, inGraph, sb);
            }
            else
            {
                int nextId = node.NextTransition?.toNodeId ?? 0;
                if (nextId == 0)
                    sb.AppendLine($"[提示] 节点 [{node.ConfigId}] 无出口（可能是结局）");
                else if (!inGraph.Contains(nextId))
                    sb.AppendLine($"[警告] [{node.ConfigId}] → {nextId} 不在本图 nodeList");
            }
        }

        static void ValidateChoices(DialogueNodeData node, HashSet<int> inGraph, StringBuilder sb)
        {
            foreach (var choice in node.ChoiceList)
            {
                if (choice == null || choice.toNodeId == 0)
                    sb.AppendLine($"[错误] [{node.ConfigId}] 选项「{choice?.labelText}」无 toNodeId");
                else if (!inGraph.Contains(choice.toNodeId))
                    sb.AppendLine($"[警告] 选项 → {choice.toNodeId} 不在本图 nodeList");
            }
        }
    }
}
#endif
