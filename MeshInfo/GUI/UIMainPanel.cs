using UnityEngine;
using ColossalFramework.UI;

using System;
using MCSI;
using UIUtils = SamsamTS.UIUtils;
using ColossalFramework;
using System.Collections.Generic;
using System.Linq;

namespace MCSI.GUI
{
    public class UIMainPanel : UIPanel
    {
        private UITitleBar m_title;
        private UIDropDown m_prefabType;
        private UIDropDown m_sorting;
        private UISprite m_sortDirection;
        private UITextField m_search;

        private UIFastList m_itemList;

        private bool m_showDefault = false;

        private bool m_isSorted = false;
        private const int m_maxIterations = 10;

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

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.K))
            {
                isVisible = true;
                InitializePreafabLists();
            }
        }

        private void SetupControls()
        {
            float offset = 40f;

            // Title Bar
            m_title = AddUIComponent<UITitleBar>();
            m_title.iconSprite = "IconAssetBuilding";
            m_title.title = "Mayoral City Service Info " + MCSI.version;

            // Type DropDown
            UILabel label = AddUIComponent<UILabel>();
            label.textScale = 0.8f;
            label.padding = new RectOffset(0, 0, 8, 0);
            label.relativePosition = new Vector3(15f, offset + 5f);
            label.text = "Type :";

            m_prefabType = UIUtils.CreateDropDown(this);
            m_prefabType.width = 110;
            m_prefabType.AddItem("All");
            foreach (var allowedService in allowedServices)
                m_prefabType.AddItem(allowedService.ToString());
            m_prefabType.selectedIndex = 0;
            m_prefabType.relativePosition = label.relativePosition + new Vector3(60f, 0f);

            m_prefabType.eventSelectedIndexChanged += (c, t) =>
            {
                m_prefabType.enabled = false;
                PopulateList();
                m_prefabType.enabled = true;
            };
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

            label = AddUIComponent<UILabel>();
            label.textScale = 0.8f;
            label.padding = new RectOffset(0, 0, 8, 0);
            label.relativePosition = m_search.relativePosition - new Vector3(60f, 0f);
            label.text = "Search :";


            m_search.eventTextChanged += (c, t) => PopulateList();
            /*
            // Labels
            label = AddUIComponent<UILabel>();
            label.textScale = 0.9f;
            label.text = "Name";
            label.relativePosition = new Vector3(15f, offset + 50f);

            label = AddUIComponent<UILabel>();
            label.textScale = 0.9f;
            label.text = "Texture";
            label.relativePosition = new Vector3(width - 135f, offset + 50f);

            UILabel label2 = AddUIComponent<UILabel>();
            label2.textScale = 0.9f;
            label2.text = "Weight";
            label2.relativePosition = label.relativePosition - new Vector3(125f, 0f);

            label = AddUIComponent<UILabel>();
            label.textScale = 0.9f;
            label.text = "Triangles";
            label.relativePosition = label2.relativePosition - new Vector3(115f, 0f);
            */
            // Item List
            m_itemList = UIFastList.Create<UIPrefabItem>(this);
            m_itemList.RowHeight = 40f;
            m_itemList.CanSelect = true;
            m_itemList.BackgroundSprite = "UnlockingPanel";
            m_itemList.width = width - 10;
            m_itemList.height = height - offset - 75;
            m_itemList.relativePosition = new Vector3(5f, offset + 70f);
        }

        private ItemClass.Service[] allowedServices = new[]{
            ItemClass.Service.Beautification
                , ItemClass.Service.Disaster
                , ItemClass.Service.Electricity
                , ItemClass.Service.FireDepartment
                , ItemClass.Service.Garbage
                , ItemClass.Service.HealthCare
                , ItemClass.Service.PlayerEducation
                , ItemClass.Service.PlayerIndustry
                , ItemClass.Service.PoliceDepartment
                , ItemClass.Service.Water };

        List<XMLBuilding> buildings = new List<XMLBuilding>();
        List<ushort> buildingMap = new List<ushort>();

        private void InitializePreafabLists()
        {
            buildings.Clear();
            buildingMap.Clear();
            BuildingManager buildingManager = Singleton<BuildingManager>.instance;
            foreach (var allowedService in allowedServices)
                GetServiceBuildings(buildingManager, allowedService);
            PopulateList();
        }

        private void GetServiceBuildings(BuildingManager buildingManager, ItemClass.Service service)
        {
            var m_size = buildingManager.GetServiceBuildings(service);
            for (ushort i = 0; i < m_size.m_size; i++)
            {
                Building building = buildingManager.m_buildings.m_buffer[m_size[i]];
                if (EnumExtensions.IsFlagSet(building.m_flags, Building.Flags.Created))
                {
                    InstanceID instanceID = InstanceID.Empty;
                    instanceID.Building = m_size[i];
                    BuildingInfo info = building.Info;
                    buildings.Add(new XMLBuilding
                    {
                        instanceID = instanceID,
                        name = EnumExtensions.IsFlagSet(building.m_flags, Building.Flags.CustomName)
                            ? buildingManager.GetBuildingName(m_size[i], info.m_instanceID)
                            : info.GetLocalizedTitle(),
                        position = building.m_position,
                        service = service,
                        stats = info.m_buildingAI.GetLocalizedStats(m_size[i], ref building).Replace(Environment.NewLine, "; "),
                        tooltip = info.GetLocalizedTooltip(),
                        upkeep = info.m_buildingAI.GetResourceRate(m_size[i], ref building, EconomyManager.Resource.Maintenance)
                    });
                    buildingMap.Add(m_size[i]);
                }
            }
        }
        string[] serviceStrings = Enum.GetNames(typeof(ItemClass.Service));

        private void PopulateList()
        {
            var prefabList = buildings.ToArray();
            int index = m_prefabType.selectedIndex;

            if (index != 0)
                prefabList = buildings.Where(building => building.service == allowedServices[index - 1]).ToArray();

            if (prefabList == null) return;

            // Filtering
            string filter = m_search.text.Trim().ToLower();
            if (!String.IsNullOrEmpty(filter))
                prefabList = buildings.Where(building => building.name.ToLower().Contains(filter.ToLower())).ToArray();

            // Sorting
            //Array.Sort(prefabList, CompareByNames);

            // Display
            m_itemList.RowsData.m_buffer = prefabList;
            m_itemList.RowsData.m_size = prefabList.Length;

            m_itemList.DisplayAt(0);
        }
        public int CompareByNames(XMLBuilding city1, XMLBuilding city2)
        {
            if (m_sortDirection.flip == UISpriteFlip.None)
                return String.Compare(serviceStrings[(int)city1.service] + city1.name, serviceStrings[(int)city2.service] + city2.name);
            else
                return String.Compare(serviceStrings[(int)city1.service] + city1.name, serviceStrings[(int)city2.service] + city2.name);
        }
    }
}
