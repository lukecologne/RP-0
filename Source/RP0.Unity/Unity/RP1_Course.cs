﻿using System;
using System.Collections.Generic;
using System.Text;
using RP0.Unity.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace RP0.Unity.Unity
{
    [RequireComponent(typeof(RectTransform))]
    class RP1_Course : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private Button m_CourseSelectButton;
        [SerializeField]
        private Text m_CourseSelectButtonText;
#pragma warning restore 649


        private RP1_MainPanel mainPanel;

        private IRP1_Course _courseInterface;
        public IRP1_Course courseInterface
        {
            get => _courseInterface;
        }

        public void setModule(IRP1_Course courseInterface, RP1_MainPanel pMainPanel)
        {
            if(courseInterface == null)
                return;

            _courseInterface = courseInterface;

            mainPanel = pMainPanel;

            if(m_CourseSelectButtonText != null) 
                m_CourseSelectButtonText.text = _courseInterface.courseName;

            //students = new List<IRP1_Astronaut>(courseInterface.getStudents);
        }

        public void onCourseSelectButtonPressed()
        {
            _courseInterface.prepareCourse();
            mainPanel.updateAllNauts(mainPanel.courseStartListAstronauts);
            mainPanel?.openCourseStartPanel(_courseInterface);
        }

    }
}