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
        /// Canvas + UI로 사용하고자 한다면 이 클래스를 상속, 구현한다. (MainUICanvas)
        /// Canvas에 속한 UIBase들을 UIManager에 등록 (LoadedUIs)
        /// Canvas 자체를 끄고 켠다. (Open)
        /// </summary>

        /// <summary>
        /// 자신이 속한 UI Scene의 구분자
        /// </summary>
        [SerializeField] private UISceneType sceneType;

        /// <summary>
        /// Canvas 자신의 구분자
        /// </summary>
        [SerializeField] private UICanvasType canvasType;

        /// <summary>
        /// Canvas에 속한 UI의 UIType.
        /// 직접적인 객체 참조는 UIManager를 통해서 한다.
        /// </summary>
        private List<UIType> childUITypes = new List<UIType>();

        public UICanvasType CanvasType { get => canvasType; set => canvasType = value; }
        public UISceneType SceneType { get => sceneType; set => sceneType = value; }

        protected virtual void Awake()
        {
            Debug.Log("CanvasBase ( " + this.name + " )");
            // 모든 Child UI의 초기화 (닫힘 처리)
            AwakeInitChildUIBases();

            // GamePlaye 인경우 UIManager에 Child UI 등록
            if (GlobalManager.Instance != null)
                RegisterLoadedUIs();
        }
        protected virtual void OnDestroy()
        {
            Debug.Log("CanvasBase OnDestroy ( SceneType : " + this.SceneType.ToString() + " , CanvasType : " + CanvasType.ToString() + " )");
            UnregisterLoadedUIs();
        }

        /// <summary>
        /// Canvas의 Child UI들의 초기화
        /// </summary>
        /// <param name="inInitAndOpen">초기화와 함께 열림처리 할 것인지?</param>
        private void AwakeInitChildUIBases()
        {
            Debug.Log("CanvasBase InitChildUIBases ( SceneType : " + this.SceneType.ToString() + " , CanvasType : " + CanvasType.ToString() + " )");

            // UIBase 초기화 > 닫힘
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
        /// Canvas의 Child인 UIBase객체들을 UIManager의 LoadedUIs에 등록
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
        /// UIManager의 loadedUIs 에서 UI를 등록 해제
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
        /// Canvas의 Visibility 확인
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
        /// Canvas의 Visibility 설정
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
        /// Canvas 전체에 대한 입력 활성/비뢀성
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