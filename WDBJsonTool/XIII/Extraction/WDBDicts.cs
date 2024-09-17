namespace WDBJsonTool.XIII.Extraction
{
    internal class WDBDicts
    {
        public static readonly Dictionary<string, string> RecordIDs = new()
        {
            // win32
            { "auto_clip", "AutoClip" },
            { "white", "Resident" },
            { "sound_fileid_dic", "SoundFileIdDic" },
            { "sound_fileid_dic_us", "SoundFileIdDic" },
            { "sound_filename_dic", "SoundFileNameDic" },
            { "sound_filename_dic_us", "SoundFileNameDic" },
            { "treasurebox", "TreasureBox" },
            { "movie_items.win32", "movie_items" },
            { "movie_items_us.win32", "movie_items" },
            { "zonelist", "ZoneList" },
            { "monster_book", "MonsterBook" },
            { "savepoint", "savepoint" },
            { "script", "Script" },
            { "bt_chainbonus", "bt_chainbonus" },
            { "bt_chara_prop", "BattleCharaProp" },
            { "bt_constants", "BattleConstants" },
            { "item", "Item" },
            { "item_consume", "item_consume" },
            { "special_ability", "SpecialAbility" },
            { "item_weapon", "ItemWeapon" },
            { "party", "Party" },

            // ps3
            { "movie_items.ps3", "movie_items_ps3" },
            { "movie_items_us.ps3", "movie_items_ps3" },

            // x360
            { "movie_items.x360", "movie_items" },
            { "movie_items_us.x360", "movie_items" },

            // zone
            { "z000", "Zone" },
            { "z001", "Zone" },
            { "z002", "Zone" },
            { "z003", "Zone" },
            { "z004", "Zone" },
            { "z005", "Zone" },
            { "z006", "Zone" },
            { "z007", "Zone" },
            { "z008", "Zone" },
            { "z009", "Zone" },
            { "z010", "Zone" },
            { "z015", "Zone" },
            { "z016", "Zone" },
            { "z017", "Zone" },
            { "z018", "Zone" },
            { "z019", "Zone" },
            { "z020", "Zone" },
            { "z021", "Zone" },
            { "z022", "Zone" },
            { "z023", "Zone" },
            { "z024", "Zone" },
            { "z025", "Zone" },
            { "z026", "Zone" },
            { "z027", "Zone" },
            { "z028", "Zone" },
            { "z029", "Zone" },
            { "z030", "Zone" },
            { "z031", "Zone" },
            { "z032", "Zone" },
            { "z033", "Zone" },
            { "z034", "Zone" },
            { "z035", "Zone" },
            { "z100", "Zone" },
            { "z101", "Zone" },
            { "z102", "Zone" },
            { "z103", "Zone" },
            { "z104", "Zone" },
            { "z105", "Zone" },
            { "z106", "Zone" },
            { "z107", "Zone" },
            { "z111", "Zone" },
            { "z200", "Zone" },
            { "z201", "Zone" },
            { "z202", "Zone" },
            { "z203", "Zone" },
            { "z204", "Zone" },
            { "z205", "Zone" },
            { "z206", "Zone" },
            { "z207", "Zone" },
            { "z208", "Zone" },
            { "z209", "Zone" },
            { "z210", "Zone" },
            { "z255", "Zone" },

            // db/script
            { "script00001", "Script" },
            { "script00002", "Script" },
            { "script00003", "Script" },
            { "script00004", "Script" },
            { "script00006", "Script" },
            { "script00008", "Script" },
            { "script00010", "Script" },
            { "script00015", "Script" },
            { "script00016", "Script" },
            { "script00017", "Script" },
            { "script00018", "Script" },
            { "script00019", "Script" },
            { "script00020", "Script" },
            { "script00021", "Script" },
            { "script00022", "Script" },
            { "script00023", "Script" },
            { "script00024", "Script" },
            { "script00026", "Script" },
            { "script00027", "Script" },
            { "script00029", "Script" },
            { "script00030", "Script" },
            { "script00105", "Script" },
            { "script00106", "Script" }
        };


        public static readonly Dictionary<string, List<string>> FieldNames = new()
        {
            { "AutoClip",
                new List<string>()
                {
                    "sTitle", "sTarget", "sTarget2", "sText", "sPicture", "u4Category", "u7Sort",
                    "u4Chapter"
                }
            },

            { "Resident",
                new List<string>()
                {
                    "fVal", "iVal1", "sResourceName", "fPosX", "fPosY", "fPosZ"
                }
            },

            { "SoundFileIdDic",
                new List<string>()
                {
                    "i31FileId", "u1IsStream"
                }
            },

            { "SoundFileNameDic",
                new List<string>()
                {
                    "sResourceName"
                }
            },

            { "TreasureBox",
                new List<string>()
                {
                    "sItemResourceId", "iItemCount", "sNextTreasureBoxResourceId"
                }
            },

            { "movie_items",
                new List<string>()
                {
                    "sZoneNumber", "uCinemaSize", "uReserved", "uCinemaStart"
                }
            },

            { "movie_items_ps3",
                new List<string>()
                {
                    "sZoneNumber", "uCinemaSize", "u64CinemaStart"
                }
            },

            { "ZoneList",
                new List<string>()
                {
                    "fMovieTotalTimeSec", "iImageSize", "u8RefZoneNum0", "u8RefZoneNum1", "u8RefZoneNum2",
                    "u8RefZoneNum3", "u8RefZoneNum4", "u8RefZoneNum5", "u8RefZoneNum6", "u8RefZoneNum7",
                    "u8RefZoneNum8", "u8RefZoneNum9", "u8RefZoneNum10", "u1OnDisk0", "u1OnDisk1", "u1OnDisk2",
                    "u1OnDisk3", "u1On1stLayerPS3", "u1On2ndtLayerPS3"
                }
            },

            { "MonsterBook",
                new List<string>()
                {
                    "u6MbookId", "u9SortId", "u9PictureId", "u1Unk"
                }
            },

            { "Zone",
                new List<string>()
                {
                    "iBaseNum", "sName0", "sName1"
                }
            },

            { "savepoint",
                new List<string>()
                {
                    "sLoadScriptId", "i17PartyPositionMarkerGroupIndex",
                    "u15SaveIconBackgroundImageIndex", "i16SaveIconOverrideImageIndex"
                }
            },

            { "Script",
                new List<string>()
                {
                    "sClassName", "sMethodName", "iAdditionalArgCount", "iAdditionalArg0", "iAdditionalArg1",
                    "iAdditionalArg2", "iAdditionalArg3", "iAdditionalStringArgCount", "sAdditionalStringArg0",
                    "sAdditionalStringArg1", "sAdditionalStringArg2"
                }
            },

            { "bt_chainbonus", 
                new List<string>()
                {
                    "u6WhoFrom", "u6When0", "u6When1", "u6When2", "u6WhatState", "u6WhoTo", "u6DoWhat", 
                    "u6Where", "u6How", "u16Bonus"
                }
            },

            { "BattleCharaProp", 
                new List<string>()
                {
                    "sInfoStrId", "sOpenCondArgS0", "u1NoLibra", "u8OpenCond", "u8AiOrderEn", "u8AiOrderJm",
                    "u4FlavorAtk", "u4FlavorBla", "u4FlavorDef"
                } 
            },

            { "BattleConstants",
                new List<string>()
                {
                    "iiVal", "ffVal", "ssVal"
                }
            },

            { "Item", 
                new List<string>()
                {
                    "sItemNameStringId", "sHelpStringId", "sScriptId", "uPurchasePrice", "uSellPrice",
                    "u8MenuIcon", "u8ItemCategory", "i16ScriptArg0", "i16ScriptArg1", "u1IsUseBattleMenu", 
                    "u1IsUseMenu", "u1IsDisposable", "u1IsSellable", "u5Rank", "u6Genre", "u1IsIgnoreGenre",
                    "u16SortAllByKCategory", "u16SortCategoryByCategory", "u16Experience", "i16Mulitplier", 
                    "u1IsUseItemChange"
                }
            },

            { "item_consume", 
                new List<string>()
                {
                    "sAbilityId", "sLearnAbilityId", "u1IsUseRemodel", "u1IsUseGrow", "u16ConsumeAP"
                }
            },

            { "SpecialAbility", 
                new List<string>()
                {
                    "sAbility", "u6Genre", "u3Count"
                }
            },

            // partial
            { "ItemWeapon",
                new List<string>()
                {
                    "sWeaponCharaSpecId", "sWeaponCharaSpecId2", "sAbility", "sAbility2", "sAbility3",
                    "sUpgradeAbility", "sAbilityHelpStringId", "uBuyPriceIncrement", "uSellPriceIncrement",
                    "sDisasItem1", "sDisasItem2", "sDisasItem3", "sDisasItem4", "sDisasItem5", "u8UnkVal1",
                    "u8UnkVal2", "u2UnkVal3", "u7MaxLvl", "u4UnkVal4", "u1UnkBool1", "u1UnkBool2", "u1UnkBool3",
                    "i10ExpRate1", "i10ExpRate2", "i10ExpRate3", "u1UnkBool4", "u1UnkBool5", "u8StatusModKind0",
                    "u8StatusModKind1", "u4StatusModType", "u1UnkBool6", "u1UnkBool7", "u16UnkVal5",
                    "i16StatusModVal", "u16UnkVal6", "i16AttackModVal", "u16UnkVal7", "i16MagicModVal",
                    "i16AtbModVal", "u16UnkVal8", "u16UnkVal9", "u16UnkVal10", "u14DisasRate1", "u7UnkVal11",
                    "u7UnkVal12", "u14DisasRate2", "u14DisasRate3", "u7UnkVal13", "u14DisasRate4",
                    "u7UnkVal14", "u14DisasRate5"
                }
            },

            { "Party",
                new List<string>()
                {
                    "sCharaSpecId", "sSubCharaSpecId0", "sSubCharaSpecId1", "sSubCharaSpecId2",
                    "sSubCharaSpecId3", "sSubCharaSpecId4", "sSubCharaSpecId5", "sSubCharaSpecId6",
                    "sSubCharaSpecId7", "sSubCharaSpecId8", "sRideObjectCharaSpecId0",
                    "sRideObjectCharaSpecId1", "sFieldFreeCameraSettingResourceId", "sIconResourceId",
                    "sScriptIdOnPartyCharaAIStarted", "sScriptIdOnIdle", "sBattleCharaSpecId", "sSummonId",
                    "fStopDistance", "fWalkDistance", "fPlayerRestraint", "u1IsEnableUserControl",
                    "u5OrderNumForCrest", "u8OrderNumForTool", "u7Expresspower", "u7Willpower",
                    "u7Brightness", "u7Cognition"
                }
            }
        };
    }
}