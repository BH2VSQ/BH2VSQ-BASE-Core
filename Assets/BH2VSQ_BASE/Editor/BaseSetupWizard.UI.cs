using System;
using System.Reflection;
using BH2VSQ.Base;
using TMPro;
using UdonSharp;
using UdonSharpEditor;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.SDK3.Components;

namespace BH2VSQ.Base.Editor
{
    public partial class BaseSetupWizard
    {
        private static RectTransform Rect(Transform parent, string name, float x, float y, float width, float height)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(x, y);
            rect.sizeDelta = new Vector2(width, height);
            return rect;
        }

        private static RectTransform Panel(Transform parent, string name, float x, float y, float width, float height, Color color)
        {
            RectTransform rect = Rect(parent, name, x, y, width, height);
            Image image = rect.gameObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return rect;
        }

        private static TMP_Text Text(Transform parent, string name, string value, float x, float y, float width, float height, int size = 24, LocalizationManager localization = null, int localizationKey = -1)
        {
            RectTransform rect = Rect(parent, name, x, y, width, height);
            TextMeshProUGUI text = rect.gameObject.AddComponent<TextMeshProUGUI>();
            text.text = value; text.fontSize = size; text.color = new Color(.78f, .95f, 1f);
            text.raycastTarget = false;
            text.alignment = TextAlignmentOptions.MidlineLeft;
            text.enableWordWrapping = true;
            TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(Root + "/Fonts/BH2VSQ_UI.asset");
            if (font == null) font = TMP_Settings.defaultFontAsset;
            if (font == null) font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Packages/com.unity.textmeshpro/Package Resources/Fonts & Materials/LiberationSans SDF.asset");
            if (font != null) text.font = font;
            if (localization != null && localizationKey >= 0)
            {
                LocalizedText localized = text.gameObject.AddUdonSharpComponent<LocalizedText>();
                localized.localization = localization; localized.target = text; localized.key = localizationKey;
            }
            return text;
        }

        private static TMP_InputField Input(Transform parent, string name, float x, float y, float width, float height, string hint, LocalizationManager localization = null, int localizationKey = -1)
        {
            RectTransform rect = Panel(parent, name, x, y, width, height, new Color(.18f, .22f, .27f, .95f));
            TMP_InputField input = rect.gameObject.AddComponent<TMP_InputField>();
            rect.GetComponent<Image>().raycastTarget = true;
            input.navigation = new Navigation { mode = Navigation.Mode.None };
            RectTransform viewport = Rect(rect, "Text Area", 12, -5, width - 24, height - 10);
            viewport.gameObject.AddComponent<RectMask2D>();
            TMP_Text text = Text(viewport, "Text", "", 0, 0, width - 24, height - 10, 21);
            TMP_Text placeholder = Text(viewport, "Placeholder", hint, 0, 0, width - 24, height - 10, 21, localization, localizationKey);
            placeholder.color = new Color(.65f, .7f, .75f);
            input.textViewport = viewport;
            input.textComponent = (TextMeshProUGUI)text;
            input.placeholder = (TextMeshProUGUI)placeholder;
            return input;
        }

        private static UIButtonAction Button(Transform parent, string name, string label, int action, int value, float x, float y, float width, float height, LocalizationManager localization = null, int localizationKey = -1)
        {
            RectTransform rect = Panel(parent, name, x, y, width, height, new Color(.02f, .35f, .62f, .74f));
            Button button = rect.gameObject.AddComponent<Button>();
            rect.GetComponent<Image>().raycastTarget = true;
            button.navigation = new Navigation { mode = Navigation.Mode.None };
            ColorBlock colors = button.colors;
            colors.highlightedColor = new Color(.08f, .7f, .92f, .95f);
            colors.pressedColor = new Color(.02f, .48f, .72f, .85f);
            button.colors = colors;
            TMP_Text text = Text(rect, "Label", label, 10, -3, width - 20, height - 6, 19);
            text.alignment = TextAlignmentOptions.Center;
            if (localization != null && localizationKey >= 0)
            {
                LocalizedText localized = text.gameObject.AddUdonSharpComponent<LocalizedText>();
                localized.localization = localization; localized.target = text; localized.key = localizationKey;
            }
            UIButtonAction handler = rect.gameObject.AddUdonSharpComponent<UIButtonAction>();
            handler.action = action; handler.value = value;
            var backing = UdonSharpEditorUtility.GetBackingUdonBehaviour(handler);
            UnityEventTools.AddStringPersistentListener(button.onClick, backing.SendCustomEvent, "Click");
            return handler;
        }

        private static Canvas Canvas(Transform parent, string name, Vector3 offset, int order)
        {
            RectTransform rect = Rect(parent, name, 0, 0, 900, 650);
            rect.pivot = new Vector2(.5f, .5f);
            rect.localScale = Vector3.one * .0015f;
            rect.gameObject.layer = 0;
            Canvas canvas = rect.gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.overrideSorting = true;
            canvas.sortingOrder = order;
            rect.gameObject.AddComponent<CanvasScaler>().dynamicPixelsPerUnit = 50;
            rect.gameObject.AddComponent<GraphicRaycaster>();
            rect.gameObject.AddComponent<VRCUiShape>();
            BoxCollider collider = rect.gameObject.GetComponent<BoxCollider>();
            if (collider == null) collider = rect.gameObject.AddComponent<BoxCollider>();
            collider.size = new Vector3(900f, 650f, 1f);
            LocalCanvasFollower follower = rect.gameObject.AddUdonSharpComponent<LocalCanvasFollower>();
            follower.target = rect;
            follower.offset = offset;
            return canvas;
        }

        private static void BuildUI(Transform parent, BaseWorldSystem world, PlayerRegistry registry, PlayerDataManager data,
            PlayerAreaTracker tracker, FloorManager floors, AreaManager areas, PermissionManager permission,
            TOTPAuthManager auth, TeleportManager teleport, TeleportRequestManager requests, RadioDutyManager radio,
            AreaPopulationManager population, AdminManager admin, FloorAdminManager floorAdmin, BroadcastManager broadcast,
            LocalizationManager localization)
        {
            GameObject eventSystem = Group(parent, "EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
            Canvas mainCanvas = Canvas(parent, "MainCanvas", new Vector3(-.5f, -.05f, 1.45f), 1);
            world.menuCanvas = mainCanvas.gameObject;
            world.menuFollower = mainCanvas.GetComponent<LocalCanvasFollower>();
            world.menuCanvasGroup = mainCanvas.gameObject.AddComponent<CanvasGroup>();
            world.menuCanvasGroup.alpha = world.menuOpacity;
            RectTransform menuRoot = Panel(mainCanvas.transform, "TabMenu", 0, 0, 900, 650, new Color(.015f, .1f, .23f, .72f));
            TabMenuController menu = menuRoot.gameObject.AddUdonSharpComponent<TabMenuController>();
            menu.permission = permission; world.menu = menu;
            Text(menuRoot, "Header", "BH2VSQ BASE", 20, -10, 860, 40, 32);
            Panel(menuRoot, "HologramAccent", 20, -98, 860, 2, new Color(.1f, .78f, 1f, .8f));
            Button(menuRoot, "PersonalTab", "Personal", 1, 0, 20, -55, 155, 38, localization, 0);
            Button(menuRoot, "TeleportTab", "Teleport", 2, 0, 190, -55, 155, 38, localization, 1);
            Button(menuRoot, "PlayersTab", "Players", 3, 0, 360, -55, 155, 38, localization, 2);
            UIButtonAction adminTab = Button(menuRoot, "AdminTab", "Admin", 4, 0, 530, -55, 155, 38, localization, 3);
            menu.adminTab = adminTab.gameObject;

            RectTransform personalRoot = Panel(menuRoot, "PersonalInfo", 20, -110, 860, 520, new Color(.08f, .16f, .23f, .95f));
            PersonalInfoPanel personal = personalRoot.gameObject.AddUdonSharpComponent<PersonalInfoPanel>();
            personal.output = Text(personalRoot, "Info", "Loading player data...", 20, -20, 500, 280, 24, localization, BaseText.Loading);
            personal.data = data; personal.permission = permission; personal.tracker = tracker; personal.floors = floors; personal.areas = areas; personal.localization = localization;
            RectTransform loginRoot = Panel(personalRoot, "PermissionLogin", 20, -310, 560, 155, new Color(.12f, .2f, .28f));
            PermissionLoginPanel login = loginRoot.gameObject.AddUdonSharpComponent<PermissionLoginPanel>();
            login.loginRoot = loginRoot.gameObject; login.auth = auth; login.menu = menu;
            login.codeInput = Input(loginRoot, "TOTP Code", 15, -15, 290, 43, "6-digit TOTP", localization, BaseText.TotpHint);
            Button(loginRoot, "LoginButton", "Login", 10, 0, 320, -15, 120, 43, localization, 4);
            login.feedback = Text(loginRoot, "LoginStatus", "Visitor", 15, -75, 480, 45, 18, localization, BaseText.Visitor);
            login.localization = localization;
            Button(personalRoot, "English", "English", 70, 0, 600, -30, 100, 38, localization, BaseText.English);
            Button(personalRoot, "Chinese", "中文", 70, 1, 715, -30, 100, 38, localization, BaseText.Chinese);
            Button(personalRoot, "ToggleTeleportConfirm", "Teleport confirm", 71, 0, 600, -85, 215, 38, localization, BaseText.TeleportConfirm);
            Button(personalRoot, "ToggleNotifications", "Notifications", 72, 0, 600, -140, 215, 38, localization, BaseText.Notifications);
            menu.personal = personalRoot.gameObject;

            RectTransform teleportRoot = Panel(menuRoot, "Teleport", 20, -110, 860, 520, new Color(.08f, .16f, .23f, .95f));
            TeleportPanel teleportPanel = teleportRoot.gameObject.AddUdonSharpComponent<TeleportPanel>();
            teleportPanel.teleport = teleport; teleportPanel.data = data; teleportPanel.localization = localization;
            teleportPanel.feedback = Text(teleportRoot, "TeleportStatus", "Select a destination", 20, -425, 800, 38, 18, localization, BaseText.SelectDestination);
            Text(teleportRoot, "LocationTitle", "Locations", 20, -10, 300, 35, 24, localization, BaseText.Locations);
            teleportPanel.locationActions = new UIButtonAction[14];
            teleportPanel.locationLabels = new TMP_Text[14];
            teleportPanel.locationObjects = new GameObject[14];
            for (int i = 0; i < 14; i++)
            {
                UIButtonAction item = Button(teleportRoot, "LocationItem_" + i, "", 20, 0,
                    20 + (i / 7) * 400, -55 - (i % 7) * 51, 380, 44);
                teleportPanel.locationActions[i] = item;
                teleportPanel.locationLabels[i] = item.GetComponentInChildren<TMP_Text>();
                teleportPanel.locationObjects[i] = item.gameObject;
                item.gameObject.SetActive(false);
            }
            teleportPanel.previousButton = Button(teleportRoot, "PreviousLocations", "Previous", 26, 0, 20, -465, 150, 38, localization, BaseText.Previous).gameObject;
            teleportPanel.pageLabel = Text(teleportRoot, "LocationPage", "Page 1 / 1", 360, -465, 170, 38, 18);
            teleportPanel.nextButton = Button(teleportRoot, "NextLocations", "Next", 27, 0, 650, -465, 150, 38, localization, BaseText.Next).gameObject;
            RectTransform confirmRoot = Panel(teleportRoot, "ConfirmTeleport", 170, -130, 520, 190, new Color(.04f, .1f, .16f, 1f));
            teleportPanel.confirmRoot = confirmRoot.gameObject;
            teleportPanel.confirmText = Text(confirmRoot, "Prompt", "Teleport?", 20, -18, 480, 60, 24, localization, BaseText.TeleportQuestion);
            Button(confirmRoot, "Confirm", "Confirm", 22, 0, 50, -112, 170, 45, localization, BaseText.Confirm);
            Button(confirmRoot, "Cancel", "Cancel", 23, 0, 300, -112, 170, 45, localization, BaseText.Cancel);
            confirmRoot.gameObject.SetActive(false);
            menu.teleport = teleportRoot.gameObject;
            teleportRoot.gameObject.SetActive(false);

            RectTransform playersRoot = Panel(menuRoot, "PlayerList", 20, -110, 860, 520, new Color(.08f, .16f, .23f, .95f));
            PlayerListPanel playerList = playersRoot.gameObject.AddUdonSharpComponent<PlayerListPanel>();
            playerList.registry = registry; playerList.tracker = tracker; playerList.data = data; playerList.radio = radio;
            playerList.floors = floors; playerList.areas = areas; playerList.localization = localization;
            playerList.output = Text(playersRoot, "Count", "Online: 0", 20, -10, 500, 38, 20, localization, BaseText.Online);
            Button(playersRoot, "RefreshPlayers", "Refresh", 30, 0, 550, -10, 140, 38, localization, 9);
            RectTransform viewport = Panel(playersRoot, "Viewport", 20, -55, 490, 420, new Color(.04f, .11f, .17f));
            viewport.gameObject.AddComponent<RectMask2D>();
            ScrollRect scroll = viewport.gameObject.AddComponent<ScrollRect>();
            scroll.horizontal = false; scroll.vertical = true;
            RectTransform content = Rect(viewport, "Content", 0, 0, 470, 80 * 58);
            scroll.viewport = viewport; scroll.content = content;
            playerList.rowTexts = new TMP_Text[BaseConstants.MaxPlayers];
            playerList.rowActions = new UIButtonAction[BaseConstants.MaxPlayers];
            playerList.rowObjects = new GameObject[BaseConstants.MaxPlayers];
            for (int i = 0; i < BaseConstants.MaxPlayers; i++)
            {
                UIButtonAction row = Button(content, "PlayerItem_" + i, "", 31, 0, 2, -i * 58, 460, 54);
                playerList.rowActions[i] = row;
                playerList.rowObjects[i] = row.gameObject;
                playerList.rowTexts[i] = row.GetComponentInChildren<TMP_Text>();
                playerList.rowTexts[i].fontSize = 16;
                row.gameObject.SetActive(false);
            }
            RectTransform detailRoot = Panel(playersRoot, "PlayerDetail", 525, -55, 310, 420, new Color(.12f, .21f, .29f));
            PlayerDetailPanel detail = detailRoot.gameObject.AddUdonSharpComponent<PlayerDetailPanel>();
            detail.output = Text(detailRoot, "DetailText", "Select a player", 10, -10, 290, 150, 22, localization, BaseText.SelectPlayer);
            detail.tracker = tracker; detail.registry = registry; detail.floors = floors; detail.areas = areas; detail.localization = localization;
            playerList.detail = detail;
            Button(detailRoot, "GoToPlayer", "Go to player", 32, 0, 15, -200, 280, 42, localization, BaseText.GoToPlayer);
            Button(detailRoot, "InvitePlayer", "Invite player", 32, 1, 15, -255, 280, 42, localization, BaseText.InvitePlayer);
            menu.players = playersRoot.gameObject;
            playersRoot.gameObject.SetActive(false);

            RectTransform adminRoot = Panel(menuRoot, "AdminPanel", 20, -110, 860, 520, new Color(.08f, .16f, .23f, .95f));
            AdminPanel adminPanel = adminRoot.gameObject.AddUdonSharpComponent<AdminPanel>();
            adminPanel.admin = admin; adminPanel.floorAdmin = floorAdmin; adminPanel.floors = floors;
            adminPanel.population = population; adminPanel.radio = radio; adminPanel.areas = areas; adminPanel.localization = localization;
            adminPanel.status = Text(adminRoot, "AdminStatus", "Admin only", 20, -10, 800, 55, 20, localization, BaseText.AdminOnly);
            RectTransform floorAdminRoot = Panel(adminRoot, "FloorAdmin", 20, -75, 820, 125, new Color(.12f, .2f, .28f));
            adminPanel.floorActions = new UIButtonAction[6];
            adminPanel.floorLabels = new TMP_Text[6];
            adminPanel.floorObjects = new GameObject[6];
            adminPanel.previousFloorButton = Button(floorAdminRoot, "PreviousFloors", "Previous", 66, 0, 5, -5, 95, 40, localization, BaseText.Previous).gameObject;
            for (int i = 0; i < 6; i++)
            {
                UIButtonAction selector = Button(floorAdminRoot, "SelectFloor_" + i, "", 60, 0, 105 + i * 100, -5, 92, 40);
                adminPanel.floorActions[i] = selector;
                adminPanel.floorLabels[i] = selector.GetComponentInChildren<TMP_Text>();
                adminPanel.floorObjects[i] = selector.gameObject;
                selector.gameObject.SetActive(false);
            }
            adminPanel.nextFloorButton = Button(floorAdminRoot, "NextFloors", "Next", 67, 0, 710, -5, 100, 40, localization, BaseText.Next).gameObject;
            Button(floorAdminRoot, "Open", "Open", 61, 0, 5, -60, 130, 38, localization, BaseText.Open);
            Button(floorAdminRoot, "Reserved", "Reserved", 61, 1, 145, -60, 130, 38, localization, BaseText.Reserved);
            Button(floorAdminRoot, "Maintenance", "Maintenance", 61, 2, 285, -60, 160, 38, localization, BaseText.Maintenance);
            Button(floorAdminRoot, "ApplyFloor", "Apply state", 62, 0, 465, -60, 150, 38, localization, BaseText.ApplyState);
            Button(adminRoot, "PlayerManagementTab", "Player Management", 65, 0, 20, -207, 190, 35, localization, BaseText.PlayerManagement);
            Button(adminRoot, "PopulationTab", "Population", 63, 0, 225, -207, 150, 35, localization, BaseText.Population);
            Button(adminRoot, "BroadcastTab", "Broadcast", 64, 0, 390, -207, 150, 35, localization, BaseText.Broadcast);
            RectTransform populationRoot = Panel(adminRoot, "PopulationPanel", 20, -247, 820, 245, new Color(.12f, .2f, .28f));
            adminPanel.populationRoot = populationRoot.gameObject;
            adminPanel.populationLeft = Text(populationRoot, "PopulationLeft", "", 15, -12, 390, 180, 18);
            adminPanel.populationRight = Text(populationRoot, "PopulationRight", "", 420, -12, 390, 180, 18);
            adminPanel.previousPopulationButton = Button(populationRoot, "PreviousPopulation", "Previous", 68, 0, 15, -198, 135, 35, localization, BaseText.Previous).gameObject;
            adminPanel.nextPopulationButton = Button(populationRoot, "NextPopulation", "Next", 69, 0, 655, -198, 135, 35, localization, BaseText.Next).gameObject;
            populationRoot.gameObject.SetActive(false);
            RectTransform broadcastRoot = Panel(adminRoot, "BroadcastPanel", 20, -247, 820, 245, new Color(.12f, .2f, .28f));
            adminPanel.broadcastRoot = broadcastRoot.gameObject;
            BroadcastPanel broadcastPanel = broadcastRoot.gameObject.AddUdonSharpComponent<BroadcastPanel>();
            broadcastPanel.manager = broadcast; broadcastPanel.localization = localization;
            Text(broadcastRoot, "BroadcastTitle", "Broadcast", 15, -10, 300, 35, 24, localization, BaseText.Broadcast);
            Button(broadcastRoot, "Normal", "Normal", 51, 0, 15, -50, 130, 38, localization, BaseText.Normal);
            Button(broadcastRoot, "Important", "Important", 51, 1, 155, -50, 130, 38, localization, BaseText.Important);
            Button(broadcastRoot, "Emergency", "Emergency", 51, 2, 295, -50, 150, 38, localization, BaseText.Emergency);
            broadcastPanel.messageInput = Input(broadcastRoot, "Message", 15, -100, 620, 50, "Message (max 180 chars)", localization, BaseText.MessageHint);
            broadcastPanel.messageInput.characterLimit = 180;
            Button(broadcastRoot, "Send", "Send", 50, 0, 650, -100, 140, 50, localization, 5);
            broadcastPanel.feedback = Text(broadcastRoot, "BroadcastStatus", "", 15, -165, 700, 42, 19);
            menu.admin = adminRoot.gameObject;
            adminRoot.gameObject.SetActive(false);

            GameObject notificationLayer = Group(parent, "NotificationLayer");
            BroadcastNotificationManager notification = notificationLayer.AddUdonSharpComponent<BroadcastNotificationManager>();
            Canvas notificationCanvas = Canvas(notificationLayer.transform, "BroadcastCanvas", new Vector3(0f, .28f, 1.1f), 10);
            BoxCollider notificationCollider = notificationCanvas.GetComponent<BoxCollider>();
            notificationCollider.center = new Vector3(0f, 237.5f, 0f);
            notificationCollider.size = new Vector3(800f, 155f, 1f);
            RectTransform notificationRoot = Panel(notificationCanvas.transform, "BroadcastNotification", 50, -10, 800, 155, new Color(.26f, .12f, .08f, .97f));
            notification.root = notificationCanvas.gameObject;
            notification.title = Text(notificationRoot, "Title", "NOTICE", 15, -8, 560, 32, 25, localization, BaseText.Notice);
            notification.messageText = Text(notificationRoot, "Message", "", 15, -43, 750, 68, 22);
            notification.senderText = Text(notificationRoot, "Sender", "", 15, -116, 590, 30, 17);
            Button(notificationRoot, "Close", "Close", 42, 0, 650, -112, 130, 34, localization, 6);
            notification.audioSource = notificationLayer.AddComponent<AudioSource>();
            notification.audioSource.playOnAwake = false;
            notification.data = data; notification.localization = localization;
            notification.normalSound = AssetDatabase.LoadAssetAtPath<AudioClip>(Root + "/Audio/Broadcast_Normal.wav");
            notification.importantSound = AssetDatabase.LoadAssetAtPath<AudioClip>(Root + "/Audio/Broadcast_Important.wav");
            notification.emergencySound = AssetDatabase.LoadAssetAtPath<AudioClip>(Root + "/Audio/Broadcast_Emergency.wav");
            broadcast.notification = notification;
            RequestPanel requestPanel = notificationLayer.AddUdonSharpComponent<RequestPanel>();
            Canvas requestCanvas = Canvas(notificationLayer.transform, "RequestCanvas", new Vector3(0f, .05f, 1f), 11);
            BoxCollider requestCollider = requestCanvas.GetComponent<BoxCollider>();
            requestCollider.center = new Vector3(0f, 82.5f, 0f);
            requestCollider.size = new Vector3(800f, 125f, 1f);
            RectTransform requestRoot = Panel(requestCanvas.transform, "TeleportRequest", 50, -180, 800, 125, new Color(.07f, .24f, .3f, .98f));
            requestPanel.root = requestCanvas.gameObject;
            requestPanel.message = Text(requestRoot, "RequestText", "Teleport request", 15, -10, 570, 42, 20, localization, BaseText.TeleportRequest);
            requestPanel.countdown = Text(requestRoot, "Countdown", "15s", 680, -10, 100, 42, 20);
            requestPanel.requests = requests; requestPanel.localization = localization;
            Button(requestRoot, "Accept", "Accept", 40, 0, 490, -67, 135, 38, localization, 7);
            Button(requestRoot, "Reject", "Reject", 41, 0, 645, -67, 135, 38, localization, 8);
            requests.panel = requestPanel;
            notificationCanvas.gameObject.SetActive(false);
            requestCanvas.gameObject.SetActive(false);

            foreach (UIButtonAction action in parent.GetComponentsInChildren<UIButtonAction>(true))
            {
                action.menu = menu; action.login = login; action.teleport = teleportPanel;
                action.players = playerList; action.detail = detail; action.requests = requests;
                action.requestPanel = requestPanel; action.notification = notification;
                action.admin = adminPanel; action.broadcast = broadcastPanel; action.data = data;
            }
            mainCanvas.gameObject.SetActive(false);
        }

        private static void CreateComponentPrefabs(GameObject core)
        {
            string ui = Root + "/Prefabs/UI/";
            SavePart(core, "UI/MainCanvas/TabMenu", ui + "BH2VSQ_TabMenu.prefab");
            SavePart(core, "UI/MainCanvas/TabMenu/PersonalInfo", ui + "BH2VSQ_PersonalInfo.prefab");
            SavePart(core, "UI/MainCanvas/TabMenu/PersonalInfo/PermissionLogin", ui + "BH2VSQ_PermissionLogin.prefab");
            SavePart(core, "UI/MainCanvas/TabMenu/Teleport", ui + "BH2VSQ_Teleport.prefab");
            SavePart(core, "UI/MainCanvas/TabMenu/PlayerList", ui + "BH2VSQ_PlayerList.prefab");
            SavePart(core, "UI/MainCanvas/TabMenu/PlayerList/Viewport/Content/PlayerItem_0", ui + "BH2VSQ_PlayerListItem.prefab");
            SavePart(core, "UI/MainCanvas/TabMenu/PlayerList/PlayerDetail", ui + "BH2VSQ_PlayerDetail.prefab");
            SavePart(core, "UI/MainCanvas/TabMenu/AdminPanel", ui + "BH2VSQ_AdminPanel.prefab");
            SavePart(core, "UI/MainCanvas/TabMenu/AdminPanel/FloorAdmin", ui + "BH2VSQ_FloorAdmin.prefab");
            SavePart(core, "UI/MainCanvas/TabMenu/AdminPanel/BroadcastPanel", ui + "BH2VSQ_BroadcastPanel.prefab");
            SavePart(core, "UI/NotificationLayer", ui + "BH2VSQ_BroadcastNotificationManager.prefab");
            SavePart(core, "UI/NotificationLayer/BroadcastCanvas/BroadcastNotification", ui + "BH2VSQ_BroadcastNotification.prefab");
            SavePart(core, "UI/NotificationLayer/RequestCanvas/TeleportRequest", ui + "BH2VSQ_TeleportRequest.prefab");
            SavePart(core, "Floor/FloorManager", Root + "/Prefabs/Floor/BH2VSQ_FloorController.prefab");
            SavePart(core, "Floor/AreaManager", Root + "/Prefabs/Floor/BH2VSQ_AreaController.prefab");
            SavePart(core, "Teleport/Point_1F_Living", Root + "/Prefabs/Teleport/BH2VSQ_TeleportPoint.prefab");
            SavePart(core, "Teleport/Point_1F_Living/AreaTrigger", Root + "/Prefabs/Floor/BH2VSQ_AreaTrigger.prefab");
            PrefabUtility.SaveAsPrefabAsset(core, Root + "/Prefabs/Demo/BH2VSQ_DemoBase.prefab");
        }

        private static void SavePart(GameObject core, string path, string destination)
        {
            Transform source = core.transform.Find(path);
            if (source == null) throw new InvalidOperationException("Missing prefab part: " + path);
            GameObject clone = UnityEngine.Object.Instantiate(source.gameObject);
            clone.name = source.name;
            clone.transform.SetParent(null, false);
            clone.SetActive(true);
            foreach (UdonSharpBehaviour behaviour in clone.GetComponentsInChildren<UdonSharpBehaviour>(true))
            {
                FieldInfo[] fields = behaviour.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                foreach (FieldInfo field in fields)
                {
                    if (!typeof(UnityEngine.Object).IsAssignableFrom(field.FieldType)) continue;
                    UnityEngine.Object reference = field.GetValue(behaviour) as UnityEngine.Object;
                    Component component = reference as Component;
                    GameObject gameObject = reference as GameObject;
                    Transform transform = component != null ? component.transform : gameObject != null ? gameObject.transform : null;
                    if (transform != null && !transform.IsChildOf(clone.transform)) field.SetValue(behaviour, null);
                }
                EditorUtility.SetDirty(behaviour);
            }
            PrefabUtility.SaveAsPrefabAsset(clone, destination);
            UnityEngine.Object.DestroyImmediate(clone);
        }
    }
}
