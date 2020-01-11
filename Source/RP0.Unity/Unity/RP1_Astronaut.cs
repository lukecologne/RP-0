using System;
using System.Collections.Generic;
using System.Text;
using RP0.Unity.Interfaces;
using UnityEngine.UI;
using UnityEngine;

namespace RP0.Unity.Unity
{
    [RequireComponent(typeof(RectTransform))]
    public  class RP1_Astronaut : MonoBehaviour
    {
        #region GUIFields

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

        private RP1_MainPanel mainPanel;

        #endregion

        private IRP1_Astronaut AstronautInterface;

        private IRP1_Course currentCourse;

        private bool isInCourseStartList;

        private bool _isSelected = false;
        public bool isSelected
        {
            get => _isSelected;
        }

        private void Awake()
        {

        }

        public void setModule(IRP1_Astronaut astronaut, bool pisInCourseStartList, RP1_MainPanel pMainPanel)
        {
            if(astronaut == null)
                return;

            AstronautInterface = astronaut;
            //currentCourse = astronaut.getCurrentCourse;

            mainPanel = pMainPanel;

            isInCourseStartList = pisInCourseStartList;

            if (isInCourseStartList && astronaut.isInCourse)
                m_AstronautNameButton.interactable = false;

            UpdateTextFields();
        }

        public void UpdateTextFields()
        {
            if (m_AstronautNameText != null && m_isAddedCheckmark != null && !isSelected)
            {
                m_AstronautNameText.text = AstronautInterface.name;
                m_isAddedCheckmark.enabled = false;
            }
            else if (m_AstronautNameText != null && m_isAddedCheckmark != null && isSelected)
            {
                m_AstronautNameText.text = AstronautInterface.name;
                m_isAddedCheckmark.enabled = true;
            }

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
                m_CourseLeaveButton.interactable = false;
            else
                m_CourseLeaveButton.interactable = true;
        }

        public void setSelected(bool selected)
        {
            if(currentCourse == null)
                return;

            _isSelected = selected;
            UpdateTextFields();
        }

        #region Listeners

        public void NameButtonListener()
        {
            if(!isInCourseStartList)
                mainPanel?.openAstronautDetailPanel(AstronautInterface);
            else if(isInCourseStartList && !isSelected)
            {
                _isSelected = true;
                m_isAddedCheckmark.enabled = true;
                AstronautInterface?.addSelfToNewCourse();
                mainPanel?.updateCourseStartPanel();
            }
            else if (isInCourseStartList && isSelected)
            {
                _isSelected = false;
                m_isAddedCheckmark.enabled = false;
                AstronautInterface?.addSelfToNewCourse();
                mainPanel?.updateCourseStartPanel();
            }
        }

        public void LeaveButtonListener()
        {
            AstronautInterface.onLeaveButtonPressed();
        }

        #endregion

        
    }
}
