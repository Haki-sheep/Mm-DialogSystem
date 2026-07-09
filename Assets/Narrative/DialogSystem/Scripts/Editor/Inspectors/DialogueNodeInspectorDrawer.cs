#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Miemie.DialogSystem.Editor
{
    /// <summary>
    /// 节点 Inspector 只显示台词内容 连线在画布上编辑
    /// </summary>
    static class DialogueNodeInspectorDrawer
    {
        public static void Draw(SerializedProperty nodeProp)
        {
            if (nodeProp == null)
                return;

            EditorGUILayout.PropertyField(nodeProp.FindPropertyRelative("nodeConfigId"));
            EditorGUILayout.PropertyField(nodeProp.FindPropertyRelative("speakType"));
            EditorGUILayout.PropertyField(nodeProp.FindPropertyRelative("speakerName"));
            EditorGUILayout.PropertyField(nodeProp.FindPropertyRelative("dialogText"));
            EditorGUILayout.PropertyField(nodeProp.FindPropertyRelative("isOptionNode"));

            EditorGUILayout.Space(6);
            EditorGUILayout.HelpBox("出口连线与条件请在画布上点击连线编辑", MessageType.Info);
        }
    }
}
#endif
