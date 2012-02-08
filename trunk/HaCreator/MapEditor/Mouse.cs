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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Security.Cryptography;
using Xna = Microsoft.Xna.Framework;
using MapleLib.WzLib.WzStructure.Data;

namespace HaCreator.MapEditor
{
    public enum MouseState
    {
        Selection,
        StaticObjectAdding,
        RandomTiles,
        Footholds,
        Ropes,
        Chairs,
        Tooltip
    }

    public class Mouse : MapleDot //inheriting mapledot to make it easier to attach maplelines to it
    {
        private Bitmap placeholder = new Bitmap(1, 1);
        private Point origin = new Point(0, 0);
        private bool isDown;
        private bool minimapBrowseOngoing;
        private bool multiSelectOngoing;
        private Xna.Point multiSelectStart;
        private MouseState state;
        private MapleDrawableInfo currAddedInfo;
        private BoardItem currAddedObj;
        private TileInfo[] tileRandomList;

        public Mouse(Board board)
            : base(board, 0, 0, false)
        {
            IsDown = false;
        }

        public static int NextInt32(int max)
        {
            byte[] bytes = new byte[sizeof(int)];
            RNGCryptoServiceProvider Gen = new RNGCryptoServiceProvider();
            Gen.GetBytes(bytes);
            return Math.Abs(BitConverter.ToInt32(bytes, 0) % max);
        }

        public void PlaceObject()
        {
            if (state == MouseState.StaticObjectAdding || state == MouseState.RandomTiles)
            {
                Board.UndoRedoMan.AddUndoBatch(new List<UndoRedoAction>() { UndoRedoManager.ItemAdded(currAddedObj) });
                //Board.BoardItems.Add(currAddedObj.CreateInstance(Board.Layers[Board.SelectedLayerIndex], Board, x, y, 50, false, false));
                currAddedObj.BeforeAdding = false;
                ReleaseItem(currAddedObj);
                if (currAddedObj is LayeredItem)
                {
                    int highestZ = 0;
                    foreach (LayeredItem item in Board.BoardItems.TileObjs)
                        if (item.Z > highestZ) highestZ = item.Z;
                    currAddedObj.Z = highestZ;
                    Board.BoardItems.Sort();
                }
                if (state == MouseState.StaticObjectAdding)
                    currAddedObj = currAddedInfo.CreateInstance(Board.SelectedLayer, Board, X + currAddedInfo.Origin.X - currAddedInfo.Image.Width / 2, Y + currAddedInfo.Origin.Y - currAddedInfo.Image.Height / 2, 50, false, true);
                else
                    currAddedObj = tileRandomList[NextInt32(tileRandomList.Length)].CreateInstance(Board.SelectedLayer, Board, X + currAddedInfo.Origin.X - currAddedInfo.Image.Width / 2, Y + currAddedInfo.Origin.Y - currAddedInfo.Image.Height / 2, 50, false, true);
                Board.BoardItems.Add(currAddedObj, false);
                BindItem(currAddedObj, new Microsoft.Xna.Framework.Point(currAddedInfo.Origin.X - currAddedInfo.Image.Width / 2, currAddedInfo.Origin.Y - currAddedInfo.Image.Height / 2));
            }
            else if (state == MouseState.Chairs)
            {
                Board.UndoRedoMan.AddUndoBatch(new List<UndoRedoAction> { UndoRedoManager.ItemAdded(currAddedObj) });
                ReleaseItem(currAddedObj);
                currAddedObj = new Chair(Board, X, Y, true);
                Board.BoardItems.Add(currAddedObj, false);
                BindItem(currAddedObj, new Microsoft.Xna.Framework.Point());
            }
            else if (state == MouseState.Ropes)
            {
                int count = BoundItems.Count;
                object[] keys = new object[count];
                BoundItems.Keys.CopyTo(keys, 0);
                RopeAnchor anchor = (RopeAnchor)keys[0];
                ReleaseItem(anchor);
                if (count == 1)
                {
                    Board.UndoRedoMan.AddUndoBatch(new List<UndoRedoAction> { UndoRedoManager.RopeAdded(anchor.ParentRope) });
                    CreateRope();
                }
            }
            else if (state == MouseState.Tooltip)
            {
                int count = BoundItems.Count;
                object[] keys = new object[count];
                BoundItems.Keys.CopyTo(keys, 0);
                ToolTipDot dot = (ToolTipDot)keys[0];
                ReleaseItem(dot);
                if (count == 1)
                {
                    Board.UndoRedoMan.AddUndoBatch(new List<UndoRedoAction> { UndoRedoManager.ItemAdded(dot.ParentTooltip) });
                    CreateTooltip();
                }
            }
        }

        private void CreateRope()
        {
            Rope rope = new Rope(Board, X, Y, Y, false, Board.SelectedLayerIndex, true);
            Board.BoardItems.Ropes.Add(rope);
            BindItem(rope.FirstAnchor, new Xna.Point());
            BindItem(rope.SecondAnchor, new Xna.Point());
        }

        private void CreateTooltip()
        {
            ToolTip tt = new ToolTip(Board, new Xna.Rectangle(X, Y, 0, 0), "Title", "Description");
            Board.BoardItems.ToolTips.Add(tt);
            BindItem(tt.PointA, new Xna.Point());
            BindItem(tt.PointC, new Xna.Point());
        }


        public void CreateFhAnchor()
        {
            FootholdAnchor fhAnchor = new FootholdAnchor(Board, X, Y, Board.SelectedLayerIndex, false);
            Board.BoardItems.FHAnchors.Add(fhAnchor);
            Board.UndoRedoMan.AddUndoBatch(new List<UndoRedoAction> { UndoRedoManager.ItemAdded(fhAnchor) });
            if (connectedLines.Count == 0)
            {
                Board.BoardItems.FootholdLines.Add(new FootholdLine(Board, fhAnchor));
            }
            else
            {
                connectedLines[0].ConnectSecondDot(fhAnchor);
                Board.BoardItems.FootholdLines.Add(new FootholdLine(Board, fhAnchor));
            }
        }

        public void TryConnectFoothold()
        {
            Xna.Point pos = new Xna.Point(X,Y);
            foreach (FootholdAnchor anchor in Board.BoardItems.FHAnchors)
            {
                if (MultiBoard.IsPointInsideRectangle(pos, anchor.Left, anchor.Top, anchor.Right, anchor.Bottom))
                {
                    if (connectedLines.Count > 0)
                    {
                        if (connectedLines[0].FirstDot != anchor && !FootholdLine.Exists(anchor.X, anchor.Y, connectedLines[0].FirstDot.X, connectedLines[0].FirstDot.Y, Board))
                        {
                            Board.UndoRedoMan.AddUndoBatch(new List<UndoRedoAction> { UndoRedoManager.LineAdded(connectedLines[0], connectedLines[0].FirstDot, anchor) });
                            connectedLines[0].ConnectSecondDot(anchor);
                            FootholdLine fh = new FootholdLine(Board, anchor);
                            Board.BoardItems.FootholdLines.Add(fh);
                        }
                    }
                    else
                    {
                        Board.BoardItems.FootholdLines.Add(new FootholdLine(Board, anchor));
                    }
                }
            }
        }

        public void Clear()
        {
            List<UndoRedoAction> foo = new List<UndoRedoAction>(); //the undoPipe here has no meaning, we don't need any undo info anyway
            if (currAddedObj != null)
            {
                currAddedObj.RemoveItem(ref foo);
                currAddedObj = null;
            }
            if (state == MouseState.Ropes || state == MouseState.Tooltip)
            {
                object[] keys = new object[BoundItems.Keys.Count];
                BoundItems.Keys.CopyTo(keys, 0);
                if (state == MouseState.Ropes)
                    ((RopeAnchor)keys[0]).RemoveItem(ref foo);
                else
                    ((ToolTipDot)keys[0]).ParentTooltip.RemoveItem(ref foo);
            }
            else if (state == MouseState.Footholds && connectedLines.Count > 0)
            {
                FootholdLine fh = (FootholdLine)connectedLines[0];
                fh.Remove(false, ref foo);
                Board.BoardItems.FootholdLines.Remove(fh);
            }
            InputHandler.ClearBoundItems(Board);
            InputHandler.ClearSelectedItems(Board);
            IsDown = false;
        }

        public void SelectionMode()
        {
            Clear();
            currAddedInfo = null;
            tileRandomList = null;
            state = MouseState.Selection;
        }

        public void SetHeldInfo(MapleDrawableInfo newInfo)
        {
            Clear();
            if (newInfo.Image == null) ((MapleExtractableInfo)newInfo).ParseImage();
            currAddedInfo = newInfo;
            currAddedObj = newInfo.CreateInstance(Board.SelectedLayer, Board, X + currAddedInfo.Origin.X - newInfo.Image.Width / 2, Y + currAddedInfo.Origin.Y - newInfo.Image.Height / 2, 50, false, true);
            Board.BoardItems.Add(currAddedObj, false);
            BindItem(currAddedObj, new Microsoft.Xna.Framework.Point(newInfo.Origin.X - newInfo.Image.Width / 2, newInfo.Origin.Y - newInfo.Image.Height / 2));
            state = MouseState.StaticObjectAdding;
        }

        public void SetRandomTilesMode(TileInfo[] tileList)
        {
            Clear();
            tileRandomList = tileList;
            SetHeldInfo(tileRandomList[NextInt32(tileRandomList.Length)]);
            state = MouseState.RandomTiles;
        }

        public void SetFootholdMode()
        {
            Clear();
            state = MouseState.Footholds;
        }

        public void SetRopeMode()
        {
            Clear();
            state = MouseState.Ropes;
            CreateRope();
        }

        public void SetChairMode()
        {
            Clear();
            currAddedObj = new Chair(Board, X, Y, true);
            Board.BoardItems.Add(currAddedObj, false);
            BindItem(currAddedObj, new Microsoft.Xna.Framework.Point());
            state = MouseState.Chairs;
        }

        public void SetTooltipMode()
        {
            Clear();
            state = MouseState.Tooltip;
            CreateTooltip();
        }

        #region Properties
        public bool IsDown
        {
            get { return isDown; }
            set 
            {
                isDown = value;
                if (!isDown)
                {
                    multiSelectOngoing = false;
                    multiSelectStart = new Xna.Point();
                    minimapBrowseOngoing = false;
                }
            }
        }

        public bool MinimapBrowseOngoing
        {
            get { return minimapBrowseOngoing; }
            set { minimapBrowseOngoing = value; }
        }

        public bool MultiSelectOngoing
        {
            get { return multiSelectOngoing; }
            set { multiSelectOngoing = value; }
        }

        public Xna.Point MultiSelectStart
        {
            get { return multiSelectStart; }
            set { multiSelectStart = value; }
        }

        public MouseState State
        {
            get { return state; }
        }
        #endregion

        #region Overrides
        public override MapleDrawableInfo BaseInfo
        {
            get { return null; }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sprite, Microsoft.Xna.Framework.Color color, int xShift, int yShift)
        {
        }

        public override bool CheckIfLayerSelected(int selectedLayer)
        {
            return true;
        }

        public override System.Drawing.Bitmap Image
        {
            get { return placeholder; }
        }

        public override Point Origin
        {
            get { return origin; }
        }

        public override ItemTypes Type
        {
            get { return ItemTypes.None; }
        }

        public override Microsoft.Xna.Framework.Color Color
        {
            get { return Microsoft.Xna.Framework.Color.White; }
        }

        public override Microsoft.Xna.Framework.Color InactiveColor
        {
            get { return Microsoft.Xna.Framework.Color.White; }
        }

        public override void BindItem(BoardItem item, Microsoft.Xna.Framework.Point distance)
        {
            if (BoundItems.Contains(item)) return;
            BoundItems[item] = distance;
            item.tempParent = item.Parent;
            item.Parent = this;
        }

        public override void ReleaseItem(BoardItem item)
        {
            if (BoundItems.Contains(item))
            {
                BoundItems.Remove(item);
                item.Parent = item.tempParent;
                item.tempParent = null;
            }
        }

        public override void RemoveItem(ref List<UndoRedoAction> undoPipe)
        {
        }
        #endregion
    }
}
