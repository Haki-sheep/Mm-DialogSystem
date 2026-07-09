#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Miemie.DialogSystem.Editor
{
    partial class DialogueGraphEditorWindow
    {
        public DialogueGraph FindGraphForNode(DialogueNodeData node)
        {
            if (node == null)
                return null;

            if (nodeToGraph.TryGetValue(node, out var graph))
                return graph;

            foreach (var guid in AssetDatabase.FindAssets($"t:{nameof(DialogueGraph)}"))
            {
                var g = AssetDatabase.LoadAssetAtPath<DialogueGraph>(AssetDatabase.GUIDToAssetPath(guid));
                if (g?.NodeList != null && g.NodeList.Contains(node))
                    return g;
            }

            return null;
        }

        /// <summary>
        /// 在图内创建节点
        /// </summary>
        public DialogueNodeData CreateNode(DialogueGraph graph)
        {
            if (graph == null)
                return null;

            var node = new DialogueNodeData();
            int maxId = graph.NodeList != null && graph.NodeList.Count > 0
                ? graph.NodeList.Where(n => n != null).Select(n => n.ConfigId).DefaultIfEmpty(0).Max()
                : 0;
            node.SetNodeId(maxId + 1);
            node.SetSpeakerName($"节点{maxId + 1}");
            return node;
        }

        internal void CommitAssetRename(Object selected)
        {
            if (selected == null || RenameBuffer == selected.name)
                return;

            if (DialogueGraphAssetRenameUtility.Apply(selected, RenameBuffer, ref renameBuffer))
                RefreshAfterExternalChange(selected);
        }

        internal void RefreshAfterExternalChange(Object target)
        {
            if (target == null)
                return;

            SetRenameTarget(target, target.name);
            ClearInspectorTree();
            RequestMenuRefresh();
            EditorApplication.delayCall += () => SelectObjectInTree(target);
        }

        internal void TryDeleteSelectedAsset(object selected)
        {
            if (selected is DialogueGraph graph)
            {
                if (!graph)
                    return;

                selectedTransition = null;
                ClearInspectorTree();

                if (!DialogueGraphAssetDeleter.TryDeleteGraph(graph))
                    return;

                lastSelectedGraph = null;
                lastSyncedSelection = null;
                ClearRenameTarget();
                ClearStaleSelectionInternal();
                RequestMenuRefresh();
                graphView?.ClearGraph();
                return;
            }

            if (selected is DialogueNodeData node)
            {
                var parentGraph = FindGraphForNode(node);
                if (parentGraph == null)
                    return;

                selectedTransition = null;
                ClearInspectorTree();

                if (!DialogueGraphAssetDeleter.TryDeleteNode(parentGraph, node))
                    return;

                lastSyncedSelection = null;
                ClearRenameTarget();
                RequestMenuRefresh();

                lastSelectedGraph = parentGraph;
                EditorApplication.delayCall += () =>
                {
                    SelectObjectInTree(parentGraph);
                    graphView?.RefreshCurrentGraph(preserveView: true);
                };
            }
        }

        internal static bool IsNodeAlive(DialogueGraph graph, DialogueNodeData node) =>
            node != null && graph?.NodeList != null && graph.NodeList.Contains(node);
    }
}
#endif
