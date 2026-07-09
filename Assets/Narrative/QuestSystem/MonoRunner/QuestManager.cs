using System.Collections.Generic;
using System.Threading;
using MiMieMVVM;
using UnityEngine;

namespace Miemie.DialogSystem.Quest
{
  public partial class QuestManager : MonoBehaviour
  {
    [Header("配置侧任务列表")]
    [SerializeField]
    private List<QuestData> questList = new();

    [Header("存档设置")]
    [SerializeField]
    private bool loadSaveOnStart = true;

    /// <summary>
    /// 已注册任务字典
    /// </summary>
    private readonly Dictionary<int, QuestRuntimeState> stateDict = new();

    /// <summary>
    /// 执行中任务列表
    /// </summary>
    private readonly List<QuestRuntimeState> activeQuestList = new();

    /// <summary>
    /// 限时取消字典
    /// </summary>
    private readonly Dictionary<int, CancellationTokenSource> timeLimitDict = new();

    /// <summary>
    /// 任务跨模块服务
    /// </summary>
    private QuestCrossService crossService;

    public static QuestManager Instance { get; private set; }

    public IReadOnlyList<QuestData> RegisteredQuests => questList;

    #region 生命周期

    /// <summary>
    /// 单例初始化
    /// </summary>
    private void Awake()
    {
      if (Instance != null && Instance != this) { Destroy(gameObject); return; }
      Instance = this;
      crossService = new QuestCrossService(this);
      BusinessModuleHub.Instance.RegisterBusinessModule(crossService);
    }

    /// <summary>
    /// 订阅玩法事件
    /// </summary>
    private void OnEnable()
    {
      ListenGameEvents();
    }

    /// <summary>
    /// 取消订阅玩法事件
    /// </summary>
    private void OnDisable()
    {
      StopListenGameEvents();
      StopAllTimeLimit();
    }

    /// <summary>
    /// 清空单例
    /// </summary>
    private void OnDestroy()
    {
      if (crossService != null)
        BusinessModuleHub.Instance.UnregisterBusinessModule<IQuestCrossService>();
      if (Instance == this) Instance = null;
    }

    /// <summary>
    /// 初始化并派发系统任务
    /// </summary>
    private void Start()
    {
      BuildRuntimeState();

      bool loadedSave = loadSaveOnStart && LoadQuests();

      RefreshAvailable();

      if (!loadedSave)
        AcceptSystemQuests();
    }

    #endregion

    #region 公开接口

    /// <summary>
    /// 接受或重接任务
    /// </summary>
    public bool Accept(int questId)
    {
      // 检查任务是否存在
      if (!stateDict.TryGetValue(questId, out var runtime)) return false;

      // 如果任务状态为不可用、提交或失败，则不接受任务
      if (runtime.eQuestState != EQuestState.可用
          && runtime.eQuestState != EQuestState.提交
          && runtime.eQuestState != EQuestState.失败) return false;
    
      // 取消该任务的限时功能
      CancelTimeLimit(questId);
      // 移出执行列表
      activeQuestList.Remove(runtime);
      // 重置该任务的进度
      runtime.ResetProgress();
      // 设置任务状态为执行中
      runtime.eQuestState = EQuestState.执行中;
      runtime.acceptedAt = Time.time;
      // 添加到执行列表
      activeQuestList.Add(runtime);
      // 如果任务有时间限制，则启动限时功能
      if (runtime.questData.HasTimeLimit)
        StartTimeLimit(questId, runtime.questData.TimeLimit);
  
      // 通知任务接受
      NotifyAccepted(runtime);
      return true;
    }

    /// <summary>
    /// 标记失败
    /// </summary>
    public bool Fail(int questId)
    {
      if (!stateDict.TryGetValue(questId, out var runtime)) return false;
      if (runtime.eQuestState != EQuestState.执行中) return false;

      CancelTimeLimit(questId);
      activeQuestList.Remove(runtime);
      runtime.eQuestState = EQuestState.失败;
      NotifyFailed(runtime);
      return true;
    }

    /// <summary>
    /// 提交任务
    /// </summary>
    public bool Submit(int questId) => TrySubmit(questId);

    /// <summary>
    /// 尝试提交
    /// </summary>
    public bool TrySubmit(int questId)
    {
      if (!stateDict.TryGetValue(questId, out var runtime)) return false;
      if (runtime.eQuestState != EQuestState.执行中) return false;
      if (!runtime.AllDone()) return false;

      CompleteQuest(runtime);
      return runtime.eQuestState == EQuestState.提交;
    }

#if UNITY_EDITOR
    /// <summary>
    /// GM 直接提交
    /// </summary>
    public bool GmSubmit(int questId)
    {
      if (!stateDict.TryGetValue(questId, out var runtime)) return false;
      if (runtime.eQuestState != EQuestState.执行中) return false;

      for (int i = 0; i < runtime.goalList.Count; i++)
      {
        if (runtime.goalList[i] == null) continue;
        int need = runtime.goalList[i].count > 0 ? runtime.goalList[i].count : 1;
        runtime.progressList[i] = need;
        NotifyProgressChanged(runtime, i, need, need);
      }

      CompleteQuest(runtime);
      return runtime.eQuestState == EQuestState.提交;
    }
#endif

    /// <summary>
    /// 查询状态
    /// </summary>
    public EQuestState GetState(int questId)
    {
      if (!stateDict.TryGetValue(questId, out var runtime)) return EQuestState.未激活;
      return runtime.eQuestState;
    }

    /// <summary>
    /// 查询任务定义
    /// </summary>
    public QuestData GetQuest(int questId)
    {
      if (!stateDict.TryGetValue(questId, out var runtime)) return null;
      return runtime.questData;
    }

    /// <summary>
    /// 查询目标数量
    /// </summary>
    public int GetGoalCount(int questId)
    {
      if (!stateDict.TryGetValue(questId, out var runtime)) return 0;
      return runtime.goalList.Count;
    }

    /// <summary>
    /// 查询目标进度
    /// </summary>
    public int GetGoalProgress(int questId, int index)
    {
      if (!stateDict.TryGetValue(questId, out var runtime)) return 0;
      if (index < 0 || index >= runtime.progressList.Count) return 0;
      return runtime.progressList[index];
    }

    #endregion
  }
}
