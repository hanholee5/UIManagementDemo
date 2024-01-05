using UIManagementDemo.Global;
using UIManagementDemo.Managers;
using UnityEngine;

namespace UIManagementDemo.Global
{
    public class GlobalManager : GenericSingleton<GlobalManager>
    {
        private GameState gameState;    // 게임 진행 상태

        // Update is called once per frame
        void Update()
        {
            if (gameState == GameState.FIELD)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    UIManager.Instance.CloseUI();


                    //UIStack.Instance.CloseUI();
                }
            }
        }
    }
}


