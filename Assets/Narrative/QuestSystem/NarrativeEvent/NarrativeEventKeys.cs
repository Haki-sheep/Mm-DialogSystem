using MiMieEventBus;

namespace Miemie.DialogSystem
{
    /// <summary>
    /// 任务/对话系统 的 事件key
    /// </summary>
    public static class NarrativeEventKeys
    {
        /// <summary> 整图对话结束信号 </summary>
        public const string DialogueGraphFinishedKey = "GraphFinished";

        #region Inbound 玩法 → 任务

        /// <summary> 击杀 enemyKey count </summary>
        public static readonly EventKey<string, int> EnemyKilled =
            new EventKey<string, int>("Gameplay.EnemyKilled");

        /// <summary> 收集 itemKey count </summary>
        public static readonly EventKey<string, int> ItemCollected =
            new EventKey<string, int>("Gameplay.ItemCollected");

        /// <summary> 进入区域 zoneKey </summary>
        public static readonly EventKey<string> ZoneEntered =
            new EventKey<string>("Gameplay.ZoneEntered");

        /// <summary> 对话信号 graph eventKey </summary>
        public static readonly EventKey<DialogueGraph, string> DialogueTriggered =
            new EventKey<DialogueGraph, string>("Dialogue.Triggered");

        #endregion

        #region Outbound 任务 → 外部

        /// <summary> 任务接受 questId </summary>
        public static readonly EventKey<int> QuestAccepted =
            new EventKey<int>("Quest.Accepted");

        /// <summary> 任务进度 questId goalIndex current need </summary>
        public static readonly EventKey<int, int, int, int> QuestProgressChanged =
            new EventKey<int, int, int, int>("Quest.ProgressChanged");

        /// <summary> 任务完成 questId </summary>
        public static readonly EventKey<int> QuestCompleted =
            new EventKey<int>("Quest.Completed");

        /// <summary> 任务失败 questId </summary>
        public static readonly EventKey<int> QuestFailed =
            new EventKey<int>("Quest.Failed");

        #endregion
    }
}
