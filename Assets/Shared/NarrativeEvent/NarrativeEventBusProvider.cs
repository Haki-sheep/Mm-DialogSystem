namespace Miemie.DialogSystem
{
    /// <summary>
    /// 叙事事件总线提供者
    /// 框架 Integration 启动时注入 Bridge
    /// </summary>
    public static class NarrativeEventBusProvider
    {
        /// <summary> 当前总线 </summary>
        public static INarrativeEventBus Instance { get; set; }
    }
}
