using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Miemie.DialogSystem
{
    /// <summary>
    /// 对话图
    /// 节点内嵌于本资产 一张图一个 SO
    /// </summary>
    [CreateAssetMenu(fileName = "New Dialogue Graph", menuName = "Dialog System/Dialogue Graph")]
    public class DialogueGraph : ScriptableObject
    {
        #region 字段
        /// <summary> 对话图ID </summary>
        [SerializeField]
        private int graphId;

        /// <summary> 对话图名称 </summary>
        [SerializeField]
        private string graphName;

        /// <summary> 开始节点ID </summary>
        [SerializeField]
        private int startNodeId;

        /// <summary> 节点列表 </summary>
        [SerializeField]
        private List<DialogueNode> nodeList = new();

        /// <summary> 图变量声明 </summary>
        [SerializeField, HideInInspector]
        private List<DialogueVariableDef> variableList = new();
        #endregion

        #region 属性
        public int GraphId => graphId;
        public string GraphName => graphName;
        public int StartNodeId => startNodeId;
        public DialogueNode StartNode => FindNode(startNodeId);
        public List<DialogueNode> NodeList => nodeList;
        public List<DialogueVariableDef> Variables => variableList;
        #endregion

        #region 方法
        /// <summary>
        /// 按 ID 查找节点
        /// </summary>
        public DialogueNode FindNode(int nodeId)
        {
            if (nodeId == 0 || nodeList == null)
                return null;

            foreach (var node in nodeList)
            {
                if (node != null && node.NodeId == nodeId)
                    return node;
            }

            return null;
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        public void AddNode(DialogueNode node)
        {
            if (nodeList is null)
                nodeList = new List<DialogueNode>();
            nodeList.Add(node);
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        public void RemoveNode(DialogueNode node)
        {
            if (nodeList is null)
            {
                Debug.LogError("Node list is null, please add node first");
                return;
            }
            nodeList.Remove(node);
        }

        /// <summary>
        /// 查找变量声明
        /// </summary>
        public DialogueVariableDef FindVariable(string variableName)
        {
            if (string.IsNullOrEmpty(variableName) || variableList == null)
                return null;

            foreach (var def in variableList)
            {
                if (def != null && def.name == variableName)
                    return def;
            }

            return null;
        }

#if UNITY_EDITOR
        /// <summary>
        /// 设置开始节点
        /// </summary>
        public void SetStartNodeInEditorWindow(DialogueNode node)
        {
            startNodeId = node?.NodeId ?? 0;
        }
#endif
        #endregion
    }
}
