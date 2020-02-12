using RP0.Unity.Interfaces;
using UnityEngine.UI;
using UnityEngine;

namespace RP0.Unity.Unity
{
    [RequireComponent(typeof(RectTransform))]
    public  class RP1_Astronaut : MonoBehaviour
    {
        #region GUIFields

#pragma warning disable 649
        [SerializeField]
        private Text m_AstronautType;
        [SerializeField]
        private Text m_AstronautCourseName;
        [SerializeField]
        private Text m_AstromautCompleteTime;
        [SerializeField]
        private Text m_AstronautRetireTime;
        [SerializeField]
        private Button m_AstronautNameButton;
        [SerializeField]
        private Text m_AstronautNameText;
        [SerializeField]
        private Button m_CourseLeaveButton;
        [SerializeField]
        private Text m_isAddedCheckmark;
        [SerializeField]
        private Button m_KACAddButton;
#pragma warning restore 649

        private RP1_MainPanel mainPanel;

        #endregion

        private IRP1_Astronaut AstronautInterface;

        //private IRP1_Course currentCourse;

        private bool isInCourseStartList;

        private bool _isSelected = false;
        public bool isSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                UpdateTextFields();
            }
        }

        private void Awake()
        {

        }

        void Start()
        {
            InvokeRepeating(nameof(UpdateTextFields), 0, 0.1f);
        }

        public void setModule(IRP1_Astronaut astronaut, bool pisInCourseStartList, RP1_MainPanel pMainPanel)
        {
            if(astronaut == null)
                return;

            AstronautInterface = astronaut;
            //currentCourse = astronaut.getCurrentCourse;

            mainPanel = pMainPanel;

            isInCourseStartList = pisInCourseStartList;
            if (!isInCourseStartList)
            {
                m_AstronautNameText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 160);
                m_AstronautNameText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
            }

            if (isInCourseStartList || !mainPanel.MainPanelInterface.KACAPIReady)
            {
                m_KACAddButton.gameObject.SetActive(false);
                m_CourseLeaveButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 25);
                m_CourseLeaveButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-32f, 0f);
            }

            UpdateTextFields();
        }

        public void UpdateTextFields()
        {
            if (m_AstronautNameText != null && m_isAddedCheckmark != null && !isSelected)
            {
                m_AstronautNameText.text = AstronautInterface.crewMemberName;
                m_isAddedCheckmark.enabled = false;
            }
            else if (m_AstronautNameText != null && m_isAddedCheckmark != null && isSelected)
            {
                m_AstronautNameText.text = AstronautInterface.crewMemberName;
                m_isAddedCheckmark.enabled = true;
            }

            if (!isInCourseStartList || (!AstronautInterface.isInCourse && (AstronautInterface.meetsCourseReqs || isSelected)))
                m_AstronautNameButton.interactable = true;
            else
                m_AstronautNameButton.interactable = false;

            if (m_AstronautType != null)
                m_AstronautType.text = AstronautInterface.type.Substring(0,1) + "\n" + AstronautInterface.level;

            if(m_AstronautCourseName != null)
                m_AstronautCourseName.text = AstronautInterface.courseName;

            if(m_AstromautCompleteTime != null)
                m_AstromautCompleteTime.text = AstronautInterface.completeTime;

            if(m_AstronautRetireTime != null)
                m_AstronautRetireTime.text = AstronautInterface.retireTime;

            if(m_AstronautRetireTime != null)
                m_AstronautRetireTime.text = AstronautInterface.retireTime;

            if (AstronautInterface.isInCourse == false)
            {
                m_CourseLeaveButton.interactable = false;
                m_KACAddButton.interactable = false;
            }
            else
            {
                m_CourseLeaveButton.interactable = true;
                m_KACAddButton.interactable = true;
            }
        }

        #region Listeners

        public void NameButtonListener()
        {
            if (!isInCourseStartList)
            {

                mainPanel?.openAstronautDetailPanel(AstronautInterface);
            }
            else if(isInCourseStartList && !isSelected)
            {
                isSelected = true;
                m_isAddedCheckmark.enabled = true;
                AstronautInterface?.addSelfToNewCourse();
                mainPanel?.updateCourseStartPanel();
            }
            else if (isInCourseStartList && isSelected)
            {
                isSelected = false;
                m_isAddedCheckmark.enabled = false;
                AstronautInterface?.removeSelfFromNewCourse();
                mainPanel?.updateCourseStartPanel();
            }
        }

        public void LeaveButtonListener()
        {
            AstronautInterface.onLeaveButtonPressed();
        }

        public void KACButtonListener()
        {
            AstronautInterface.onKACButtonPressed();
        }

        #endregion

        
    }
}
