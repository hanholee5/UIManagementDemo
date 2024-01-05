using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UIManagementDemo.Global;
using UIManagementDemo.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagementDemo.Utils
{
    public static class Utilities
    {
        /// <summary>
        /// DB���� �� ����Ŭ�� �°� ��ȯ
        /// </summary>
        public static string TranslateStringForClient(string inValue)
        {
            if (string.IsNullOrEmpty(inValue))
                return null;
            else
            {
                string tempstr = inValue;
                TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
                //string.ToLower = ��� ���ڸ� �ҹ��ڷ� ��ȯ
                //string.ToTitleCase �������� ù ���ڸ� �빮�ڷ� ��ȯ�ϰ� �������� �ҹ��ڷ� ��ȯ
                inValue = ti.ToTitleCase(tempstr.ToLower());
                return inValue;
            }
        }

        /// <summary>
        /// Ŭ���� ������ DB�� �°� ��ȯ
        /// </summary>
        /// <param name="inValue"></param>
        /// <returns></returns>
        public static string TranslateStringForDB(string inValue)
        {
            if (string.IsNullOrEmpty(inValue))
                return null;
            else
                return inValue.ToUpper();
        }

        public static void DeleteCloneText(GameObject obj)
        {
            int index = obj.name.IndexOf("(Clone)");
            if (index > 0)
            {
                obj.name = obj.name.Substring(0, index);
            }
        }

        public static bool GetWordRectInText(this Text textUI, out Rect rect, string word)
        {
            rect = new Rect();
            if (string.IsNullOrEmpty(textUI.text) || string.IsNullOrEmpty(word) || !textUI.text.Contains(word))
            {
                return false;
            }

            Canvas.ForceUpdateCanvases();

            TextGenerator textGenerator = textUI.cachedTextGenerator;
            if (textGenerator.characterCount == 0)
            {
                textGenerator = textUI.cachedTextGeneratorForLayout;
            }

            if (textGenerator.characterCount == 0 || textGenerator.lineCount == 0)
            {
                return false;
            }

            List<UILineInfo> lines = textGenerator.lines as List<UILineInfo>;
            List<UICharInfo> characters = textGenerator.characters as List<UICharInfo>;

            int startIndex = textUI.text.IndexOf(word);
            UILineInfo lineInfo = new UILineInfo();
            for (int i = textGenerator.lineCount - 1; i >= 0; i--)
            {
                if (lines != null && lines[i].startCharIdx <= startIndex)
                {
                    lineInfo = lines[i];
                    break;
                }
            }

            if (lines != null && characters != null)
            {
                var anchoredPosition = textUI.rectTransform.anchoredPosition;

                var screenRatio = 1080f / Screen.height; // �����ػ�

                rect.x = characters[startIndex].cursorPos.x * screenRatio + anchoredPosition.x;

                var temp = anchoredPosition.y / screenRatio;
                var yRatio = (float)Math.Round(temp, MidpointRounding.AwayFromZero); // �ݿø� �������
                rect.y = lineInfo.topY + yRatio;

                for (var index = startIndex; index < startIndex + word.Length; index++)
                {
                    var info = characters[index];
                    rect.width += info.charWidth * screenRatio;
                }

                rect.height = lineInfo.height * screenRatio;
            }

            return true;
        }

        public static string PrintTextLimit(string inStr, int inLength)
        {
            //���� ���ڼ��� inLength�� �ʰ��϶� inLength�ڱ��� ����ϰ� �ڿ� ...���δ�
            if (inStr.Length > inLength)
            {
                string realShowTitle = inStr.Substring(0, inLength) + "...";
                return realShowTitle;
            }
            else
            {
                return inStr;
            }
        }

        /// <summary>
        /// ���ڸ������� �޸� ���
        /// </summary>
        /// <param name="inNumber"></param>
        /// <returns></returns>
        public static string CommaEveryThreeNumbers(int inNumber)
        {
            return string.Format("{0:#,0}", inNumber);
        }

        /// <summary>
        /// ��ο� ����ϴ� string���� ��/�ҹ��� ǥ��ȭ
        /// ù ���ڸ� �빮��, ������ ���ڴ� �ҹ��� �Ǵ� ����, '_' ex : Hair, Top, Glove
        /// </summary>
        /// <param name="inString"></param>
        /// <returns></returns>
        public static string ConvertCodeToPathString(this string inString)
        {
            return inString.Substring(0, 1).ToUpper() + inString.Substring(1, inString.Length - 1).ToLower();
        }

        /// <summary>
        /// �� ���� ������ ����
        /// </summary>
        /// <param name="vStart">����1</param>
        /// <param name="vEnd">����2</param>
        /// <returns></returns>
        public static float GetAngle(Vector3 vStart, Vector3 vEnd)
        {
            Vector3 v = vEnd - vStart;

            return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg * 2;
        }

        /// <summary>
        /// ���ڿ��� ������ ����(_)���� �����մϴ�.
        /// </summary>
        /// <param name="inPartsName">������ �Է� ���ڿ��Դϴ�.</param>
        /// <returns>������ �������� ���ҵ� ���ڿ� �迭�� ��ȯ�մϴ�. ���� ������ ���� ���, ���� ���ڿ��� �� ���ڿ��� ������ �迭�� ��ȯ�մϴ�.</returns>
        public static string[] SplitAtLastUnderscore(string inPartsName)
        {
            // ���ڿ����� ������ ������ ã���ϴ�.
            int lastUnderscore = inPartsName.LastIndexOf('_');

            // ���� ������ ������, ���� ���ڿ��� �� ���ڿ��� ������ �迭�� ��ȯ�մϴ�.
            if (lastUnderscore == -1)
                return new string[] { inPartsName, "" };

            // �׷��� ������, ���ڿ��� ������ ���ٿ��� �� �κ����� �����ϰ� �迭�� ��ȯ�մϴ�.
            return new string[] { inPartsName.Substring(0, lastUnderscore), inPartsName.Substring(lastUnderscore + 1) };
        }

        /// <summary>
        /// �ٸ����̼� ��ȣ�� �����̽� ������ ���� �� UV �ε����� ��ȯ�մϴ�.
        /// </summary>
        /// <param name="inVariationNumber">�ٸ����̼� ��ȣ</param>
        /// <param name="inSliceCount">�����̽� ���� (�⺻��: 4)</param>
        /// <returns>�� UV �ε����� ��� �ִ� float �迭</returns>
        public static float[] GetEyeUVIndex(string inVariationNumber, int inSliceCount = 4)
        {
            float[] eyeUV = new float[2];
            float offset = (100 / inSliceCount) * 0.01f;
            var variationNumber = int.Parse(inVariationNumber);

            // �� UV �ε��� ���
            eyeUV[0] = (variationNumber % inSliceCount) * offset;   // ���� �ε���
            eyeUV[1] = (variationNumber / inSliceCount) * -offset;  // ���� �ε���

            return eyeUV;
        }

        public static string ConvertUISceneTypeToUISceneName(UISceneType inUIScene)
        {
            switch (inUIScene)
            {
                case UISceneType.MAIN:
                    return GlobalStrings.UI_SCENENAME_MAIN;

                case UISceneType.LOGIN:
                    return GlobalStrings.UI_SCENENAME_LOGIN;

                case UISceneType.LOADING:
                    return GlobalStrings.UI_SCENENAME_LOADING;

                case UISceneType.FIELD:
                    return GlobalStrings.UI_SCENENAME_FIELD;

                case UISceneType.VOICEQUIZE:
                    return GlobalStrings.UI_SCENENAME_VOICEQUIZ;

                case UISceneType.SOCIAL:
                    return GlobalStrings.UI_SCENENAME_SOCIAL;

                case UISceneType.CUSTOMIZE:
                    return GlobalStrings.UI_SCENENAME_CUSTOMIZE;

                case UISceneType.INVENTORY:
                    return GlobalStrings.UI_SCENENAME_INVENTORY;

                case UISceneType.MINIMAP:
                    return GlobalStrings.UI_SCENENAME_MINIMAP;

                case UISceneType.ACHIEVEMENTS:
                    return GlobalStrings.UI_SCENENAME_ACHIEVEMENTS;

                case UISceneType.MAIL:
                    return GlobalStrings.UI_SCENENAME_MAIL;

                case UISceneType.ATTANDANCE:
                    return GlobalStrings.UI_SCENENAME_ATTANDANCE;

                case UISceneType.OPTIONS:
                    return GlobalStrings.UI_SCENENAME_OPTIONS;

                case UISceneType.PROFILE:
                    return GlobalStrings.UI_SCENENAME_PROFILE;

                case UISceneType.NAMETAG:
                    return GlobalStrings.UI_SCENENAME_NAMETAG;

                case UISceneType.STORE:
                    return GlobalStrings.UI_SCENENAME_STORE;

                case UISceneType.PLAYERINTERACT:
                    return GlobalStrings.UI_SCENENAME_PLAYERINTERACT;
            }
            return string.Empty;
        }

        public static UISceneType ConvertSceneNameToUISceneType(string inUISceneName)
        {
            if (inUISceneName.Equals(GlobalStrings.UI_SCENENAME_MAIN))
                return UISceneType.MAIN;
            else if (inUISceneName.Equals(GlobalStrings.UI_SCENENAME_LOGIN))
                return UISceneType.LOGIN;
            else if (inUISceneName.Equals(GlobalStrings.UI_SCENENAME_LOADING))
                return UISceneType.LOADING;
            else if (inUISceneName.Equals(GlobalStrings.UI_SCENENAME_FIELD))
                return UISceneType.FIELD;
            else if (inUISceneName.Equals(GlobalStrings.UI_SCENENAME_VOICEQUIZ))
                return UISceneType.VOICEQUIZE;
            else if (inUISceneName.Equals(GlobalStrings.UI_SCENENAME_CUSTOMIZE))
                return UISceneType.CUSTOMIZE;
            else if (inUISceneName.Equals(GlobalStrings.UI_SCENENAME_INVENTORY))
                return UISceneType.INVENTORY;
            else if (inUISceneName.Equals(GlobalStrings.UI_SCENENAME_MINIMAP))
                return UISceneType.MINIMAP;
            else if (inUISceneName.Equals(GlobalStrings.UI_SCENENAME_ACHIEVEMENTS))
                return UISceneType.ACHIEVEMENTS;
            else if (inUISceneName.Equals(GlobalStrings.UI_SCENENAME_MAIL))
                return UISceneType.MAIL;
            else if (inUISceneName.Equals(GlobalStrings.UI_SCENENAME_ATTANDANCE))
                return UISceneType.ATTANDANCE;
            else if (inUISceneName.Equals(GlobalStrings.UI_SCENENAME_OPTIONS))
                return UISceneType.OPTIONS;
            else if (inUISceneName.Equals(GlobalStrings.UI_SCENENAME_PROFILE))
                return UISceneType.PROFILE;
            else if (inUISceneName.Equals(GlobalStrings.UI_SCENENAME_NAMETAG))
                return UISceneType.NAMETAG;
            else if (inUISceneName.Equals(GlobalStrings.UI_SCENENAME_PLAYERINTERACT))
                return UISceneType.PLAYERINTERACT;
            else
                return UISceneType.NONE;
        }

        public static string ConvertSceneNumberToSceneName(SceneNumber inSceneNumber)
        {
            switch ((int)inSceneNumber)
            {
                case (int)SceneNumber.LOGINSCENE:
                    return GlobalStrings.SCENENAME_LOGIN;

                case (int)SceneNumber.LOADINGSCENE:
                    return GlobalStrings.SCENENAME_LOADING;

                case (int)SceneNumber.FIELDSCENE:
                    return GlobalStrings.SCENENAME_FIELD;

                case (int)SceneNumber.VOICEQUIZESCENE:
                    return GlobalStrings.SCENENAME_VOICEQUIZ;

                case (int)SceneNumber.INITIALQUIZSCENE:
                    return GlobalStrings.SCENENAME_INITIALQUIZ;
                case (int)SceneNumber.NPCWORLD:
                    return GlobalStrings.SCENENAME_NPCWORLD;

                default:
                    return null;
            }
        }

        private static Transform focusCamera = null; // ��Ŀ�� ī�޶�
        private static Transform camerafocusTransform = null; // ���� (��ġ/����)
        private static Transform orignCameraTransform = null;
        private static Camera mainCamera = null;
        public static bool isFocus = false;

       

        /// <summary>
        /// �ش� ������Ʈ�� ���̾ �����ϴ� �޼���
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="newLayer"></param>
        public static void ChangeLayerRecursively(GameObject obj, int newLayer)
        {
            // ���� ������Ʈ�� ���̾ �����մϴ�.
            obj.layer = newLayer;

            // �ڽ� ������Ʈ�鿡 ���� ��������� ȣ���մϴ�.
            foreach (Transform child in obj.transform)
            {
                ChangeLayerRecursively(child.gameObject, newLayer);
            }
        }

        private static GameObject npc;

        public static void SetNowTalkNPC(GameObject inNpc)
        {
            npc = inNpc;
        }

        public static GameObject GetNowTalkNPC()
        {
            return npc;
        }

        /// <summary>
        /// ������ ����� ������ ������� �߶��ִ� �޼���
        /// </summary>
        /// <param name="inClip"></param>
        /// <param name="inEndTime"></param>
        /// <param name="inDelay"></param>
        /// <returns></returns>
        public static AudioClip TrimSilence(AudioClip inClip, float inEndTime, int inDelay = 1)
        {
            float[] samples = new float[inClip.samples * inClip.channels];
            inClip.GetData(samples, 0);

            int endIndex = (int)(inEndTime * inClip.frequency) * inClip.channels;

            int i;
            for (i = endIndex - 1; i >= 0; i--)
            {
                if (Mathf.Abs(samples[i]) > 0.01f)
                {
                    break;
                }
            }

            int extraSamples = inClip.channels * inClip.frequency * inDelay;
            int newEndIndex = i + extraSamples + 1;

            Array.Resize(ref samples, newEndIndex);
            Array.Clear(samples, i + 1, extraSamples);

            float[] trimmedSamples = new float[newEndIndex];
            Array.Copy(samples, trimmedSamples, newEndIndex);

            AudioClip trimmedClip = AudioClip.Create("trimmed", trimmedSamples.Length / inClip.channels, inClip.channels, inClip.frequency, false);
            trimmedClip.SetData(trimmedSamples, 0);

            return trimmedClip;
        }

        /// <summary>
        /// �迭�� ����� �������� ���δ�.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcArray"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static T[] setSizeIntArray<T>(T[] srcArray, int size)
        {
            List<T> copied = new List<T>();

            for (int loop = 0; loop < size; loop++)
            {
                if (srcArray.Length > loop)
                {
                    copied.Add(srcArray[loop]);
                }
                else
                {
                    copied.Add(default);
                }
            }

            return copied.ToArray();
        }
    }
}

