using MiMieMVVM;

namespace Miemie.DialogSystem
{
    /// <summary>
    /// 对话图配表模型
    /// </summary>
    public class DialogueGraphConfig : IModelConfig
    {
        /// <summary> 对话图资产 </summary>
        readonly DialogueGraph graph;

        public DialogueGraph Graph => graph;
        public int ConfigId => graph != null ? graph.GraphId : 0;
        public string Name => graph != null ? graph.GraphName : string.Empty;

        public DialogueGraphConfig(DialogueGraph graph)
        {
            this.graph = graph;
        }
    }
}
