#if UNITY_EDITOR
using UnityEngine;

namespace Miemie.DialogSystem.Editor
{
    /// <summary>
    /// 画布上选中的连线句柄
    /// </summary>
    public class DialogueTransitionHandle
    {
        public DialogueGraph graph;
        public DialogueNodeData sourceNode;
        public DialogueNodeData targetNode;
        public DialogueTransLineData choiceTransition;

        public bool IsOptionTransition => choiceTransition != null;

        public string Title
        {
            get
            {
                string from = sourceNode != null ? $"[{sourceNode.ConfigId}]" : "?";
                string to = targetNode != null ? $"[{targetNode.ConfigId}]" : "?";
                return IsOptionTransition ? $"{from} 选项 → {to}" : $"{from} → {to}";
            }
        }
    }
}
#endif
