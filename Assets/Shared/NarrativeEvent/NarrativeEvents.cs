namespace Miemie.DialogSystem
{
    /// <summary>
    /// 叙事模块事件标识表
    /// </summary>
    public static class NarrativeEvents
    {
        /// <summary> 击杀 </summary>
        public static readonly NarrativeEventToken<EnemyKilledEventData> EnemyKilled =
            new("Gameplay.EnemyKilled");

        /// <summary> 收集 </summary>
        public static readonly NarrativeEventToken<ItemCollectedEventData> ItemCollected =
            new("Gameplay.ItemCollected");

        /// <summary> 进入区域 </summary>
        public static readonly NarrativeEventToken<ZoneEnteredEventData> ZoneEntered =
            new("Gameplay.ZoneEntered");

        /// <summary> 对话事件 </summary>
        public static readonly NarrativeEventToken<DialogueFinishedEventData> DialogueTriggered =
            new("Dialogue.EventTriggered");

        /// <summary> 任务接受 </summary>
        public static readonly NarrativeEventToken<int> QuestAccepted =
            new("Quest.Accepted");

        /// <summary> 任务进度变化 </summary>
        public static readonly NarrativeEventToken<QuestProgressChangedEventData> QuestProgressChanged =
            new("Quest.ProgressChanged");

        /// <summary> 任务完成 </summary>
        public static readonly NarrativeEventToken<int> QuestCompleted =
            new("Quest.Completed");

        /// <summary> 任务失败 </summary>
        public static readonly NarrativeEventToken<int> QuestFailed =
            new("Quest.Failed");
    }
}
