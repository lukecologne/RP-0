using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RP0.Unity.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngineInternal;

namespace RP0.Unity.Unity
{
    [RequireComponent(typeof(RectTransform))]
    public class RP1_MainPanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region GUIElements

        [SerializeField] 
        private Text m_version = null;

#pragma warning disable 649
        //Tab Toggles
        [SerializeField]
        internal Toggle m_MaintenanceToggle;
        [SerializeField]
        private Toggle m_ToolingToggle;
        [SerializeField]
        private Toggle m_AstronautsToggle;
        [SerializeField]
        private Toggle m_CoursesToggle;
        [SerializeField]
        private Toggle m_AvionicsToggle;

        //Avionics Menu
        [SerializeField]
        private Text m_AvionicsSupportsMass;
        [SerializeField]
        private Text m_AvionicsVesselMass;
        [SerializeField]
        private Text m_AvionicsIsSufficientText;

        //Astronauts Menu
        [SerializeField]
        private GameObject m_AstronautMainMenu;
        [SerializeField]
        private Transform m_AstronautScrollViewportContent = null;
        [SerializeField]
        private GameObject m_AstronautListRowPrefab = null;

        [SerializeField]
        private GameObject m_AstronautDetailMenu;

        [SerializeField]
        private Text m_AstronautName;
        [SerializeField]
        private Text m_AstronautTypeLevel;
        [SerializeField]
        private Text m_AstronautRetiresText;
        [SerializeField]
        private Text m_AstronautStudyingText;
        [SerializeField]
        private Text m_AstronautTrainingText;
        [SerializeField]
        private Button m_AstronautLeaveButton;

        //Maintenance Menu
        [SerializeField]
        private GameObject m_MaintenanceMenu;

        //Courses Menu
        [SerializeField]
        private GameObject m_courseSelectButtonPrefab;
        [SerializeField]
        private Transform m_courseSelectScrollViewContent;
        [SerializeField]
        private GameObject m_courseStartMenu;
        [SerializeField]
        private GameObject m_courseListMenu;

        [SerializeField]
        private Transform m_courseStartScrollViewContent;
        [SerializeField]
        private Text m_courseStartTitle;
        [SerializeField]
        private Text m_CourseDescriptionAndInfo;
        [SerializeField]
        private Text m_courseDurationAndFinish;

        //Tooling Menu
        [SerializeField]
        private GameObject m_ToolingMainMenu;
        [SerializeField]
        private Text m_ToolingMainMenuHeader;
        [SerializeField]
        private GameObject m_ToolingTypesMenu;
        [SerializeField]
        private Text m_ExistingToolingTypeNameText;
        [SerializeField]
        private GameObject m_UntooledPartsSubwindow;
        [SerializeField]
        private GameObject m_ToolingListRowPrefab;
        [SerializeField]
        private Button m_ExistingToolingButtonPrefab;
        [SerializeField]
        private GameObject m_ExistingToolingRowPrefab;
        [SerializeField]
        private GameObject m_ExistingToolingSizeRowPrefab;
        [SerializeField]
        private Text m_TooledTotalVesselCost;
        [SerializeField]
        private Transform m_ToolingListScrollViewContent;
#pragma warning restore 649
        #endregion

        #region General Elements/Fields

        private Vector2 mouseStart;
        private Vector2 windowStart;
        private RectTransform rect;
        //private Canvas windowCanvas;

#pragma warning disable 649
        [SerializeField]
        private GameObject ToolingMenu;
        [SerializeField]
        private GameObject AvionicsMainMenu;
        [SerializeField]
        private GameObject CoursesMainMenu;
        [SerializeField]
        private GameObject AstronautsMenu;
#pragma warning restore 649

        private bool dragging;

        public RP1_Maintenance maintenance;

        private IRP1_MainPanel mainPanelInterface;
        public IRP1_MainPanel MainPanelInterface
        {
            get { return mainPanelInterface; }
        }
        private bool loaded = true; // TODO change for KSP

        public enum Tabs
        {
            None,
            Maintenance,
            Tooling,
            Astronauts,
            Courses,
            Avionics
        }
        
        private Tabs currentTab = Tabs.None;

        #endregion


        #region Astronauts Fields

        public List<RP1_Astronaut> astronauts = new List<RP1_Astronaut>();

        private IRP1_Astronaut selectedAstronaut;

        #endregion

        #region Courses Fields

        private List<RP1_Course> courses = new List<RP1_Course>();

        public List<RP1_Astronaut> courseStartListAstronauts = new List<RP1_Astronaut>();

        private IRP1_Course activeCourse;

        #endregion

        #region Tooling Fields

        List<RP1_ToolingListItem> untooledPartsList = new List<RP1_ToolingListItem>();

        List<GameObject> ExistingToolingRows = new List<GameObject>();

        private string selectedExistingToolingType = null;

        #endregion




        #region General Methods

        protected void Awake()
        {
            rect = GetComponent<RectTransform>();
           
            //windowCanvas = GetComponentInParent<Canvas>();
        }

        private void Start()
        {
            InvokeRepeating(nameof(updateCourseStartPanel),0,0.1f);
            InvokeRepeating(nameof(updateAstronautDetailPanel),0,0.1f);
        }

        public void Open()
        {
            if (gameObject != null)
                gameObject.SetActive(true);
        }

        public void Close()
        {
            Hide();
        }

        private void Hide()
        {
            if (gameObject != null)
                gameObject.SetActive(false);
        }

        public void setPosition(Rect r)
        {
            if (rect == null)
                return;

            rect.anchoredPosition3D = new Vector3(r.x, r.y, 0);
        }

        public IEnumerator setWindow(IRP1_MainPanel window)
        {
            if (window == null)
                yield break;

            maintenance = m_MaintenanceMenu.GetComponent<RP1_Maintenance>();

            mainPanelInterface = window;

            if (m_version != null)
                m_version.text = window.Version;

            if (m_MaintenanceToggle != null)
                m_MaintenanceToggle.isOn = false;

            if (m_ToolingToggle != null)
                m_ToolingToggle.isOn = false;

            if (m_AstronautsToggle != null)
                m_AstronautsToggle.isOn = false;

            if (m_CoursesToggle != null)
                m_CoursesToggle.isOn = false;

            if (m_AvionicsToggle != null)
                m_AvionicsToggle.isOn = false;

            if(maintenance != null)
                maintenance.setWindow();

            CreateNauts(window.getAstronauts, m_AstronautScrollViewportContent, false, astronauts);

            CreateCourseList(window.getCourseButtons);
            
            updateToolingMainMenu(window.getUntooledParts);

            updateAvionicsMenu(0f, 0f, false);

            if (window.currentScene == 1 && m_AvionicsToggle != null && m_MaintenanceToggle != null)
            {
                m_AvionicsToggle.gameObject.SetActive(false);
                m_MaintenanceToggle.gameObject.SetActive(true);
                updateMaintenanceWindow();
                Canvas.ForceUpdateCanvases();
            }
            else if (window.currentScene == 0 && m_AvionicsToggle != null && m_MaintenanceToggle != null)
            {
                m_AvionicsToggle.gameObject.SetActive(true);
                m_MaintenanceToggle.gameObject.SetActive(false);
                Canvas.ForceUpdateCanvases();
            }

            loaded = true;
        }

        #endregion

        #region Astronaut Methods

        public void CreateNauts(IList<IRP1_Astronaut> nautList, Transform parent, bool isInCourseList, List<RP1_Astronaut> toAddTo)
        {
            if (nautList == null)
                return;

            if (m_AstronautListRowPrefab == null || m_AstronautScrollViewportContent == null)
                return;

            for (int i = 0; i < nautList.Count; i++)
            {
                IRP1_Astronaut naut = nautList[i];

                if(naut == null)
                    continue;

                CreateNaut(naut, parent, isInCourseList, toAddTo);
            }
        }

        public void CreateNaut(IRP1_Astronaut module, Transform parent, bool isInCourseList, List<RP1_Astronaut>toAddTo)
        {
            GameObject mod = Instantiate(m_AstronautListRowPrefab);

            if (mod == null)
                return;

            mod.transform.SetParent(parent, false);

            RP1_Astronaut bMod = mod.GetComponent<RP1_Astronaut>();

            if (bMod == null)
                return;

            bMod.setModule(module, isInCourseList, this);

            bMod.gameObject.SetActive(true);

            toAddTo.Add(bMod);
        }

        public void updateAllNauts(List<RP1_Astronaut> toUpdate)
        {
            if(toUpdate == null)
                return;
        
            foreach (RP1_Astronaut naut in toUpdate)
            {
                naut.UpdateTextFields();
            }
        }

        public void deleteAstronaut(string nautName)
        {
            RP1_Astronaut match = astronauts.Find(s => s.name.Equals(nautName));
            match.gameObject.SetActive(false);
            Destroy(match.gameObject);
        }

        public void openAstronautDetailPanel(IRP1_Astronaut switchTo)
        {
            selectedAstronaut = switchTo;
            updateAstronautDetailPanel();
            m_AstronautMainMenu.SetActive(false);
            m_AstronautDetailMenu.SetActive(true);
        }

        public void updateAstronautDetailPanel()
        {
            if(!m_AstronautDetailMenu.activeSelf)
                return;

            m_AstronautName.text = selectedAstronaut.crewMemberName;
            m_AstronautTypeLevel.text = selectedAstronaut.type + "  Level " + selectedAstronaut.level;
            m_AstronautRetiresText.text = "Retires NET: " + selectedAstronaut.retireTime;
            if (selectedAstronaut.isInCourse)
                m_AstronautStudyingText.text = $"Studying {selectedAstronaut.courseName} until {selectedAstronaut.completeTime}";
            else
                m_AstronautStudyingText.text = "Current Course: (n/a)";
            m_AstronautTrainingText.text = selectedAstronaut.education;
            if (selectedAstronaut.isInCourse && m_AstronautLeaveButton != null)
                m_AstronautLeaveButton.interactable = true;
            else if (!selectedAstronaut.isInCourse && m_AstronautLeaveButton != null)
                m_AstronautLeaveButton.interactable = false;
        }

        #endregion

        #region Courses Methods

        public void CreateCourseList(IList<IRP1_Course> buttonList)
        {
            if (buttonList == null)
                return;

            if (m_courseSelectButtonPrefab == null || m_courseSelectScrollViewContent == null)
                return;

            for (int i = 0; i < buttonList.Count; i++)
            {
                IRP1_Course button = buttonList[i];

                if(button == null)
                    continue;

                CreateButton(button);
            }
        }

        public void CreateButton(IRP1_Course module)
        {
            GameObject mod = Instantiate(m_courseSelectButtonPrefab);

            if (mod == null)
                return;

            mod.transform.SetParent(m_courseSelectScrollViewContent, false);

            RP1_Course bMod = mod.GetComponent<RP1_Course>();

            if (bMod == null)
                return;

            bMod.setModule(module, this);

            bMod.gameObject.SetActive(true);

            courses.Add(bMod);
        }

        public void openCourseStartPanel(IRP1_Course switchTo)
        {
            activeCourse = switchTo;
            if(courseStartListAstronauts.Count == 0)
                CreateNauts(mainPanelInterface.getAstronauts, m_courseStartScrollViewContent, true, courseStartListAstronauts);
            m_courseListMenu.SetActive(false);
            m_courseStartMenu.SetActive(true);
            updateCourseStartPanel();
        }

        public void updateCourseStartPanel()
        {
            if(activeCourse == null)
                return;

            m_courseStartTitle.text = activeCourse.courseName;
            m_courseDurationAndFinish.text = "Will take " + mainPanelInterface.newActiveCourseDuration + " and finish on " +
                                             mainPanelInterface.newActiveCourseEndTime;
            m_CourseDescriptionAndInfo.text = (!String.IsNullOrEmpty(activeCourse.description)
                                                  ? activeCourse.description + "\n"
                                                  : "") +
                                              (activeCourse.isTemporary
                                                  ? "Tech for this part is still being researched"
                                                  : "") + "\n" +
                                              (activeCourse.seatMax > 0
                                                  ? $"{activeCourse.seatMax - mainPanelInterface.addedCrewCount}  remaining seat(s)."
                                                  : "") + (activeCourse.seatMin > mainPanelInterface.addedCrewCount
                                                  ? $" {activeCourse.seatMin - mainPanelInterface.addedCrewCount} more student(s) required." : "");
        }

        public void onStartNewCourse()
        {
            mainPanelInterface.startNewCourse();

            foreach (RP1_Astronaut naut in courseStartListAstronauts)
            {
                naut.isSelected = false;
                naut.UpdateTextFields();
            }

            activeCourse.prepareCourse();
        }

        #endregion

        #region Tooling Methods

        public void updateToolingMainMenu(IList<IRP1_Tooling> pUntooledPartsList)
        {
            m_TooledTotalVesselCost.text = $"Total vessel cost if all parts are tooled: {mainPanelInterface.allTooledCost:N0}";

            if (ExistingToolingRows.Count > 0)
            {
                foreach (var obj in ExistingToolingRows)
                {
                    Destroy(obj);
                }
            }

            if (mainPanelInterface.getExistingToolingList.Count > 0)
            {
                m_ToolingMainMenuHeader.gameObject.SetActive(true);

                int counter = 0;
                GameObject tempRow = Instantiate(m_ExistingToolingRowPrefab, m_ToolingMainMenu.transform);
                tempRow.transform.SetSiblingIndex(1);
                ExistingToolingRows.Add(tempRow);
                foreach (var type in mainPanelInterface.getExistingToolingList)
                {
                    if (counter % 4 == 0 && counter != 0)
                    {
                        tempRow = Instantiate(m_ExistingToolingRowPrefab, m_ToolingMainMenu.transform);
                        tempRow.transform.SetSiblingIndex((counter / 4) + 1);
                        ExistingToolingRows.Add(tempRow);
                    }

                    Button toAdd = Instantiate(m_ExistingToolingButtonPrefab, tempRow.transform);
                    toAdd.transform.SetSiblingIndex(counter % 4);
                    toAdd.GetComponent<RP1_ExistingToolingButton>().setWindow(this, type);

                    counter++;
                }
            }
            else
            {
                m_ToolingMainMenuHeader.gameObject.SetActive(false);
            }

            foreach (RP1_ToolingListItem VARIABLE in untooledPartsList.ToArray()) //.ToArray to prevent the list from being edited while iterating 
            {
                untooledPartsList.Remove(VARIABLE);
                Destroy(VARIABLE.gameObject);
            }

            foreach (IRP1_Tooling untooledPart in pUntooledPartsList.ToArray())
            {
                if(untooledPart == null)
                    continue;

                createToolingRowItem(untooledPart);
            }
        }

        public void onExistingToolingSelected(string toSet)
        {
            selectedExistingToolingType = toSet;
            updateToolingTypesMenu();

            m_ToolingMainMenu.SetActive(false);
            m_ToolingTypesMenu.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(ToolingMenu.GetComponent<RectTransform>());
        }

        public void updateToolingTypesMenu()
        {
            m_ExistingToolingTypeNameText.text = $"Toolings for type {selectedExistingToolingType}";
        }

        public void createToolingRowItem(IRP1_Tooling module)
        {
            GameObject mod = Instantiate(m_ToolingListRowPrefab);

            if (mod == null)
                return;

            mod.transform.SetParent(m_ToolingListScrollViewContent, false);

            RP1_ToolingListItem bMod = mod.GetComponent<RP1_ToolingListItem>();

            if (bMod == null)
                return;

            bMod.setModule(module);

            bMod.gameObject.SetActive(true);

            untooledPartsList.Add(bMod);
        }

        #endregion

        #region Avionics Methods

        public void updateAvionicsMenu(float maxMass, float vesselMass, bool isControlLocked)
        {
            if(!AvionicsMainMenu.activeSelf)
                return;

            if (m_AvionicsIsSufficientText != null && m_AvionicsSupportsMass != null && m_AvionicsVesselMass != null)
            {
                m_AvionicsSupportsMass.text = $"{maxMass:N3}t";
                m_AvionicsVesselMass.text = $"{vesselMass:N3}t";
                if (isControlLocked)
                    m_AvionicsIsSufficientText.text = "Insufficient avionics!";
                else
                    m_AvionicsIsSufficientText.text = "Avionics are sufficient";
            }
        }

        #endregion

        public void updateMaintenanceWindow()
        {
            maintenance.updateAllMaintenanceWindows();
        }

        #region DragHandling

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (rect == null)
                return;

            dragging = true;

            mouseStart = eventData.position;
            windowStart = rect.anchoredPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (rect == null)
                return;

            rect.anchoredPosition = windowStart + (eventData.position - mouseStart);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            dragging = false;

            if (rect == null)
                return;

            mainPanelInterface.SetWindowPosition(new Rect(rect.anchoredPosition.x, rect.anchoredPosition.y, rect.sizeDelta.x, rect.sizeDelta.y));
        }

        #endregion

        #region Listeners

        //Tab Toggles
        public void MaintenanceTabToggle(bool isOn)
        {
            if (!loaded)
                return;

            if (m_MaintenanceToggle == null || m_MaintenanceMenu == null)
                return;

            if (isOn)
            {
                m_MaintenanceMenu.SetActive(true);
                maintenance.showMainMenu();
                LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
                currentTab = Tabs.Maintenance;
            }
            else
            {
                m_MaintenanceMenu.SetActive(false);
            }
                
        }
        public void ToolingTabToggle(bool isOn)
        {
            if (!loaded)
                return;

            if (ToolingMenu == null || m_ToolingMainMenu == null)
                return;

            if (isOn)
            {
                switch (mainPanelInterface.currentScene)
                {
                    case 1:
                        m_UntooledPartsSubwindow.SetActive(false);
                        break;
                    case 0:
                        m_UntooledPartsSubwindow.SetActive(true);
                        break;
                }

                if(m_ToolingTypesMenu.activeSelf)
                    m_ToolingTypesMenu.SetActive(false);

                LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());

                if(!m_ToolingMainMenu.activeSelf)
                    m_ToolingMainMenu.SetActive(true);

                ToolingMenu.SetActive(true);

                currentTab = Tabs.Tooling;
            }
            else
            {
                ToolingMenu.SetActive(false);
                if(m_ToolingTypesMenu.activeSelf)
                    m_ToolingTypesMenu.SetActive(false);
            }
        }
        public void AstronautsTabToggle(bool isOn)
        {
            if (!loaded)
                return;

            if (m_AstronautsToggle == null || AstronautsMenu == null || m_AstronautMainMenu == null)
                return;

            if (isOn)
            {
                AstronautsMenu.SetActive(true);
                if(!m_AstronautMainMenu.activeSelf)
                    m_AstronautMainMenu.SetActive(true);
                if(m_AstronautDetailMenu.activeSelf)
                    m_AstronautDetailMenu.SetActive(false);
                currentTab = Tabs.Astronauts;
            }
            else
            {
                AstronautsMenu.SetActive(false);
                if(m_AstronautDetailMenu.activeSelf)
                    m_AstronautDetailMenu.SetActive(false);
            }
        }
        public void CoursesTabToggle(bool isOn)
        {
            if (!loaded)
                return;

            if (m_CoursesToggle == null || CoursesMainMenu == null)
                return;

            if (isOn)
            {
                //updateAllNauts(courseStartListAstronauts);
                CoursesMainMenu.SetActive(true);
                if(!m_courseListMenu.activeSelf)
                    m_courseListMenu?.SetActive(true);
                if(m_courseStartMenu.activeSelf)
                    m_courseStartMenu?.SetActive(false);
                currentTab = Tabs.Courses;
            }
            else
            {
                CoursesMainMenu.SetActive(false);
                //updateAllNauts(courseStartListAstronauts);
            }
                
        }
        public void AvionicsTabToggle(bool isOn)
        {
            if (!loaded)
                return;

            if (m_AvionicsToggle == null || AvionicsMainMenu == null)
                return;

            if (isOn)
            {
                AvionicsMainMenu.SetActive(true);
                currentTab = Tabs.Avionics;
            }
            else
                AvionicsMainMenu.SetActive(false);
        }

        public void onAstronautDetailLeaveButton()
        {
            selectedAstronaut.onLeaveButtonPressed();
        }

        public void onToolAllButton()
        {
            mainPanelInterface.toolAllParts();
        }

        #endregion
    }
}
