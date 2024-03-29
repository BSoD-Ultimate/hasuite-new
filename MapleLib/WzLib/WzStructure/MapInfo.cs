﻿/*  MapleLib - A general-purpose MapleStory library
 * Copyright (C) 2009, 2010 Snow and haha01haha01
   
 * This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

 * This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

 * You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
//using HaCreator.Helpers;
using MapleLib.WzLib.WzStructure.Data;
using MapleLib.WzLib.WzStructure;
using System.Drawing;
using MapleLib.Helpers;

namespace MapleLib.WzLib.WzStructure
{
    public class MapInfo //Credits to Bui
    {


        public static MapInfo Default = new MapInfo();
        public Dictionary<string, int> props = new Dictionary<string,int>();

        public MapInfo()
        {
        }

        public void loadIntProps(WzImage image)
        {
            foreach (IWzImageProperty prop in image["info"].WzProperties)
            {
                if (prop is WzCompressedIntProperty)
                    props.Add(prop.Name, InfoTool.GetInt(prop));
            }
        }

        public MapInfo(WzImage image, string strMapName, string strStreetName)
        {
            int? startHour;
            int? endHour;
            this.strMapName = strMapName;
            this.strStreetName = strStreetName;
            this.mapImage = image;
            foreach (IWzImageProperty prop in image["info"].WzProperties) switch (prop.Name)
                {
                    case "bgm":
                        bgm = InfoTool.GetString(prop);
                        break;
                    case "cloud":
                        cloud = (MapleBool)((WzCompressedIntProperty)prop).Value;
                        break;
                    case "swim":
                        swim = (MapleBool)((WzCompressedIntProperty)prop).Value;
                        break;
                    case "forcedReturn":
                        forcedReturn = InfoTool.GetInt(prop);
                        break;
                    case "hideMinimap":
                        hideMinimap = (MapleBool)((WzCompressedIntProperty)prop).Value;
                        break;
                    case "mapDesc":
                        mapDesc = InfoTool.GetString(prop);
                        break;
                    case "mapMark":
                        MapMark = InfoTool.GetString(prop);
                        break;
                    case "mobRate":
                        mobRate = InfoTool.GetFloat(prop);
                        break;
                    case "moveLimit":
                        moveLimit = InfoTool.GetInt(prop);
                        break;
                    case "returnMap":
                        returnMap = InfoTool.GetInt(prop);
                        break;
                    case "town":
                        town = (MapleBool)((WzCompressedIntProperty)prop).Value;
                        break;
                    case "version":
                        version = InfoTool.GetInt(prop);
                        break;
                    case "fieldLimit":
                        int fl = InfoTool.GetInt(prop);
                        if (fl >= (int)Math.Pow(2, 23))
                            fl = fl & ((int)Math.Pow(2, 23) - 1);
                        fieldLimit = (FieldLimit)fl;
                        break;
                    case "VRTop":
                    case "VRBottom":
                    case "VRLeft":
                    case "VRRight":
                        break;
                    case "link":
                        //link = InfoTool.GetInt(prop);
                        break;
                    case "timeLimit":
                        timeLimit = InfoTool.GetInt(prop);
                        break;
                    case "lvLimit":
                        lvLimit = InfoTool.GetInt(prop);
                        break;
                    case "onFirstUserEnter":
                        onFirstUserEnter = InfoTool.GetString(prop);
                        break;
                    case "onUserEnter":
                        onUserEnter = InfoTool.GetString(prop);
                        break;
                    case "fly":
                        fly = (MapleBool)((WzCompressedIntProperty)prop).Value;
                        break;
                    case "noMapCmd":
                        noMapCmd = (MapleBool)((WzCompressedIntProperty)prop).Value;
                        break;
                    case "partyOnly":
                        partyOnly = (MapleBool)((WzCompressedIntProperty)prop).Value;
                        break;
                    case "fieldType":
                        int ft = InfoTool.GetInt(prop);
                        if (!Enum.IsDefined(typeof(FieldType), ft))
                            ft = 0;
                        fieldType = (FieldType)ft;
                        break;
                    case "miniMapOnOff":
                        miniMapOnOff = (MapleBool)((WzCompressedIntProperty)prop).Value;
                        break;
                    case "reactorShuffle":
                        reactorShuffle = (MapleBool)((WzCompressedIntProperty)prop).Value;
                        break;
                    case "reactorShuffleName":
                        reactorShuffleName = InfoTool.GetString(prop);
                        break;
                    case "personalShop":
                        personalShop = (MapleBool)((WzCompressedIntProperty)prop).Value;
                        break;
                    case "entrustedShop":
                        entrustedShop = (MapleBool)((WzCompressedIntProperty)prop).Value;
                        break;
                    case "effect":
                        effect = InfoTool.GetString(prop);
                        break;
                    case "lvForceMove":
                        lvForceMove = InfoTool.GetInt(prop);
                        break;
                    case "timeMob":
                        startHour = InfoTool.GetOptionalInt(prop["startHour"]);
                        endHour = InfoTool.GetOptionalInt(prop["endHour"]);
                        int? id = InfoTool.GetOptionalInt(prop["id"]);
                        string message = InfoTool.GetOptionalString(prop["message"]);
                        if (id == null || message == null || (startHour == null ^ endHour == null))
                        {
                            //System.Windows.Forms.MessageBox.Show("Warning", "Warning - incorrect timeMob structure in map data. Skipped and error log was saved.");
                            WzFile file = (WzFile)image.WzFileParent;
                            if (file != null)
                                ErrorLogger.Log(ErrorLevel.IncorrectStructure, "timeMob, map " + image.Name + " of version " + Enum.GetName(typeof(WzMapleVersion), file.MapleVersion) + ", v" + file.Version.ToString());
                        }
                        else
                            timeMob = new TimeMob((int?)startHour, (int?)endHour, (int)id, message);
                        break;
                    case "help":
                        help = InfoTool.GetString(prop);
                        break;
                    case "snow":
                        snow = (MapleBool)((WzCompressedIntProperty)prop).Value;
                        break;
                    case "rain":
                        rain = (MapleBool)((WzCompressedIntProperty)prop).Value;
                        break;
                    case "dropExpire":
                        dropExpire = InfoTool.GetInt(prop);
                        break;
                    case "decHP":
                        decHP = InfoTool.GetInt(prop);
                        break;
                    case "decInterval":
                        decInterval = InfoTool.GetInt(prop);
                        break;
                    case "autoLieDetector":
                        startHour = InfoTool.GetOptionalInt(prop["startHour"]);
                        endHour = InfoTool.GetOptionalInt(prop["endHour"]);
                        int? interval = InfoTool.GetOptionalInt(prop["interval"]);
                        int? propInt = InfoTool.GetOptionalInt(prop["prop"]);
                        if (startHour == null || endHour == null || interval == null || propInt == null)
                        {
                            //System.Windows.Forms.MessageBox.Show("Warning", "Warning - incorrect autoLieDetector structure in map data. Skipped and error log was saved.");
                            WzFile file = (WzFile)image.WzFileParent;
                            if (file != null)
                                ErrorLogger.Log(ErrorLevel.IncorrectStructure, "autoLieDetector, map " + image.Name + " of version " + Enum.GetName(typeof(WzMapleVersion), file.MapleVersion) + ", v" + file.Version.ToString());
                        }
                        else
                            autoLieDetector = new AutoLieDetector((int)startHour, (int)endHour, (int)interval, (int)propInt);
                        break;
                    case "expeditionOnly":
                        expeditionOnly = (MapleBool)((WzCompressedIntProperty)prop).Value;
                        break;
                    case "fs":
                        fs = InfoTool.GetFloat(prop);
                        break;
                    case "protectItem":
                        protectItem = InfoTool.GetInt(prop);
                        break;
                    case "createMobInterval":
                        createMobInterval = InfoTool.GetInt(prop);
                        break;
                    case "fixedMobCapacity":
                        fixedMobCapacity = InfoTool.GetInt(prop);
                        break;
                    case "streetName":
                        streetName = InfoTool.GetString(prop);
                        break;
                    case "noRegenMap":
                        noRegenMap = (MapleBool)((WzCompressedIntProperty)prop).Value;
                        break;
                    case "allowedItems":
                        allowedItems = new List<int>();
                        if (prop.WzProperties != null && prop.WzProperties.Count > 0)
                            foreach (IWzImageProperty item in prop.WzProperties)
                                allowedItems.Add((int)item);
                        break;
                    case "recovery":
                        recovery = InfoTool.GetFloat(prop);
                        break;
                    case "blockPBossChange":
                        blockPBossChange = (MapleBool)((WzCompressedIntProperty)prop).Value;
                        break;
                    case "everlast":
                        everlast = (MapleBool)((WzCompressedIntProperty)prop).Value;
                        break;
                    case "damageCheckFree":
                        damageCheckFree = (MapleBool)((WzCompressedIntProperty)prop).Value;
                        break;
                    case "dropRate":
                        dropRate = InfoTool.GetFloat(prop);
                        break;
                    case "scrollDisable":
                        scrollDisable = (MapleBool)((WzCompressedIntProperty)prop).Value;
                        break;
                    case "needSkillForFly":
                        needSkillForFly = (MapleBool)((WzCompressedIntProperty)prop).Value;
                        break;
                    case "zakum2Hack":
                        zakum2Hack = (MapleBool)((WzCompressedIntProperty)prop).Value;
                        break;
                    case "allMoveCheck":
                        allMoveCheck = (MapleBool)((WzCompressedIntProperty)prop).Value;
                        break;
                    case "VRLimit":
                        VRLimit = (MapleBool)((WzCompressedIntProperty)prop).Value;
                        break;
                    case "consumeItemCoolTime":
                        consumeItemCoolTime = (MapleBool)((WzCompressedIntProperty)prop).Value;
                        break;
                    default:
                        additionalProps.Add(prop);
                        break;
                }
            if (image["info"]["VRLeft"] != null)
            {
                IWzImageProperty info = image["info"];
                int left = InfoTool.GetInt(info["VRLeft"]);
                int right = InfoTool.GetInt(info["VRRight"]);
                int top = InfoTool.GetInt(info["VRTop"]);
                int bottom = InfoTool.GetInt(info["VRBottom"]);
                VR = new Rectangle(left, top, right - left, bottom - top);
            }
        }

        public void SaveMapInfo()
        {
            foreach (IWzImageProperty prop in mapImage["info"].WzProperties) switch (prop.Name)
                {
                    case "bgm":
                        ((WzStringProperty)prop).Value = bgm;
                        break;
                    case "cloud":
                        ((WzCompressedIntProperty)prop).Value = cloud ? 2 : 1;
                        break;
                    case "swim":
                        ((WzCompressedIntProperty)prop).Value = swim ? 2 : 1;
                        break;
                    case "forcedReturn":
                        ((WzCompressedIntProperty)prop).Value = forcedReturn;
                        break;
                    case "hideMinimap":
                        ((WzCompressedIntProperty)prop).Value = hideMinimap ? 2 : 1;
                        break;
                    case "mapDesc":
                        ((WzStringProperty)prop).Value = mapDesc;
                        break;
                    case "mapMark":
                        ((WzStringProperty)prop).Value = MapMark;
                        break;
                    case "mobRate":
                        ((WzByteFloatProperty)prop).Value = mobRate;
                        break;
                    case "moveLimit":
                        ((WzCompressedIntProperty)prop).Value = (int)moveLimit;
                        break;
                    case "returnMap":
                        ((WzCompressedIntProperty)prop).Value = returnMap;
                        break;
                    case "town":
                        ((WzCompressedIntProperty)prop).Value = town ? 2 : 1;
                        break;
                    case "version":
                        ((WzCompressedIntProperty)prop).Value = version;
                        break;
                    case "fieldLimit":
                        /*int fl = InfoTool.GetInt(prop);
                        if (fl >= (int)Math.Pow(2, 23))
                            fl = fl & ((int)Math.Pow(2, 23) - 1);
                        fieldLimit = (FieldLimit)fl;
                        
                         Sorry, we're just gonna go without fieldLimit. -DeathRight*/
                        break;
                    case "VRTop":
                    case "VRBottom":
                    case "VRLeft":
                    case "VRRight":
                        break;
                    case "link":
                        //link = InfoTool.GetInt(prop);
                        break;
                    case "timeLimit":
                        ((WzCompressedIntProperty)prop).Value = (int)timeLimit;
                        break;
                    case "lvLimit":
                        ((WzCompressedIntProperty)prop).Value = (int)lvLimit;
                        break;
                    case "onFirstUserEnter":
                        ((WzStringProperty)prop).Value = onFirstUserEnter;
                        break;
                    case "onUserEnter":
                        ((WzStringProperty)prop).Value = onUserEnter;
                        break;
                    case "fly":
                        ((WzCompressedIntProperty)prop).Value = fly ? 2 : 1;
                        break;
                    case "noMapCmd":
                        ((WzCompressedIntProperty)prop).Value = noMapCmd ? 2 : 1;
                        break;
                    case "partyOnly":
                        ((WzCompressedIntProperty)prop).Value = partyOnly ? 2 : 1;
                        break;
                    case "fieldType":
                        int ft = (int)fieldType;
                        if (!Enum.IsDefined(typeof(FieldType), ft))
                            ft = 0;
                        ((WzCompressedIntProperty)prop).Value = ft;
                        break;
                    case "miniMapOnOff":
                        ((WzCompressedIntProperty)prop).Value = miniMapOnOff ? 2 : 1;
                        break;
                    case "reactorShuffle":
                        ((WzCompressedIntProperty)prop).Value = reactorShuffle ? 2 : 1;
                        break;
                    case "reactorShuffleName":
                        ((WzStringProperty)prop).Value = reactorShuffleName;
                        break;
                    case "personalShop":
                        ((WzCompressedIntProperty)prop).Value = personalShop ? 2 : 1;
                        break;
                    case "entrustedShop":
                        ((WzCompressedIntProperty)prop).Value = entrustedShop ? 2 : 1;
                        break;
                    case "effect":
                        ((WzStringProperty)prop).Value = effect;
                        break;
                    case "lvForceMove":
                        ((WzCompressedIntProperty)prop).Value = (int)lvForceMove;
                        break;
                    case "timeMob":
                    //TODO: How the fuck am I supposed to convert this shit?! -DeathRight
                         
                        /*
                        startHour = InfoTool.GetOptionalInt(prop["startHour"]);
                        endHour = InfoTool.GetOptionalInt(prop["endHour"]);
                        int? id = InfoTool.GetOptionalInt(prop["id"]);
                        string message = InfoTool.GetOptionalString(prop["message"]);
                        if (id == null || message == null || (startHour == null ^ endHour == null))
                        {
                            //System.Windows.Forms.MessageBox.Show("Warning", "Warning - incorrect timeMob structure in map data. Skipped and error log was saved.");
                            WzFile file = (WzFile)mapImage.WzFileParent;
                            if (file != null)
                                ErrorLogger.Log(ErrorLevel.IncorrectStructure, "timeMob, map " + image.Name + " of version " + Enum.GetName(typeof(WzMapleVersion), file.MapleVersion) + ", v" + file.Version.ToString());
                        }
                        else
                            timeMob = new TimeMob((int?)startHour, (int?)endHour, (int)id, message);
                         */
                        break;
                    case "help":
                        ((WzStringProperty)prop).Value = help;
                        break;
                    case "snow":
                        ((WzCompressedIntProperty)prop).Value = snow ? 2 : 1;
                        break;
                    case "rain":
                        ((WzCompressedIntProperty)prop).Value = rain ? 2 : 1;
                        break;
                    case "dropExpire":
                        ((WzCompressedIntProperty)prop).Value = (int)dropExpire;
                        break;
                    case "decHP":
                        ((WzCompressedIntProperty)prop).Value = (int)decHP;
                        break;
                    case "decInterval":
                        ((WzCompressedIntProperty)prop).Value = (int)decInterval;
                        break;
                    case "autoLieDetector":
                        //TODO: Again with this shit?! -DeathRight
                        
                        /*
                        startHour = InfoTool.GetOptionalInt(prop["startHour"]);
                        endHour = InfoTool.GetOptionalInt(prop["endHour"]);
                        int? interval = InfoTool.GetOptionalInt(prop["interval"]);
                        int? propInt = InfoTool.GetOptionalInt(prop["prop"]);
                        if (startHour == null || endHour == null || interval == null || propInt == null)
                        {
                            //System.Windows.Forms.MessageBox.Show("Warning", "Warning - incorrect autoLieDetector structure in map data. Skipped and error log was saved.");
                            WzFile file = (WzFile)image.WzFileParent;
                            if (file != null)
                                ErrorLogger.Log(ErrorLevel.IncorrectStructure, "autoLieDetector, map " + image.Name + " of version " + Enum.GetName(typeof(WzMapleVersion), file.MapleVersion) + ", v" + file.Version.ToString());
                        }
                        else
                            autoLieDetector = new AutoLieDetector((int)startHour, (int)endHour, (int)interval, (int)propInt);
                        */
                        break;
                    case "expeditionOnly":
                        ((WzCompressedIntProperty)prop).Value = expeditionOnly ? 2 : 1;
                        break;
                    case "fs":
                        ((WzByteFloatProperty)prop).Value = (float)fs;
                        break;
                    case "protectItem":
                        ((WzCompressedIntProperty)prop).Value = (int)protectItem;
                        break;
                    case "createMobInterval":
                        ((WzCompressedIntProperty)prop).Value = (int)createMobInterval;
                        break;
                    case "fixedMobCapacity":
                        ((WzCompressedIntProperty)prop).Value = (int)fixedMobCapacity;
                        break;
                    case "streetName":
                        ((WzStringProperty)prop).Value = streetName;
                        break;
                    case "noRegenMap":
                        ((WzCompressedIntProperty)prop).Value = noRegenMap ? 2 : 1;
                        break;
                    case "allowedItems":
                        //TODO: Convert this. -DeathRight

                        /*allowedItems = new List<int>();
                        if (prop.WzProperties != null && prop.WzProperties.Count > 0)
                            foreach (IWzImageProperty item in prop.WzProperties)
                                allowedItems.Add((int)item);*/
                        break;
                    case "recovery":
                        ((WzByteFloatProperty)prop).Value = (float)recovery;
                        break;
                    case "blockPBossChange":
                        ((WzCompressedIntProperty)prop).Value = blockPBossChange ? 2 : 1;
                        break;
                    case "everlast":
                        ((WzCompressedIntProperty)prop).Value = everlast ? 2 : 1;
                        break;
                    case "damageCheckFree":
                        ((WzCompressedIntProperty)prop).Value = damageCheckFree ? 2 : 1;
                        break;
                    case "dropRate":
                        ((WzByteFloatProperty)prop).Value = (float)dropRate;
                        break;
                    case "scrollDisable":
                        ((WzCompressedIntProperty)prop).Value = scrollDisable ? 2 : 1;
                        break;
                    case "needSkillForFly":
                        ((WzCompressedIntProperty)prop).Value = needSkillForFly ? 2 : 1;
                        break;
                    case "zakum2Hack":
                        ((WzCompressedIntProperty)prop).Value = zakum2Hack ? 2 : 1;
                        break;
                    case "allMoveCheck":
                        ((WzCompressedIntProperty)prop).Value = allMoveCheck ? 2 : 1;
                        break;
                    case "VRLimit":
                        ((WzCompressedIntProperty)prop).Value = VRLimit ? 2 : 1;
                        break;
                    case "consumeItemCoolTime":
                        consumeItemCoolTime = InfoTool.GetBool(prop);
                        break;
                    default:
                        //additionalProps.Add(prop);
                        break;
                }
            /*if (image["info"]["VRLeft"] != null)
            {
                IWzImageProperty info = image["info"];
                int left = InfoTool.GetInt(info["VRLeft"]);
                int right = InfoTool.GetInt(info["VRRight"]);
                int top = InfoTool.GetInt(info["VRTop"]);
                int bottom = InfoTool.GetInt(info["VRBottom"]);
                VR = new Rectangle(left, top, right - left, bottom - top);
            }*/
            mapImage.Changed = true;
        }


        //Cannot change
        public int version = 10;

        //Must have
        public string bgm = "Bgm00/GoPicnic";
        public string MapMark = "None";
        public FieldLimit fieldLimit = FieldLimit.FIELDOPT_NONE;
        public int returnMap = 999999999;
        public int forcedReturn = 999999999;
        public bool cloud = false;
        public bool swim = false;
        public bool hideMinimap = false;
        public bool town = false;
        public float mobRate = 1.5f;

        //Optional
        //public int link = -1;
        public int? timeLimit = null;
        public int? lvLimit = null;
        public FieldType? fieldType = null;
        public string onFirstUserEnter = null;
        public string onUserEnter = null;
        public MapleBool fly = null;
        public MapleBool noMapCmd = null;
        public MapleBool partyOnly = null;
        public MapleBool reactorShuffle = null;
        public string reactorShuffleName = null;
        public MapleBool personalShop = null;
        public MapleBool entrustedShop = null;
        public string effect = null; //Bubbling; 610030550 and many others
        public int? lvForceMove = null; //limit FROM value
        public TimeMob? timeMob = null;
        public string help = null; //help string
        public MapleBool snow = null;
        public MapleBool rain = null;
        public int? dropExpire = null; //in seconds
        public int? decHP = null;
        public int? decInterval = null;
        public AutoLieDetector? autoLieDetector = null;
        public MapleBool expeditionOnly = null;
        public float? fs = null; //slip on ice speed, default 0.2
        public int? protectItem = null; //ID, item protecting from cold
        public int? createMobInterval = null; //used for massacre pqs
        public int? fixedMobCapacity = null; //mob capacity to target (used for massacre pqs)

        //Unknown optional
        public int? moveLimit = null;
        public string mapDesc = null;
        public string mapName = null;
        public string streetName = null;
        public MapleBool miniMapOnOff = null;
        public MapleBool noRegenMap = null; //610030400
        public List<int> allowedItems = null;
        public float? recovery = null; //recovery rate, like in sauna (3)
        public MapleBool blockPBossChange = null; //something with monster carnival
        public MapleBool everlast = null; //something with bonus stages of PQs
        public MapleBool damageCheckFree = null; //something with fishing event
        public float? dropRate = null;
        public MapleBool scrollDisable = null;
        public MapleBool needSkillForFly = null;
        public MapleBool zakum2Hack = null; //JQ hack protection
        public MapleBool allMoveCheck = null; //another JQ hack protection
        public MapleBool VRLimit = null; //use vr's as limits?
        public MapleBool consumeItemCoolTime = null; //cool time of consume item

        //Special
        public Rectangle? VR = null;
        public List<IWzImageProperty> additionalProps = new List<IWzImageProperty>();
        public string strMapName = "<Untitled>";
        public string strStreetName = "<Untitled>";
        public WzImage mapImage;
        public int id = 0;

        //Editor related, not actual properties
        public MapType mapType = MapType.RegularMap;

        public struct TimeMob
        {
            public int? startHour, endHour;
            public int id;
            public string message;

            public TimeMob(int? startHour, int? endHour, int id, string message)
            {
                this.startHour = startHour;
                this.endHour = endHour;
                this.id = id;
                this.message = message;
            }
        }

        public struct AutoLieDetector
        {
            public int startHour, endHour, interval, prop; //interval in mins, prop default = 80

            public AutoLieDetector(int startHour, int endHour, int interval, int prop)
            {
                this.startHour = startHour;
                this.endHour = endHour;
                this.interval = interval;
                this.prop = prop;
            }
        }
    }
}