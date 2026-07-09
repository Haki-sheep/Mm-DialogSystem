#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Miemie.DialogSystem.Editor
{
    partial class DialogueGraphEditorWindow
    {
        internal bool RenameFieldWasFocused;

        void SyncSelectionToGraphView()
        {
            if (graphView == null || MenuTree?.Selection == null)
                return;

            var selected = MenuTree.Selection.SelectedValue;
            if (selected is UnityEngine.Object obj && !IsAssetAlive(obj))
            {
                ClearStaleSelectionInternal();
                graphView.ClearGraph();
                return;
            }

            var dialogueNode = selected as DialogueNodeData;
            if (dialogueNode != null && !IsNodeAlive(FindGraphForNode(dialogueNode), dialogueNode))
            {
                ClearStaleSelectionInternal();
                graphView.ClearGraph();
                return;
            }

            if (ReferenceEquals(selected, lastSyncedSelection))
                return;

            lastSyncedSelection = selected;

            if (selected is DialogueGraph graph)
            {
                lastSelectedGraph = graph;
                DialogueEditorContext.CurrentGraph = graph;
            }
            else if (dialogueNode != null)
            {
                lastSelectedGraph = FindGraphForNode(dialogueNode);
                DialogueEditorContext.CurrentGraph = lastSelectedGraph;
            }

            QueueGraphViewSelectionUpdate(selected);
        }

        void QueueGraphViewSelectionUpdate(object selected)
        {
            pendingGraphSelection = selected;
            if (graphView == null || graphSelectionUpdateScheduled)
                return;

            graphSelectionUpdateScheduled = true;
            graphView.schedule.Execute(() =>
            {
                graphSelectionUpdateScheduled = false;
                graphView?.ApplySelection(pendingGraphSelection);
            });
        }

        public void ResetSelectionSync() => lastSyncedSelection = null;

        /// <summary>
        /// 新建节点后同步选中 避免残留已销毁引用
        /// </summary>
        public void OnNodeCreated(DialogueNodeData node, DialogueGraph graph)
        {
            if (node == null || graph == null)
                return;

            selectedTransition = null;
            ClearInspectorTree();
            lastSelectedGraph = graph;
            lastSyncedSelection = null;

            EditorApplication.delayCall += () =>
            {
                if (!IsNodeAlive(graph, node))
                    return;

                ForceMenuTreeRebuild();
                SelectObjectInTree(node);
                graphView?.SelectNode(node);
            };
        }

        /// <summary>
        /// 画布点击后反向同步到左侧树
        /// </summary>
        public void SelectObjectInTree(object target)
        {
            if (MenuTree == null || target == null)
                return;

            selectedTransition = null;

            foreach (var item in MenuTree.EnumerateTree())
            {
                if (!ReferenceEquals(item.Value, target))
                    continue;

                if (!ReferenceEquals(MenuTree.Selection.SelectedValue, target))
                {
                    item.Select();
                    MenuTree.Selection.Clear();
                    MenuTree.Selection.Add(item);
                }

                RefreshFromMenuSelection();
                return;
            }
        }

        /// <summary>
        /// 画布选中连线后同步到右侧 Inspector
        /// </summary>
        public void SelectTransition(DialogueTransitionHandle handle)
        {
            selectedTransition = handle;
            lastSyncedSelection = handle;

            if (handle?.graph != null)
            {
                lastSelectedGraph = handle.graph;
                DialogueEditorContext.CurrentGraph = handle.graph;
            }

            ClearInspectorTree();
            Repaint();
        }

        internal void ClearStaleSelectionInternal()
        {
            selectedTransition = null;
            lastSyncedSelection = null;
            ClearRenameTarget();
            ClearInspectorTree();

            if (MenuTree?.Selection == null)
                return;

            MenuTree.Selection.Clear();
        }

        internal void QueueGraphViewRefreshFromInspector(object selected)
        {
            if (graphView == null)
                return;

            graphView.schedule.Execute(() =>
            {
                graphView.RefreshCurrentGraph(preserveView: true);
                if (selected is DialogueNodeData node)
                    graphView.SelectNode(node);
            });
        }
    }
}
#endif
