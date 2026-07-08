#if UNITY_EDITOR
using UnityEngine;

namespace Miemie.DialogSystem.Editor
{
    /// <summary>
    /// 所有对话图的编辑器布局数据库
    /// </summary>
    public class DialogueGraphLayoutDatabase : ScriptableObject
    {
        /// <summary> 各图布局条目 </summary>
        public System.Collections.Generic.List<GraphLayoutEntry> graphs = new();
    }

    /// <summary>
    /// 单张对话图在编辑器中的节点布局
    /// </summary>
    [System.Serializable]
    public class GraphLayoutEntry
    {
        /// <summary> 对话图 </summary>
        public DialogueGraph graph;

        /// <summary> 节点布局列表 </summary>
        public System.Collections.Generic.List<NodeLayoutEntry> layouts = new();
    }

    /// <summary>
    /// 节点在 GraphView 画布上的坐标
    /// </summary>
    [System.Serializable]
    public class NodeLayoutEntry
    {
        /// <summary> 节点ID </summary>
        public int nodeId;

        /// <summary> 是否已保存坐标 </summary>
        public bool hasPosition;

        /// <summary> 画布坐标 </summary>
        public Vector2 position;
    }
}
#endif
