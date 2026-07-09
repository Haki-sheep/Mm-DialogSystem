#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Miemie.DialogSystem.Editor
{
    /// <summary>
    /// 节点在图资产中的序列化定位
    /// </summary>
    static class DialogueNodeEditorUtility
    {
        /// <summary>
        /// 查找节点在 nodeList 中的序列化属性
        /// </summary>
        public static SerializedProperty FindNodeProperty(DialogueGraph graph, DialogueNodeData node, out SerializedObject graphSo)
        {
            graphSo = new SerializedObject(graph);
            var list = graphSo.FindProperty("nodeList");
            if (list == null || node == null)
                return null;

            for (int i = 0; i < list.arraySize; i++)
            {
                var elem = list.GetArrayElementAtIndex(i);
                var idProp = elem.FindPropertyRelative("nodeConfigId");
                if (idProp != null && idProp.intValue == node.ConfigId)
                    return elem;
            }

            return null;
        }
    }
}
#endif
