using System.Collections.Generic;
using Sirenix.Serialization;

namespace Miemie.DialogSystem
{
    /// <summary>
    /// 对话图上的黑板
    /// </summary>
    public class DialogueVariablesBlackBoard
    {
        [OdinSerialize]
        private Dictionary<string, bool> boolDict = new();

        [OdinSerialize]
        private Dictionary<string, float> floatDict = new();

        [OdinSerialize]
        private Dictionary<string, int> intDict = new();

        /// <summary>
        /// 用变量声明初始化默认值
        /// </summary>
        public void InitDefaultVariables(IReadOnlyList<DialogueVariableData> variableList)
        {
            if (variableList == null)
                return;

            foreach (var item in variableList)
            {
                if (item == null || string.IsNullOrEmpty(item.name))
                    continue;

                switch (item.variableType)
                {
                    case EDialogueVariableType.Float:
                        SetFloat(item.name, item.defaultFloat);
                        break;
                    case EDialogueVariableType.Int:
                        SetInt(item.name, item.defaultInt);
                        break;
                    case EDialogueVariableType.Bool:
                        SetBool(item.name, item.defaultBool);
                        break;
                }
            }
        }

        /// <summary>
        /// 获取布尔值
        /// </summary>
        public bool GetBool(string variableName, bool defaultValue = false)
        {
            if (string.IsNullOrEmpty(variableName))
                return defaultValue;

            return boolDict.TryGetValue(variableName, out var value) ? value : defaultValue;
        }

        /// <summary>
        /// 设置布尔值
        /// </summary>
        public void SetBool(string variableName, bool value)
        {
            if (string.IsNullOrEmpty(variableName))
                return;

            boolDict[variableName] = value;
        }

        /// <summary>
        /// 获取浮点值
        /// </summary>
        public float GetFloat(string variableName, float defaultValue = 0f)
        {
            if (string.IsNullOrEmpty(variableName))
                return defaultValue;

            return floatDict.TryGetValue(variableName, out var value) ? value : defaultValue;
        }

        /// <summary>
        /// 设置浮点值
        /// </summary>
        public void SetFloat(string variableName, float value)
        {
            if (string.IsNullOrEmpty(variableName))
                return;

            floatDict[variableName] = value;
        }

        /// <summary>
        /// 获取整数值
        /// </summary>
        public int GetInt(string variableName, int defaultValue = 0)
        {
            if (string.IsNullOrEmpty(variableName))
                return defaultValue;

            return intDict.TryGetValue(variableName, out var value) ? value : defaultValue;
        }

        /// <summary>
        /// 设置整数值
        /// </summary>
        public void SetInt(string variableName, int value)
        {
            if (string.IsNullOrEmpty(variableName))
                return;

            intDict[variableName] = value;
        }
    }
}
