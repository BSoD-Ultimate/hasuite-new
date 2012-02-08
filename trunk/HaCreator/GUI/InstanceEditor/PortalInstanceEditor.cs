﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using HaCreator.MapEditor;
using MapleLib.WzLib.WzStructure.Data;
using DevComponents.DotNetBar.Controls;
using System.Collections;
using DevComponents.Editors;

namespace HaCreator.GUI.InstanceEditor
{
    public partial class PortalInstanceEditor : Office2007Form
    {
        public PortalInstance item;
        private ControlRowManager rowMan;

        public PortalInstanceEditor(PortalInstance item)
        {
            InitializeComponent();
            ptComboBox.Items.AddRange((object[])Tables.PortalTypeNames.Cast<object>());
            this.item = item;
            styleManager.ManagerStyle = UserSettings.applicationStyle;

            rowMan = new ControlRowManager(new ControlRow[] { 
                new ControlRow(new Control[] { pnLabel, pnBox }, 26, "pn"),
                new ControlRow(new Control[] { tmLabel, tmBox, btnBrowseMap, thisMap }, 26, "tm"),
                new ControlRow(new Control[] { tnLabel, tnBox, btnBrowseTn, leftBlankLabel }, 26, "tn"),
                new ControlRow(new Control[] { scriptLabel, scriptBox }, 26, "script"),
                new ControlRow(new Control[] { delayEnable, delayBox }, 26, "delay"),
                new ControlRow(new Control[] { rangeEnable, xRangeLabel, hRangeBox, yRangeLabel, vRangeBox }, 26, "range"),
                new ControlRow(new Control[] { impactLabel, hImpactEnable, hImpactBox, vImpactEnable, vImpactBox }, 26, "impact"),
                new ControlRow(new Control[] { hideTooltip, onlyOnce }, 26, "bool"),
                new ControlRow(new Control[] { imageLabel, portalImageList, portalImageBox }, okButton.Top - portalImageList.Top, "image"),
                new ControlRow(new Control[] { okButton, cancelButton }, 26, "buttons")
            }, this);

            delayEnable.Tag = delayBox;
            hImpactEnable.Tag = hImpactBox;
            vImpactEnable.Tag = vImpactBox;

            xInput.Value = item.X;
            yInput.Value = item.Y;
            ptComboBox.SelectedIndex = (int)item.pt;
            pnBox.Text = item.pn;
            if (item.tm == item.Board.MapInfo.id) thisMap.Checked = true;
            else tmBox.Value = item.tm;
            tnBox.Text = item.tn;
            if (item.script != null) scriptBox.Text = item.script;
            SetOptionalInt(item.delay, delayEnable, delayBox);
            SetOptionalInt(item.hRange, rangeEnable, hRangeBox);
            SetOptionalInt(item.vRange, rangeEnable, vRangeBox);
            SetOptionalInt(item.horizontalImpact, hImpactEnable, hImpactBox);
            if (item.verticalImpact != null) vImpactBox.Value = (int)item.verticalImpact;
            onlyOnce.Checked = item.onlyOnce;
            hideTooltip.Checked = item.hideTooltip;
            if (item.image != null)
            {
                portalImageList.SelectedItem = item.image;
            }
        }

        private void SetOptionalInt(int? value, CheckBoxX enabler, IntegerInput input)
        {
            if (value == null) enabler.Checked = false;
            else { enabler.Checked = true; input.Value = (int)value; }
        }

        private int? GetOptionalInt(CheckBoxX enabler, IntegerInput input)
        {
            return enabler.Checked ? (int?)input.Value : null;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            List<UndoRedoAction> actions = new List<UndoRedoAction>();
            if (xInput.Value != item.X || yInput.Value != item.Y)
            {
                actions.Add(UndoRedoManager.ItemMoved(item, new Microsoft.Xna.Framework.Point(item.X, item.Y), new Microsoft.Xna.Framework.Point(xInput.Value, yInput.Value)));
                item.Move(xInput.Value, yInput.Value);
            }
            if (actions.Count > 0)
                item.Board.UndoRedoMan.AddUndoBatch(actions);
            
            item.pt = (PortalType)ptComboBox.SelectedIndex;
            switch (item.pt)
            {
                case PortalType.PORTALTYPE_STARTPOINT:
                    item.pn = "sp";
                    item.tm = 999999999;
                    item.tn = "";
                    item.hRange = null;
                    item.vRange = null;
                    item.horizontalImpact = null;
                    item.verticalImpact = null;
                    item.delay = null;
                    item.script = null;
                    item.onlyOnce = null;
                    item.hideTooltip = null;
                    break;
                case PortalType.PORTALTYPE_INVISIBLE:
                    /*item.pn = pnBox.Text;
                    item.tm = thisMap.Checked ? item.Board.MapInfo.id : tmBox.Value;
                    item.tn = tnBox.Text;
                    item.hRange = GetOptionalInt(hRangeEnable, hRangeBox);
                    item.vRange = GetOptionalInt(vRangeEnable, vRangeBox);
                    item.horizontalImpact = GetOptionalInt(hImpactEnable, hImpactBox);
                    item.verticalImpact = GetOptionalInt(vImpactEnable, vImpactBox);
                    item.delay = GetOptionalInt(delayEnable, delayBox);
                    item.script = scriptEnable.Checked ? scriptBox.Text : null;
                    item.onlyOnce = onlyOnce.Checked;
                    item.hideTooltip = hideTooltip.Checked;*/
                    item.pn = pnBox.Text;
                    item.tm = thisMap.Checked ? item.Board.MapInfo.id : tmBox.Value;
                    item.tn = tnBox.Text;
                    item.hRange = null;
                    item.vRange = null;
                    item.horizontalImpact = null;
                    item.verticalImpact = null;
                    item.delay = GetOptionalInt(delayEnable, delayBox);
                    item.script = null;
                    item.onlyOnce = onlyOnce.Checked;
                    item.hideTooltip = hideTooltip.Checked;
                    break;
                case PortalType.PORTALTYPE_VISIBLE:
                    item.pn = pnBox.Text;
                    item.tm = thisMap.Checked ? item.Board.MapInfo.id : tmBox.Value;
                    item.tn = tnBox.Text;
                    item.hRange = null;
                    item.vRange = null;
                    item.horizontalImpact = null;
                    item.verticalImpact = null;
                    item.delay = GetOptionalInt(delayEnable, delayBox);
                    item.script = null;
                    item.onlyOnce = onlyOnce.Checked;
                    item.hideTooltip = hideTooltip.Checked;
                    break;
                case PortalType.PORTALTYPE_COLLISION:
                    item.pn = pnBox.Text;
                    item.tm = thisMap.Checked ? item.Board.MapInfo.id : tmBox.Value;
                    item.tn = tnBox.Text;
                    item.hRange = GetOptionalInt(rangeEnable, hRangeBox);
                    item.vRange = GetOptionalInt(rangeEnable, vRangeBox);
                    item.horizontalImpact = null;
                    item.verticalImpact = null;
                    item.delay = GetOptionalInt(delayEnable, delayBox);
                    item.script = null;
                    item.onlyOnce = onlyOnce.Checked;
                    item.hideTooltip = hideTooltip.Checked;
                    break;
                case PortalType.PORTALTYPE_CHANGABLE:
                    item.pn = pnBox.Text;
                    item.tm = thisMap.Checked ? item.Board.MapInfo.id : tmBox.Value;
                    item.tn = tnBox.Text;
                    item.hRange = null;
                    item.vRange = null;
                    item.horizontalImpact = null;
                    item.verticalImpact = null;
                    item.delay = GetOptionalInt(delayEnable, delayBox);
                    item.script = null;
                    item.onlyOnce = onlyOnce.Checked;
                    item.hideTooltip = hideTooltip.Checked;
                    break;
                case PortalType.PORTALTYPE_CHANGABLE_INVISIBLE:
                    item.pn = pnBox.Text;
                    item.tm = thisMap.Checked ? item.Board.MapInfo.id : tmBox.Value;
                    item.tn = tnBox.Text;
                    item.hRange = null;
                    item.vRange = null;
                    item.horizontalImpact = null;
                    item.verticalImpact = null;
                    item.delay = GetOptionalInt(delayEnable, delayBox);
                    item.script = null;
                    item.onlyOnce = onlyOnce.Checked;
                    item.hideTooltip = hideTooltip.Checked;
                    break;
                case PortalType.PORTALTYPE_TOWNPORTAL_POINT:
                    item.pn = "tp";
                    item.tm = 999999999;
                    item.tn = "";
                    item.hRange = null;
                    item.vRange = null;
                    item.horizontalImpact = null;
                    item.verticalImpact = null;
                    item.delay = null;
                    item.script = null;
                    item.onlyOnce = null;
                    item.hideTooltip = null;
                    break;
                case PortalType.PORTALTYPE_SCRIPT:
                    item.pn = pnBox.Text;
                    item.tm = 999999999;
                    item.tn = "";
                    item.hRange = null;
                    item.vRange = null;
                    item.horizontalImpact = null;
                    item.verticalImpact = null;
                    item.delay = null;
                    item.script = scriptBox.Text;
                    item.onlyOnce = onlyOnce.Checked;
                    item.hideTooltip = hideTooltip.Checked;
                    break;
                case PortalType.PORTALTYPE_SCRIPT_INVISIBLE:
                    item.pn = pnBox.Text;
                    item.tm = 999999999;
                    item.tn = "";
                    item.hRange = null;
                    item.vRange = null;
                    item.horizontalImpact = null;
                    item.verticalImpact = null;
                    item.delay = null;
                    item.script = null;
                    item.onlyOnce = onlyOnce.Checked;
                    item.hideTooltip = hideTooltip.Checked;
                    break;
                case PortalType.PORTALTYPE_COLLISION_SCRIPT:
                    item.pn = pnBox.Text;
                    item.tm = 999999999;
                    item.tn = "";
                    item.hRange = null;
                    item.vRange = null;
                    item.horizontalImpact = null;
                    item.verticalImpact = null;
                    item.delay = GetOptionalInt(delayEnable, delayBox);
                    item.script = scriptBox.Text;
                    item.onlyOnce = onlyOnce.Checked;
                    item.hideTooltip = hideTooltip.Checked;
                    break;
                case PortalType.PORTALTYPE_HIDDEN:
                    item.pn = pnBox.Text;
                    item.tm = thisMap.Checked ? item.Board.MapInfo.id : tmBox.Value;
                    item.tn = tnBox.Text;
                    item.hRange = null;
                    item.vRange = null;
                    item.horizontalImpact = null;
                    item.verticalImpact = null;
                    item.delay = GetOptionalInt(delayEnable, delayBox);
                    item.script = null;
                    item.onlyOnce = onlyOnce.Checked;
                    item.hideTooltip = hideTooltip.Checked;
                    break;
                case PortalType.PORTALTYPE_SCRIPT_HIDDEN:
                    item.pn = pnBox.Text;
                    item.tm = 999999999;
                    item.tn = "";
                    item.hRange = null;
                    item.vRange = null;
                    item.horizontalImpact = null;
                    item.verticalImpact = null;
                    item.delay = GetOptionalInt(delayEnable, delayBox);
                    item.script = null;
                    item.onlyOnce = onlyOnce.Checked;
                    item.hideTooltip = hideTooltip.Checked;
                    break;
                case PortalType.PORTALTYPE_COLLISION_VERTICAL_JUMP:
                    item.pn = pnBox.Text;
                    item.tm = 999999999;
                    item.tn = tnBox.Text;
                    item.hRange = null;
                    item.vRange = null;
                    item.horizontalImpact = null;
                    item.verticalImpact = null;
                    item.delay = GetOptionalInt(delayEnable, delayBox);
                    item.script = null;
                    item.onlyOnce = onlyOnce.Checked;
                    item.hideTooltip = hideTooltip.Checked;
                    break;
                case PortalType.PORTALTYPE_COLLISION_CUSTOM_IMPACT:
                    item.pn = pnBox.Text;
                    item.tm = 999999999;
                    item.tn = "";
                    item.hRange = null;
                    item.vRange = null;
                    item.horizontalImpact = GetOptionalInt(hImpactEnable, hImpactBox);
                    item.verticalImpact = vImpactBox.Value;
                    item.delay = GetOptionalInt(delayEnable, delayBox);
                    item.script = null;
                    item.onlyOnce = onlyOnce.Checked;
                    item.hideTooltip = hideTooltip.Checked;
                    break;
                case PortalType.PORTALTYPE_COLLISION_UNKNOWN_PCIG:
                    item.pn = pnBox.Text;
                    item.tm = thisMap.Checked ? item.Board.MapInfo.id : tmBox.Value;
                    item.tn = tnBox.Text;
                    item.hRange = null;
                    item.vRange = null;
                    item.horizontalImpact = GetOptionalInt(hImpactEnable, hImpactBox);
                    item.verticalImpact = GetOptionalInt(vImpactEnable, vImpactBox);
                    item.delay = GetOptionalInt(delayEnable, delayBox);
                    item.script = null;
                    item.onlyOnce = onlyOnce.Checked;
                    item.hideTooltip = hideTooltip.Checked;
                    break;
            }

            if (portalImageList.SelectedItem != null && Program.InfoManager.GamePortals[(PortalType)ptComboBox.SelectedIndex] != null)
            {
               item.image = (string)portalImageList.SelectedItem;
            }
            Close();
        }

        private void thisMap_CheckedChanged(object sender, EventArgs e)
        {
            tmBox.Enabled = !thisMap.Checked;
            btnBrowseMap.Enabled = !thisMap.Checked;
            btnBrowseTn.Enabled = thisMap.Checked;
        }

        private void EnablingCheckBoxCheckChanged(object sender, EventArgs e)
        {
            ((Control)((CheckBoxX)sender).Tag).Enabled = ((CheckBoxX)sender).Checked;
        }

        private void ptComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnBrowseTn.Enabled = thisMap.Checked;
            switch ((PortalType)ptComboBox.SelectedIndex)
            {
                case PortalType.PORTALTYPE_STARTPOINT:
                    rowMan.SetInvisible("pn");
                    rowMan.SetInvisible("tm");
                    rowMan.SetInvisible("tn");
                    rowMan.SetInvisible("script");
                    rowMan.SetInvisible("delay");
                    rowMan.SetInvisible("range");
                    rowMan.SetInvisible("impact");
                    rowMan.SetInvisible("bool");
                    break;
                case PortalType.PORTALTYPE_INVISIBLE:
                    rowMan.SetVisible("pn");
                    rowMan.SetVisible("tm");
                    rowMan.SetVisible("tn");
                    rowMan.SetInvisible("script");
                    rowMan.SetVisible("delay");
                    rowMan.SetInvisible("range");
                    rowMan.SetInvisible("impact");
                    rowMan.SetVisible("bool");
                    break;
                case PortalType.PORTALTYPE_VISIBLE:
                    rowMan.SetVisible("pn");
                    rowMan.SetVisible("tm");
                    rowMan.SetVisible("tn");
                    rowMan.SetInvisible("script");
                    rowMan.SetVisible("delay");
                    rowMan.SetInvisible("range");
                    rowMan.SetInvisible("impact");
                    rowMan.SetVisible("bool");
                    break;
                case PortalType.PORTALTYPE_COLLISION:
                    rowMan.SetVisible("pn");
                    rowMan.SetVisible("tm");
                    rowMan.SetVisible("tn");
                    rowMan.SetInvisible("script");
                    rowMan.SetVisible("delay");
                    rowMan.SetVisible("range");
                    rowMan.SetInvisible("impact");
                    rowMan.SetVisible("bool");
                    break;
                case PortalType.PORTALTYPE_CHANGABLE:
                    rowMan.SetVisible("pn");
                    rowMan.SetVisible("tm");
                    rowMan.SetVisible("tn");
                    rowMan.SetInvisible("script");
                    rowMan.SetVisible("delay");
                    rowMan.SetInvisible("range");
                    rowMan.SetInvisible("impact");
                    rowMan.SetVisible("bool");
                    break;
                case PortalType.PORTALTYPE_CHANGABLE_INVISIBLE:
                    rowMan.SetVisible("pn");
                    rowMan.SetVisible("tm");
                    rowMan.SetVisible("tn");
                    rowMan.SetInvisible("script");
                    rowMan.SetVisible("delay");
                    rowMan.SetInvisible("range");
                    rowMan.SetInvisible("impact");
                    rowMan.SetVisible("bool");
                    break;
                case PortalType.PORTALTYPE_TOWNPORTAL_POINT:
                    rowMan.SetInvisible("pn");
                    rowMan.SetInvisible("tm");
                    rowMan.SetInvisible("tn");
                    rowMan.SetInvisible("script");
                    rowMan.SetInvisible("delay");
                    rowMan.SetInvisible("range");
                    rowMan.SetInvisible("impact");
                    rowMan.SetInvisible("bool");
                    break;
                case PortalType.PORTALTYPE_SCRIPT:
                    rowMan.SetVisible("pn");
                    rowMan.SetInvisible("tm");
                    rowMan.SetInvisible("tn");
                    rowMan.SetVisible("script");
                    rowMan.SetVisible("delay");
                    rowMan.SetInvisible("range");
                    rowMan.SetInvisible("impact");
                    rowMan.SetVisible("bool");
                    break;
                case PortalType.PORTALTYPE_SCRIPT_INVISIBLE:
                    rowMan.SetVisible("pn");
                    rowMan.SetInvisible("tm");
                    rowMan.SetInvisible("tn");
                    rowMan.SetVisible("script");
                    rowMan.SetVisible("delay");
                    rowMan.SetInvisible("range");
                    rowMan.SetInvisible("impact");
                    rowMan.SetVisible("bool");
                    break;
                case PortalType.PORTALTYPE_COLLISION_SCRIPT:
                    rowMan.SetVisible("pn");
                    rowMan.SetInvisible("tm");
                    rowMan.SetInvisible("tn");
                    rowMan.SetVisible("script");
                    rowMan.SetVisible("delay");
                    rowMan.SetVisible("range");
                    rowMan.SetInvisible("impact");
                    rowMan.SetVisible("bool");
                    break;
                case PortalType.PORTALTYPE_HIDDEN:
                    rowMan.SetVisible("pn");
                    rowMan.SetVisible("tm");
                    rowMan.SetVisible("tn");
                    rowMan.SetInvisible("script");
                    rowMan.SetVisible("delay");
                    rowMan.SetInvisible("range");
                    rowMan.SetInvisible("impact");
                    rowMan.SetVisible("bool");
                    break;
                case PortalType.PORTALTYPE_SCRIPT_HIDDEN:
                    rowMan.SetVisible("pn");
                    rowMan.SetInvisible("tm");
                    rowMan.SetInvisible("tn");
                    rowMan.SetVisible("script");
                    rowMan.SetVisible("delay");
                    rowMan.SetInvisible("range");
                    rowMan.SetInvisible("impact");
                    rowMan.SetVisible("bool");
                    break;
                case PortalType.PORTALTYPE_COLLISION_VERTICAL_JUMP:
                    rowMan.SetVisible("pn");
                    rowMan.SetInvisible("tm");
                    rowMan.SetVisible("tn");
                    rowMan.SetInvisible("script");
                    rowMan.SetVisible("delay");
                    rowMan.SetInvisible("range");
                    rowMan.SetInvisible("impact");
                    rowMan.SetVisible("bool");
                    break;
                case PortalType.PORTALTYPE_COLLISION_CUSTOM_IMPACT:
                    rowMan.SetVisible("pn");
                    rowMan.SetInvisible("tm");
                    rowMan.SetVisible("tn");
                    rowMan.SetInvisible("script");
                    rowMan.SetVisible("delay");
                    rowMan.SetInvisible("range");
                    rowMan.SetVisible("impact");
                    rowMan.SetVisible("bool");
                    break;
                case PortalType.PORTALTYPE_COLLISION_UNKNOWN_PCIG:
                    rowMan.SetVisible("pn");
                    rowMan.SetVisible("tm");
                    rowMan.SetVisible("tn");
                    rowMan.SetInvisible("script");
                    rowMan.SetVisible("delay");
                    rowMan.SetInvisible("range");
                    rowMan.SetVisible("impact");
                    rowMan.SetVisible("bool");
                    break;
            }
            leftBlankLabel.Visible = (PortalType)ptComboBox.SelectedIndex == PortalType.PORTALTYPE_COLLISION_VERTICAL_JUMP;
            if (ptComboBox.SelectedIndex == (int)PortalType.PORTALTYPE_COLLISION_VERTICAL_JUMP)
                btnBrowseTn.Enabled = true;
            PortalGameImageInfo imageInfo = Program.InfoManager.GamePortals[(PortalType)ptComboBox.SelectedIndex];
            if (imageInfo == null) rowMan.SetInvisible("image");
            else
            {
                portalImageList.Items.Clear();
                portalImageList.Items.Add("default");
                portalImageBox.Image = null;
                rowMan.SetVisible("image");
                foreach (DictionaryEntry image in imageInfo)
                    portalImageList.Items.Add(image.Key);
                portalImageList.SelectedIndex = 0;
            }
        }

        private void portalImageList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (portalImageList.SelectedItem == null) return;
            else if ((string)portalImageList.SelectedItem == "default") portalImageBox.Image = Program.InfoManager.GamePortals[(PortalType)ptComboBox.SelectedIndex].DefaultImage;
            else portalImageBox.Image = Program.InfoManager.GamePortals[(PortalType)ptComboBox.SelectedIndex][(string)portalImageList.SelectedItem];
        }

        private void rangeEnable_CheckedChanged(object sender, EventArgs e)
        {
            hRangeBox.Enabled = rangeEnable.Checked;
            vRangeBox.Enabled = rangeEnable.Checked;
        }

        private void btnBrowseMap_Click(object sender, EventArgs e)
        {
            int? mapId = MapBrowser.Show();
            if (mapId != null) tmBox.Value = (int)mapId;
        }

        private void btnBrowseTn_Click(object sender, EventArgs e)
        {
            string tn = TnSelector.Show(item.Board);
            if (tn != null) tnBox.Text = tn;
        }
    }

    public class ControlRow
    {
        public Control[] controls;
        public bool invisible = false;
        public int rowSize;
        public string rowName;

        public ControlRow(Control[] controls, int rowSize, string rowName)
        {
            this.controls = controls;
            this.rowSize = rowSize;
            this.rowName = rowName;
        }
    }

    public class ControlRowManager
    {
        ControlRow[] rows;
        Hashtable names = new Hashtable();
        Form form;

        public ControlRowManager(ControlRow[] rows, Form form)
        {
            this.form = form;
            this.rows = rows;
            int index = 0;
            foreach (ControlRow row in rows)
                names[row.rowName] = index++;
        }

        public void SetInvisible(string name)
        {
            SetInvisible((int)names[name]);
        }

        public void SetInvisible(int index)
        {
            ControlRow row = rows[index];
            if (row.invisible) return;
            row.invisible = true;
            foreach (Control c in row.controls)
                c.Visible = false;
            int size = row.rowSize;
            for (int i = index + 1; i < rows.Length; i++)
            {
                row = rows[i];
                foreach (Control c in row.controls)
                    c.Location = new Point(c.Location.X, c.Location.Y - size);
            }
            form.Height -= size;
        }

        public void SetVisible(string name)
        {
            SetVisible((int)names[name]);
        }

        public void SetVisible(int index)
        {
            ControlRow row = rows[index];
            if (!row.invisible) return;
            row.invisible = false;
            foreach (Control c in row.controls)
                c.Visible = true;
            int size = row.rowSize;
            for (int i = index + 1; i < rows.Length; i++)
            {
                row = rows[i];
                foreach (Control c in row.controls)
                    c.Location = new Point(c.Location.X, c.Location.Y + size);
            }
            form.Height += size;
        }
    }
}
