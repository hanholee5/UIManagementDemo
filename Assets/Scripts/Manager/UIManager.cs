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

        // �����ִ� ��� UI
        private UIStack stackedUIs;

        // Canvas ������ LoadedUIs���� ���� �Ѵ�.
        protected AsyncOperationHandle<SceneInstance> sceneHandle;

        protected override void Awake()
        {
            Debug.Log("UIManager Awake");
            base.Awake();
        }

        #region UI Scene ����

        /// <summary>
        /// UI Scene�� (�񵿱�)�ε� �Ѵ�.
        /// Load ������ �ʱ�ȭ�� inLoadCompleteAction ���� �����Ѵ�.
        /// Load ���� �������� �ʱ�ȭ �ǹǷ� ����� ���Ѵٸ� ������� ��.
        /// </summary>
        /// <param name="inUISceneType">Load �� SceneType</param>
        /// <param name="inLoadCompleteAction">�ʱ�ȭ�� ���� �ε� �Ϸ� �̺�Ʈ</param>
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
        /// �ѹ��� �������� UIScene�� �ε�
        /// </summary>
        /// <param name="inUIScenesArray">�ε� �� UISceneType Array</param>
        /// <param name="inLoadCompleteAction">��ü �ε� �Ϸ� �̺�Ʈ</param>
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
        /// UI Scene�� ��ε��Ѵ�.
        /// </summary>
        /// <param name="inUISceneName">UI Scene�� �̸�. (GlobalStrings���� ���ǵ�.)</param>
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
                // �ε� �ν��Ͻ��� ������ ������ ���� �Ǵ� ���� ��ٷ���� �Ѵ�.
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
        /// UI Scene�� ���� UI�� ��� Canvas�� Show ����
        /// </summary>
        /// <param name="inUIScene">������ UISceneType</param>
        /// <param name="inVisible">���, ����</param>
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

        #endregion UI Scene ����

        #region UIBase ����

        /// <summary>
        /// UI Type�� �����ڷ� �Ͽ� UIBase�� Show ����
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
        /// UI�� ����.
        /// ���ڰ� ������ Stack Pop���� �ݰ�, ������ �ش� UI�� Ư���ؼ� ����
        /// </summary>
        /// <param name="inUIType">�ݰ��� �ϴ� UI ������</param>
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
        /// UI�� �ݴ´�.
        /// </summary>
        /// <param name="inUIType">���� UI�� Type</param>
        public void CloseUI(UIType inUIType)
        {
            UIBase UI = GetUI(inUIType);
            if (UI != null)
                CloseUI(UI);
        }

        /// <summary>
        /// UI�� ���� ���Ŀ� Stack�� �߰� ���ִ� �Լ�.
        /// </summary>
        /// <param name="inUIBase">Stack�� �߰��� ���� UI</param>
        public void OpenedUI(UIBase inUIBase)
        {
            if (stackedUIs != null)
                stackedUIs.OpenedUI(inUIBase);
        }

        /// <summary>
        /// UI�� ���� ���Ŀ� �ش� UI�� Stack���� ����
        /// </summary>
        /// <param name="inUIBase">Stack���� ������ UI</param>
        public void ClosedUI(UIBase inUIBase)
        {
            if (stackedUIs != null)
                stackedUIs.ClosedUI(inUIBase);
        }

        /// <summary>
        /// UIBase �� ȹ��
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
        /// UIBase�� ȹ��
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

        #endregion UIBase ����

        #region Canvas ����

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

        #endregion Canvas ����

        #region ��Ÿ ���� �Լ���

        /// <summary>
        /// �����ִ� ��� UI�� ����
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

        #endregion ��Ÿ ���� �Լ���

        #region Canvas ���� ��� �Լ�

        /// <summary>
        /// Canvas �ʱ�ȭ �ÿ� �ڽ��� ��ü���� �����.
        /// </summary>
        /// <param name="inUIBase">Canvas�� ���� �ڽ� UI ��ü</param>
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
        /// Canvas �Ҹ�ÿ� ��ϵǾ��� UI�� ������.
        /// </summary>
        /// <param name="inUIType">UI ������</param>
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
