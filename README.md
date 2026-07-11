# M Dialog & Quest System

Unity 叙事子系统：节点式对话 + 任务进度，通过事件总线解耦联动。

- **Unity**：2022.3.53f1c1
- **命名空间**：`Miemie.DialogSystem` / `Miemie.DialogSystem.Quest`
- **仓库**：https://github.com/Haki-sheep/M_DialogSystem

---

## 能做什么

### 对话系统

- GraphView 可视化编辑对话图（节点 / 选项 / 条件连线）
- 图级变量（Float / Int / Bool）驱动分支
- 选项 `eventKey` 与整图结束信号，供任务等外部系统订阅
- JSON 导入导出
- MVVM：`DialogueRunner` + `DialogueViewModel` + `StandDialogView`
- 跨模块服务 `IDialogueCrossService`（播放指定对话图）

### 任务系统

- 配置侧 `QuestData` SO：目标组、前置任务、接取模式、限时
- 目标类型：对话 / 击杀 / 收集 / 到达
- 运行时：接受 → 推进 → 提交 / 失败；系统派发与手动接取
- 玩法事件进、任务生命周期事件出（`NarrativeEventBus`）
- 任务专属逻辑：`IQuestBehaviour` 虚方法（`OnAccepted` 等）
- 存档读写、限时失败
- 编辑器配置页 + Play 模式 GM（模拟击杀/对话、强制提交等）

### 二者如何联动

对话选项选中或整图结束时发布 `DialogueTriggered(graph, eventKey)`。  
任务目标配置为「对话」类型时，匹配同一张 `DialogueGraph` + `dialogueEventKey`（默认 `GraphFinished`）即可推进。

---

## 依赖

| 依赖 | 用途 |
|------|------|
| [Odin Inspector](https://odininspector.com/) | 编辑器菜单树、部分序列化 |
| Newtonsoft.Json | 对话图 JSON |
| UniTask | 异步（限时等） |
| `com.hakisheep.mm-mvvm` | MVVM / Cross 业务模块 |
| `com.hakisheep.mm-eventbus` | 类型安全事件总线 |

---

## 目录结构

```
Assets/Narrative/
├── DialogSystem/
│   ├── Demo/                 # 示例场景与演示图
│   ├── NodeSo/               # 对话图 SO + 布局资产
│   ├── Export/               # JSON 导出目录
│   ├── UIPrefab/             # 立绘对话面板预制体
│   └── Scripts/
│       ├── MonoRunner/       # DialogueRunner 入口
│       ├── MVVM/
│       │   ├── Model/        # Graph / Node / 跳转 / 条件 / 变量
│       │   ├── ViewModel/    # DialogueViewModel
│       │   ├── View/         # StandDialogView
│       │   └── Cross/        # DialogueCrossService
│       └── Editor/           # GraphView 窗口与工具
├── QuestSystem/
│   ├── QuestSo/              # 任务配置 SO
│   ├── NarrativeEvent/       # EventBus 封装与 EventKey
│   ├── MonoRunner/           # QuestManager（partial）
│   ├── MVVM/
│   │   ├── Model/            # 配置 / 运行时 / 存档
│   │   └── Cross/            # QuestCrossService
│   └── Editor/               # MmQuestWindow + GM 模拟
└── GraphViewFrame/           # 对话编辑器共用框架（面板布局等）
```

---

## 对话系统 · 使用

### 打开编辑器

菜单：**Tools → MmDialogWindow**

### 编辑

1. **新建对话图**（默认落在 `Assets/Narrative/DialogSystem/NodeSo/`）
2. 画布右键创建节点，拖拽 Out / 选项口连线
3. 点连线编条件；点节点编台词与是否选项节点
4. 左侧 Variables 定义图变量
5. 选项跳转可填 `eventKey`（给任务用）
6. **Ctrl+S** 保存；工具栏 `*` 表示未保存

### 运行时

1. 场景挂 `DialogueRunner`，指定图与变量黑板
2. 或通过 `IDialogueCrossService` / `DialogueRunner.PlayGraph` 播放
3. Demo：`Assets/Narrative/DialogSystem/Demo/DemoScene.unity`

### JSON

工具栏 **导出 JSON** / **导入 JSON**（默认目录 `DialogSystem/Export/`）

### 运行时 Debug 键

| 操作 | 按键 |
|------|------|
| 普通节点前进 | 空格 |
| 选选项 | 1 ~ 9 |

---

## 任务系统 · 使用

### 打开编辑器

菜单：**Tools → MmQuestWindow**

- **配置**：新建 / 编辑 `QuestData`（目标、前置、限时等）
- **GM**（需 Play + 场景有 `QuestManager`）：模拟事件、接受、提交、失败

### 场景接入

1. 挂 `QuestManager`，在 Inspector 把要用的 `QuestData` 拖进 `questList`
2. 可选：启动时读档 `loadSaveOnStart`
3. 玩法侧按需发布：

| 事件 | 含义 |
|------|------|
| `EnemyKilled(enemyKey, count)` | 击杀 |
| `ItemCollected(itemKey, count)` | 收集 |
| `ZoneEntered(zoneKey)` | 到达 |
| `DialogueTriggered(graph, eventKey)` | 对话信号（对话系统会发） |

4. 外部可订阅：`QuestAccepted` / `QuestProgressChanged` / `QuestCompleted` / `QuestFailed`
5. 跨模块：`IQuestCrossService`（查状态、接取、提交）

### 目标配置要点

- **击杀 / 收集 / 到达**：填 `targetKey`，与玩法事件 Key 一致
- **对话**：拖 `dialogueGraph`，填 `dialogueEventKey`（整图结束用 `GraphFinished`，或选项上的 `eventKey`）

### 公开 API（节选）

```csharp
QuestManager.Instance.Accept(questId);
QuestManager.Instance.TrySubmit(questId);
QuestManager.Instance.Fail(questId);
QuestManager.Instance.GetState(questId);
QuestManager.Instance.SaveQuests();
QuestManager.Instance.LoadQuests();
```

---

## 事件 Key 一览

定义在 `Assets/Narrative/QuestSystem/NarrativeEvent/NarrativeEventKeys.cs`。

**玩法 → 任务**：`EnemyKilled` / `ItemCollected` / `ZoneEntered` / `DialogueTriggered`  
**任务 → 外部**：`QuestAccepted` / `QuestProgressChanged` / `QuestCompleted` / `QuestFailed`  
**对话结束约定**：`DialogueGraphFinishedKey = "GraphFinished"`

---

## 架构简述

```
玩法 / 对话
    │ Publish(Inbound Keys)
    ▼
NarrativeEventBus
    │
    ├─► QuestManager（订阅玩法事件 → 推进 activeQuestList）
    │       │ Publish(Outbound Keys) + QuestData.OnXxx(context)
    │       ▼
    │   UI / 成就 / 关卡逻辑 …
    │
    └─► DialogueViewModel（播图、发 DialogueTriggered）
```

配置（SO）与运行时状态分离；任务存档只存 id / 状态 / 进度 / 限时，不重复存配置正文。

---

## License

学习与个人项目用途。Odin、第三方包请遵循各自授权。
