#if UNITY_EDITOR
using Miemie.Narrative.GraphViewFrame.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Miemie.DialogSystem.Editor
{
    /// <summary>
    /// 右侧 Inspector 面板
    /// </summary>
    static class DialogueGraphInspectorPanel
    {
        public static void Draw(DialogueGraphEditorWindow window)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(window.GetRightPanelWidth()), GUILayout.ExpandHeight(true));
            window.EnsureMenuTreeInternal();

            if (window.SelectedTransition != null)
            {
                DrawTransitionInspector(window);
                EditorGUILayout.EndVertical();
                return;
            }

            var selected = window.MenuTreeAccessor?.Selection?.SelectedValue;
            if (selected is UnityEngine.Object obj && !DialogueGraphEditorWindow.IsAssetAlive(obj))
            {
                window.ClearStaleSelectionInternal();
                selected = null;
            }

            GraphViewFramePanelStyles.DrawPanelHeader("属性", selected != null ? GetSelectionSubtitle(window, selected) : "未选中");

            if (selected == null)
            {
                GraphViewFramePanelStyles.BeginPaddedContent();
                GraphViewFramePanelStyles.DrawEmptyHint("在左侧选中 Graph 或节点以编辑属性。");
                GraphViewFramePanelStyles.EndPaddedContent();
                EditorGUILayout.EndVertical();
                return;
            }

            GraphViewFramePanelStyles.BeginPaddedContent();
            DrawSelectedObjectInspector(window, selected);
            GraphViewFramePanelStyles.EndPaddedContent();
            EditorGUILayout.EndVertical();
        }

        static void DrawTransitionInspector(DialogueGraphEditorWindow window)
        {
            InspectorPanelShell.DrawSection("属性", "Transition", () => DialogueTransitionInspectorDrawer.Draw(window.SelectedTransition));
        }

        static string GetSelectionSubtitle(DialogueGraphEditorWindow window, object selected)
        {
            if (selected is DialogueGraph graph && DialogueGraphEditorWindow.IsAssetAlive(graph))
                return "Dialogue Graph";

            if (selected is DialogueNodeData node)
            {
                var parent = window.FindGraphForNode(node);
                if (DialogueGraphEditorWindow.IsNodeAlive(parent, node))
                    return $"Dialogue Node  ·  [{node.ConfigId}] {node.SpeakerName}";
            }

            if (selected is UnityEngine.Object obj)
                return obj.name;

            return "未选中";
        }

        static void DrawSelectedObjectInspector(DialogueGraphEditorWindow window, object selected)
        {
            if (selected is DialogueGraph graph && DialogueGraphEditorWindow.IsAssetAlive(graph))
            {
                if (window.RenameTarget != graph)
                    window.SetRenameTarget(graph, graph.name);

                DrawAssetRenameField(window, graph);
                EditorGUILayout.Space(4);
                DrawGraphInspector(window, graph);
                DrawDeleteButton(window, graph);
                return;
            }

            if (selected is DialogueNodeData node)
            {
                var parentGraph = window.FindGraphForNode(node);
                if (!DialogueGraphEditorWindow.IsNodeAlive(parentGraph, node))
                    return;

                DrawNodeInspector(window, parentGraph, node);
                DrawDeleteButton(window, node);
            }
        }

        static void DrawNodeInspector(DialogueGraphEditorWindow window, DialogueGraph graph, DialogueNodeData node)
        {
            var nodeProp = DialogueNodeEditorUtility.FindNodeProperty(graph, node, out var graphSo);
            if (nodeProp == null)
                return;

            var scroll = window.InspectorScroll;
            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUI.BeginChangeCheck();
            DialogueNodeInspectorDrawer.Draw(nodeProp);
            bool changed = EditorGUI.EndChangeCheck();
            EditorGUILayout.EndScrollView();
            window.InspectorScroll = scroll;

            if (node.IsOptionNode)
                DrawChoiceButtons(window, node);

            if (!changed)
                return;

            graphSo.ApplyModifiedProperties();
            EditorUtility.SetDirty(graph);
            window.QueueGraphViewRefreshFromInspector(node);
            window.RequestMenuLabelRefreshOnly();
        }

        static void DrawGraphInspector(DialogueGraphEditorWindow window, Object selected)
        {
            if (window.InspectorTreeTarget != selected || window.InspectorTree == null)
            {
                window.SetInspectorTreeTarget(selected);
                window.SetInspectorTree(PropertyTree.Create(new SerializedObject(selected)));
                window.InspectorScroll = Vector2.zero;
            }

            var scroll = window.InspectorScroll;
            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUI.BeginChangeCheck();
            window.InspectorTree.Draw(false);
            bool treeChanged = EditorGUI.EndChangeCheck();
            EditorGUILayout.EndScrollView();
            window.InspectorScroll = scroll;

            if (!treeChanged)
                return;

            window.InspectorTree.ApplyChanges();
            EditorUtility.SetDirty(selected);
            window.QueueGraphViewRefreshFromInspector(selected);
            window.RequestMenuLabelRefreshOnly();
        }

        static void DrawDeleteButton(DialogueGraphEditorWindow window, object selected)
        {
            if (selected is not DialogueGraph and not DialogueNodeData)
                return;

            EditorGUILayout.Space(12);

            var prevColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.85f, 0.35f, 0.35f);

            string label = selected is DialogueGraph ? "删除对话图" : "删除节点";
            if (GUILayout.Button(label, GUILayout.Height(28)))
            {
                if (selected is DialogueGraph graph)
                    window.TryDeleteSelectedAsset(graph);
                else if (selected is DialogueNodeData node)
                    window.TryDeleteSelectedAsset(node);
            }

            GUI.backgroundColor = prevColor;
        }

        static void DrawChoiceButtons(DialogueGraphEditorWindow window, DialogueNodeData node)
        {
            EditorGUILayout.Space(6);
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("添加选项", GUILayout.Height(24)))
            {
                DialogueNodeChoiceEditorUtility.AddChoice(node);
                window.QueueGraphViewRefreshFromInspector(node);
            }

            EditorGUI.BeginDisabledGroup((node.ChoiceList?.Count ?? 0) <= 1);
            if (GUILayout.Button("删除最后选项", GUILayout.Height(24))
                && DialogueNodeChoiceEditorUtility.TryRemoveLastChoice(node))
            {
                window.QueueGraphViewRefreshFromInspector(node);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();
        }

        static void DrawAssetRenameField(DialogueGraphEditorWindow window, Object selected)
        {
            GUI.SetNextControlName(DialogueGraphEditorConstants.RenameFieldControl);
            window.RenameBuffer = EditorGUILayout.TextField("名称", window.RenameBuffer);

            var evt = Event.current;
            if (evt.type == EventType.KeyDown && GUI.GetNameOfFocusedControl() == DialogueGraphEditorConstants.RenameFieldControl)
            {
                if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                {
                    window.CommitAssetRename(selected);
                    GUI.FocusControl(null);
                    evt.Use();
                }
                else if (evt.keyCode == KeyCode.Escape)
                {
                    window.RenameBuffer = selected.name;
                    GUI.FocusControl(null);
                    evt.Use();
                }
            }

            if (evt.type == EventType.Repaint)
            {
                bool focused = GUI.GetNameOfFocusedControl() == DialogueGraphEditorConstants.RenameFieldControl;
                if (window.RenameFieldWasFocused && !focused)
                    window.CommitAssetRename(selected);
                window.RenameFieldWasFocused = focused;
            }
        }
    }
}
#endif
