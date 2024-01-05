using UIManagementDemo.Global;
using UIManagementDemo.Managers;
using UnityEngine;

namespace UIManagementDemo.Base
{
    public class UIBase : MonoBehaviour
    {
        /// <summary>
        /// UI�� ���� Scene�� ������
        /// </summary>
        [SerializeField] private UISceneType ownerSceneType;

        /// <summary>
        /// UI ������
        /// </summary>
        [SerializeField] private UIType type;

        /// <summary>
        /// ���� ������ �ٸ� UI�� ��쿡 ���� ������
        /// </summary>
        [SerializeField] private uint id = 0;

        [SerializeField] private bool activeOnStart = false;
        [SerializeField] private GameObject blurBackground;
        private CanvasBase ownerCanvas;

        /// <summary>
        /// UI ���� Stack ��� ���� (true : Escape�� UI �ݱ� ��� ����, false: ���� ���� ���� ���� ���� UI)
        /// </summary>
        [SerializeField] private bool registerable = true;

        public UIType Type { get => type; set => type = value; }
        public UISceneType OwerSceneType { get => ownerSceneType; set => ownerSceneType = value; }
        public CanvasBase OwnerCanvas { get => ownerCanvas; set => ownerCanvas = value; }
        public bool Registerable { get => registerable; set => registerable = value; }

        public uint Id { get => id; set => id = value; }
        public bool ActiveOnStart { get => activeOnStart; }

        protected bool isOpened = false;
        /// <summary>
        /// UI ����
        /// </summary>       
        /// 
        protected virtual void Start()
        {
            Debug.Log("UIBase Start ( " + this.name + " )");
        }
        public virtual void Open()
        {
            Debug.Log("UIBase Open ( " + this.name + " )");

            if (ownerCanvas != null && ownerCanvas.IsCanvasEnabled() == false)
            {
                Debug.LogWarning("UIBase Show failed. Canvas is invisibled.");
                return;
            }

            if (!IsOpened())
            {
                this.gameObject?.SetActive(true);
                if (Registerable)
                {
                    UIManager.Instance.OpenedUI(this);
                }
            }

            ActivateBlurBackground(true);
        }

        /// <summary>
        /// UI �ݱ�
        /// </summary>
        public virtual void Close()
        {
            Debug.Log("UIBase Close ( " + this.name + " )");

            if (ownerCanvas != null && ownerCanvas.IsCanvasEnabled() == false)
            {
                Debug.LogWarning("UIBase Show failed. Canvas is invisibled.");
                return;
            }

            if (IsOpened())
            {
                this.gameObject?.SetActive(false);

                if (Registerable)
                {
                    UIManager.Instance?.CloseUI(this);
                }
            }
            ActivateBlurBackground(false);
        }

        private void ActivateBlurBackground(bool inIsOn)
        {
            if (blurBackground != null)
            {
                if (inIsOn)
                {
                    if (!blurBackground.activeSelf)
                    {
                        blurBackground.SetActive(true);
                    }
                }
                else
                {
                    if (blurBackground.activeSelf)
                    {
                        blurBackground.SetActive(false);
                    }
                }

            }
        }

        /// <summary>
        /// UI�� �����ִ��� Ȯ��
        /// </summary>
        /// <returns>true : ��������</returns>
        public virtual bool IsOpened()
        {
            Debug.Log("UIBase IsOpened ( " + this.name + " )");

            if (this.gameObject != null)
            {
                isOpened = this.gameObject.activeSelf;
                return isOpened;
            }

            return isOpened;
        }

    }
}