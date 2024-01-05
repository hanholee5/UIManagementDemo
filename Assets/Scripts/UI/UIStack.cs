using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIManagementDemo.Base
{
    public class UIStack : MonoBehaviour
    {
        private List<UIBase> openedUIs = new List<UIBase>();

        private bool IsOpenedQuitPopup = false;

        /// <summary>
        /// UI 가 열렸다. > 열린 UI 객체를 Stack에 추가
        /// </summary>
        /// <param name="inOpenedUI">열린  UI 객체</param>
        public void OpenedUI(UIBase inOpenedUI)
        {
            openedUIs.Add(inOpenedUI);
            Debug.Log("StackUIManager - OpenedUI (" + inOpenedUI.ToString() + ")");
        }

        /// <summary>
        /// 특정한 UI가 닫혔다. > 닫힌 UI 객체를 Stack에서 제거
        /// </summary>
        /// <param name="inClosedUI"></param>
        public void ClosedUI(UIBase inClosedUI)
        {
            Debug.Log("StackUIManager - ClosedUI (" + inClosedUI.ToString() + ")");

            if (openedUIs.Count > 0)
            {
                for (int i = 0; i < openedUIs.Count; i++)
                {
                    if (openedUIs[i].Equals(inClosedUI))
                    {
                        openedUIs.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        public void CloseUI(UIBase inUIBase = null)
        {
            if (inUIBase != null)
            {
                foreach (UIBase uiBase in openedUIs)
                {
                    if (uiBase.Equals(inUIBase))
                    {
                        Debug.Log("StackUIManager - CloseUI ( " + uiBase.name + ")");
                        uiBase.Close();
                        openedUIs.Remove(uiBase);
                        break;
                    }
                }
            }
            else
            {
                if (openedUIs.Count > 0)
                {
                    UIBase openedUI = openedUIs[openedUIs.Count - 1];
                    Debug.Log("StackUIManager - CloseUI ( " + openedUI.ToString() + ")");
                    openedUI.Close();
                }
                else
                {
                    ShowQuitPopupWindow();
                }
            }
        }
        /// <summary>
        /// 씬로드와 같은것이 발생되었을때 열린 모든 UI를 닫는다.
        /// </summary>
        public void CloseAllUI()
        {
            while (openedUIs.Count > 0)
            {
                UIBase openedUI = openedUIs[openedUIs.Count - 1];
                Debug.Log("StackUIManager - CloseUI ( " + openedUI.ToString() + ")");
                openedUI.Close();
            }
        }
        /// <summary>
        /// Stack이 모두 비워진 경우 마지막에 앱 종료 확인 팝업 출력
        /// </summary>
        private void ShowQuitPopupWindow()
        {
            if (IsOpenedQuitPopup)
            {
                return;
            }
            else
            {
                IsOpenedQuitPopup = true;
            }

            //            PopupWindowController.Instance.ShowOkCancel("UI_P_67", "UI_P_66", () =>
            //            {
            //                // OK Action
            //                Debug.Log("SettingsUIController::OnClickQuitAppButton > ShowOKCancel PopUp. > Selected OK.");

            //                IsOpenedQuitPopup = false;
            //#if UNITY_EDITOR
            //                UnityEditor.EditorApplication.isPlaying = false;
            //#else
            //                Application.Quit();
            //#endif
            //            },
            //            () =>
            //            {
            //                IsOpenedQuitPopup = false;
            //                // Cancel Action
            //                Debug.Log("SettingsUIController::OnClickQuitAppButton > ShowOKCancel PopUp. > Selected Cancel.");
            //            });
        }
        /// <summary>
        /// 종료 팝업창 활성화 이후 예/아니오/X 버튼이 아닌 esc를 이용해 종료 시
        /// IsOpenedQuitPopup이 false로 바뀌지 않아 팝업창이 다시 안나오는 현상이 있어
        /// 팝업창을 닫는곳에서 체크 (true일때 false로 변경)
        /// </summary>
        public void IsOpenedQuitPopupValueCheck()
        {
            if (IsOpenedQuitPopup)
                IsOpenedQuitPopup = false;
        }

        public bool IsOpenedUI(UIBase inUIBase)
        {
            foreach (var uiBase in openedUIs)
            {
                // 메모리 주소로 비교 > 내부의 값들로 비교해야할까?
                if (uiBase == inUIBase)
                {
                    return true;
                }
            }
            return false;
        }

        public void Clear()
        {
            openedUIs.Clear();
        }
    }
}
