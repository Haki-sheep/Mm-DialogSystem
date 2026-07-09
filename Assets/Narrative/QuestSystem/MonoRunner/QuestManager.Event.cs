using System;
using MiMieEventBus;
using Miemie.DialogSystem;

namespace Miemie.DialogSystem.Quest
{
  public partial class QuestManager
  {
    /// <summary> 玩法事件订阅令牌 </summary>
    private IDisposable enemyKilledSub;
    private IDisposable itemCollectedSub;
    private IDisposable zoneEnteredSub;
    private IDisposable dialogueTriggeredSub;

    /// <summary>
    /// 订阅玩法事件
    /// </summary>
    private void ListenGameEvents()
    {
      var bus = NarrativeEventBus.NarrytiveBus;
      enemyKilledSub = bus.Subscribe(NarrativeEventKeys.EnemyKilled, OnEnemyKilled);
      itemCollectedSub = bus.Subscribe(NarrativeEventKeys.ItemCollected, OnItemCollected);
      zoneEnteredSub = bus.Subscribe(NarrativeEventKeys.ZoneEntered, OnZoneEntered);
      dialogueTriggeredSub = bus.Subscribe(NarrativeEventKeys.DialogueTriggered, OnDialogueTriggered);
    }

    /// <summary>
    /// 取消订阅玩法事件
    /// </summary>
    private void StopListenGameEvents()
    {
      enemyKilledSub?.Dispose();
      itemCollectedSub?.Dispose();
      zoneEnteredSub?.Dispose();
      dialogueTriggeredSub?.Dispose();

      enemyKilledSub = null;
      itemCollectedSub = null;
      zoneEnteredSub = null;
      dialogueTriggeredSub = null;
    }

    /// <summary>
    /// 收到击杀
    /// </summary>
    private void OnEnemyKilled(string enemyKey, int count)
    {
      AdvanceMatchedGoals(EQuestGoalType.击杀, count,
        goal => !string.IsNullOrEmpty(goal.targetKey) && goal.targetKey == enemyKey);
    }

    /// <summary>
    /// 收到收集
    /// </summary>
    private void OnItemCollected(string itemKey, int count)
    {
      AdvanceMatchedGoals(EQuestGoalType.收集, count,
        goal => !string.IsNullOrEmpty(goal.targetKey) && goal.targetKey == itemKey);
    }

    /// <summary>
    /// 收到进入区域
    /// </summary>
    private void OnZoneEntered(string zoneKey)
    {
      AdvanceMatchedGoals(EQuestGoalType.到达, 1,
        goal => !string.IsNullOrEmpty(goal.targetKey) && goal.targetKey == zoneKey);
    }

    /// <summary>
    /// 收到对话信号
    /// </summary>
    private void OnDialogueTriggered(DialogueGraph graph, string eventKey)
    {
      AdvanceMatchedGoals(EQuestGoalType.对话, 1,
        goal => goal.dialogueGraph == graph && goal.dialogueEventKey == eventKey);
    }
  }
}
