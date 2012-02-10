﻿/*  HaCreator - MapleStory Map Editor
 * Copyright (C) 2009, 2010  haha01haha01
   
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using DevComponents.Editors;
using HaCreator.MapEditor;
using MapleLib.WzLib.WzStructure;
using MapleLib.WzLib.WzStructure.Data;

namespace HaCreator
{
    public partial class InfoEditor : Office2007Form
    {
        public MapInfo info;

        public InfoEditor(MapInfo info)
        {
            InitializeComponent();
            styleManager.ManagerStyle = UserSettings.applicationStyle;
            timeLimitEnable.Tag = timeLimit;
            lvLimitEnable.Tag = lvLimit;
            lvForceMoveUse.Tag = lvForceMove;
            firstUserEnable.Tag = firstUserEnter;
            userEnterEnable.Tag = userEnter;
            fieldTypeEnable.Tag = fieldType;
            moveLimitEnable.Tag = moveLimit;
            mapNameEnable.Tag = mapName;
            mapDescEnable.Tag = mapDesc;
            streetNameEnable.Tag = streetNameBox;
            effectEnable.Tag = effectBox;
            dropExpireEnable.Tag = dropExpire;
            dropRateEnable.Tag = dropRate;
            recoveryEnable.Tag = recovery;
            reactorShuffle.Tag = new Control[] { reactorNameShuffle, reactorNameBox };
            reactorNameShuffle.Tag = reactorNameBox;
            fsEnable.Tag = fsBox;
            massEnable.Tag = new Control[] { createMobInterval, fixedMobCapacity };
            hpDecEnable.Tag = new Control[] { decHP, protectItem, decIntervalEnable, decInterval, protectEnable };
            protectEnable.Tag = new Control[] { protectItem };
            decIntervalEnable.Tag = decInterval;
            helpEnable.Tag = helpBox;
            timedMobEnable.Tag = new Control[] { timedMobEnd, timedMobStart };
            summonMobEnable.Tag = new Control[] { timedMobId, timedMobEnable, timedMobMessage };
            autoLieDetectorEnable.Tag = new Control[] { autoLieEnd, autoLieInterval, autoLieProp, autoLieStart };
            allowedItemsEnable.Tag = new Control[] { allowedItems, allowedItemsAdd, allowedItemsRemove };

            this.info = info;
            this.fieldType.SelectedIndex = 0;

            if (info.mapType != MapType.CashShopPreview)
            {
                List<string> sortedBGMs = new List<string>();
                foreach (DictionaryEntry bgm in Program.InfoManager.BGMs)
                    sortedBGMs.Add((string)bgm.Key);
                sortedBGMs.Sort();
                foreach (string bgm in sortedBGMs)
                    bgmBox.Items.Add(bgm);
                bgmBox.SelectedItem = info.bgm;

                List<string> sortedMarks = new List<string>();
                foreach (DictionaryEntry mark in Program.InfoManager.MapMarks)
                    sortedMarks.Add((string)mark.Key);
                sortedMarks.Sort();
                foreach (string mark in sortedMarks)
                    markBox.Items.Add(mark);
                markBox.SelectedIndex = 0;
            }
            switch (info.mapType)
            {
                case MapType.CashShopPreview:
                    IDLabel.Text = "CashShopPreview";
                    nameBox.Text = "CashShopPreview";
                    streetBox.Text = "CashShopPreview";
                    //                    idBox.Enabled = false;
                    nameBox.Enabled = false;
                    streetBox.Enabled = false;
                    bgmBox.Enabled = false;
                    soundPlayer.Enabled = false;
                    markBox.Enabled = false;
                    markImage.Image = new Bitmap(1, 1);
                    break;
                case MapType.MapLogin:
                    IDLabel.Text = "MapLogin";
                    nameBox.Text = "MapLogin";
                    streetBox.Text = "MapLogin";
                    //                    idBox.Enabled = false;
                    nameBox.Enabled = false;
                    streetBox.Enabled = false;
                    break;
                case MapType.RegularMap:
                    if (info.id == -1) IDLabel.Text = "";
                    else IDLabel.Text = info.id.ToString();
                    //LoadOptionalInt(info.id, IDLabel);
                    nameBox.Text = info.strMapName;
                    streetBox.Text = info.strStreetName;
                    break;
            }
            if (info.mapType != MapType.CashShopPreview) markBox.SelectedItem = info.MapMark;
            if (info.returnMap == info.id) cannotReturnCBX.Checked = true;
            else returnBox.Text = info.returnMap.ToString();
            if (info.forcedReturn == 999999999) returnHereCBX.Checked = true;
            else forcedRet.Text = info.forcedReturn.ToString();
            mobRate.Value = info.mobRate;
            //LoadOptionalInt(info.link, linkBox);
            LoadOptionalInt(info.timeLimit, timeLimit, timeLimitEnable);
            LoadOptionalInt(info.lvLimit, lvLimit, lvLimitEnable);
            LoadOptionalInt(info.lvForceMove, lvForceMove, lvForceMoveUse);
            LoadOptionalString(info.onFirstUserEnter, firstUserEnter, firstUserEnable);
            LoadOptionalString(info.onUserEnter, userEnter, userEnterEnable);
            LoadOptionalString(info.mapName, mapName, mapNameEnable);
            LoadOptionalString(info.mapDesc, mapDesc, mapDescEnable);
            LoadOptionalString(info.streetName, streetNameBox, streetNameEnable);
            LoadOptionalString(info.effect, effectBox, effectEnable);
            LoadOptionalInt(info.moveLimit, moveLimit, moveLimitEnable);
            LoadOptionalInt(info.dropExpire, dropExpire, dropExpireEnable);
            LoadOptionalFloat(info.dropRate, dropRate, dropRateEnable);
            LoadOptionalFloat(info.recovery, recovery, recoveryEnable);
            reactorShuffle.Checked = info.reactorShuffle;
            LoadOptionalString(info.reactorShuffleName, reactorNameBox, reactorNameShuffle);
            LoadOptionalFloat(info.fs, fsBox, fsEnable);
            LoadOptionalInt(info.createMobInterval, createMobInterval, massEnable);
            LoadOptionalInt(info.fixedMobCapacity, fixedMobCapacity, massEnable);
            LoadOptionalInt(info.decHP, decHP, hpDecEnable);
            LoadOptionalInt(info.decInterval, decInterval, decIntervalEnable);
            LoadOptionalInt(info.protectItem, protectItem, protectEnable);
            helpEnable.Checked = info.help != null;
            if (info.help != null)
                helpBox.Text = info.help.Replace(@"\n", "\r\n");
            if (info.timeMob != null)
            {
                MapInfo.TimeMob tMob = (MapInfo.TimeMob)info.timeMob;
                summonMobEnable.Checked = true;
                LoadOptionalInt(tMob.startHour, timedMobStart, timedMobEnable);
                LoadOptionalInt(tMob.endHour, timedMobEnd, timedMobEnable);
                timedMobId.Value = tMob.id;
                timedMobMessage.Text = tMob.message.Replace(@"\n", "\r\n");
            }
            if (info.autoLieDetector != null)
            {
                MapInfo.AutoLieDetector ald = (MapInfo.AutoLieDetector)info.autoLieDetector;
                autoLieDetectorEnable.Checked = true;
                autoLieStart.Value = ald.startHour;
                autoLieEnd.Value = ald.endHour;
                autoLieInterval.Value = ald.interval;
                autoLieProp.Value = ald.prop;
            }
            if (info.allowedItems != null)
            {
                allowedItemsEnable.Checked = true;
                foreach (int id in info.allowedItems)
                    allowedItems.Items.Add(id.ToString());
            }
            optionsList.SetChecked(0, info.cloud);
            optionsList.SetChecked(1, info.snow);
            optionsList.SetChecked(2, info.rain);
            optionsList.SetChecked(3, info.swim);
            optionsList.SetChecked(4, info.fly);
            optionsList.SetChecked(5, info.town);
            optionsList.SetChecked(6, info.partyOnly);
            optionsList.SetChecked(7, info.expeditionOnly);
            optionsList.SetChecked(8, info.noMapCmd);
            optionsList.SetChecked(9, info.hideMinimap);
            optionsList.SetChecked(10, info.miniMapOnOff);
            optionsList.SetChecked(11, info.personalShop);
            optionsList.SetChecked(12, info.entrustedShop);
            optionsList.SetChecked(13, info.noRegenMap);
            optionsList.SetChecked(14, info.blockPBossChange);
            optionsList.SetChecked(15, info.everlast);
            optionsList.SetChecked(16, info.damageCheckFree);
            optionsList.SetChecked(17, info.scrollDisable);
            optionsList.SetChecked(18, info.needSkillForFly);
            optionsList.SetChecked(19, info.zakum2Hack);
            optionsList.SetChecked(20, info.allMoveCheck);
            optionsList.SetChecked(21, info.VRLimit);

            for (int i = 0; i < fieldLimitList.Items.Count; i++)
            {
                int value = (int)Math.Pow(2, i);
                fieldLimitList.SetChecked(i, ((int)info.fieldLimit & value) == value);
            }
            if (info.fieldType != null)/* fieldType.SelectedIndex = -1;
            else*/
            {
                fieldType.SelectedIndex = 0;
                if ((int)info.fieldType <= 0x22)
                    fieldType.SelectedIndex = (int)info.fieldType;
                else switch (info.fieldType)
                    {
                        case FieldType.FIELDTYPE_WEDDING:
                            fieldType.SelectedIndex = 0x23;
                            break;
                        case FieldType.FIELDTYPE_WEDDINGPHOTO:
                            fieldType.SelectedIndex = 0x24;
                            break;
                        case FieldType.FIELDTYPE_FISHINGKING:
                            fieldType.SelectedIndex = 0x25;
                            break;
                        case FieldType.FIELDTYPE_SHOWABATH:
                            fieldType.SelectedIndex = 0x26;
                            break;
                        case FieldType.FIELDTYPE_BEGINNERCAMP:
                            fieldType.SelectedIndex = 0x27;
                            break;
                        case FieldType.FIELDTYPE_SNOWMAN:
                            fieldType.SelectedIndex = 0x28;
                            break;
                        case FieldType.FIELDTYPE_SHOWASPA:
                            fieldType.SelectedIndex = 0x29;
                            break;
                        case FieldType.FIELDTYPE_HORNTAILPQ:
                            fieldType.SelectedIndex = 0x2A;
                            break;
                        case FieldType.FIELDTYPE_CRIMSONWOODPQ:
                            fieldType.SelectedIndex = 0x2B;
                            break;
                    }
            }
            foreach (IWzImageProperty prop in info.additionalProps)
            {
                TreeNode node = unknownProps.Nodes.Add(prop.Name);
                node.Tag = prop;
                if (prop.WzProperties != null && prop.WzProperties.Count > 0)
                    ExtractPropList(prop.WzProperties, node);
            }
        }

        private void ExtractPropList(List<IWzImageProperty> properties, TreeNode parent)
        {
            foreach (IWzImageProperty prop in properties)
            {
                TreeNode node = parent.Nodes.Add(prop.Name);
                node.Tag = prop;
                if (prop.WzProperties != null && prop.WzProperties.Count > 0)
                    ExtractPropList(prop.WzProperties, node);
            }
        }

        private void LoadOptionalInt(int? source, IntegerInput target, CheckBoxX checkBox)
        {
            checkBox.Checked = source != null;
            if (source != null) target.Value = source.Value;
        }

        private int? GetOptionalInt(IntegerInput textbox, CheckBoxX checkBox)
        {
            return checkBox.Checked ? (int?)textbox.Value : null;
        }

        private void LoadOptionalFloat(float? source, DoubleInput target, CheckBoxX checkBox)
        {
            checkBox.Checked = source != null;
            if (source != null) target.Value = source.Value;
        }

        private float? GetOptionalFloat(DoubleInput textbox, CheckBoxX checkBox)
        {
            return checkBox.Checked ? (float?)textbox.Value : null;
        }

        private void LoadOptionalString(string source, TextBoxX target, CheckBoxX checkBox)
        {
            checkBox.Checked = source != null;
            if (source != null) target.Text = source;
        }

        private string GetOptionalString(TextBoxX textbox, CheckBoxX checkBox)
        {
            return checkBox.Checked ? textbox.Text : null;
        }

        private void bgmBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            fieldLimitList.CheckOnClick = true;
            bool a = fieldLimitList.Checked(0);
            soundPlayer.SoundProperty = Program.InfoManager.BGMs[(string)bgmBox.SelectedItem];
        }

        private void InfoEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            soundPlayer.SoundProperty = null;
        }

        private void markBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            markImage.Image = Program.InfoManager.MapMarks[(string)markBox.SelectedItem];
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /*public WzSubProperty GetMapStringProp(string id)
        {
            id = RemoveLeadingZeros(id);
            WzImage mapNameParent = (WzImage)Program.WzManager.String.WzDirectory["Map.img"];
            foreach (WzSubProperty mapNameCategory in mapNameParent.WzProperties)
            {
                WzSubProperty mapNameDirectory = (WzSubProperty)mapNameCategory[id];
                if (mapNameDirectory != null)
                    return mapNameDirectory;
            }
            return null;
        }*/

        /*private void ChangeID(string oldID, string newID)
        {
            string mapid = WzInfoTools.AddLeadingZeros(oldID, 9);
            string mapcat = "Map" + mapid.Substring(0, 1);
            WzImage mapImage = (WzImage)Program.WzManager["map"].GetObjectFromPath("Map.wz/Map/" + mapcat + "/" + mapid + ".img");
            mapImage.Name = WzInfoTools.AddLeadingZeros(newID, 9) + ".img";*/
        /*WzSubProperty mapStringParent = GetMapStringProp(mapid);
        if (mapStringParent != null) ((IPropertyContainer)mapStringParent.Parent).RemoveProperty(mapStringParent.Name);
        mapStringParent = new WzSubProperty(WzInfoTools.RemoveLeadingZeros(newID));
        Program.WzManager.String["Map.img"]["etc"]*/
        /*}*/

        /*private void changeStringwz(int mapId, string name, string street)
        {
            WzStringProperty nameProp = GetMapNameProp(mapId);
            WzStringProperty streetProp = GetMapNameProp(mapId);
        }*/


        private void okButton_Click(object sender, EventArgs e)
        {
            if (info.mapType != MapType.CashShopPreview)
            {
                info.bgm = (string)bgmBox.SelectedItem;
                info.MapMark = (string)markBox.SelectedItem;
            }
            if (info.mapType == MapType.RegularMap)
            {
                /*if (WzInfoTools.RemoveLeadingZeros(info.id.ToString()) != WzInfoTools.RemoveLeadingZeros(idBox.Text))
                {
                    ChangeID(info.id.ToString(), idBox.Text);
                    info.id = int.Parse(idBox.Text);
                }*/
                /*if (nameBox.Text != info.strMapName || nameBox.Text != info.strStreetName)
                    changeStringwz(info.id, nameBox.Text, streetBox.Text);*/
                info.strMapName = nameBox.Text;
                info.strStreetName = streetBox.Text;
            }
            info.returnMap = cannotReturnCBX.Checked ? 999999999 : returnBox.Value;
            info.forcedReturn = returnHereCBX.Checked ? 999999999 : forcedRet.Value;
            info.mobRate = (float)mobRate.Value;
            info.timeLimit = GetOptionalInt(timeLimit, timeLimitEnable);
            info.lvLimit = GetOptionalInt(lvLimit, lvLimitEnable);
            info.lvForceMove = GetOptionalInt(lvForceMove, lvForceMoveUse);
            info.onFirstUserEnter =GetOptionalString(firstUserEnter, firstUserEnable);
            info.onUserEnter=GetOptionalString(userEnter, userEnterEnable);
            info.mapName=GetOptionalString( mapName, mapNameEnable);
            info.mapDesc=GetOptionalString( mapDesc, mapDescEnable);
            info.streetName=GetOptionalString( streetNameBox, streetNameEnable);
            info.effect=GetOptionalString( effectBox, effectEnable);
            info.moveLimit=GetOptionalInt( moveLimit, moveLimitEnable);
            info.dropExpire=GetOptionalInt( dropExpire, dropExpireEnable);
            info.dropRate=GetOptionalFloat( dropRate, dropRateEnable);
            info.recovery=GetOptionalFloat( recovery, recoveryEnable);
            info.reactorShuffle = reactorShuffle.Checked;
            info.reactorShuffleName=GetOptionalString( reactorNameBox, reactorNameShuffle);
            info.fs=GetOptionalFloat( fsBox, fsEnable);
            info.createMobInterval= GetOptionalInt( createMobInterval, massEnable);
            info.fixedMobCapacity=GetOptionalInt( fixedMobCapacity, massEnable);
            info.decHP=GetOptionalInt( decHP, hpDecEnable);
            info.decInterval=GetOptionalInt( decInterval, decIntervalEnable);
            info.protectItem=GetOptionalInt( protectItem, protectEnable);

            if (helpEnable.Checked) info.help = helpBox.Text.Replace("\r\n", @"\n");
            if (summonMobEnable.Checked)
                info.timeMob = new MapInfo.TimeMob(
                    GetOptionalInt(timedMobStart, timedMobEnable), 
                    GetOptionalInt(timedMobEnd, timedMobEnable), 
                    timedMobId.Value, 
                    timedMobMessage.Text.Replace("\r\n", @"\n"));
            if (autoLieDetectorEnable.Checked)
                info.autoLieDetector = new MapInfo.AutoLieDetector(
                    autoLieStart.Value, 
                    autoLieEnd.Value, 
                    autoLieInterval.Value, 
                    autoLieProp.Value);
            if (allowedItemsEnable.Checked)
            {
                info.allowedItems = new List<int>();
                foreach (string id in allowedItems.Items)
                    info.allowedItems.Add(int.Parse(id));
            }
            info.cloud = optionsList.Checked(0);
            info.snow = optionsList.Checked(1);
            info.rain = optionsList.Checked(2);
            info.swim = optionsList.Checked(3);
            info.fly = optionsList.Checked(4);
            info.town = optionsList.Checked(5);
            info.partyOnly = optionsList.Checked(6);
            info.expeditionOnly = optionsList.Checked(7);
            info.noMapCmd = optionsList.Checked(8);
            info.hideMinimap = optionsList.Checked(9);
            info.miniMapOnOff = optionsList.Checked(10);
            info.personalShop = optionsList.Checked(11);
            info.entrustedShop = optionsList.Checked(12);
            info.noRegenMap = optionsList.Checked(13);
            info.blockPBossChange = optionsList.Checked(14);
            info.everlast = optionsList.Checked(15);
            info.damageCheckFree = optionsList.Checked(16);
            info.scrollDisable = optionsList.Checked(17);
            info.needSkillForFly = optionsList.Checked(18);
            info.zakum2Hack = optionsList.Checked(19);
            info.allMoveCheck = optionsList.Checked(20);
            info.VRLimit = optionsList.Checked(21);
            int fieldLimitInt = 0;
            for (int i = 0; i < fieldLimitList.Items.Count; i++)
            {
                int value = (int)Math.Pow(2, i);
                if (fieldLimitList.Checked(i))
                    fieldLimitInt += value;
            }
            info.fieldLimit = (FieldLimit)fieldLimitInt;
            if (fieldType.SelectedIndex <= 0x22)
                info.fieldType = (FieldType)fieldType.SelectedIndex;
            else switch (fieldType.SelectedIndex)
                {
                    case 0x23:
                        info.fieldType = FieldType.FIELDTYPE_WEDDING;
                        break;
                    case 0x24:
                        info.fieldType = FieldType.FIELDTYPE_WEDDINGPHOTO;
                        break;
                    case 0x25:
                        info.fieldType = FieldType.FIELDTYPE_FISHINGKING;
                        break;
                    case 0x26:
                        info.fieldType = FieldType.FIELDTYPE_SHOWABATH;
                        break;
                    case 0x27:
                        info.fieldType = FieldType.FIELDTYPE_BEGINNERCAMP;
                        break;
                    case 0x28:
                        info.fieldType = FieldType.FIELDTYPE_SNOWMAN;
                        break;
                    case 0x29:
                        info.fieldType = FieldType.FIELDTYPE_SHOWASPA;
                        break;
                    case 0x2A:
                        info.fieldType = FieldType.FIELDTYPE_HORNTAILPQ;
                        break;
                    case 0x2B:
                        info.fieldType = FieldType.FIELDTYPE_CRIMSONWOODPQ;
                        break;
                }
            info.SaveMapInfo();
            Close();
        }

        private void enablingCheckBox_CheckChanged(object sender, EventArgs e)
        {
            CheckBoxX cbx = (CheckBoxX)sender;
            bool featureActivated = cbx.Checked && cbx.Enabled;
            if (cbx.Tag is Control) ((Control)cbx.Tag).Enabled = featureActivated;
            else
            {
                foreach (Control control in (Control[])cbx.Tag) control.Enabled = featureActivated;
                foreach (Control control in (Control[])cbx.Tag) if (control is CheckBoxX) enablingCheckBox_CheckChanged(control, e);
            }
        }

        private int lastret = 0;
        private int lastforcedret = 0;

        private void cannotReturnCBX_CheckedChanged(object sender, EventArgs e)
        {
            returnBox.Enabled = !cannotReturnCBX.Checked;
            if (cannotReturnCBX.Checked) { lastret = returnBox.Value; returnBox.Value = info.id; }
            else returnBox.Value = lastret;
        }

        private void returnHereCBX_CheckedChanged(object sender, EventArgs e)
        {
            forcedRet.Enabled = !returnHereCBX.Checked;
            if (returnHereCBX.Checked) { lastforcedret = forcedRet.Value; forcedRet.Value = 999999999; }
            else forcedRet.Value = lastforcedret;
        }

        private void allowedItemsRemove_Click(object sender, EventArgs e)
        {
            if (allowedItems.SelectedIndex != -1)
                allowedItems.Items.RemoveAt(allowedItems.SelectedIndex);
        }

        private void allowedItemsAdd_Click(object sender, EventArgs e)
        {
            allowedItems.Items.Add(Microsoft.VisualBasic.Interaction.InputBox("Insert item ID", "Add Allowed Item", "", -1, -1));
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
    }
}