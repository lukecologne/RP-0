using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RP0.UI
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    class RP1Loader : MonoBehaviour
    {
        private const string prefabAssetName = "/rp1gui.ksp";
        private const string toolbarIconPath = "RP-0/Resources/maintecost";

        private static AssetBundle prefabs;
        private static GameObject _windowPrefab;
        private static GameObject _astronautListRowPrefab;
        private static GameObject _courseSelectButtonPrefab;
        private static GameObject[] loadedPrefabs;

        private static string path;

        private bool loaded;
        private bool prefabsLoaded;
        private bool prefabsProcessed;

        private static Texture2D _toolbarIcon;
        public static Texture2D toolbarIcon
        {
            get { return _toolbarIcon; }
        }

        public static GameObject WindowPrefab
        {
            get { return _windowPrefab; }
        }

        public static GameObject AstronautListRowPrefab
        {
            get { return _astronautListRowPrefab; }
        }

        private void Awake()
        {
            if (loaded)
            {
                Destroy(gameObject);
                return;
            }

            path = KSPUtil.ApplicationRootPath + "GameData/RP-0/Resources";

            if(!prefabsLoaded)
                LoadPrefabs();

            if (prefabsLoaded)
            {
                loaded = true;
                Debug.Log("[RP-1] UI loaded!");
            }

            if (toolbarIcon == null)
                _toolbarIcon = GameDatabase.Instance.GetTexture(toolbarIconPath, false);
        }

        private void LoadPrefabs()
        {
            prefabs = AssetBundle.LoadFromFile(path + prefabAssetName);

            if (prefabs != null)
                loadedPrefabs = prefabs.LoadAllAssets<GameObject>();

            if (loadedPrefabs == null)
            {
                Debug.Log("[RP-1] Error in prefab loading!");
                return;
            }

            if(!prefabsProcessed)
                processPrefabs();

            Debug.Log("[RP-1] Prefab loading complete!");

            prefabsLoaded = true;
        }

        private void processPrefabs()
        {
            for (int i = loadedPrefabs.Length - 1; i >= 0; i--)
            {
                GameObject o = loadedPrefabs[i];

                if (o == null)
                    continue;

                if (o.name == "RP1GUITopPanel")
                    _windowPrefab = o;

                if (o.name == "AstronautListRowPrefab")
                    _astronautListRowPrefab = o;

                if (o.name == "CourseSelectButtonPrefab")
                    _courseSelectButtonPrefab = o;
            }

            prefabsProcessed = true;
        }
    }
}
