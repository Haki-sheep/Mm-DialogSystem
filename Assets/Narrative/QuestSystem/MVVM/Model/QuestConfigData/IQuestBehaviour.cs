namespace Miemie.DialogSystem.Quest
{
  /// <summary>
  /// 任务生命周期
  /// </summary>
  public interface IQuestBehaviour
  {
    /// <summary>
    /// 接受任务
    /// </summary>
    void OnAccepted(QuestStateContext context);

    /// <summary>
    /// 任务进度变化
    /// </summary>
    void OnProgressChanged(QuestStateContext context);

    /// <summary>
    /// 完成任务
    /// </summary>
    void OnCompleted(QuestStateContext context);

    /// <summary>
    /// 任务失败
    /// </summary>
    void OnFailed(QuestStateContext context);
  }
}
