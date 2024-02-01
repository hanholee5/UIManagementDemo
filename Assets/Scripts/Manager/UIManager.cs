using System.Collections;
using System.Collections.Generic;
using UIManagementDemo.Base;
using UIManagementDemo.Global;
using UIManagementDemo.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;

namespace UIManagementDemo.Managers
{
    public class UIManager : GenericSingleton<UIManager>
    {
        private Dictionary<UIType, UIBase> loadedUIs = new Dictionary<UIType, UIBase>();

        private Dictionary<string, SceneInstance> loadedSceneInstances = new Dictionary<string, SceneInstance>();

        // 열려있는 모든 UI
        private UIStack stackedUIs;

        // Canvas 정보는 LoadedUIs에서 참조 한다.
        protected AsyncOperationHandle<SceneInstance> sceneHandle;

        protected override void Awake()
        {
            Debug.Log("UIManager Awake");
            base.Awake();
        }

        #region UI Scene 관리

        /// <summary>
        /// UI Scene을 (비동기)로드 한다.
        /// Load 이후의 초기화는 inLoadCompleteAction 에서 수행한다.
        /// Load 이후 닫힘으로 초기화 되므로 출력을 원한다면 열어줘야 함.
        /// </summary>
        /// <param name="inUISceneType">Load 할 SceneType</param>
        /// <param name="inLoadCompleteAction">초기화를 위한 로드 완료 이벤트</param>
        public void LoadUIScene(UISceneType inUISceneType, Action inLoadCompleteAction = null)
        {
            Debug.Log("UIManager LoadUIScene ( SceneType : " + inUISceneType.ToString() + " )");

            string uiSceneName = Utilities.ConvertUISceneTypeToUISceneName(inUISceneType);
            if (string.IsNullOrEmpty(uiSceneName))
            {
                Debug.LogError("UIManager LoadUIScene  - Not found SceneName string.");
                return;
            }

            if (!IsUISceneLoadedAdditive(uiSceneName))
            {
                sceneHandle = Addressables.LoadSceneAsync(uiSceneName, LoadSceneMode.Additive, true);
                sceneHandle.Completed += (handle) =>
                {
                    inLoadCompleteAction?.Invoke();

                    loadedSceneInstances.Add(uiSceneName, sceneHandle.Result);
                };
            }
        }

        /// <summary>
        /// 한번에 여러개의 UIScene을 로드
        /// </summary>
        /// <param name="inUIScenesArray">로드 할 UISceneType Array</param>
        /// <param name="inLoadCompleteAction">전체 로드 완료 이벤트</param>
        public void LoadUIScenesAtOnce(UISceneType[] inUIScenesArray, Action inLoadCompleteAction = null)
        {
            List<string> uiSceneNames = new List<string>();

            for (int i = 0; i < inUIScenesArray.Length; i++)
            {
                uiSceneNames.Add(Utilities.ConvertUISceneTypeToUISceneName(inUIScenesArray[i]));
            }

            StartCoroutine(CoLoadUIScenesAtOnce(uiSceneNames, inLoadCompleteAction));
        }

        private IEnumerator CoLoadUIScenesAtOnce(List<string> uiSceneNames, Action inLoadCompleteAction = null)
        {
            for (int i = 0; i < uiSceneNames.Count; i++)
            {
                sceneHandle = Addressables.LoadSceneAsync(uiSceneNames[i], LoadSceneMode.Additive);
                sceneHandle.Completed += (handle) =>
                {
                    Debug.Log("CoLoadUIScenesAtOnce Load complete " + uiSceneNames[i]);
                    loadedSceneInstances.Add(uiSceneNames[i], sceneHandle.Result);
                };
                while (!sceneHandle.IsDone)
                {
                    yield return null;
                }
            }
            if (inLoadCompleteAction != null)
            {
                Debug.Log("CoLoadUIScenesAtOnce ALL Load complete ");
                inLoadCompleteAction.Invoke();
            }
        }

        /// <summary>
        /// UI Scene을 언로드한다.
        /// </summary>
        /// <param name="inUISceneName">UI Scene의 이름. (GlobalStrings에서 정의됨.)</param>
        public void UnloadUIScene(UISceneType inUISceneType)
        {
            Debug.Log("UIManager UnloadUIScene (UISceneType " + inUISceneType.ToString() + " )");

            string uiSceneName = Utilities.ConvertUISceneTypeToUISceneName(inUISceneType);
            if (string.IsNullOrEmpty(uiSceneName)) return;

            if (!IsUISceneLoadedAdditive(uiSceneName)) return;

            if (loadedSceneInstances.ContainsKey(uiSceneName))
            {
                SceneInstance sceneInstance = loadedSceneInstances[uiSceneName];
                Addressables.UnloadSceneAsync(sceneInstance);

                loadedSceneInstances.Remove(uiSceneName);
            }
        }

        public void ClearLoadedSceneInstances()
        {
            foreach (var key in new List<string>(loadedSceneInstances.Keys))
            {
                // 로딩 인스턴스는 스스로 스스로 삭제 되는 것을 기다려줘야 한다.
                if (key == Utilities.ConvertUISceneTypeToUISceneName(UISceneType.LOADING))
                    continue;

                loadedSceneInstances.Remove(key);
            }

            //loadedSceneInstances.Clear();
        }

        public void UnLoadAll()
        {
            foreach (var loadedSceneInstance in loadedSceneInstances)
            {
                string uiSceneName = loadedSceneInstance.Key;
                SceneInstance sceneInstance = loadedSceneInstance.Value;

                Addressables.UnloadSceneAsync(sceneInstance);
            }

            ClearStack();
        }

        /// <summary>
        /// UI Scene에 속한 UI의 모든 Canvas의 Show 제어
        /// </summary>
        /// <param name="inUIScene">제어할 UISceneType</param>
        /// <param name="inVisible">출력, 숨김</param>
        public void OpenUIScene(UISceneType inUISceneType, bool inVisible)
        {
            string uiSceneName = Utilities.ConvertUISceneTypeToUISceneName(inUISceneType);
            if (string.IsNullOrEmpty(uiSceneName)) return;

            if (!IsUISceneLoadedAdditive(uiSceneName)) return;

            foreach (var uiBase in loadedUIs)
            {
                if (uiBase.Value.OwerSceneType == inUISceneType)
                {
                    if (uiBase.Value.OwnerCanvas.IsCanvasEnabled() != inVisible)
                        uiBase.Value.OwnerCanvas.SetEnableCanvas(inVisible);

                    uiBase.Value.Open();
                }
            }
        }

        public bool IsUISceneLoaded(UISceneType uiSceneType)
        {
            string uiSceneName = Utilities.ConvertUISceneTypeToUISceneName(UISceneType.LOADING);
            if (string.IsNullOrEmpty(uiSceneName))
                return false;

            return IsUISceneLoadedAdditive(uiSceneName);
        }

        public bool IsUISceneLoadedAdditive(string inSceneName)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == inSceneName && scene.isLoaded)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion UI Scene 관리

        #region UIBase 관리

        /// <summary>
        /// UI Type을 구분자로 하여 UIBase의 Show 제어
        /// </summary>
        /// <param name="inUIType">UIType</param>
        public void OpenUI(UIType inUIType)
        {
            Debug.Log("UIManager OpenUI ( " + inUIType.ToString() + " )");

            if (loadedUIs.ContainsKey(inUIType) /*&& openedUIs.ContainsKey(inUIType) == false*/)
            {
                UIBase uIBase = loadedUIs[inUIType];
                if (uIBase != null)
                {
                    uIBase.Open();

                    Debug.Log("UIManager OpenUI - Added to Stack (" + inUIType.ToString() + " )");
                }
            }
        }

        /// <summary>
        /// UI를 닫음.
        /// 인자가 없으면 Stack Pop으로 닫고, 있으면 해당 UI를 특정해서 닫음
        /// </summary>
        /// <param name="inUIType">닫고자 하는 UI 구분자</param>
        public void CloseUI(UIBase inUIBase = null)
        {
            if (inUIBase != null)
                Debug.Log("UIManager CloseUI ( " + inUIBase.name + " )");
            else
                Debug.Log("UIManager CloseUI ()");

            if (inUIBase == null)
            {
                stackedUIs.CloseUI();
            }
            else
            {
                if (inUIBase.Registerable)
                    stackedUIs.CloseUI(inUIBase);
                else
                    inUIBase.Close();
            }
        }

        /// <summary>
        /// UI를 닫는다.
        /// </summary>
        /// <param name="inUIType">닫을 UI의 Type</param>
        public void CloseUI(UIType inUIType)
        {
            UIBase UI = GetUI(inUIType);
            if (UI != null)
                CloseUI(UI);
        }

        /// <summary>
        /// UI가 열린 이후에 Stack에 추가 해주는 함수.
        /// </summary>
        /// <param name="inUIBase">Stack에 추가할 열린 UI</param>
        public void OpenedUI(UIBase inUIBase)
        {
            if (stackedUIs != null)
                stackedUIs.OpenedUI(inUIBase);
        }

        /// <summary>
        /// UI가 닫힌 이후에 해당 UI를 Stack에서 제거
        /// </summary>
        /// <param name="inUIBase">Stack에서 제거할 UI</param>
        public void ClosedUI(UIBase inUIBase)
        {
            if (stackedUIs != null)
                stackedUIs.ClosedUI(inUIBase);
        }

        /// <summary>
        /// UIBase 를 획득
        /// </summary>
        /// <param name="inCanvasType">CanvasType</param>
        /// <param name="inUIType">UIType</param>
        /// <returns>UIBase</returns>
        public UIBase GetUI(UICanvasType inCanvasType, UIType inUIType)
        {
            if (loadedUIs.ContainsKey(inUIType))
            {
                if (loadedUIs[inUIType].OwnerCanvas.CanvasType == inCanvasType)
                    return loadedUIs[inUIType];
            }
            return null;
        }

        /// <summary>
        /// UIBase를 획득
        /// </summary>
        /// <param name="inUIType">UIType</param>
        /// <returns>UIBase</returns>
        public UIBase GetUI(UIType inUIType)
        {
            if (loadedUIs.ContainsKey(inUIType))
            {
                return loadedUIs[inUIType];
            }
            return null;
        }

        #endregion UIBase 관리

        #region Canvas 관리

        /// <summary>
        /// Canvas Enable/Disable
        /// </summary>
        /// <param name="inCanvasType"></param>
        /// <param name="inEnabled"></param>
        public void SetEnableCanvas(UICanvasType inCanvasType, bool inEnabled)
        {
            Debug.Log("UIManager SetEnableCanvas ( CanvasType" + inCanvasType.ToString() + " )");

            foreach (KeyValuePair<UIType, UIBase> pair in loadedUIs)
            {
                CanvasBase canvasBase = pair.Value.OwnerCanvas as CanvasBase;
                if (canvasBase.CanvasType == inCanvasType)
                {
                    canvasBase.SetEnableCanvas(inEnabled);
                    break;
                }
            }
        }

        #endregion Canvas 관리

        #region 기타 지원 함수들

        /// <summary>
        /// 열려있는 모든 UI를 닫음
        /// </summary>
        public void CloseAllUIs()
        {
            Debug.Log("UIManager CloseAllUIs");
            stackedUIs.CloseAllUI();
            //while (openedUIs.Count > 0)
            //{
            //    UIBase uiBase = openedUIs.Pop();
            //    uiBase.Close();

            //    Debug.Log("UIManager CloseAllUIs - Closed : " + uiBase.Type.ToString());
            //}
        }

        public void ClearStack()
        {
            stackedUIs.Clear();
        }

        #endregion 기타 지원 함수들

        #region Canvas 전용 등록 함수

        /// <summary>
        /// Canvas 초기화 시에 자신의 객체들을 등록함.
        /// </summary>
        /// <param name="inUIBase">Canvas에 속한 자식 UI 객체</param>
        public void RegisterLoadedUI(UIBase inUIBase)
        {
            Debug.Log("RegisterLoadedUI - ( CanvasType : " + inUIBase.OwnerCanvas.CanvasType + " , UIType : " + inUIBase.Type.ToString() + " )");

            if (inUIBase.Type == UIType.NONE || inUIBase == null)
            {
                Debug.LogError("RegisterLoadedUI Error. ( " + inUIBase.name + " )");
                return;
            }
            if (loadedUIs.ContainsKey(inUIBase.Type))
            {
                Debug.LogError("UIManager RegisterLoadedUI - Error. Already Loaded UI. - " + inUIBase.Type.ToString());
                return;
            }

            loadedUIs.Add(inUIBase.Type, inUIBase);

            //Debug.Log("RegisterLoadedUI - added ( " + inUIType.ToString() + " ), loadedUIs Count : " + loadedUIs.Count.ToString());
        }

        /// <summary>
        /// Canvas 소멸시에 등록되었던 UI를 제거함.
        /// </summary>
        /// <param name="inUIType">UI 구분자</param>
        public void UnregisterLoadedUI(UIType inUIType)
        {
            Debug.Log("UnregisterLoadedUI - ( " + inUIType + " ), loadedUIs Count : " + loadedUIs.Count.ToString());

            if (loadedUIs.ContainsKey(inUIType))
                loadedUIs.Remove(inUIType);

            Debug.Log("UnregisterLoadedUI - Removed ( " + inUIType + " ), loadedUIs Count : " + loadedUIs.Count.ToString());
        }

        public void ClearLoading()
        {
            StartCoroutine(CoClearLoading());
        }

        private IEnumerator CoClearLoading()
        {
            string loadingSceneName = "2_LoadingScene";
            if (IsUISceneLoadedAdditive(loadingSceneName))
            {
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene scene = SceneManager.GetSceneAt(i);
                    if (scene.name == loadingSceneName && scene.isLoaded)
                    {
                        Debug.Log("FadeT - Unload Loading Scene");
                        SceneManager.UnloadSceneAsync(scene);
                    }
                }
            }

            Debug.Log("FadeT - ClearLoading");
            if (IsUISceneLoaded(UISceneType.LOADING))
            {
                Debug.Log("FadeT -Unload Loading UI Scene");
                UnloadUIScene(UISceneType.LOADING);
            }

            yield return null;
        }


    }
    #endregion
}
