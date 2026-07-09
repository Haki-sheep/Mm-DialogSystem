#if UNITY_EDITOR
using Miemie.DialogSystem;
using UnityEngine;

namespace Miemie.DialogSystem.Quest.Editor
{
  /// <summary>
  /// GM 面板按目标类型模拟一次游戏事件
  /// </summary>
  static class QuestGmSimulation
  {
    /// <summary>
    /// 模拟按钮显示文本
    /// </summary>
    public static string ButtonLabel(QuestGoal goal)
    {
      if (goal == null) return "模拟";
      return goal.type switch
      {
        EQuestGoalType.对话 => "对话",
        EQuestGoalType.击杀 => "击杀",
        EQuestGoalType.收集 => "收集",
        EQuestGoalType.到达 => "到达",
        _ => "模拟",
      };
    }

    /// <summary>
    /// 按目标类型触发一次游戏事件
    /// </summary>
    public static void FireOnce(QuestGoal goal)
    {
      if (goal == null) return;

      var bus = NarrativeEventBus.NarrytiveBus;
      switch (goal.type)
      {
        case EQuestGoalType.对话:
          bus.Publish(NarrativeEventKeys.DialogueTriggered, goal.dialogueGraph, goal.dialogueEventKey);
          break;
        case EQuestGoalType.击杀:
          bus.Publish(NarrativeEventKeys.EnemyKilled, goal.targetKey, 1);
          break;
        case EQuestGoalType.收集:
          bus.Publish(NarrativeEventKeys.ItemCollected, goal.targetKey, 1);
          break;
        case EQuestGoalType.到达:
          bus.Publish(NarrativeEventKeys.ZoneEntered, goal.targetKey);
          break;
      }
    }
  }
}
#endif
