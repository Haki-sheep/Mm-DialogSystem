using MiMieMVVM;

namespace Miemie.DialogSystem.Quest
{
    /// <summary>
    /// 任务跨模块服务
    /// </summary>
    public interface IQuestCrossService : ICrossBusinessModuleService
    {
        QuestManager Manager { get; }
        EQuestState GetState(int questId);
        bool Accept(int questId);
        bool TrySubmit(int questId);
    }

    /// <summary>
    /// 任务跨模块服务实现
    /// </summary>
    public class QuestCrossService : IQuestCrossService
    {
        /// <summary> 任务管理器 </summary>
        readonly QuestManager manager;

        public QuestManager Manager => manager;

        public QuestCrossService(QuestManager manager)
        {
            this.manager = manager;
        }

        /// <summary>
        /// 获取任务状态
        /// </summary>
        /// <param name="questId"></param>
        /// <returns></returns>
        public EQuestState GetState(int questId) => manager.GetState(questId);

        /// <summary>
        /// 接受任务
        /// </summary>
        /// <param name="questId"></param>
        /// <returns></returns>
        public bool Accept(int questId) => manager.Accept(questId);

        /// <summary>
        /// 尝试提交任务
        /// </summary>
        /// <param name="questId"></param>
        /// <returns></returns>
        public bool TrySubmit(int questId) => manager.TrySubmit(questId);
    }
}
