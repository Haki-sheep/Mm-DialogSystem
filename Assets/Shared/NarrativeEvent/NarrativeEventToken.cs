namespace Miemie.DialogSystem
{
    /// <summary>
    /// 叙事事件标识
    /// </summary>
    public sealed class NarrativeEventToken<T>
    {
        /// <summary> 事件名 </summary>
        public string Name { get; }

        /// <summary>
        /// 创建事件标识
        /// </summary>
        internal NarrativeEventToken(string name)
        {
            Name = name;
        }
    }
}
