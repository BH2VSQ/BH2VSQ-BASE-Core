using System;
using System.IO;
using BH2VSQ.Base;
using TMPro;
using UdonSharp;
using UdonSharpEditor;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VRC.Udon;

namespace BH2VSQ.Base.Editor
{
    public partial class BaseSetupWizard : EditorWindow
    {
        private const string Root = "Assets/BH2VSQ_BASE";
        private const string CorePath = Root + "/Prefabs/Core/BH2VSQ_BASE_Core.prefab";
        private const string ConfigPath = Root + "/Data/Default/DefaultBaseConfig.asset";
        private Vector2 scroll;

        [MenuItem("BH2VSQ BASE/Setup Wizard")]
        public static void Open() { GetWindow<BaseSetupWizard>("BH2VSQ BASE Setup"); }

        private void OnGUI()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            GUILayout.Label("BH2VSQ BASE Core", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Run Build Package after scripts compile. Do not place real TOTP secrets in source control. TOTP in Udon is convenience gating only.", MessageType.Warning);
            if (GUILayout.Button("1. Create Default Data")) CreateData();
            if (GUILayout.Button("2. Build Prefabs and Demo Scene")) BuildAll();
            if (GUILayout.Button("3. Validate Setup")) BaseValidator.ValidateSelection(true);
            if (GUILayout.Button("4. Export Unitypackage")) Export();
            EditorGUILayout.EndScrollView();
        }

        [MenuItem("BH2VSQ BASE/Build Package")]
        public static void BuildAll()
        {
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                throw new OperationCanceledException("Scene save was canceled.");
            CreateFolders();
            CreateProgramAssets();
            CreateData();
            CreateFontAsset();
            GameObject root = BuildCore();
            PrefabUtility.SaveAsPrefabAsset(root, CorePath);
            CreateComponentPrefabs(root);
            UnityEngine.Object.DestroyImmediate(root);
            CreateDemoScene();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("BH2VSQ BASE: generated core, UI parts, points, data and demo scene.");
        }

        private static void CreateFolders()
        {
            string[] paths = { "Data", "Data/Default", "Data/Localization", "Fonts", "Prefabs", "Prefabs/Core", "Prefabs/UI", "Prefabs/Floor", "Prefabs/Teleport", "Prefabs/Demo", "Scenes" };
            foreach (string path in paths)
            {
                string current = Root;
                string[] segments = path.Split('/');
                foreach (string segment in segments)
                {
                    string next = current + "/" + segment;
                    if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(current, segment);
                    current = next;
                }
            }
        }

        private static void CreateProgramAssets()
        {
            string[] guids = AssetDatabase.FindAssets("t:MonoScript", new[] { Root + "/Scripts" });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                Type type = script != null ? script.GetClass() : null;
                if (type == null || !typeof(UdonSharpBehaviour).IsAssignableFrom(type)) continue;
                string assetPath = Path.ChangeExtension(path, ".asset");
                if (AssetDatabase.LoadAssetAtPath<UdonSharpProgramAsset>(assetPath) != null) continue;
                UdonSharpProgramAsset asset = ScriptableObject.CreateInstance<UdonSharpProgramAsset>();
                asset.sourceCsScript = script;
                AssetDatabase.CreateAsset(asset, assetPath);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            UdonSharpProgramAsset.CompileAllCsPrograms(true);
            if (UdonSharpProgramAsset.AnyUdonSharpScriptHasError())
                throw new InvalidOperationException("UdonSharp compilation failed. Check Unity Console.");
        }

        [MenuItem("BH2VSQ BASE/Create Default Data")]
        public static void CreateData()
        {
            CreateFolders();
            FloorDatabase floors = AssetDatabase.LoadAssetAtPath<FloorDatabase>(Root + "/Data/Default/DefaultFloorDatabase.asset");
            if (floors == null) { floors = ScriptableObject.CreateInstance<FloorDatabase>(); AssetDatabase.CreateAsset(floors, Root + "/Data/Default/DefaultFloorDatabase.asset"); }
            AreaDatabase areas = AssetDatabase.LoadAssetAtPath<AreaDatabase>(Root + "/Data/Default/DefaultAreaDatabase.asset");
            if (areas == null) { areas = ScriptableObject.CreateInstance<AreaDatabase>(); AssetDatabase.CreateAsset(areas, Root + "/Data/Default/DefaultAreaDatabase.asset"); }
            BaseConfig config = AssetDatabase.LoadAssetAtPath<BaseConfig>(ConfigPath);
            if (config == null) { config = ScriptableObject.CreateInstance<BaseConfig>(); AssetDatabase.CreateAsset(config, ConfigPath); }
            config.floors = floors;
            config.areas = areas;
            if (AssetDatabase.LoadAssetAtPath<LocalizationDatabase>(Root + "/Data/Localization/DefaultLocalization.asset") == null)
                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<LocalizationDatabase>(), Root + "/Data/Localization/DefaultLocalization.asset");
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
        }

        private static void CreateFontAsset()
        {
            const string path = Root + "/Fonts/BH2VSQ_UI.asset";
            if (AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path) != null) return;
            Font source = AssetDatabase.LoadAssetAtPath<Font>(Root + "/Fonts/NotoSansSC-Regular.ttf");
            if (source == null) throw new InvalidOperationException("NotoSansSC-Regular.ttf is missing.");
            TMP_FontAsset font = TMP_FontAsset.CreateFontAsset(source);
            font.atlasPopulationMode = AtlasPopulationMode.Dynamic;
            AssetDatabase.CreateAsset(font, path);
            foreach (Texture2D atlas in font.atlasTextures)
                if (atlas != null && !EditorUtility.IsPersistent(atlas)) AssetDatabase.AddObjectToAsset(atlas, font);
            if (font.material != null && !EditorUtility.IsPersistent(font.material))
            {
                font.material.mainTexture = font.atlasTexture;
                AssetDatabase.AddObjectToAsset(font.material, font);
            }
            EditorUtility.SetDirty(font);
            AssetDatabase.SaveAssets();
        }

        private static T Manager<T>(Transform parent, string name) where T : UdonSharpBehaviour
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            return go.AddUdonSharpComponent<T>();
        }

        private static GameObject Group(Transform parent, string name)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            return go;
        }

        private static GameObject BuildCore()
        {
            BaseConfig config = AssetDatabase.LoadAssetAtPath<BaseConfig>(ConfigPath);
            if (config == null || config.floors == null || config.areas == null)
                throw new InvalidOperationException("Default BaseConfig is missing. Run Create Default Data.");
            if (config.floors.names == null || config.floors.names.Length != 10 || config.floors.requiredRanks == null || config.floors.requiredRanks.Length != 10 || config.floors.xpMultipliers == null || config.floors.xpMultipliers.Length != 10)
                throw new InvalidOperationException("Floor database needs 10 aligned entries.");
            AreaDatabase areaData = config.areas;
            if (areaData.ids == null || areaData.names == null || areaData.floorIds == null || areaData.requiredRanks == null || areaData.xpMultipliers == null || areaData.ids.Length != areaData.names.Length || areaData.ids.Length != areaData.floorIds.Length || areaData.ids.Length != areaData.requiredRanks.Length || areaData.ids.Length != areaData.xpMultipliers.Length)
                throw new InvalidOperationException("Area database arrays must have matching lengths.");
            GameObject root = new GameObject("BH2VSQ_BASE_Core");
            BaseWorldSystem world = root.AddUdonSharpComponent<BaseWorldSystem>();
            GameObject coreGroup = Group(root.transform, "Core");
            GameObject authGroup = Group(root.transform, "Authentication");
            GameObject floorGroup = Group(root.transform, "Floor");
            GameObject teleportGroup = Group(root.transform, "Teleport");
            GameObject adminGroup = Group(root.transform, "Admin");
            GameObject uiGroup = Group(root.transform, "UI");

            PlayerRegistry registry = Manager<PlayerRegistry>(coreGroup.transform, "PlayerRegistry");
            PlayerDataManager data = Manager<PlayerDataManager>(coreGroup.transform, "PlayerDataManager");
            PlayerAreaTracker tracker = Manager<PlayerAreaTracker>(coreGroup.transform, "PlayerAreaTracker");
            PlayerLevelSystem level = Manager<PlayerLevelSystem>(coreGroup.transform, "PlayerLevelSystem");
            LocalizationManager localization = Manager<LocalizationManager>(coreGroup.transform, "LocalizationManager");
            AuthenticationSession session = Manager<AuthenticationSession>(authGroup.transform, "AuthenticationSession");
            PermissionManager permission = Manager<PermissionManager>(authGroup.transform, "PermissionManager");
            TOTPAuthManager auth = Manager<TOTPAuthManager>(authGroup.transform, "TOTPAuthManager");
            FloorManager floors = Manager<FloorManager>(floorGroup.transform, "FloorManager");
            AreaManager areas = Manager<AreaManager>(floorGroup.transform, "AreaManager");
            AccessManager access = Manager<AccessManager>(floorGroup.transform, "AccessManager");
            AreaPopulationManager population = Manager<AreaPopulationManager>(floorGroup.transform, "AreaPopulationManager");
            RadioDutyManager radio = Manager<RadioDutyManager>(floorGroup.transform, "RadioDutyManager");
            TeleportManager teleport = Manager<TeleportManager>(teleportGroup.transform, "TeleportManager");
            TeleportRequestManager requests = Manager<TeleportRequestManager>(teleportGroup.transform, "TeleportRequestManager");
            AdminManager admin = Manager<AdminManager>(adminGroup.transform, "AdminManager");
            FloorAdminManager floorAdmin = Manager<FloorAdminManager>(adminGroup.transform, "FloorAdminManager");
            BroadcastManager broadcast = Manager<BroadcastManager>(adminGroup.transform, "BroadcastManager");

            floors.floorNames = (string[])config.floors.names.Clone();
            floors.requiredRanks = (int[])config.floors.requiredRanks.Clone();
            floors.xpMultipliers = (float[])config.floors.xpMultipliers.Clone();
            areas.ids = (int[])config.areas.ids.Clone();
            areas.names = (string[])config.areas.names.Clone();
            areas.floorIds = (int[])config.areas.floorIds.Clone();
            areas.requiredRanks = (int[])config.areas.requiredRanks.Clone();
            areas.xpMultipliers = (float[])config.areas.xpMultipliers.Clone();
            tracker.areas = areas;
            level.data = data; level.tracker = tracker; level.floors = floors; level.areas = areas;
            localization.data = data;
            LocalizationDatabase translations = AssetDatabase.LoadAssetAtPath<LocalizationDatabase>(Root + "/Data/Localization/DefaultLocalization.asset");
            localization.english = (string[])translations.english.Clone();
            localization.chinese = (string[])translations.chinese.Clone();
            session.registry = registry; permission.session = session; permission.registry = registry; auth.session = session;
            access.floors = floors; access.areas = areas; access.permission = permission;
            population.tracker = tracker; radio.population = population;
            teleport.access = access;
            requests.teleport = teleport;
            requests.timeoutSeconds = config.requestTimeoutSeconds;
            admin.permission = permission; floorAdmin.admin = admin; floorAdmin.floors = floors;
            broadcast.admin = admin; broadcast.normalSeconds = config.broadcastNormalSeconds;
            broadcast.importantSeconds = config.broadcastImportantSeconds;
            world.registry = registry; world.playerData = data; world.session = session;
            world.floors = floors; world.access = access; world.teleport = teleport;

            TeleportPoint[] points = new TeleportPoint[areas.ids.Length];
            for (int i = 0; i < points.Length; i++)
            {
                TeleportPoint point = Manager<TeleportPoint>(teleportGroup.transform, "Point_" + areas.names[i].Replace(' ', '_'));
                point.pointId = i; point.floorId = areas.floorIds[i]; point.areaId = areas.ids[i]; point.pointName = areas.names[i];
                point.transform.localPosition = new Vector3(i * 3f, 0f, 0f);
                point.destination = point.transform;
                points[i] = point;
                GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                visual.name = "Visual";
                visual.transform.SetParent(point.transform, false);
                visual.transform.localPosition = new Vector3(0f, .1f, 0f);
                visual.transform.localScale = new Vector3(.35f, .1f, .35f);
                UnityEngine.Object.DestroyImmediate(visual.GetComponent<Collider>());
                GameObject gizmo = Group(point.transform, "Gizmo");
                GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                marker.name = "PointMarker";
                marker.transform.SetParent(gizmo.transform, false);
                marker.transform.localPosition = new Vector3(0f, .35f, 0f);
                marker.transform.localScale = Vector3.one * .16f;
                UnityEngine.Object.DestroyImmediate(marker.GetComponent<Collider>());
                GameObject trigger = Group(point.transform, "AreaTrigger");
                BoxCollider collider = trigger.AddComponent<BoxCollider>();
                collider.isTrigger = true; collider.size = new Vector3(2f, 2.5f, 2f);
                AreaTrigger areaTrigger = trigger.AddUdonSharpComponent<AreaTrigger>();
                areaTrigger.tracker = tracker; areaTrigger.access = access; areaTrigger.teleport = teleport; areaTrigger.areaId = areas.ids[i];
            }
            teleport.points = points;

            BuildUI(uiGroup.transform, world, registry, data, tracker, floors, areas, permission, auth, teleport, requests, radio, population, admin, floorAdmin, broadcast, localization);
            return root;
        }

        private static void CreateDemoScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            GameObject core = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(CorePath));
            core.transform.position = Vector3.zero;
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Demo Ground";
            ground.transform.localScale = new Vector3(15f, 1f, 5f);
            GameObject vrcWorld = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.vrchat.worlds/Samples/UdonExampleScene/Prefabs/VRCWorld.prefab");
            if (vrcWorld != null) PrefabUtility.InstantiatePrefab(vrcWorld);
            EventSystem eventSystem = UnityEngine.Object.FindObjectOfType<EventSystem>();
            if (eventSystem == null) { GameObject e = new GameObject("EventSystem"); e.AddComponent<EventSystem>(); e.AddComponent<StandaloneInputModule>(); }
            EditorSceneManager.SaveScene(scene, Root + "/Scenes/BH2VSQ_BASE_Demo.unity");
        }

        [MenuItem("BH2VSQ BASE/Export Unitypackage")]
        public static void Export()
        {
            string directory = Path.Combine(Directory.GetCurrentDirectory(), "Releases");
            Directory.CreateDirectory(directory);
            AssetDatabase.ExportPackage(Root, Path.Combine(directory, "BH2VSQ_BASE_Core.unitypackage"), ExportPackageOptions.Recurse);
            Debug.Log("BH2VSQ BASE package exported to " + directory);
        }

        // UI and component prefab builders are defined in the other partial file.
    }
}
