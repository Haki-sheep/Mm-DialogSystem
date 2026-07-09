using Miemie.DialogSystem;
using UnityEngine;

namespace Miemie.DialogSystem.Quest
{
  public partial class QuestManager
  {
    /// <summary>
    /// 创建运行时数据
    /// </summary>
    private void BuildRuntimeState()
    {
      stateDict.Clear();
      activeQuestList.Clear();

      foreach (var quest in questList)
      {
        if (quest == null) continue;

        if (stateDict.ContainsKey(quest.QuestId))
        {
          Debug.LogWarning($"[Quest] 任务ID重复 {quest.QuestId} 请检查 {quest.name}");
          continue;
        }

        stateDict[quest.QuestId] = new QuestRuntimeState(quest);
        if (stateDict[quest.QuestId].goalList.Count == 0)
          Debug.LogWarning($"[Quest] {quest.Title} 无目标 请在 goalList 添加步骤");
      }

      if (stateDict.Count == 0)
        Debug.LogWarning("[Quest] questList 为空 请在 Inspector 拖入 Quest 资源");
    }

    /// <summary>
    /// 接受系统派发任务
    /// </summary>
    private void AcceptSystemQuests()
    {
      // 遍历所有任务
      foreach (var runtime in stateDict.Values)
      {
        // 如果任务状态为可用且接受模式为系统派发，则接受任务
        if (runtime.eQuestState == EQuestState.可用
                && runtime.questData.AcceptMode == EQuestAcceptMode.系统派发)

          Accept(runtime.questData.QuestId);
      }
    }

    /// <summary>
    /// 刷新可接任务
    /// </summary>
    private void RefreshAvailable()
    {
      foreach (var runtime in stateDict.Values)
      {
        if (runtime.eQuestState != EQuestState.未激活) continue;

        bool ready = true;
        // 遍历前置任务列表
        foreach (int preId in runtime.questData.PrerequisiteIdList)
        {
          // 如果前置任务未完成，则当前任务不可用
          if (GetState(preId) != EQuestState.提交)
          {
            ready = false;
            break;
          }
        }

        // 如果前置任务都完成，则当前任务可用
        if (ready) runtime.eQuestState = EQuestState.可用;
      }
    }

    /// <summary>
    /// 推进匹配的任务目标
    /// </summary>
    private void AdvanceMatchedGoals(EQuestGoalType type, int delta, System.Func<QuestGoal, bool> isMatch)
    {
      // 遍历执行中任务
      for (int q = 0; q < activeQuestList.Count; q++)
      {
        var runtime = activeQuestList[q];
        if (runtime.eQuestState != EQuestState.执行中) continue;

        // 遍历任务目标
        for (int i = 0; i < runtime.goalList.Count; i++)
        {
          var goal = runtime.goalList[i];
          if (goal == null) continue;
          if (goal.type != type) continue;
          if (!isMatch(goal)) continue;

          int need = goal.count > 0 ? goal.count : 1;
          if (runtime.progressList[i] >= need) continue;

          int currentCount = Mathf.Min(runtime.progressList[i] + delta, need);
          runtime.progressList[i] = currentCount;
          NotifyProgressChanged(runtime, i, currentCount, need);
        }
      }
    }

    /// <summary>
    /// 完成任务
    /// </summary>
    private void CompleteQuest(QuestRuntimeState runtime)
    {
      if (!runtime.AllDone()) return;
      if (runtime.eQuestState != EQuestState.执行中) return;

      CancelTimeLimit(runtime.questData.QuestId);
      activeQuestList.Remove(runtime);
      runtime.eQuestState = EQuestState.提交;

      Debug.Log($"[Quest] 提交 {runtime.questData.Title} (id={runtime.questData.QuestId})");
      NotifyCompleted(runtime);
      RefreshAvailable();
    }

    /// <summary>
    /// 通知接受任务
    /// </summary>
    private void NotifyAccepted(QuestRuntimeState runtime)
    {
      // 创建上下文
      var context = CreateContext(runtime, -1, 0, 0);
      // 调用任务的接受回调
      runtime.questData.OnAccepted(context);
      // 事件总线发布任务接受事件
      NarrativeEventBus.NarrytiveBus.Publish(NarrativeEventKeys.QuestAccepted, runtime.questData.QuestId);
    }

    /// <summary>
    /// 通知进度变化
    /// </summary>
    private void NotifyProgressChanged(QuestRuntimeState runtime, int goalIndex, int currentCount, int needCount)
    {
      // 创建上下文
      var context = CreateContext(runtime, goalIndex, currentCount, needCount);
      // 调用任务的进度变化回调
      runtime.questData.OnProgressChanged(context);
      // 事件总线发布任务进度变化事件
      NarrativeEventBus.NarrytiveBus.Publish(
        NarrativeEventKeys.QuestProgressChanged,
        runtime.questData.QuestId,
        goalIndex,
        currentCount,
        needCount);
    }

    /// <summary>
    /// 通知完成任务
    /// </summary>
    private void NotifyCompleted(QuestRuntimeState runtime)
    {
      var context = CreateContext(runtime, -1, 0, 0);
      runtime.questData.OnCompleted(context);
      NarrativeEventBus.NarrytiveBus.Publish(NarrativeEventKeys.QuestCompleted, runtime.questData.QuestId);
    }

    /// <summary>
    /// 通知任务失败
    /// </summary>
    private void NotifyFailed(QuestRuntimeState runtime)
    {
      var context = CreateContext(runtime, -1, 0, 0);
      runtime.questData.OnFailed(context);
      NarrativeEventBus.NarrytiveBus.Publish(NarrativeEventKeys.QuestFailed, runtime.questData.QuestId);
    }

    /// <summary>
    /// 创建生命周期上下文
    /// </summary>
    /// <param name="runtime">任务运行时数据</param>
    /// <param name="goalIndex">目标序号</param>
    /// <param name="currentCount">当前进度</param>
    /// <param name="needCount">目标数量</param>
    /// <returns>生命周期上下文</returns>
    private QuestStateContext CreateContext(QuestRuntimeState runtime,
                                                int goalIndex,
                                                int currentCount,
                                                int needCount)
    {
      return new QuestStateContext(
        runtime.questData,
        runtime.eQuestState,
        goalIndex,
        currentCount,
        needCount,
        GetRemainSeconds(runtime));
    }
  }
}
