namespace Miemie.DialogSystem.Quest
{
  /// <summary>
  /// 任务状态上下文
  /// </summary>
  public readonly struct QuestStateContext
  {
    /// <summary>
    /// 任务定义
    /// </summary>
    public readonly QuestData quest;

    /// <summary>
    /// 任务状态
    /// </summary>
    public readonly EQuestState eQuestState;

    /// <summary>
    /// 目标序号
    /// </summary>
    public readonly int goalIndex;

    /// <summary>
    /// 当前进度
    /// </summary>
    public readonly int currentCount;

    /// <summary>
    /// 目标数量
    /// </summary>
    public readonly int needCount;

    /// <summary>
    /// 剩余时间
    /// </summary>
    public readonly float remainSeconds;

    /// <summary>
    /// 创建生命周期上下文
    /// </summary>
    public QuestStateContext(
      QuestData quest,
      EQuestState eQuestState,
      int goalIndex,
      int currentCount,
      int needCount,
      float remainSeconds)
    {
      this.quest = quest;
      this.eQuestState = eQuestState;
      this.goalIndex = goalIndex;
      this.currentCount = currentCount;
      this.needCount = needCount;
      this.remainSeconds = remainSeconds;
    }
  }
}
