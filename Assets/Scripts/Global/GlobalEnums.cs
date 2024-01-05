using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIManagementDemo.Global
{
    public enum GameState
    {
        NONE = -1,
        FIRST,
        LOGIN,
        LOADING,
        FIELD,
        VOICEQUIZE,
        INITIALQUIZ,
        NPCWORLD
    }

    public enum GameQuality
    {
        LOW,
        MIDDLE,
        HIGH
    }

    public enum ServerConnectionType
    {
        DEVELOPMENT,
        LIVE,
    }

    public enum UISortOrder
    {
        NONE,
        // 강제로 SortOrder를 부여하는 경우 여기서 정의하여 사용함.
        // Start

        // End
        GLOBAL = 255,     // 제일 높음
    }

    public enum PlayerType
    {
        NONE,
        NORMAL,
        GM,
    }

    public enum SocialState
    {
        NONE,
        FRIEND,
        RECVREQ,
        SENDREQ
    }

    public enum ConnectionState
    {
        OFFLINE,
        UNKNOWN,
        ONLINE,
    }

    public enum ChatUIState
    {
        STANBY,
        NORMAL,
        EXPANDED,
    }

    public enum SceneNumber
    {
        NONE = -1,
        FIRSTSCENE = 0,
        LOGINSCENE = 1,
        LOADINGSCENE = 2,
        FIELDSCENE = 3,
        VOICEQUIZESCENE = 4,
        INITIALQUIZSCENE = 5,
        NPCWORLD = 6,
    }

    public enum UISceneType
    {
        NONE,
        GLOBAL, // SceneNumber - FirstScene
        MAIN,
        LOADING,
        LOGIN,
        FIELD,
        VOICEQUIZE,
        SOCIAL,
        CUSTOMIZE,
        INVENTORY,
        MINIMAP,
        ACHIEVEMENTS,
        MAIL,
        ATTANDANCE,
        OPTIONS,
        PROFILE,
        NAMETAG,
        STORE,
        PLAYERINTERACT,
    }

    public enum UICanvasType
    {
        NONE,
        GLOBAL,
        MAIN,
        LOADING,
        LOGIN,
        FIELD,
        FADE_EFFECT,
        AI_CHATCANVAS,
        SOCIAL_CHAT,
        CREATE_CHARACTER,
        VOICE_QUIZE,
        MINIGAME_ROOM_LIST_CANVAS,
        OPTIONS,
        SOCIAL_FRIENDS,
        MAIL,
        CUSTOMIZE,
        INVENTORY,
        MINIMAP,
        ACHIEVEMENTS,
        ATTANDANCE,
        MAINPROFILE,
        PROFILE,
        NAMETAG,
        SOCIAL_CHATBUBBLE,
        STORE_CANVAS,
        PLAYERINTERACT,
    }

    public enum UIType
    {
        NONE,
        MAIN,
        LOADING,
        LOGIN,
        SAMPLE,
        FIELD,
        FADE_EFFECT,
        CUSTOMIZE,
        AI_CHAT,
        SOCIAL_CHAT_BASIC,
        SOCIAL_CHAT_EXPANDED,
        CREATE_CHARACTER,
        VOICE_QUIZE,
        MINIGAME_ROOM_LIST,
        OPTIONS,
        SOCIAL_FRIENDS,
        MAIL,
        INVENTORY,
        MINIMAP,
        ACHIEVEMENTS,
        ATTANDANCE,
        MAINPROFILE,
        PROFILE,
        SIDE_MENU,
        SOCIAL_CHATBUBBLE,
        STORE,
        PLAYERINTERACT,

        POPUP = 100,
    }

    public enum SoundType
    {
        MASTER,
        BGM,
        ABS,
        SFX,
        NPC_VOICE,
    }

    public enum UISoundType
    {
        CLICK,
        POPUP_OPEN,
        POPUP_CLOSE,
        PANEL_OPEN,
        PANEL_CLOSE,
        CAMERA_SHUTTER,
    }

    public enum LoginState
    {
        NONE,
        LOGIN_SUCCESS,
        LOGIN_FAILURE,
        CHAR_CREATE
    }

    public enum NPCUnitType
    {
        NONE_NPC,     // 일반 대화용? NPC
        AI_CHAT_NPC,  // AI 대화용 NPC
        MINIGAME_NPC, // 미니게임 관련 방 목록 출력용 NPC
    }

    public enum ShortCutType
    {
        NONE,
        STORE,

    }

    public enum MiniGameType
    {
        NONE = -1,
        VOICEQUIZ,
        INITIALQUIZ,
        MINIGAME2,
        MINIGAME3,
    }

    public enum CurrencyType
    {
        NONE = -1,
        GOLD,
        DIA,
        TICKET,
    }

    public enum ItemCategory
    {
        NONE,
        TOY,    // 1
        FOOD,   // 2
        BOX,    // 3
        EXTRA,  // 4
        PET,    // 5
        RIDE,   // 6
    }

    public enum InventoryTabCategory
    {
        NONE = -2,
        FAVORITE,
        ALL,
        TOY,    //  1
        FOOD,   //  2
        BOX,    //  3
        EXTRA,  //  4
        PET,    //  5
        RIDE,   //  6
    }

    public enum MailTabCategory
    {
        NONE,
        ALL,
        REGULAR,
        REWARD,
    }

    public enum StoreCategory
    {
        NONE,
        GOLD,
        DIA,
        CASH,
        EVENT
    }


    public enum StoreSubCategory
    {
        NONE,
        ALL,
        TOY,
        FOOD,
        BOX,
        PET,
        RIDE,
        ETC
    }

    public enum ENetworkLobby
    {
        NONE,
        FIELD,
        VOICEQUIZ,
        INITIALQUIZ,
    }
}
