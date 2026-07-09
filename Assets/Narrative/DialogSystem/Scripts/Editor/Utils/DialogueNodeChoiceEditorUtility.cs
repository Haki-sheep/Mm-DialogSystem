#if UNITY_EDITOR
namespace Miemie.DialogSystem.Editor
{
    /// <summary>
    /// 选项节点选项增删
    /// </summary>
    static class DialogueNodeChoiceEditorUtility
    {
        public static void AddChoice(DialogueNodeData node)
        {
            if (node == null || !node.IsOptionNode)
                return;

            int nextIndex = (node.ChoiceList?.Count ?? 0) + 1;
            node.AddChoice(new DialogueTransLineData { labelText = $"选项{nextIndex}" });
        }

        public static bool TryRemoveLastChoice(DialogueNodeData node) =>
            node != null && node.TryRemoveLastChoice();
    }
}
#endif
