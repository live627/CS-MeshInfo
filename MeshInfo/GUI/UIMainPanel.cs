using UnityEngine;
using ColossalFramework.UI;

using System;

using UIUtils = SamsamTS.UIUtils;
using ColossalFramework;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.Globalization;

namespace MCSI.GUI
{
    public class UIMainPanel : UIPanel
    {
        private UITitleBar m_title;
        private UITabstrip _tabstrip;
        private UIDropDown m_prefabType;
        private UIDropDown m_sorting;
        private UISprite m_sortDirection;
        private UITextField m_search;

        private UIFastList m_itemList;
        private static readonly PositionData<ItemClass.Service>[] kServices = Utils.GetOrderedEnumData<ItemClass.Service>("Game").Where(s => CanShowService(s.enumValue)).ToArray();

        public override void Start()
        {
            base.Start();

            name = "MCSI";
            atlas = UIUtils.GetAtlas("Ingame");
            backgroundSprite = "UnlockingPanel2";
            isVisible = false;
            canFocus = true;
            isInteractive = true;
            width = 770;
            height = 475;
            relativePosition = new Vector3(Mathf.Floor((GetUIView().fixedWidth - width) / 2), Mathf.Floor((GetUIView().fixedHeight - height) / 2));

            SetupControls();
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.K))
            {
                isVisible = true;
                PopulateList();
            }
        }

        private void SetupControls()
        {
            float offset = 40f;

            m_title = AddUIComponent<UITitleBar>();
            m_title.iconSprite = "IconAssetBuilding";
            m_title.title = "Mayoral City Service Info " + MCSI.version;

            _tabstrip = UIUtils.CreateTabStrip(this);
            _tabstrip.tabPages = UIUtils.CreateTabContainer(this);
            _templateButton = UIUtils.CreateTabButton(this);
            for (int i = 0; i < kServices.Length; i++)
                SpawnSubEntry(_tabstrip, kServices[i]);
            _tabstrip.relativePosition = new Vector3(15f, offset);
            _tabstrip.eventSelectedIndexChanged += (c, t) => PopulateList();
            Destroy(_templateButton);
            /*
            // Sorting DropDown
            label = AddUIComponent<UILabel>();
            label.textScale = 0.8f;
            label.padding = new RectOffset(0, 0, 8, 0);
            label.relativePosition = m_prefabType.relativePosition + new Vector3(130f, 0f);
            label.text = "Sort by :";

            m_sorting = UIUtils.CreateDropDown(this);
            m_sorting.width = 125;
            m_sorting.AddItem("Name");
            m_sorting.AddItem("Triangles");
            m_sorting.AddItem("LOD Triangles");
            m_sorting.AddItem("Weight");
            m_sorting.AddItem("LOD Weight");
            m_sorting.AddItem("Texture");
            m_sorting.AddItem("LOD Texture");
            m_sorting.selectedIndex = 0;
            m_sorting.relativePosition = label.relativePosition + new Vector3(60f, 0f);

            m_sorting.eventSelectedIndexChanged += (c, t) =>
            {
                m_sorting.enabled = false;
                m_isSorted = false;
                PopulateList();
                m_sorting.enabled = true;
            };

            // Sorting direction
            m_sortDirection = AddUIComponent<UISprite>();
            m_sortDirection.atlas = UIUtils.GetAtlas("Ingame");
            m_sortDirection.spriteName = "IconUpArrow";
            m_sortDirection.relativePosition = m_sorting.relativePosition + new Vector3(130f, 0f);

            m_sortDirection.eventClick += (c, t) =>
            {
                m_sortDirection.flip = (m_sortDirection.flip == UISpriteFlip.None) ? UISpriteFlip.FlipVertical : UISpriteFlip.None;
                m_isSorted = false;
                PopulateList();
            };
            */
            // Search
            m_search = UIUtils.CreateTextField(this);
            m_search.width = 150f;
            m_search.height = 30f;
            m_search.padding = new RectOffset(6, 6, 6, 6);
            m_search.relativePosition = new Vector3(width - m_search.width - 15f, offset + 5f);

            UILabel label = AddUIComponent<UILabel>();
            label.textScale = 0.8f;
            label.padding = new RectOffset(0, 0, 8, 0);
            label.relativePosition = m_search.relativePosition - new Vector3(60f, 0f);
            label.text = "Search :";

            m_search.eventTextChanged += (c, t) => PopulateList();

            // Labels
            label = AddUIComponent<UILabel>();
            label.textScale = 0.9f;
            label.text = Locale.Get("EXTENDED_PUBLIC_TRANSPORT_UI_ITEM_1");
            label.relativePosition = new Vector3(15f, offset + 45f);

            label = AddUIComponent<UILabel>();
            label.textScale = 0.9f;
            label.text = Locale.Get("TUTORIAL_ADVISER_TITLE", "District");
            label.relativePosition = new Vector3(550f, offset + 45f);

            label = AddUIComponent<UILabel>();
            label.textScale = 0.9f;
            label.text = Locale.Get("CAMPUSPANEL_VARSITYSPORTS_UPKEEP");
            label.relativePosition = new Vector3(675f, offset + 45f);
            
            // Item List
            m_itemList = UIFastList.Create<UIPrefabItem>(this);
            m_itemList.RowHeight = 40f;
            m_itemList.CanSelect = true;
            m_itemList.BackgroundSprite = "UnlockingPanel";
            m_itemList.width = width - 10;
            m_itemList.height = height - offset - 75;
            m_itemList.relativePosition = new Vector3(5f, offset + 70f);
        }

        protected UIButton SpawnSubEntry(UITabstrip strip, PositionData<ItemClass.Service> kService)
        {
            UIButton uIButton = strip.AddTab(kService.GetLocalizedName(), _templateButton, false);
            string text = kService.GetIconSpriteName();
            uIButton.normalFgSprite = text;
            uIButton.focusedFgSprite = text + "Focused";
            uIButton.hoveredFgSprite = text + "Hovered";
            uIButton.pressedFgSprite = text + "Pressed";
            uIButton.disabledFgSprite = text + "Disabled";
            uIButton.tooltip = kService.GetLocalizedName();
            return uIButton;
        }

        private static bool CanShowService(ItemClass.Service service)
            => service == ItemClass.Service.Beautification
                || service == ItemClass.Service.Disaster
                || service == ItemClass.Service.Education
                || service == ItemClass.Service.Electricity
                || service == ItemClass.Service.FireDepartment
                || service == ItemClass.Service.Garbage
                || service == ItemClass.Service.HealthCare
                || service == ItemClass.Service.PlayerEducation && SteamHelper.IsDLCOwned(SteamHelper.DLC.CampusDLC)
                || service == ItemClass.Service.PlayerIndustry && SteamHelper.IsDLCOwned(SteamHelper.DLC.IndustryDLC)
                || service == ItemClass.Service.PoliceDepartment
                || service == ItemClass.Service.Water;

        List<XMLBuilding> buildings = new List<XMLBuilding>();
        List<ushort> buildingMap = new List<ushort>();

        private void PopulateList()
        {
            buildings.Clear();
            buildingMap.Clear();
            string filter = m_search.text.Trim().ToLower();
            BuildingManager buildingManager = Singleton<BuildingManager>.instance;
            DistrictManager districtManager = Singleton<DistrictManager>.instance;
            for (int i = 0; i < kServices.Length; i++)
                if (_tabstrip.selectedIndex == i)
                    GetServiceBuildings(buildingManager, districtManager, kServices[i].enumValue, filter);

            // !!! Temporaary. Make buildings an array.
            var prefabList = buildings.ToArray();

            //Array.Sort(prefabList, CompareByNames);

            m_itemList.RowsData.m_buffer = prefabList;
            m_itemList.RowsData.m_size = prefabList.Length;

            m_itemList.DisplayAt(0);
        }

        private int GetServiceBuildingCount(BuildingManager buildingManager)
        {
            int num = 0;
            for (int i = 0; i < kServices.Length; i++)
                num += buildingManager.GetServiceBuildings(kServices[i].enumValue).m_size;
            return num;
        }

        private void GetServiceBuildings(BuildingManager buildingManager, DistrictManager districtManager, ItemClass.Service service, string filter)
        {
            var m_size = buildingManager.GetServiceBuildings(service);
            for (ushort i = 0; i < m_size.m_size; i++)
            {
                Building building = buildingManager.m_buildings.m_buffer[m_size[i]];
                if (EnumExtensions.IsFlagSet(building.m_flags, Building.Flags.Created))
                {
                    BuildingInfo info = building.Info;
                    string name = EnumExtensions.IsFlagSet(building.m_flags, Building.Flags.CustomName)
                        ? buildingManager.GetBuildingName(m_size[i], info.m_instanceID)
                        : info.GetLocalizedTitle();

                    if (!String.IsNullOrEmpty(filter) && name.ToLower().Contains(filter.ToLower()))
                        continue;

                    InstanceID instanceID = InstanceID.Empty;
                    instanceID.Building = m_size[i];
                    byte districtID = districtManager.GetDistrict(building.m_position);
                    byte parkID = districtManager.GetPark(building.m_position);
                    buildings.Add(new XMLBuilding
                    {
                        instanceID = instanceID,
                        name = name,
                        district = parkID != 0 ? districtManager.GetParkName(parkID) : districtID == 0 
                            ? Singleton<SimulationManager>.instance.m_metaData.m_CityName
                            : districtManager.GetDistrictName(districtID),
                        position = building.m_position,
                        service = service,
                        stats = info.m_buildingAI.GetLocalizedStats(m_size[i], ref building).Replace(Environment.NewLine, "; "),
                        status = info.m_buildingAI.GetLocalizedStatus(m_size[i], ref building),
                        tooltip = info.GetLocalizedTooltip(),
                        upkeep = info.m_buildingAI.GetResourceRate(m_size[i], ref building, EconomyManager.Resource.Maintenance)
                    });
                    buildingMap.Add(m_size[i]);
                }
            }
        }
        private UIButton _templateButton;
        
        public int CompareByNames(XMLBuilding city1, XMLBuilding city2)
        {
            if (m_sortDirection.flip == UISpriteFlip.None)
                return String.Compare(city1.name, city2.name);
            else
                return String.Compare(city1.name, city2.name);
        }
    }
}
