using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RP0.Unity.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace RP0.Unity.Unity
{
    [RequireComponent(typeof(RectTransform))]
    public class RP1_Maintenance : MonoBehaviour
    {
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
        private Text m_LaunchPadLevels;
        [SerializeField]
        private Text m_LaunchPadLevelsCost;
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
        private Text mTrackingStationTotalCost;
        [SerializeField]
        private Text m_AstronautComplexTotalCost;
        [SerializeField]
        private Text m_TotalFacilityCostCost;

        //Maintenance Integration Menu
        [SerializeField]
        private Text m_IntegrationSites;
        [SerializeField]
        private Text m_IntegrationRates;
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

        [SerializeField]
        private RP1_MainPanel MainPanel;

        [SerializeField]
        private Text HeaderText;

        private void Awake()
        {

        }

        public void setWindow()
        {

        }

        public void updateAllMaintenanceWindows()
        {
            updateMaintenanceMainMenu();
            updateMaintenanceFacilityMainMenu();
            updateIntegrationMenu();
            updateAstronautMenu();
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

        public void updateMaintenanceFacilityMainMenu()
        {
            if (m_LaunchPadTotalCost != null)
                m_LaunchPadTotalCost.text = (MainPanel.MainPanelInterface.LaunchPadTotalCost * perFactor).ToString(perFormat);

            //if (m_LaunchPadLevels != null)
            //{
            //    var sb = new StringBuilder();
            //
            //    for (int i = 0; i < MainPanel.MainPanelInterface.LaunchPadLevels.Length; i++)
            //    {
            //        sb.AppendLine(MainPanel.MainPanelInterface.LaunchPadLevels[i]);
            //    }
            //
            //    m_LaunchPadLevels.text = sb.ToString();
            //}
                
                

            //if (m_LaunchPadLevelsCost != null)
            //{
            //    string toPrint = "";
            //    for (int i = 0; i < MainPanel.MainPanelInterface.LauchPadLevelsCost.Length; i++)
            //        toPrint.Concat((MainPanel.MainPanelInterface.LauchPadLevelsCost[i] * perFactor).ToString(perFormat));
            //
            //    Debug.Log("[RP-1]" + toPrint);
            //    m_LaunchPadLevelsCost.text = toPrint;
            //}
                
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
            if (mTrackingStationTotalCost != null)
                mTrackingStationTotalCost.text = (MainPanel.MainPanelInterface.TrackingStationTotalCost * perFactor).ToString(perFormat);
            if (m_AstronautComplexTotalCost != null)
                m_AstronautComplexTotalCost.text = (MainPanel.MainPanelInterface.AstronautComplexTotalCost * perFactor).ToString(perFormat);
            if (m_TotalFacilityCostCost != null)
                m_TotalFacilityCostCost.text = (MainPanel.MainPanelInterface.TotalFacilityCostCost * perFactor).ToString(perFormat);

            //LayoutRebuilder.ForceRebuildLayoutImmediate(m_LaunchPadLevels.gameObject.transform.parent
            //    .GetComponent<RectTransform>());
            //
            //LayoutRebuilder.ForceRebuildLayoutImmediate(m_LaunchPadLevelsCost.gameObject.transform.parent
            //    .GetComponent<RectTransform>());
        }

        public void updateIntegrationMenu()
        {
            //if (m_IntegrationSites != null)
            //    m_IntegrationSites.text = MainPanel.MainPanelInterface.integrationSites[0];
            //
            //if (m_IntegrationRates != null)
            //{
            //    string toPrint = "";
            //    for (int i = 0; i < MainPanel.MainPanelInterface.integrationRates.Length; i++)
            //        toPrint.Concat((MainPanel.MainPanelInterface.integrationRates[i] * perFactor).ToString(perFormat));
            //
            //    Debug.Log("[RP-1]" + toPrint);
            //    m_IntegrationRates.text = toPrint;
            //}

            if (m_TotalIntegrationCostCost != null)
                m_TotalIntegrationCostCost.text = (MainPanel.MainPanelInterface.integrationUpkeep * perFactor).ToString(perFormat);

            //LayoutRebuilder.ForceRebuildLayoutImmediate(m_IntegrationSites.gameObject.transform.parent
            //    .GetComponent<RectTransform>());
            //
            //LayoutRebuilder.ForceRebuildLayoutImmediate(m_IntegrationRates.gameObject.transform.parent
            //    .GetComponent<RectTransform>());
        }

        public void updateAstronautMenu()
        {
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
            m_FacilitiesMenu?.SetActive(true);
            if (HeaderText != null)
                HeaderText.text = "Facility costs (per ";
            gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.transform.parent.GetComponent<RectTransform>());
        }

        public void OnIntegrationButton()
        {
            m_MainMenu?.SetActive(false);
            m_IntegrationMenu?.SetActive(true);
            if (HeaderText != null)
                HeaderText.text = "Integration Team costs (per ";
            gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.transform.parent.GetComponent<RectTransform>());
        }

        public void OnAstronautsButton()
        {
            m_MainMenu?.SetActive(false);
            m_AstronautMenu?.SetActive(true);
            if (HeaderText != null)
                HeaderText.text = "Astronaut costs (per ";
            gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.transform.parent.GetComponent<RectTransform>());
        }

        #endregion

    }
}
