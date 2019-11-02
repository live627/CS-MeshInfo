using ColossalFramework.UI;
using ICities;
using System;
using UnityEngine;

namespace MCSI
{
    public class MCSI : LoadingExtensionBase, IUserMod
    {
        #region IUserMod implementation
        public string Name => $"Mayoral City Service Info (v{version})";

        public string Description => "Information on every city service building all in one place. Triggered by Ctrl+Shift+K.";
        #endregion

        private static GameObject m_gameObject;
        private static GUI.UIMainPanel m_mainPanel;
        public static readonly Version version = typeof(Localization).Assembly.GetName().Version;

        #region LoadingExtensionBase overrides
        /// <summary>
        /// Called when the level (game, map editor, asset editor) is loaded
        /// </summary>
        public override void OnLevelLoaded(LoadMode mode)
        {
            try
            {
                UIView view = UIView.GetAView();
                m_gameObject = new GameObject("MCSI");
                m_gameObject.transform.SetParent(view.transform);

                m_mainPanel = m_gameObject.AddComponent<GUI.UIMainPanel>();
            }
            catch(Exception e)
            {
                DebugUtils.Warning("Couldn't create the UI. Try relaunching the game.");
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Called when the level is unloaded
        /// </summary>
        public override void OnLevelUnloading()
        {
            try
            {
                if (m_gameObject == null) return;

                UnityEngine.Object.Destroy(m_gameObject);
                m_gameObject = null;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        #endregion
    }

    public class Localization
    {
    }
}
