using System.Collections.Generic;
using MiMieMVVM;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Miemie.DialogSystem
{
    /// <summary>
    /// 对话图
    /// 节点内嵌于本资产 一张图一个 SO
    /// </summary>
    [CreateAssetMenu(fileName = "New Dialogue Graph", menuName = "Dialog System/Dialogue Graph")]
    public class DialogueGraph : ScriptableObject,IModelConfig
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
        private List<DialogueNodeData> nodeList = new();

        /// <summary> 图变量声明 </summary>
        [SerializeField, HideInInspector]
        private List<DialogueVariableData> variableList = new();
        #endregion

        #region 属性
        public int ConfigId => graphId;
        public string Name => graphName;
        public int StartNodeId => startNodeId;
        public DialogueNodeData StartNode => FindNode(startNodeId);
        public List<DialogueNodeData> NodeList => nodeList;
        public List<DialogueVariableData> VariableList => variableList;
        #endregion

        #region 方法
        /// <summary>
        /// 按 ID 查找节点
        /// </summary>
        public DialogueNodeData FindNode(int nodeId)
        {
            if (nodeId == 0 || nodeList == null)
                return null;

            foreach (var node in nodeList)
            {
                if (node != null && node.ConfigId == nodeId)
                    return node;
            }

            return null;
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        public void AddNode(DialogueNodeData node)
        {
            if (nodeList is null)
                nodeList = new List<DialogueNodeData>();
            nodeList.Add(node);
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        public void RemoveNode(DialogueNodeData node)
        {
            if (nodeList is null)
            {
                Debug.LogError("Node list is null, please add node first");
                return;
            }
            nodeList.Remove(node);
        }

        /// <summary>
        /// 查找黑板变量
        /// </summary>
        public DialogueVariableData FindBlackBoardVariable(string variableName)
        {
            if (string.IsNullOrEmpty(variableName) || variableList == null)
                return null;

            foreach (var item in variableList)
            {
                if (item != null && item.name == variableName)
                    return item;
            }

            return null;
        }

#if UNITY_EDITOR
        /// <summary>
        /// 设置开始节点
        /// </summary>
        public void SetStartNodeInEditorWindow(DialogueNodeData node)
        {
            startNodeId = node?.ConfigId ?? 0;
        }
#endif
        #endregion
    }
}
