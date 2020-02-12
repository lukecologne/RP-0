using UnityEngine;
using UnityEngine.UI;

namespace RP0.Unity.Unity
{
    public class RP1_ExistingToolingButton : MonoBehaviour
    {
        [SerializeField]
        private Text m_ExistingToolingButtonText;

        private RP1_MainPanel mainPanel;

        private string name;

        public void setWindow(RP1_MainPanel pMainPanel, string pName)
        {
            if(pMainPanel == null)
                return;

            mainPanel = pMainPanel;

            name = pName;
            m_ExistingToolingButtonText.text = name;
        }

        public void onExistingToolingButton()
        {
            mainPanel.onExistingToolingSelected(name);
        }
    }
}
