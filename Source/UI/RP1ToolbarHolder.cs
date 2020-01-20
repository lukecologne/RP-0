using System;
using System.Collections;
using UnityEngine;
using KSP.UI.Screens;
using RP0.UI;

namespace RP0.UI
{
    [KSPAddon(KSPAddon.Startup.FlightEditorAndKSC, false)]
    class RP1ToolbarHolder : MonoBehaviour
    {
        // GUI
        private bool guiEnabled = false;
        private ApplicationLauncherButton button;
        //private TopWindow tw;

        public ApplicationLauncherButton Button
        {
            get
            {
                return button;
            }
        }

        private static RP1ToolbarHolder instance;
		
        public static RP1ToolbarHolder Instance
        {
            get { return instance; }
        }

        protected void Awake()
        {
            instance = this;

            /*try {
                GameEvents.onGUIApplicationLauncherReady.Add(this.OnGuiAppLauncherReady);
            } catch (Exception ex) {
                Debug.LogError("RP0 failed to register RP1ToolbarHolder.OnGuiAppLauncherReady");
                Debug.LogException(ex);
            }*/
        }

        protected void Start()
        {
            //tw = new TopWindow();
            StartCoroutine(addButton());
        }

        public void OnDestroy()
        {
            GameEvents.onGUIApplicationLauncherUnreadifying.Remove(removeButton);

            removeButton(HighLogic.LoadedScene);
        }

        private IEnumerator addButton()
        {
            while (!ApplicationLauncher.Ready)
                yield return null;

            button = ApplicationLauncher.Instance.AddModApplication(ShowWindow, HideWindow, null, null, null, null,
                ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH, RP1Loader.toolbarIcon);

            GameEvents.onGameSceneLoadRequested.Add(this.OnSceneChange);

            GameEvents.onGUIApplicationLauncherUnreadifying.Add(removeButton);
        }

        private void removeButton(GameScenes scene)
        {
            if (button != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(button);
                button = null;
            }
        }

        private void ShowWindow()
        {
            if (TopWindow._Instance == null)
                return;

            TopWindow._Instance.Open();

            guiEnabled = true;
        }

        private void HideWindow()
        {
            if (TopWindow._Instance == null)
                return;

            TopWindow._Instance.Close();

            guiEnabled = false;
        }

        private void OnSceneChange(GameScenes s)
        {
            if (s == GameScenes.FLIGHT)
                HideWindow();
        }

        /*private void OnGuiAppLauncherReady()
        {
            try {
                button = ApplicationLauncher.Instance.AddModApplication(
                    ShowWindow,
                    HideWindow,
                    null,
                    null,
                    null,
                    null,
                    ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH,
                    RP1Loader.toolbarIcon);
                GameEvents.onGameSceneLoadRequested.Add(this.OnSceneChange);
            } catch (Exception ex) {
                Debug.LogError("RP0 failed to register RP1ToolbarHolder");
                Debug.LogException(ex);
            }
        }*/

        
    }
}

