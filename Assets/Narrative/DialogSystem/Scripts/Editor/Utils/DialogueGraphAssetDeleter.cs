#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Miemie.DialogSystem.Editor
{
    /// <summary>
    /// 对话图与节点删除
    /// </summary>
    static class DialogueGraphAssetDeleter
    {
        /// <summary>
        /// 删除整张对话图
        /// </summary>
        public static bool TryDeleteGraph(DialogueGraph graph)
        {
            if (!graph)
                return false;

            string graphName = graph.name;
            int nodeCount = graph.NodeList?.Count ?? 0;
            if (!EditorUtility.DisplayDialog(
                    "删除对话图",
                    $"确定删除「{graphName}」及其 {nodeCount} 个节点吗\n此操作不可撤销",
                    "删除",
                    "取消"))
                return false;

            DialogueGraphLayoutStore.RemoveGraph(graph);

            string graphPath = AssetDatabase.GetAssetPath(graph);
            if (!string.IsNullOrEmpty(graphPath))
                AssetDatabase.DeleteAsset(graphPath);

            AssetDatabase.SaveAssets();
            Debug.Log($"已删除对话图: {graphName}");
            return true;
        }

        /// <summary>
        /// 删除图内单个节点
        /// </summary>
        public static bool TryDeleteNode(DialogueGraph graph, DialogueNode node)
        {
            if (graph == null || node == null)
                return false;

            int nodeId = node.NodeId;
            if (!EditorUtility.DisplayDialog(
                    "删除节点",
                    $"确定删除节点 [{nodeId}] {node.SpeakerName} 吗\n此操作不可撤销",
                    "删除",
                    "取消"))
                return false;

            DeleteNodeInternal(graph, node);
            AssetDatabase.SaveAssets();
            Debug.Log($"已删除节点: [{nodeId}]");
            return true;
        }

        static void DeleteNodeInternal(DialogueGraph graph, DialogueNode node)
        {
            ClearReferencesToNode(graph, node);

            if (graph.StartNodeId == node.NodeId)
                graph.SetStartNodeInEditorWindow(null);

            graph.RemoveNode(node);
            DialogueGraphLayoutStore.RemoveNode(graph, node);
            EditorUtility.SetDirty(graph);
        }

        static void ClearReferencesToNode(DialogueGraph graph, DialogueNode target)
        {
            if (graph?.NodeList == null)
                return;

            foreach (var node in graph.NodeList)
            {
                if (node == null || node == target)
                    continue;

                if (node.NextTransition?.toNodeId == target.NodeId)
                    node.ClearNextNode();

                if (node.ChoiceList == null)
                    continue;

                foreach (var choice in node.ChoiceList)
                {
                    if (choice != null && choice.toNodeId == target.NodeId)
                        choice.toNodeId = 0;
                }
            }
        }
    }
}
#endif
