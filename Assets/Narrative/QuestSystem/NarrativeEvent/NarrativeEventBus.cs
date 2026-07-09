using MiMieEventBus;

namespace Miemie.DialogSystem
{
    /// <summary>
    /// 叙事模块共用事件总线入口
    /// </summary>
    public static class NarrativeEventBus
    {
        /// <summary> 全局总线实例 </summary>
        public static EventBusCore NarrytiveBus { get; } = new();
    }
}
