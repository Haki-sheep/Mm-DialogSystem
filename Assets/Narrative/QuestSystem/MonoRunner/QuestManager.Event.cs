using Miemie.DialogSystem;

namespace Miemie.DialogSystem.Quest
{
  public partial class QuestManager
  {
    /// <summary>
    /// 订阅玩法事件
    /// </summary>
    private void ListenGameEvents()
    {
      var bus = NarrativeEventBusProvider.Instance;
      if (bus == null)
        return;

      bus.Subscribe(NarrativeEvents.EnemyKilled, OnEnemyKilled);
      bus.Subscribe(NarrativeEvents.ItemCollected, OnItemCollected);
      bus.Subscribe(NarrativeEvents.ZoneEntered, OnZoneEntered);
      bus.Subscribe(NarrativeEvents.DialogueTriggered, OnDialogueEvent);
    }

    /// <summary>
    /// 停止订阅玩法事件
    /// </summary>
    private void StopListenGameEvents()
    {
      var bus = NarrativeEventBusProvider.Instance;
      if (bus == null)
        return;

      bus.Unsubscribe(NarrativeEvents.EnemyKilled, OnEnemyKilled);
      bus.Unsubscribe(NarrativeEvents.ItemCollected, OnItemCollected);
      bus.Unsubscribe(NarrativeEvents.ZoneEntered, OnZoneEntered);
      bus.Unsubscribe(NarrativeEvents.DialogueTriggered, OnDialogueEvent);
    }

    /// <summary>
    /// 收到击杀事件
    /// </summary>
    private void OnEnemyKilled(EnemyKilledEventData eventData)
    {
      AdvanceByKey(EQuestObjectiveType.击杀, eventData.enemyKey, eventData.count);
    }

    /// <summary>
    /// 收到收集事件
    /// </summary>
    private void OnItemCollected(ItemCollectedEventData eventData)
    {
      AdvanceByKey(EQuestObjectiveType.收集, eventData.itemKey, eventData.count);
    }

    /// <summary>
    /// 收到进入区域事件
    /// </summary>
    private void OnZoneEntered(ZoneEnteredEventData eventData)
    {
      AdvanceByKey(EQuestObjectiveType.到达, eventData.zoneKey, 1);
    }

    /// <summary>
    /// 收到对话事件
    /// </summary>
    private void OnDialogueEvent(DialogueFinishedEventData eventData)
    {
      AdvanceDialogue(eventData.graph, eventData.eventKey, 1);
    }
  }
}
