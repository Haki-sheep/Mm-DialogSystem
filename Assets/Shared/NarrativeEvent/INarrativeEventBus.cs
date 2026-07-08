using System;

namespace Miemie.DialogSystem
{
    /// <summary>
    /// 叙事事件总线契约
    /// 叙事模块不依赖框架总线具体实现
    /// </summary>
    public interface INarrativeEventBus
    {
        /// <summary>
        /// 发布事件
        /// </summary>
        void Publish<T>(NarrativeEventToken<T> token, T payload);

        /// <summary>
        /// 订阅事件
        /// </summary>
        void Subscribe<T>(NarrativeEventToken<T> token, Action<T> handler);

        /// <summary>
        /// 取消订阅
        /// </summary>
        void Unsubscribe<T>(NarrativeEventToken<T> token, Action<T> handler);
    }
}
