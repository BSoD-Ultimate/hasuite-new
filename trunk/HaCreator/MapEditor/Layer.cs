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

namespace HaCreator.MapEditor
{
    //the difference between LayeredItem and this is that LayeredItems are actually 
    //ordered according to their layer (tiles\objs) in the editor. IContainsLayerInfo only
    //contains info about layers, and is not necessarily drawn according to it.
    public interface IContainsLayerInfo
    {
        int LayerNumber { get; set; }
    }

    /// <summary>
    /// Foothold group
    /// </summary>
    public class fhGroup
    {
        private Layer layer;
        /// <summary>
        /// Lines that are connected to this group.
        /// </summary>
        public List<FootholdLine> Lines = new List<FootholdLine>();

        /// <summary>
        /// The layer this group is under
        /// </summary>
        public Layer groupLayer
        {
            get { return layer; }
            set { layer = value; }
        }
    }

    public class Layer
    {
        private List<LayeredItem> items = new List<LayeredItem>(); //needed?
        public List<int> fhGroups = new List<int>();
        private int num;
        private Board board;
        private string _tS = null;

        public Layer(Board board)
        {
            this.board = board;
            if (board.Layers.Count == 10) throw new NotSupportedException("Cannot add more than 10 layers (why would you need that much anyway?)");
            num = board.Layers.Count;
            board.Layers.Add(this);
        }

        public List<LayeredItem> Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
            }
        }

        public int LayerNumber
        {
            get
            {
                return num;
            }
        }

        public string tS
        {
            get { return _tS; }
            set { _tS = value; board.ParentControl.LayerTSChanged(this); }
        }

        public static Layer GetLayerByNum(Board board, int num)
        {
            return board.Layers[num];
        }

        public void RecheckTileSet()
        {
            foreach (LayeredItem item in items)
                if (item is TileInstance)
                {
                    tS = ((TileInfo)item.BaseInfo).tS;
                    return;
                }
            tS = null;
        }
    }

    public abstract class LayeredItem : BoardItem, IContainsLayerInfo
    {
        private Layer layer;

        public LayeredItem(Board board, Layer layer, int x, int y, int z, bool beforeAdding)
            : base(board, x, y, z, beforeAdding)
        {
            this.layer = layer;
            layer.Items.Add(this);
        }

        public override void RemoveItem(ref List<UndoRedoAction> undoPipe)
        {
            layer.Items.Remove(this);
            base.RemoveItem(ref undoPipe);
        }

        public override void InsertItem()
        {
            layer.Items.Add(this);
            base.InsertItem();
        }

        public Layer Layer
        {
            get
            {
                return layer;
            }
            set
            {
                layer.Items.Remove(this);
                layer = value;
                layer.Items.Add(this);
                Board.BoardItems.Sort();
            }
        }

        public int LayerNumber
        {
            get { return Layer.LayerNumber; }
            set { Layer = Board.Layers[value]; }
        }
    }
}
