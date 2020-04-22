using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RP0.Unity.Unity
{
    [RequireComponent(typeof(RectTransform))]
    public class RP1_Maintenance : MonoBehaviour
    {
        #region GUI Fields

        #pragma warning disable 649
        //Submenus
        [SerializeField]
        private GameObject m_MainMenu;
        [SerializeField]
        private GameObject m_FacilitiesMenu;
        [SerializeField]
        private GameObject m_IntegrationMenu;
        [SerializeField]
        private GameObject m_AstronautMenu;

        //Maintenance Main Menu
        [SerializeField]
        private Text m_MaintenanceMainFacilities;
        [SerializeField]
        private Text m_MaintenanceMainIntegration;
        [SerializeField]
        private Text m_MaintenanceMainResearch;
        [SerializeField]
        private Text m_MaintenanceMainAstronauts;
        [SerializeField]
        private Text m_MaintenanceMainTotal;

        //Maintenance Facility Menu
        [SerializeField]
        private Text m_LaunchPadTotalCost;
        [SerializeField]
        private Text m_LaunchPadLevelsPrefab;
        [SerializeField]
        private Text m_RunwayTotalCost;
        [SerializeField]
        private Text m_VABTotalCost;
        [SerializeField]
        private Text m_SPHTotalCost;
        [SerializeField]
        private Text m_RnDTotalCost;
        [SerializeField]
        private Text m_MissionControlTotalCost;
        [SerializeField]
        private Text m_TrackingStationTotalCost;
        [SerializeField]
        private Text m_AstronautComplexTotalCost;
        [SerializeField]
        private Text m_TotalFacilityCostCost;

        //Maintenance Integration Menu
        [SerializeField]
        private Text m_IntegrationSitesPrefab;
        [SerializeField]
        private Text m_TotalIntegrationCostCost;

        //Maintenance Astronauts Menu
        [SerializeField]
        private Text m_CorpsSize;
        [SerializeField]
        private Text m_AstronautBaseCost;
        [SerializeField]
        private Text m_AstronautOperationCost;
        [SerializeField]
        private Text m_AstronautTrainingCost;
        [SerializeField]
        private Text m_AstronautTotalCostCost;
#pragma warning restore 649

        #endregion


        private enum per { DAY, MONTH, YEAR };
        private per displayPer = per.YEAR;
        private string perFormat => displayPer == per.DAY ? "N1" : "N0";

        private double perFactor
        {
            get
            {
                switch (displayPer)
                {
                    case per.DAY:
                        return 1d;
                    case per.MONTH:
                        return 30d;
                    case per.YEAR:
                        return 365d;
                    default: // can't happen
                        return 0d;
                }
            }
        }

        public struct padLevelsAndCosts
        {
            public int level;
            public int count;
            public double cost;

            public padLevelsAndCosts(int pLevel, int pCount, double pCost)
            {
                level = pLevel;
                count = pCount;
                cost = pCost;
            }
        }

#pragma warning disable 649
        [SerializeField]
        private RP1_MainPanel MainPanel;

        [SerializeField]
        private Text HeaderText;
#pragma warning restore 649

        private List<Text> LaunchPadLevels = new List<Text>();
        //We can use an Array here because the count is already known
        private Text[] IntegrationsSites;

        private void Awake()
        {

        }

        void Start()
        {
            InvokeRepeating(nameof(updateAllMaintenanceWindows), 0, 1f);
        }

        public void setWindow()
        {
            updateAllMaintenanceWindows(true);
        }

        public void updateAllMaintenanceWindows(bool force = false)
        {
            updateMaintenanceMainMenu();
            updateMaintenanceFacilityMainMenu(force);
            updateIntegrationMenu(force);
            updateAstronautMenu(force);
        }

        public void updateMaintenanceMainMenu()
        {
            if(MainPanel.MainPanelInterface == null)
                return;

            if (m_MaintenanceMainFacilities != null)
                m_MaintenanceMainFacilities.text = (MainPanel.MainPanelInterface.facilityUpkeep * perFactor).ToString(perFormat);

            if (m_MaintenanceMainIntegration != null)
                m_MaintenanceMainIntegration.text = (MainPanel.MainPanelInterface.integrationUpkeep * perFactor).ToString(perFormat);

            if (m_MaintenanceMainResearch != null)
                m_MaintenanceMainResearch.text = (MainPanel.MainPanelInterface.researchUpkeep * perFactor).ToString(perFormat);

            if (m_MaintenanceMainAstronauts != null)
                m_MaintenanceMainAstronauts.text = (MainPanel.MainPanelInterface.nautTotalUpkeep * perFactor).ToString(perFormat);

            if (m_MaintenanceMainTotal != null)
                m_MaintenanceMainTotal.text = (MainPanel.MainPanelInterface.totalUpkeep * perFactor).ToString(perFormat);
        }

        public void updateMaintenanceFacilityMainMenu(bool force)
        {
            //Updating the prefabs and then Rebuilding the Layout is probably very inefficient, but shouldn't matter too much because it's only called every second
            if(!m_FacilitiesMenu.activeSelf && !force)
                return;

            if (m_LaunchPadTotalCost != null)
                m_LaunchPadTotalCost.text = (MainPanel.MainPanelInterface.LaunchPadTotalCost * perFactor).ToString(perFormat);


            if (LaunchPadLevels != null && LaunchPadLevels.Count > 0)
            {
                foreach(var padLevel in LaunchPadLevels)
                {
                    Destroy(padLevel.gameObject);
                }

                LaunchPadLevels.Clear();
            }

            padLevelsAndCosts[] padLevelsAndCosts = MainPanel.MainPanelInterface.LaunchPadLevelsAndCosts;

            int counter = 0;
            foreach (var padLevel in padLevelsAndCosts)
            {
                if(padLevel.count == 0)
                    continue;

                Text toAdd = Instantiate(m_LaunchPadLevelsPrefab, m_FacilitiesMenu.transform);
                toAdd.gameObject.transform.SetSiblingIndex(counter + 1);
                toAdd.text = $"\tLevel {padLevel.level} × {padLevel.count}";
                toAdd.gameObject.transform.GetChild(0).GetComponent<Text>().text = (padLevel.cost * perFactor).ToString(perFormat);
                LaunchPadLevels.Add(toAdd);

                counter++;
            }

            if (m_RunwayTotalCost != null)
                m_RunwayTotalCost.text = (MainPanel.MainPanelInterface.RunwayTotalCost * perFactor).ToString(perFormat);
            if (m_VABTotalCost != null)
                m_VABTotalCost.text = (MainPanel.MainPanelInterface.VABTotalCost * perFactor).ToString(perFormat);
            if (m_SPHTotalCost != null)
                m_SPHTotalCost.text = (MainPanel.MainPanelInterface.SPHTotalCost * perFactor).ToString(perFormat);
            if (m_RnDTotalCost != null)
                m_RnDTotalCost.text = (MainPanel.MainPanelInterface.RnDTotalCost * perFactor).ToString(perFormat);
            if (m_MissionControlTotalCost != null)
                m_MissionControlTotalCost.text = (MainPanel.MainPanelInterface.MissionControlTotalCost * perFactor).ToString(perFormat);
            if (m_TrackingStationTotalCost != null)
                m_TrackingStationTotalCost.text = (MainPanel.MainPanelInterface.TrackingStationTotalCost * perFactor).ToString(perFormat);
            if (m_AstronautComplexTotalCost != null)
                m_AstronautComplexTotalCost.text = (MainPanel.MainPanelInterface.AstronautComplexTotalCost * perFactor).ToString(perFormat);
            if (m_TotalFacilityCostCost != null)
                m_TotalFacilityCostCost.text = (MainPanel.MainPanelInterface.TotalFacilityCostCost * perFactor).ToString(perFormat);

            LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
        }

        public void updateIntegrationMenu(bool force)
        {
            //Updating the prefabs and then Rebuilding the Layout is probably very inefficient, but shouldn't matter too much because it's only called every second
            if(!m_IntegrationMenu.activeSelf && !force)
                return;

            if (IntegrationsSites != null && IntegrationsSites.Length != 0)
            {
                foreach (var site in IntegrationsSites)
                {
                    Destroy(site.gameObject);
                }
            }
            
            IntegrationsSites = new Text[MainPanel.MainPanelInterface.integrationSites.Length];
            for (int i = 0; i<IntegrationsSites.Length; i++)
            {
                IntegrationsSites[i] = Instantiate(m_IntegrationSitesPrefab, m_IntegrationMenu.gameObject.transform);
                IntegrationsSites[i].gameObject.transform.SetSiblingIndex(i);
                IntegrationsSites[i].text = MainPanel.MainPanelInterface.integrationSites[i];
                IntegrationsSites[i].gameObject.transform.GetChild(0).GetComponent<Text>().text = (MainPanel.MainPanelInterface.integrationRates[i] * perFactor).ToString(perFormat);
            }

            if (m_TotalIntegrationCostCost != null)
                m_TotalIntegrationCostCost.text = (MainPanel.MainPanelInterface.integrationUpkeep * perFactor).ToString(perFormat);

            LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
        }

        public void updateAstronautMenu(bool force)
        {
            if(!m_AstronautMenu.activeSelf && !force)
                return;

            if (m_CorpsSize != null)
                m_CorpsSize.text = "Corps: " + MainPanel.MainPanelInterface.CorpsSize + " Astronauts";
            if (m_AstronautBaseCost != null)
                m_AstronautBaseCost.text = (MainPanel.MainPanelInterface.AstronautBaseCost * perFactor).ToString(perFormat);
            if (m_AstronautOperationCost != null)
                m_AstronautOperationCost.text = (MainPanel.MainPanelInterface.AstronautOperationCost * perFactor).ToString(perFormat);
            if (m_AstronautComplexTotalCost != null)
                m_AstronautTrainingCost.text = (MainPanel.MainPanelInterface.AstronautTrainingCost * perFactor).ToString(perFormat);
            if (m_AstronautTotalCostCost != null)
                m_AstronautTotalCostCost.text = (MainPanel.MainPanelInterface.nautTotalUpkeep * perFactor).ToString(perFormat);
        }

        public void showMainMenu()
        {
            if (m_MainMenu == null)
                return;

            if (HeaderText != null)
                HeaderText.text = "Maintenance costs (per ";

            m_IntegrationMenu.SetActive(false);
            m_FacilitiesMenu.SetActive(false);
            m_AstronautMenu.SetActive(false);
            m_MainMenu.SetActive(true);
        }

        #region Listeners

        public void OnPerDay(bool isOn)
        {
            if (isOn)
            {
                displayPer = per.DAY;
                updateAllMaintenanceWindows();
            }
        }

        public void OnPerMonth(bool isOn)
        {
            if (isOn)
            {
                displayPer = per.MONTH;
                updateAllMaintenanceWindows();
            }
        }

        public void OnPerYear(bool isOn)
        {
            if (isOn)
            {
                displayPer = per.YEAR;
                updateAllMaintenanceWindows();
            }
        }

        public void OnFacilitiesButton()
        {
            m_MainMenu?.SetActive(false);
            updateAllMaintenanceWindows(true);
            m_FacilitiesMenu?.SetActive(true);
            if (HeaderText != null)
                HeaderText.text = "Facility costs (per ";
            gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.transform.parent.GetComponent<RectTransform>());
        }

        public void OnIntegrationButton()
        {
            m_MainMenu?.SetActive(false);
            updateAllMaintenanceWindows(true);
            m_IntegrationMenu?.SetActive(true);
            if (HeaderText != null)
                HeaderText.text = "Integration Team costs (per ";
            gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.transform.parent.GetComponent<RectTransform>());
        }

        public void OnAstronautsButton()
        {
            m_MainMenu?.SetActive(false);
            updateAllMaintenanceWindows(true);
            m_AstronautMenu?.SetActive(true);
            if (HeaderText != null)
                HeaderText.text = "Astronaut costs (per ";
            gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.transform.parent.GetComponent<RectTransform>());
        }

        #endregion

    }
}
