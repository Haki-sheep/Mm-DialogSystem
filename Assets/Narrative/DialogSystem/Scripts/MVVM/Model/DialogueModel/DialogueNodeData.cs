using System.Collections.Generic;
using MiMieMVVM;
using UnityEngine;

namespace Miemie.DialogSystem
{
    /// <summary>
    /// 对话节点数据 内嵌于 DialogueGraph
    /// </summary>
    [System.Serializable]
    public class DialogueNodeData : IModelConfig
    {
        /// <summary> 节点ID </summary>
        [SerializeField]
        private int nodeConfigId;

        /// <summary> 说话类型 </summary>
        [SerializeField]
        private SpeakEnums speakType;

        /// <summary> 说话者名称 </summary>
        [SerializeField]
        private string speakerName;

        /// <summary> 对话文本 </summary>
        [SerializeField]
        private string dialogText;

        /// <summary> 是否是选项节点 </summary>
        [SerializeField]
        private bool isOptionNode;

        /// <summary> 普通节点下一跳 </summary>
        [SerializeField]
        private DialogueTransLineData nextTransition = new();

        /// <summary> 选项出口 </summary>
        [SerializeField]
        private List<DialogueTransLineData> choiceList = new();
  
        public int ConfigId => nodeConfigId;
        public string Name => nodeConfigId.ToString();
        public SpeakEnums SpeakType => speakType;
        public string SpeakerName => speakerName;
        public string DialogText { get => dialogText; set => dialogText = value; }
        public bool IsOptionNode { get => isOptionNode; set => isOptionNode = value; }
        public DialogueTransLineData NextTransition
        {
            get
            {
                if (nextTransition == null)
                    nextTransition = new DialogueTransLineData();
                return nextTransition;
            }
        }
        public List<DialogueTransLineData> ChoiceList => choiceList;

        #region 方法
        /// <summary>
        /// 验证节点
        /// </summary>
        public void VaildNode(DialogueGraph graph)
        {
            if (isOptionNode)
            {
                if (choiceList is null || choiceList.Count == 0)
                    Debug.LogError("ChoiceList is null or empty");
            }

            if (NextTransition.toNodeId == 0)
                Debug.LogWarning("Node is Over");
            else if (graph != null && graph.FindNode(NextTransition.toNodeId) == null)
                Debug.LogWarning($"Node [{nodeConfigId}] next target {NextTransition.toNodeId} not found");
        }

        /// <summary>
        /// 设置下一节点
        /// </summary>
        public void SetNextNode(DialogueNodeData node)
        {
            if (nextTransition == null)
                nextTransition = new DialogueTransLineData();

            nextTransition.toNodeId = node?.ConfigId ?? 0;
        }

        /// <summary>
        /// 清除下一节点
        /// </summary>
        public void ClearNextNode()
        {
            if (nextTransition != null)
                nextTransition.toNodeId = 0;
        }

        /// <summary>
        /// 添加选项节点
        /// </summary>
        public void AddChoice(DialogueTransLineData choice)
        {
            if (choiceList is null)
                choiceList = new List<DialogueTransLineData>();
            choiceList.Add(choice);
        }

        /// <summary>
        /// 移除选项
        /// </summary>
        public void RemoveChoice(DialogueTransLineData choice)
        {
            if (choiceList is not null)
                choiceList.Remove(choice);
        }

        /// <summary>
        /// 从末尾删除一个选项 至少保留一个
        /// </summary>
        public bool TryRemoveLastChoice()
        {
            if (choiceList is null || choiceList.Count <= 1)
                return false;

            choiceList.RemoveAt(choiceList.Count - 1);
            return true;
        }

        /// <summary>
        /// 获取选项
        /// </summary>
        public DialogueTransLineData GetChoice(int index)
        {
            if (choiceList is not null && index >= 0 && index < choiceList.Count)
                return choiceList[index];
            return null;
        }

        /// <summary>
        /// 清空选项
        /// </summary>
        public void ClearChoices()
        {
            if (choiceList is not null)
                choiceList.Clear();
        }

#if UNITY_EDITOR
        /// <summary>
        /// 编辑器设置节点ID
        /// </summary>
        public void SetNodeId(int id) => nodeConfigId = id;

        /// <summary>
        /// 编辑器设置说话类型
        /// </summary>
        public void SetSpeakType(SpeakEnums type) => speakType = type;

        /// <summary>
        /// 编辑器设置说话者名称
        /// </summary>
        public void SetSpeakerName(string name) => speakerName = name;
#endif
        #endregion
    }
}
