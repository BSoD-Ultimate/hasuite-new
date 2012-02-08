﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HaCreator.MapEditor
{
    public class UndoRedoManager
    {
        public List<UndoRedoBatch> UndoList = new List<UndoRedoBatch>();
        public List<UndoRedoBatch> RedoList = new List<UndoRedoBatch>();
        private Board parentBoard;

        public UndoRedoManager(Board parentBoard)
        {
            this.parentBoard = parentBoard;
        }

        public void AddUndoBatch(List<UndoRedoAction> actions)
        {
            UndoRedoBatch batch = new UndoRedoBatch() { Actions = actions };
            UndoList.Add(batch);
            RedoList.Clear();
            parentBoard.ParentControl.UndoListChanged();
            parentBoard.ParentControl.RedoListChanged();
        }

        #region Undo Actions Creation
        public static UndoRedoAction ItemAdded(BoardItem item)
        {
            return new UndoRedoAction(item, UndoRedoType.ItemAdded, null, null);
        }

        public static UndoRedoAction ItemDeleted(BoardItem item)
        {
            return new UndoRedoAction(item, UndoRedoType.ItemDeleted, null, null);
        }

        public static UndoRedoAction ItemMoved(BoardItem item, Point oldPos, Point newPos)
        {
            return new UndoRedoAction(item, UndoRedoType.ItemMoved, oldPos, newPos);
        }

        public static UndoRedoAction VRChanged(Rectangle oldVR, Rectangle newVR)
        {
            return new UndoRedoAction(null, UndoRedoType.VRChanged, oldVR, newVR);
        }

        public static UndoRedoAction MapCenterChanged(Point oldCenter, Point newCenter)
        {
            return new UndoRedoAction(null, UndoRedoType.MapCenterChanged, oldCenter, newCenter);
        }

        public static UndoRedoAction ItemFlipped(IFlippable item)
        {
            return new UndoRedoAction((BoardItem)item, UndoRedoType.ItemFlipped, null, null);
        }

        public static UndoRedoAction LineRemoved(MapleLine line, MapleDot a, MapleDot b)
        {
            return new UndoRedoAction(null, UndoRedoType.LineRemoved, a, b, line);
        }

        public static UndoRedoAction LineAdded(MapleLine line, MapleDot a, MapleDot b)
        {
            return new UndoRedoAction(null, UndoRedoType.LineAdded, a, b, line);
        }

        public static UndoRedoAction ToolTipLinked(ToolTip tt, ToolTipChar ttc)
        {
            return new UndoRedoAction(tt, UndoRedoType.ToolTipLinked, ttc, null);
        }

        public static UndoRedoAction ToolTipUnlinked(ToolTip tt, ToolTipChar ttc)
        {
            return new UndoRedoAction(tt, UndoRedoType.ToolTipUnlinked, ttc, null);
        }

        public static UndoRedoAction BackgroundMoved(BackgroundInstance item, Point oldPos, Point newPos)
        {
            return new UndoRedoAction(item, UndoRedoType.BackgroundMoved, oldPos, newPos);
        }

        public static UndoRedoAction ItemsLinked(BoardItem parent, BoardItem child, Point distance)
        {
            return new UndoRedoAction(parent, UndoRedoType.ItemsLinked, child, distance);
        }

        public static UndoRedoAction ItemsUnlinked(BoardItem parent, BoardItem child, Point distance)
        {
            return new UndoRedoAction(parent, UndoRedoType.ItemsUnlinked, child, distance);
        }

        public static UndoRedoAction ItemsLayerChanged(List<IContainsLayerInfo> items, int oldLayerIndex, int newLayerIndex)
        {
            return new UndoRedoAction(null, UndoRedoType.ItemsLayerChanged, oldLayerIndex, newLayerIndex, items);
        }

        public static UndoRedoAction RopeRemoved(Rope rope)
        {
            return new UndoRedoAction(null, UndoRedoType.RopeRemoved, rope, null);
        }

        public static UndoRedoAction RopeAdded(Rope rope)
        {
            return new UndoRedoAction(null, UndoRedoType.RopeAdded, rope, null);
        }

        public static UndoRedoAction ItemZChanged(BoardItem item, int oldZ, int newZ)
        {
            return new UndoRedoAction(item, UndoRedoType.ItemZChanged, oldZ, newZ);
        }
        #endregion

        public void Undo()
        {
            UndoRedoBatch action = UndoList[UndoList.Count - 1];
            action.UndoRedo();
            action.SwitchActions();
            UndoList.RemoveAt(UndoList.Count - 1);
            RedoList.Add(action);
            parentBoard.ParentControl.UndoListChanged();
            parentBoard.ParentControl.RedoListChanged();
        }

        public void Redo()
        {
            UndoRedoBatch action = RedoList[RedoList.Count - 1];
            action.UndoRedo();
            action.SwitchActions();
            RedoList.RemoveAt(RedoList.Count - 1);
            UndoList.Add(action);
            parentBoard.ParentControl.UndoListChanged();
            parentBoard.ParentControl.RedoListChanged();
        }
    }

    public class UndoRedoBatch
    {
        public List<UndoRedoAction> Actions = new List<UndoRedoAction>();

        public void UndoRedo()
        {
            Board boardToSort = null;
            foreach (UndoRedoAction action in Actions) action.UndoRedo(out boardToSort);
            if (boardToSort != null) boardToSort.BoardItems.Sort();
        }

        public void SwitchActions()
        {
            foreach (UndoRedoAction action in Actions) action.SwitchAction();
        }
    }

    public class UndoRedoAction
    {
        private BoardItem item;
        private UndoRedoType type;
        private object ParamA;
        private object ParamB;
        private object ParamC;

        public UndoRedoAction(BoardItem item, UndoRedoType type, object ParamA, object ParamB)
        {
            this.item = item;
            this.type = type;
            this.ParamA = ParamA;
            this.ParamB = ParamB;
        }

        public UndoRedoAction(BoardItem item, UndoRedoType type, object ParamA, object ParamB, object ParamC)
            : this(item, type, ParamA, ParamB)
        {
            this.ParamC = ParamC;
        }

        public void UndoRedo(out Board boardToSort)
        {
            boardToSort = null;
            Board board;
            List<UndoRedoAction> foo;
            switch (type)
            {
                case UndoRedoType.ItemDeleted:
                    //item.Board.BoardItems.Add(item, true);
                    item.InsertItem();
                    break;
                case UndoRedoType.ItemAdded:
                    foo = new List<UndoRedoAction>(); //the undoPipe here has no meaning, we don't need any undo info anyway
                    item.RemoveItem(ref foo);
                    break;
                case UndoRedoType.ItemMoved:
                    Point oldPos = (Point)ParamA;
                    item.Move(oldPos.X, oldPos.Y);
                    break;
                case UndoRedoType.ItemFlipped:
                    ((IFlippable)item).Flip = !((IFlippable)item).Flip;
                    break;
                case UndoRedoType.LineRemoved:
                    board = ((MapleDot)ParamB).Board;
                    if (ParamC is FootholdLine)
                        board.BoardItems.FootholdLines.Add((FootholdLine)ParamC);
                    else if (ParamC is RopeLine)
                        board.BoardItems.RopeLines.Add((RopeLine)ParamC);
                    else throw new Exception("wrong type at undoredo, lineremoved");
                    ((MapleLine)ParamC).FirstDot = (MapleDot)ParamA;
                    ((MapleLine)ParamC).SecondDot = (MapleDot)ParamB;
                    ((MapleDot)ParamA).connectedLines.Add((MapleLine)ParamC);
                    ((MapleDot)ParamB).connectedLines.Add((MapleLine)ParamC);
                    break;
                case UndoRedoType.LineAdded:
                    foo = new List<UndoRedoAction>();
                    board = ((MapleDot)ParamB).Board;
                    if (ParamC is FootholdLine)
                        board.BoardItems.FootholdLines.Remove((FootholdLine)ParamC);
                    else if (ParamC is RopeLine)
                        board.BoardItems.RopeLines.Remove((RopeLine)ParamC);
                    else throw new Exception("wrong type at undoredo, lineadded");
                    ((MapleLine)ParamC).Remove(false, ref foo);
                    break;
                case UndoRedoType.ToolTipLinked:
                    ((ToolTip)item).CharacterToolTip = null;
                    ((ToolTipChar)ParamA).BoundTooltip = null;
                    break;
                case UndoRedoType.ToolTipUnlinked:
                    ((ToolTipChar)ParamA).BoundTooltip = (ToolTip)item;
                    break;
                case UndoRedoType.BackgroundMoved:
                    ((BackgroundInstance)item).BaseX = ((Point)ParamA).X;
                    ((BackgroundInstance)item).BaseY = ((Point)ParamA).Y;
                    break;
                case UndoRedoType.ItemsLinked:
                    item.ReleaseItem((BoardItem)ParamA);
                    break;
                case UndoRedoType.ItemsUnlinked:
                    item.BindItem((BoardItem)ParamA, (Microsoft.Xna.Framework.Point)ParamB);
                    break;
                case UndoRedoType.ItemsLayerChanged:
                    InputHandler.ClearSelectedItems(((BoardItem)((List<IContainsLayerInfo>)ParamC)[0]).Board);
                    foreach (IContainsLayerInfo layerInfoItem in (List<IContainsLayerInfo>)ParamC)
                        layerInfoItem.LayerNumber = (int)ParamA;
                    ((BoardItem)((List<IContainsLayerInfo>)ParamC)[0]).Board.Layers[(int)ParamA].RecheckTileSet();
                    ((BoardItem)((List<IContainsLayerInfo>)ParamC)[0]).Board.Layers[(int)ParamB].RecheckTileSet();
                    ((BoardItem)((List<IContainsLayerInfo>)ParamC)[0]).Board.ParentControl.RenderFrame();
                    break;
                case UndoRedoType.RopeAdded:
                    foo = new List<UndoRedoAction>();
                    ((Rope)ParamA).Remove(ref foo);
                    break;
                case UndoRedoType.RopeRemoved:
                    ((Rope)ParamA).Create();
                    break;
                case UndoRedoType.ItemZChanged:
                    item.Z = (int)ParamA;
                    item.Board.BoardItems.Sort();
                    break;
                case UndoRedoType.VRChanged:
                    //TODO
                    break;
                case UndoRedoType.MapCenterChanged:
                    //TODO
                    break;
            }
        }


        public void SwitchAction()
        {
            switch (type)
            {
                case UndoRedoType.ItemAdded:
                    type = UndoRedoType.ItemDeleted;
                    break;
                case UndoRedoType.ItemDeleted:
                    type = UndoRedoType.ItemAdded;
                    break;
                case UndoRedoType.LineAdded:
                    type = UndoRedoType.LineRemoved;
                    break;
                case UndoRedoType.LineRemoved:
                    type = UndoRedoType.LineAdded;
                    break;
                case UndoRedoType.ToolTipLinked:
                    type = UndoRedoType.ToolTipUnlinked;
                    break;
                case UndoRedoType.ToolTipUnlinked:
                    type = UndoRedoType.ToolTipLinked;
                    break;
                case UndoRedoType.ItemsLinked:
                    type = UndoRedoType.ItemsUnlinked;
                    break;
                case UndoRedoType.ItemsUnlinked:
                    type = UndoRedoType.ItemsLinked;
                    break;
                case UndoRedoType.RopeAdded:
                    type = UndoRedoType.RopeRemoved;
                    break;
                case UndoRedoType.RopeRemoved:
                    type = UndoRedoType.RopeAdded;
                    break;
                case UndoRedoType.ItemsLayerChanged:
                case UndoRedoType.BackgroundMoved:
                case UndoRedoType.ItemMoved:
                case UndoRedoType.MapCenterChanged:
                case UndoRedoType.ItemZChanged:
                case UndoRedoType.VRChanged:
                    object ParamBTemp = ParamB;
                    object ParamATemp = ParamA;
                    ParamA = ParamBTemp;
                    ParamB = ParamATemp;
                    break;
                case UndoRedoType.ItemFlipped:
                    break;
            }
        }
    }

    public enum UndoRedoType
    {
        ItemDeleted,
        ItemAdded,
        ItemMoved,
        ItemFlipped,
        LineRemoved,
        LineAdded,
        ToolTipLinked,
        ToolTipUnlinked,
        BackgroundMoved,
        ItemsUnlinked,
        ItemsLinked,
        ItemsLayerChanged,
        RopeRemoved,
        RopeAdded,
        ItemZChanged,
        VRChanged,
        MapCenterChanged
    }
}
