using System.Collections.Generic;
using MiMieMVVM;

namespace Miemie.DialogSystem.Quest
{
  /// <summary>
  /// 单条任务运行时数据
  /// </summary>
  public class QuestRuntimeState : IModelState
  {
    /// <summary> 任务 </summary>
    public readonly QuestData questData;

    /// <summary> 目标列表 </summary>
    public readonly List<QuestGoal> goalList;

    /// <summary> 进度列表 </summary>
    public readonly List<int> progressList = new();

    /// <summary> 当前状态 </summary>
    public EQuestState eQuestState = EQuestState.未激活;

    /// <summary> 接取时间 </summary>
    public float acceptedAt;

    /// <summary>
    /// 限时结束时间
    /// </summary>
    public float timeLimitEndAt;

    /// <summary>
    /// 创建运行时数据
    /// </summary>
    public QuestRuntimeState(QuestData questData)
    {
      this.questData = questData;
      goalList = new List<QuestGoal>(questData.GetGoals());
      for (int i = 0; i < goalList.Count; i++)
        progressList.Add(0);
    }

    /// <summary>
    /// 目标是否全部完成
    /// </summary>
    public bool AllDone()
    {
      for (int i = 0; i < goalList.Count; i++)
      {
        if (goalList[i] == null) continue;
        int need = goalList[i].count > 0 ? goalList[i].count : 1;
        if (progressList[i] < need) return false;
      }
      return true;
    }

    /// <summary>
    /// 重置进度
    /// </summary>
    public void ResetProgress()
    {
      for (int i = 0; i < progressList.Count; i++)
        progressList[i] = 0;
    }

    /// <summary>
    /// 写入存档数据
    /// </summary>
    public QuestSaveData ToSaveData(float remainSeconds)
    {
      return new QuestSaveData
      {
        questId = questData.QuestId,
        eQuestState = eQuestState,
        progressList = new List<int>(progressList),
        acceptedAt = acceptedAt,
        remainSeconds = remainSeconds,
      };
    }

    /// <summary>
    /// 读取存档数据
    /// </summary>
    public void ApplySaveData(QuestSaveData saveData)
    {
      eQuestState = saveData.eQuestState;
      acceptedAt = saveData.acceptedAt;

      for (int i = 0; i < progressList.Count; i++)
      {
        if (saveData.progressList == null || i >= saveData.progressList.Count)
        {
          progressList[i] = 0;
          continue;
        }

        progressList[i] = saveData.progressList[i];
      }
    }
  }
}
