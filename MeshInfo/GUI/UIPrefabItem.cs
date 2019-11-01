using UnityEngine;
using ColossalFramework.UI;

using UIUtils = SamsamTS.UIUtils;
using ColossalFramework.Globalization;

namespace MCSI.GUI
{
    public class UIPrefabItem : UIFastListRow
    {
        private UILabel m_name;
        private UILabel m_triangles;
        private UILabel m_lodTriangles;
        private UILabel m_weight;
        private UILabel m_lodWeight;
        private UILabel m_textureSize;
        private UILabel m_lodTextureSize;
        private UIPanel m_background;

        private XMLBuilding m_meshData;
        private float baseBgOpacity;

        public UIPanel Background
        {
            get
            {
                if (m_background == null)
                {
                    m_background = AddUIComponent<UIPanel>();
                    m_background.atlas = UIUtils.GetAtlas("MCSI");
                    m_background.width = width;
                    m_background.height = 40f;
                    m_background.relativePosition = Vector2.zero;

                    m_background.zOrder = 0;
                }

                return m_background;
            }
        }
        
        public override void Start()
        {
            base.Start();

            isVisible = true;
            canFocus = true;
            isInteractive = true;
            width = 740f;
            height = 40f;

            m_name = UIUtils.CreateLabel(this, 90f, height);
            m_name.textAlignment = UIHorizontalAlignment.Left;
            m_name.pivot = UIPivotPoint.MiddleLeft;
            m_name.relativePosition = new Vector3(10f, 0f);
            m_lodTextureSize = UIUtils.CreateLabelForGrid(this, m_name.relativePosition, 200f, height);
            m_textureSize = UIUtils.CreateLabelForGrid(this, m_lodTextureSize.relativePosition, 90f, height);

            eventMouseEnter += (component, eventParam) => Background.opacity = Mathf.Lerp(baseBgOpacity, 1, 0.5f);
            eventMouseLeave += (component, eventParam) => Background.opacity = baseBgOpacity;
            eventClick += (component, p) =>
            {
                Background.opacity = Mathf.Lerp(baseBgOpacity, 1, 0.75f);
                WorldInfoPanel.Show<CityServiceWorldInfoPanel>(m_meshData.position, m_meshData.instanceID);
                ToolsModifierControl.cameraController.SetTarget(m_meshData.instanceID, m_meshData.position, true);
            };
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Destroy(m_background);
        }

        public override void Display(object obj, bool isRowOdd)
        {
            m_meshData = obj as XMLBuilding;

            if (m_meshData == null || m_name == null) return; 

            m_name.text = m_meshData.name;
            m_name.tooltip = m_meshData.tooltip;

            m_lodTextureSize.text = m_meshData.status;
            float num = m_meshData.upkeep * 0.0016f;
            m_textureSize.text = num.ToString((num < 10f) ? Settings.moneyFormat : Settings.moneyFormatNoCents, LocaleManager.cultureInfo);

            /*
            m_weight.text = (m_meshData.weight > 0) ? m_meshData.weight.ToString("N2") : "-";
            if (m_meshData.weight >= 200)
                m_weight.textColor = new Color32(255, 0, 0, 255);
            else if (m_meshData.weight >= 100)
                m_weight.textColor = new Color32(255, 255, 0, 255);
            else if (m_meshData.weight > 0)
                m_weight.textColor = new Color32(0, 255, 0, 255);
            else
                m_weight.textColor = new Color32(255, 255, 255, 255);

            m_lodWeight.text = (m_meshData.lodWeight > 0) ? m_meshData.lodWeight.ToString("N2") : "-";
            if (m_meshData.lodWeight >= 10)
                m_lodWeight.textColor = new Color32(255, 0, 0, 255);
            else if (m_meshData.lodWeight >= 5)
                m_lodWeight.textColor = new Color32(255, 255, 0, 255);
            else if (m_meshData.lodWeight > 0)
                m_lodWeight.textColor = new Color32(0, 255, 0, 255);
            else
                m_lodWeight.textColor = new Color32(255, 255, 255, 255);

            m_textureSize.text = (m_meshData.textureSize != Vector2.zero) ? m_meshData.textureSize.x + "x" + m_meshData.textureSize.y : "-";
            m_lodTextureSize.text = (m_meshData.lodTextureSize != Vector2.zero) ? m_meshData.lodTextureSize.x + "x" + m_meshData.lodTextureSize.y : "-";
            */
            baseBgOpacity = isRowOdd ? 0.25f : 0.1f;
            Background.backgroundSprite = "1";
            Background.opacity = baseBgOpacity;
        }
    }
}
