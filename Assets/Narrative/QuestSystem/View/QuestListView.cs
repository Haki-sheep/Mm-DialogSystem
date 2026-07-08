using MiMieMVVM;
using UnityEngine;

namespace Miemie.DialogSystem.Quest
{
    /// <summary>
    /// 任务列表 View 占位
    /// 后续可绑定 QuestViewModel 驱动 UI
    /// </summary>
    public class QuestListView : MonoBehaviour, IView
    {
        QuestViewModel boundViewModel;

        public IViewModel ViewModel => boundViewModel;

        /// <summary>
        /// 绑定 ViewModel
        /// </summary>
        public void Bind(IViewModel viewModel)
        {
            Unbind();
            boundViewModel = viewModel as QuestViewModel;
        }

        /// <summary>
        /// 解绑 ViewModel
        /// </summary>
        public void Unbind()
        {
            boundViewModel = null;
        }
    }
}
