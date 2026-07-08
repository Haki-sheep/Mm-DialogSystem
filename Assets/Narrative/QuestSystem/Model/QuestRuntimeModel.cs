using MiMieMVVM;

namespace Miemie.DialogSystem.Quest
{
    /// <summary>
    /// 任务运行时模型
    /// </summary>
    public class QuestRuntimeModel : IModelState
    {
        /// <summary> 运行时状态 </summary>
        public readonly QuestRuntimeState State;

        public QuestRuntimeModel(QuestRuntimeState state)
        {
            State = state;
        }
    }
}
