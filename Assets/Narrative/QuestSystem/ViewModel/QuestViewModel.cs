using System;
using MiMieMVVM;

namespace Miemie.DialogSystem.Quest
{
    /// <summary>
    /// 任务面板 ViewModel
    /// </summary>
    public class QuestViewModel : IViewModel
    {
        /// <summary> 跨模块服务 </summary>
        IQuestCrossService questService;

        /// <summary> 任务状态变更 </summary>
        public event Action<int, EQuestState> QuestStateChanged;

        public IQuestCrossService QuestService => questService;

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            questService = BusinessModuleHub.Instance.GetBusinessModule<IQuestCrossService>();
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Shutdown()
        {
            questService = null;
            QuestStateChanged = null;
        }

        /// <summary>
        /// 查询任务状态
        /// </summary>
        public EQuestState QueryState(int questId) =>
            questService?.GetState(questId) ?? EQuestState.未激活;

        /// <summary>
        /// 接受任务
        /// </summary>
        public bool AcceptQuest(int questId)
        {
            bool accepted = questService != null && questService.Accept(questId);
            if (accepted)
                QuestStateChanged?.Invoke(questId, questService.GetState(questId));
            return accepted;
        }

        /// <summary>
        /// 提交任务
        /// </summary>
        public bool SubmitQuest(int questId)
        {
            bool submitted = questService != null && questService.TrySubmit(questId);
            if (submitted)
                QuestStateChanged?.Invoke(questId, questService.GetState(questId));
            return submitted;
        }
    }
}
