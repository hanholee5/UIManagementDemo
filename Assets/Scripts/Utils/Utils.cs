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
        /// DB에서 온 정보클라에 맞게 변환
        /// </summary>
        public static string TranslateStringForClient(string inValue)
        {
            if (string.IsNullOrEmpty(inValue))
                return null;
            else
            {
                string tempstr = inValue;
                TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
                //string.ToLower = 모든 문자를 소문자로 변환
                //string.ToTitleCase 각문자의 첫 문자를 대문자로 변환하고 나머지를 소문자로 변환
                inValue = ti.ToTitleCase(tempstr.ToLower());
                return inValue;
            }
        }

        /// <summary>
        /// 클라의 정보를 DB에 맞게 변환
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

                var screenRatio = 1080f / Screen.height; // 기준해상도

                rect.x = characters[startIndex].cursorPos.x * screenRatio + anchoredPosition.x;

                var temp = anchoredPosition.y / screenRatio;
                var yRatio = (float)Math.Round(temp, MidpointRounding.AwayFromZero); // 반올림 해줘야함
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
            //제목 글자수가 inLength자 초과일때 inLength자까지 출력하고 뒤에 ...붙인다
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
        /// 세자리수마다 콤마 찍기
        /// </summary>
        /// <param name="inNumber"></param>
        /// <returns></returns>
        public static string CommaEveryThreeNumbers(int inNumber)
        {
            return string.Format("{0:#,0}", inNumber);
        }

        /// <summary>
        /// 경로에 사용하는 string으로 대/소문자 표준화
        /// 첫 문자만 대문자, 나머지 문자는 소문자 또는 숫자, '_' ex : Hair, Top, Glove
        /// </summary>
        /// <param name="inString"></param>
        /// <returns></returns>
        public static string ConvertCodeToPathString(this string inString)
        {
            return inString.Substring(0, 1).ToUpper() + inString.Substring(1, inString.Length - 1).ToLower();
        }

        /// <summary>
        /// 두 벡터 사이의 각도
        /// </summary>
        /// <param name="vStart">벡터1</param>
        /// <param name="vEnd">벡터2</param>
        /// <returns></returns>
        public static float GetAngle(Vector3 vStart, Vector3 vEnd)
        {
            Vector3 v = vEnd - vStart;

            return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg * 2;
        }

        /// <summary>
        /// 문자열을 마지막 밑줄(_)에서 분할합니다.
        /// </summary>
        /// <param name="inPartsName">분할할 입력 문자열입니다.</param>
        /// <returns>밑줄을 기준으로 분할된 문자열 배열을 반환합니다. 만약 밑줄이 없는 경우, 원래 문자열과 빈 문자열을 포함한 배열을 반환합니다.</returns>
        public static string[] SplitAtLastUnderscore(string inPartsName)
        {
            // 문자열에서 마지막 밑줄을 찾습니다.
            int lastUnderscore = inPartsName.LastIndexOf('_');

            // 만약 밑줄이 없으면, 원래 문자열과 빈 문자열을 포함한 배열을 반환합니다.
            if (lastUnderscore == -1)
                return new string[] { inPartsName, "" };

            // 그렇지 않으면, 문자열을 마지막 밑줄에서 두 부분으로 분할하고 배열로 반환합니다.
            return new string[] { inPartsName.Substring(0, lastUnderscore), inPartsName.Substring(lastUnderscore + 1) };
        }

        /// <summary>
        /// 바리에이션 번호와 슬라이스 개수에 대한 눈 UV 인덱스를 반환합니다.
        /// </summary>
        /// <param name="inVariationNumber">바리에이션 번호</param>
        /// <param name="inSliceCount">슬라이스 개수 (기본값: 4)</param>
        /// <returns>눈 UV 인덱스를 담고 있는 float 배열</returns>
        public static float[] GetEyeUVIndex(string inVariationNumber, int inSliceCount = 4)
        {
            float[] eyeUV = new float[2];
            float offset = (100 / inSliceCount) * 0.01f;
            var variationNumber = int.Parse(inVariationNumber);

            // 눈 UV 인덱스 계산
            eyeUV[0] = (variationNumber % inSliceCount) * offset;   // 가로 인덱스
            eyeUV[1] = (variationNumber / inSliceCount) * -offset;  // 세로 인덱스

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

        private static Transform focusCamera = null; // 포커스 카메라
        private static Transform camerafocusTransform = null; // 초점 (위치/방향)
        private static Transform orignCameraTransform = null;
        private static Camera mainCamera = null;
        public static bool isFocus = false;

       

        /// <summary>
        /// 해당 오브젝트의 레이어를 변경하는 메서드
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="newLayer"></param>
        public static void ChangeLayerRecursively(GameObject obj, int newLayer)
        {
            // 현재 오브젝트의 레이어를 변경합니다.
            obj.layer = newLayer;

            // 자식 오브젝트들에 대해 재귀적으로 호출합니다.
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
        /// 녹음된 오디오 파일의 빈공간을 잘라주는 메서드
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
        /// 배열의 사이즈를 동적으로 줄인다.
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

