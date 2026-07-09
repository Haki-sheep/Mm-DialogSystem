using System;
using System.Collections.Generic;
using UnityEngine;

namespace Miemie.DialogSystem
{
    /// <summary>
    /// 对话跳转
    /// 普通节点 Out 与选项节点出口共用
    /// labelText 仅选项出口使用
    /// </summary>
    [Serializable]
    public class DialogueTransLineData
    {
        /// <summary> 选项文本 普通跳转可留空 </summary>
        public string labelText;

        /// <summary> 对话事件Key </summary>
        public string eventKey;

        /// <summary> 跳向的节点ID </summary>
        public int toNodeId;

        /// <summary> 条件列表 </summary>
        [SerializeField]
        List<DialogueConditionData> conditionList = new();

        public List<DialogueConditionData> ConditionList => conditionList;

        /// <summary>
        /// 解析目标节点
        /// </summary>
        public DialogueNodeData ResolveToNode(DialogueGraph graph) => graph?.FindNode(toNodeId);

        /// <summary>
        /// 判断跳转是否可通过
        /// </summary>
        public bool CanPass(DialogueVariablesBlackBoard variables)
        {
            if (conditionList == null || conditionList.Count == 0)
                return true;

            foreach (var item in conditionList)
            {
                if (item == null || item.NoneContion)
                    continue;

                if (!item.MeetCondition(variables))
                    return false;
            }

            return true;
        }
    }
}
