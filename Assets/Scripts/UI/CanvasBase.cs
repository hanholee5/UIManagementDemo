using System.Collections;
using System.Collections.Generic;
using UIManagementDemo.Global;
using UIManagementDemo.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagementDemo.Base
{
    public class CanvasBase : MonoBehaviour
    {
        /// <summary>
        /// Canvas + UI�� ����ϰ��� �Ѵٸ� �� Ŭ������ ���, �����Ѵ�. (MainUICanvas)
        /// Canvas�� ���� UIBase���� UIManager�� ��� (LoadedUIs)
        /// Canvas ��ü�� ���� �Ҵ�. (Open)
        /// </summary>

        /// <summary>
        /// �ڽ��� ���� UI Scene�� ������
        /// </summary>
        [SerializeField] private UISceneType sceneType;

        /// <summary>
        /// Canvas �ڽ��� ������
        /// </summary>
        [SerializeField] private UICanvasType canvasType;

        /// <summary>
        /// Canvas�� ���� UI�� UIType.
        /// �������� ��ü ������ UIManager�� ���ؼ� �Ѵ�.
        /// </summary>
        private List<UIType> childUITypes = new List<UIType>();

        public UICanvasType CanvasType { get => canvasType; set => canvasType = value; }
        public UISceneType SceneType { get => sceneType; set => sceneType = value; }

        protected virtual void Awake()
        {
            Debug.Log("CanvasBase ( " + this.name + " )");
            // ��� Child UI�� �ʱ�ȭ (���� ó��)
            AwakeInitChildUIBases();

            // GamePlaye �ΰ�� UIManager�� Child UI ���
            if (GlobalManager.Instance != null)
                RegisterLoadedUIs();
        }
        protected virtual void OnDestroy()
        {
            Debug.Log("CanvasBase OnDestroy ( SceneType : " + this.SceneType.ToString() + " , CanvasType : " + CanvasType.ToString() + " )");
            UnregisterLoadedUIs();
        }

        /// <summary>
        /// Canvas�� Child UI���� �ʱ�ȭ
        /// </summary>
        /// <param name="inInitAndOpen">�ʱ�ȭ�� �Բ� ����ó�� �� ������?</param>
        private void AwakeInitChildUIBases()
        {
            Debug.Log("CanvasBase InitChildUIBases ( SceneType : " + this.SceneType.ToString() + " , CanvasType : " + CanvasType.ToString() + " )");

            // UIBase �ʱ�ȭ > ����
            for (int i = 0; i < this.transform.childCount; i++)
            {
                Transform child = this.transform.GetChild(i);
                UIBase uiBase = child.GetComponent<UIBase>();
                if (uiBase != null)
                {
                    uiBase.OwnerCanvas = this;
                    uiBase.Close();
                }
            }
        }

        /// <summary>
        /// Canvas�� Child�� UIBase��ü���� UIManager�� LoadedUIs�� ���
        /// </summary>
        protected virtual void RegisterLoadedUIs()
        {
            Debug.Log("CanvasBase RegisterUIBase ( " + this.name + " )");

            childUITypes.Clear();

            for (int i = 0; i < this.transform.childCount; i++)
            {
                Transform child = this.transform.GetChild(i);
                UIBase uiBase = child.GetComponent<UIBase>();
                if (uiBase != null)
                {
                    Debug.Log(" Found Object " + uiBase.Type.ToString());

                    UIManager.Instance.RegisterLoadedUI(uiBase);

                    childUITypes.Add(uiBase.Type);
                }
            }
        }
        /// <summary>
        /// UIManager�� loadedUIs ���� UI�� ��� ����
        /// </summary>
        protected virtual void UnregisterLoadedUIs()
        {
            Debug.Log("CanvasBase UnregisterUIBases ( " + this.name + " )");
            if (childUITypes.Count > 0)
            {
                foreach (UIType uiType in childUITypes)
                {
                    Debug.Log(" Unregistered UI : " + uiType.ToString());
                    UIManager.Instance?.UnregisterLoadedUI(uiType);
                }
            }
        }
        /// <summary>
        /// Canvas�� Visibility Ȯ��
        /// </summary>
        /// <returns></returns>
        public bool IsCanvasEnabled()
        {
            Canvas canvas = GetComponent<Canvas>();
            if (canvas != null)
            {
                Debug.Log("CanvasBase IsCanvasEnabled : " + canvas.enabled.ToString());
                return canvas.enabled;
            }

            return false;
        }

        /// <summary>
        /// Canvas�� Visibility ����
        /// </summary>
        /// <param name="inEnabled"></param>
        public void SetEnableCanvas(bool inEnabled)
        {
            Debug.Log("CanvasBase SetVisible ( " + inEnabled.ToString() + " )");
            Canvas canvas = GetComponent<Canvas>();
            if (canvas != null && IsCanvasEnabled() != inEnabled)
            {
                canvas.enabled = inEnabled;
            }
        }

        /// <summary>
        /// Canvas ��ü�� ���� �Է� Ȱ��/�����
        /// </summary>
        /// <param name="inDisabled"></param>
        public void SetCanvasInputDisable(bool inDisabled)
        {
            Debug.Log("CanvasBase SetInputDisable ( " + inDisabled.ToString() + " )");
            GraphicRaycaster graphicRaycaster = GetComponent<GraphicRaycaster>();
            if (graphicRaycaster != null)
            {
                graphicRaycaster.enabled = !inDisabled;
            }
        }


        public bool IsChild(UIType inUIType)
        {
            for (int i = 0; i < this.transform.childCount; i++)
            {
                Transform child = this.transform.GetChild(i);
                UIBase uiBase = child.GetComponent<UIBase>();
                if (uiBase != null && uiBase.Type == inUIType)
                {
                    return true;
                }
            }
            return false;
        }
    }
}