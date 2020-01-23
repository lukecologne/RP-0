using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RP0.Unity.Styles;
using UnityEngine;
using UnityEngine.UI;

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
        private static GameObject _toolingListRowPrefab;
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

        public static GameObject ToolingListRowPrefab
        {
            get { return _toolingListRowPrefab; }
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

                processUIComponent(o);

                if (o.name == "RP1GUITopPanel")
                    _windowPrefab = o;

                if (o.name == "AstronautListRowPrefab")
                    _astronautListRowPrefab = o;

                if (o.name == "CourseSelectButtonPrefab")
                    _courseSelectButtonPrefab = o;

                if (o.name == "ToolingScrollListItem")
                    _toolingListRowPrefab = o;
            }

            prefabsProcessed = true;
        }

        private void processUIComponent(GameObject o)
        {
            RP1_GUIStyles[] styles = o.GetComponentsInChildren<RP1_GUIStyles>(true);

            if(styles == null)
                return;

            for (int i = 0; i < styles.Length; i++)
            {
                processComponent(styles[i]);
            }
        }

        private void processComponent(RP1_GUIStyles style)
        {
            if (style == null)
                return;

            UISkinDef skin = UISkinManager.defaultSkin;

            if (skin == null)
                return;

            switch (style.ElementType)
            {
                case RP1_GUIStyles.ElementTypes.Window: 
                    style.setImage(skin.window.normal.background, Image.Type.Sliced);
                    break;

                case RP1_GUIStyles.ElementTypes.Box:
                    style.setImage(skin.box.normal.background, Image.Type.Sliced);
                    break;

                case RP1_GUIStyles.ElementTypes.Button:
                    style.setButton(skin.button.normal.background, skin.button.highlight.background, skin.button.active.background, skin.button.disabled.background);
                    break;

                case RP1_GUIStyles.ElementTypes.Toggle:
                    style.setToggle(skin.button.normal.background, skin.button.highlight.background, skin.button.active.background, skin.button.disabled.background);
                    break;
                default:
                        break;
            }
        }
    }
}
