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
        /// UI �� ���ȴ�. > ���� UI ��ü�� Stack�� �߰�
        /// </summary>
        /// <param name="inOpenedUI">����  UI ��ü</param>
        public void OpenedUI(UIBase inOpenedUI)
        {
            openedUIs.Add(inOpenedUI);
            Debug.Log("StackUIManager - OpenedUI (" + inOpenedUI.ToString() + ")");
        }

        /// <summary>
        /// Ư���� UI�� ������. > ���� UI ��ü�� Stack���� ����
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
        /// ���ε�� �������� �߻��Ǿ����� ���� ��� UI�� �ݴ´�.
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
        /// Stack�� ��� ����� ��� �������� �� ���� Ȯ�� �˾� ���
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
        /// ���� �˾�â Ȱ��ȭ ���� ��/�ƴϿ�/X ��ư�� �ƴ� esc�� �̿��� ���� ��
        /// IsOpenedQuitPopup�� false�� �ٲ��� �ʾ� �˾�â�� �ٽ� �ȳ����� ������ �־�
        /// �˾�â�� �ݴ°����� üũ (true�϶� false�� ����)
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
                // �޸� �ּҷ� �� > ������ ����� ���ؾ��ұ�?
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
