using RP0.Unity.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace RP0.Unity.Unity
{
    public class RP1_ToolingListItem : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private Text m_ListPartNameText;
        [SerializeField]
        private Text m_PartToolingCost;
        [SerializeField]
        private Text m_PartUntooledCost;
        [SerializeField]
        private Text m_PartTooledCost;
#pragma warning restore 649

        private IRP1_Tooling toolingInterface;

        public void setModule(IRP1_Tooling pToolingInterface)
        {
            toolingInterface = pToolingInterface;
            updateTextPanels();
        }

        public void updateTextPanels()
        {
            if(toolingInterface == null)
                return;

            m_ListPartNameText.text = toolingInterface.partName;
            m_PartToolingCost.text = $"{toolingInterface.partToolingCost:N0}f";
            m_PartUntooledCost.text = $"{toolingInterface.partUntooledCost:N0}f";
            m_PartTooledCost.text = $"{toolingInterface.partTooledCost:N0}f";
        }
    }
}
