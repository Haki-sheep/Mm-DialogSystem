#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Miemie.DialogSystem.Editor
{
    /// <summary>
    /// 连线 Transition Inspector 绘制
    /// </summary>
    static class DialogueTransitionInspectorDrawer
    {
        public static void Draw(DialogueTransitionHandle handle)
        {
            if (handle?.sourceNode == null || handle.graph == null)
            {
                EditorGUILayout.LabelField("未选中连线");
                return;
            }

            if (handle.IsOptionTransition)
            {
                EditorGUILayout.LabelField("Option Transition", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(handle.Title, EditorStyles.miniLabel);
                EditorGUILayout.HelpBox("选项节点的出口 选项文本为运行时按钮文案 条件不满足时该按钮不显示", MessageType.None);
            }
            else
            {
                EditorGUILayout.LabelField("Transition", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(handle.Title, EditorStyles.miniLabel);
                EditorGUILayout.HelpBox("普通节点的 Out 出口 按空格前进时检查条件 不满足则无法跳转", MessageType.None);
            }

            EditorGUILayout.Space(4);

            var nodeProp = DialogueNodeEditorUtility.FindNodeProperty(handle.graph, handle.sourceNode, out var graphSo);
            if (nodeProp == null)
            {
                EditorGUILayout.HelpBox("找不到节点序列化数据", MessageType.Warning);
                return;
            }

            graphSo.Update();

            if (handle.IsOptionTransition)
                DrawOptionTransition(handle, nodeProp);
            else
                DrawLinearTransition(handle, nodeProp);

            graphSo.ApplyModifiedProperties();
            EditorUtility.SetDirty(handle.graph);
        }

        static void DrawLinearTransition(DialogueTransitionHandle handle, SerializedProperty nodeProp)
        {
            var transitionProp = nodeProp.FindPropertyRelative("nextTransition");
            if (transitionProp == null)
            {
                EditorGUILayout.HelpBox("找不到 nextTransition", MessageType.Warning);
                return;
            }

            DrawConditionsBlock(handle.graph, transitionProp.FindPropertyRelative("conditionList"), "Conditions");
        }

        static void DrawOptionTransition(DialogueTransitionHandle handle, SerializedProperty nodeProp)
        {
            var choiceListProp = nodeProp.FindPropertyRelative("choiceList");
            int choiceIndex = FindChoiceIndex(handle.sourceNode, handle.choiceTransition);
            if (choiceIndex < 0)
            {
                EditorGUILayout.HelpBox("找不到对应选项跳转数据", MessageType.Warning);
                return;
            }

            var optionProp = choiceListProp.GetArrayElementAtIndex(choiceIndex);
            var labelProp = optionProp.FindPropertyRelative("labelText");
            labelProp.stringValue = EditorGUILayout.TextField("选项文本", labelProp.stringValue);
            EditorGUILayout.Space(4);
            DrawConditionsBlock(handle.graph, optionProp.FindPropertyRelative("conditionList"), "Conditions");
        }

        static void DrawConditionsBlock(DialogueGraph graph, SerializedProperty conditionsProp, string title)
        {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

            if (conditionsProp == null)
                return;

            for (int i = 0; i < conditionsProp.arraySize; i++)
            {
                var conditionProp = conditionsProp.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                DrawConditionRow(graph, conditionProp);

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("-", GUILayout.Width(24)))
                {
                    conditionsProp.DeleteArrayElementAtIndex(i);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    return;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(2);
            }

            if (GUILayout.Button("+ 添加条件"))
                AddCondition(graph, conditionsProp);
        }

        static void DrawConditionRow(DialogueGraph graph, SerializedProperty conditionProp)
        {
            var variableNameProp = conditionProp.FindPropertyRelative("variableName");
            var typeProp = conditionProp.FindPropertyRelative("eCondition");
            var floatProp = conditionProp.FindPropertyRelative("targetFloat");
            var intProp = conditionProp.FindPropertyRelative("targetInt");

            string newName = DialogueGraphVariablesDrawer.DrawVariablePopup(graph, variableNameProp.stringValue);
            variableNameProp.stringValue = newName;

            var variableDef = graph?.FindBlackBoardVariable(newName);
            if (variableDef == null)
            {
                var current = (ECondition)typeProp.intValue;
                typeProp.intValue = (int)(ECondition)EditorGUILayout.EnumPopup("条件", current);
            }
            else
            {
                var options = DialogueConditionEditorUtility.GetConditionOptions(variableDef.variableType);
                var currentType = (ECondition)typeProp.intValue;

                int currentIndex = System.Array.IndexOf(options, currentType);
                if (currentIndex < 0)
                {
                    typeProp.intValue = (int)options[0];
                    currentIndex = 0;
                }

                string[] labels = System.Array.ConvertAll(options, DialogueConditionEditorUtility.GetDisplayLabel);
                int picked = EditorGUILayout.Popup("条件", currentIndex, labels);
                typeProp.intValue = (int)options[picked];
            }

            var conditionType = (ECondition)typeProp.intValue;
            if (DialogueConditionEditorUtility.IsFloatThresholdType(conditionType))
                floatProp.floatValue = EditorGUILayout.FloatField("阈值", floatProp.floatValue);
            else if (DialogueConditionEditorUtility.IsIntThresholdType(conditionType))
                intProp.intValue = EditorGUILayout.IntField("阈值", intProp.intValue);
        }

        static void AddCondition(DialogueGraph graph, SerializedProperty conditionsProp)
        {
            int index = conditionsProp.arraySize;
            conditionsProp.InsertArrayElementAtIndex(index);
            var conditionProp = conditionsProp.GetArrayElementAtIndex(index);

            string defaultName = graph?.VariableList != null && graph.VariableList.Count > 0
                ? graph.VariableList[0]?.name ?? string.Empty
                : string.Empty;

            conditionProp.FindPropertyRelative("variableName").stringValue = defaultName;
            var variableDef = graph?.FindBlackBoardVariable(defaultName);
            var defaultType = variableDef != null
                ? DialogueConditionEditorUtility.GetConditionOptions(variableDef.variableType)[0]
                : ECondition.None;
            conditionProp.FindPropertyRelative("eCondition").intValue = (int)defaultType;
        }

        static int FindChoiceIndex(DialogueNodeData node, DialogueTransLineData choice)
        {
            if (node?.ChoiceList == null || choice == null)
                return -1;

            for (int i = 0; i < node.ChoiceList.Count; i++)
            {
                if (ReferenceEquals(node.ChoiceList[i], choice))
                    return i;
            }

            return -1;
        }
    }
}
#endif
