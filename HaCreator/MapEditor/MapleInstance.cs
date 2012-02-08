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
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MapleLib.WzLib.WzStructure.Data;
using MapleLib.WzLib.WzStructure;

namespace HaCreator.MapEditor
{
    public interface IFlippable
    {
        bool Flip { get; set; }
    }


    public struct ObjectInstanceQuest
    {
        public int questId;
        public QuestState state;

        public ObjectInstanceQuest(int questId, QuestState state)
        {
            this.questId = questId;
            this.state = state;
        }

        public override string ToString()
        {
            return questId.ToString() + " - " + Enum.GetName(typeof(QuestState), state);
        }
    }

    public class ObjectInstance : LayeredItem, IFlippable
    {
        private ObjectInfo baseInfo;
        private bool flip;
        private MapleBool _r;
        private string name;
        private MapleBool _hide;
        private MapleBool _reactor;
        private MapleBool _flow;
        private int? _rx, _ry, _cx, _cy;
        private string _tags;
        private List<ObjectInstanceQuest> questInfo;
        

        //private int zM;

        public ObjectInstance(ObjectInfo baseInfo, Layer layer, Board board, int x, int y, int z, MapleBool r, MapleBool hide, MapleBool reactor, MapleBool flow, int? rx, int? ry, int? cx, int? cy, string name, string tags, List<ObjectInstanceQuest> questInfo, bool flip, bool beforeAdding)
            : base(board, layer, x, y, z, beforeAdding)
        {
            this.baseInfo = baseInfo;
            this.flip = flip;
            this._r = r;
            this.name = name;
            this._hide = hide;
            this._reactor = reactor;
            this._flow = flow;
            this._rx = rx;
            this._ry = ry;
            this._cx = cx;
            this._cy = cy;
            this._tags = tags;
            this.questInfo = questInfo;
            //this.zM = zM;
            if (flip)
                X -= Width - 2 * Origin.X;
        }

        public override ItemTypes Type
        {
            get { return ItemTypes.Objects; }
        }

        public override MapleDrawableInfo BaseInfo
        {
            get { return baseInfo; }
        }

        public override Color GetColor(ItemTypes editedTypes, int selectedLayer, bool selected)
        {
            Color c = base.GetColor(editedTypes, selectedLayer, selected);
            if (_hide) c.R = (byte)UserSettings.HiddenLifeR;
            return c;
        }

        public bool Flip
        {
            get
            {
                return flip;
            }
            set
            {
                flip = value;
                int xFlipShift = Width - 2 * Origin.X;
                if (flip) X -= xFlipShift;
                else X += xFlipShift;
            }
        }

        public override void Draw(SpriteBatch sprite, Color color, int xShift, int yShift)
        {
            Rectangle destinationRectangle = new Rectangle((int)X + xShift - Origin.X, (int)Y + yShift - Origin.Y, Width, Height);
            //if (baseInfo.Texture == null) baseInfo.CreateTexture(sprite.GraphicsDevice);
            sprite.Draw(baseInfo.GetTexture(sprite), destinationRectangle, null, color, 0f, new Vector2(0, 0), Flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0 /*Layer.LayerNumber / 10f + Z / 1000f*/);
        }

        public override bool CheckIfLayerSelected(int selectedLayer)
        {
            return Layer.LayerNumber == selectedLayer;
        }

        public override void InsertItem()
        {
            base.InsertItem();
            //baseInfo.ParseOffsets(this, Layer, Board, X, Y);
        }

        public override System.Drawing.Bitmap Image
        {
            get
            {
                return baseInfo.Image;
            }
        }

        public override System.Drawing.Point Origin
        {
            get
            {
                return baseInfo.Origin;
            }
        }

        public string Name { get { return name; } set { name = value; } }
        public string tags { get { return _tags; } set { _tags = value; } }
        public MapleBool r { get { return _r; } set { _r = value; } }
        public MapleBool hide { get { return _hide; } set { _hide = value; } }
        public MapleBool flow { get { return _flow; } set { _flow = value; } }
        public MapleBool reactor { get { return _reactor; } set { _reactor = value; } }
        public int? rx { get { return _rx; } set { _rx = value; } }
        public int? ry { get { return _ry; } set { _ry = value; } }
        public int? cx { get { return _cx; } set { _cx = value; } }
        public int? cy { get { return _cy; } set { _cy = value; } }
        public List<ObjectInstanceQuest> QuestInfo { get { return questInfo; } set { questInfo = value; } }
    }

    public class TileInstance : LayeredItem, ISnappable
    {
        private TileInfo baseInfo;

        public TileInstance(TileInfo baseInfo, Layer layer, Board board, int x, int y, int z, bool beforeAdding)
            : base(board, layer, x, y, z, beforeAdding)
        {
            this.baseInfo = baseInfo;
            AttachToLayer(layer);
            //baseInfo.ParseOffsets(this, Board, Layer, X, Y);
        }

        public void AttachToLayer(Layer layer)
        {
            if (layer.tS != null && layer.tS != baseInfo.tS)
            {
                Board.BoardItems.TileObjs.Remove(this);
                layer.Items.Remove(this);
                throw new Exception("tile added to a layer with different tS");
            }
            else layer.tS = baseInfo.tS;
        }

        public void DoSnap()
        {
            TileSnapGroup tilegroup = (TileSnapGroup)TileSnap.tileCats[baseInfo.u];
            double closestDistance = 201d;
            TileInstance closestTile = null;
            TileSnapInfo closestInfo = null;
            foreach (BoardItem item in Board.BoardItems)
            {
                if (item is TileInstance)
                {
                    if (item.Selected || item.Equals(this)) continue;
                    TileInstance tile = (TileInstance)item;
                    int dx = tile.X - this.X, dy = tile.Y - this.Y;
                    if (dx > MultiBoard.FirstSnapVerification || dy > MultiBoard.FirstSnapVerification || Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)) > MultiBoard.FirstSnapVerification) //first verification to save time
                        continue;
                    foreach (TileSnapInfo snapInfo in tilegroup.tileList)
                    {
                        if (snapInfo.tileCat != tile.baseInfo.u) continue;
                        double distance = Math.Sqrt(Math.Pow(this.X - tile.X + snapInfo.snapx, 2) + Math.Pow(this.Y - tile.Y + snapInfo.snapy, 2));
                        if (distance > UserSettings.SnapDistance) continue;
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestTile = tile;
                            closestInfo = snapInfo;
                        }
                    }
                }
            }
            if (closestTile != null)
            {
                Point parentOffs = (Point)this.Parent.BoundItems[this];
                Point snapOffs = new Point(this.Parent.X + parentOffs.X - (closestTile.X - closestInfo.snapx), this.Parent.Y + parentOffs.Y - (closestTile.Y - closestInfo.snapy));
                foreach (BoardItem item in Board.SelectedItems)
                {
                    if (item.tempParent != null || item.Parent == null) continue;
                    parentOffs = (Point)item.Parent.BoundItems[item];
                    item.SnapMove(item.Parent.X + parentOffs.X - snapOffs.X, item.Parent.Y + parentOffs.Y - snapOffs.Y);
                }
                this.SnapMove(closestTile.X - closestInfo.snapx, closestTile.Y - closestInfo.snapy);
            }
        }

        public override ItemTypes Type
        {
            get { return ItemTypes.Tiles; }
        }

        public override MapleDrawableInfo BaseInfo
        {
            get { return baseInfo; }
        }

        public override void RemoveItem(ref List<UndoRedoAction> undoPipe)
        {
            Layer thisLayer = Layer;
            base.RemoveItem(ref undoPipe);
            thisLayer.RecheckTileSet();
        }

        public override void InsertItem()
        {
            base.InsertItem();
            AttachToLayer(Layer);
            //baseInfo.ParseOffsets(this, Board, Layer, X, Y);
        }

        public override void Draw(SpriteBatch sprite, Color color, int xShift, int yShift)
        {
            Rectangle destinationRectangle = new Rectangle((int)X + xShift - Origin.X, (int)Y + yShift - Origin.Y, Width, Height);
            //if (baseInfo.Texture == null) baseInfo.CreateTexture(sprite.GraphicsDevice);
            sprite.Draw(baseInfo.GetTexture(sprite), destinationRectangle, null, color, 0f, new Vector2(0f, 0f), /*Flip ? SpriteEffects.FlipHorizontally : */SpriteEffects.None, 0 /*Layer.LayerNumber / 10f + Z / 1000f*/);
        }

        public override bool CheckIfLayerSelected(int selectedLayer)
        {
            return Layer.LayerNumber == selectedLayer;
        }

        public override System.Drawing.Bitmap Image
        {
            get
            {
                return baseInfo.Image;
            }
        }

        public override System.Drawing.Point Origin
        {
            get
            {
                return baseInfo.Origin;
            }
        }
    }

    public class LifeInstance : BoardItem, IFlippable
    {
        private MapleDrawableInfo baseInfo;
        private int _rx0;
        private int _rx1;
        private int? mobTime;
        private string limitedname;
        private MapleBool flip;
        private MapleBool hide;
        private string type;
        private int? info; //no idea
        private int? team; //for carnival

        public LifeInstance(MapleDrawableInfo baseInfo, string type, Board board, int x, int y, int rx0, int rx1, string limitedname, int? mobTime, MapleBool flip, MapleBool hide, int? info, int? team, bool beforeAdding)
            : base(board, x, y, -1, beforeAdding)
        {
            if (type != "m" && type != "n") throw new NotSupportedException("Unknown life type @ LifeInstance..ctor");
            this.limitedname = limitedname;
            this.type = type;
            this.baseInfo = baseInfo;
            this._rx0 = rx0;
            this._rx1 = rx1;
            this.mobTime = mobTime;
            this.info = info;
            this.team = team;
            this.flip = flip;
            if (flip == true)
                X -= Width - 2 * Origin.X;
        }

        public override void Draw(SpriteBatch sprite, Color color, int xShift, int yShift)
        {
            Rectangle destinationRectangle = new Rectangle((int)X + xShift - Origin.X, (int)Y + yShift - Origin.Y, Width, Height);
            //if (baseInfo.Texture == null) baseInfo.CreateTexture(sprite.GraphicsDevice);
            sprite.Draw(baseInfo.GetTexture(sprite), destinationRectangle, null, color, 0f, new Vector2(0f, 0f), Flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
        }

        public bool Flip
        {
            get
            {
                return flip == true;
            }
            set
            {
                bool changeX = (flip == true) == value;
                flip = value;
                if (changeX)
                {
                    int xFlipShift = Width - 2 * Origin.X;
                    if (flip == true) X -= xFlipShift;
                    else X += xFlipShift;
                }
            }
        }

        public string LimitedName
        {
            get { return limitedname; }
            set { limitedname = value; }
        }

        public override Color GetColor(ItemTypes editedTypes, int selectedLayer, bool selected)
        {
            Color c = base.GetColor(editedTypes, selectedLayer, selected);
            if (hide == true) c.R = (byte)UserSettings.HiddenLifeR;
            return c;
        }

        public MapleBool Hide
        {
            get { return hide; }
            set { hide = value; }
        }

        public override MapleDrawableInfo BaseInfo
        {
            get { return baseInfo; }
        }

        public override bool CheckIfLayerSelected(int selectedLayer)
        {
            return true;
        }
        public override ItemTypes Type
        {
            get { return type == "m" ? ItemTypes.Mobs : ItemTypes.NPCs; }
        }

        public override System.Drawing.Bitmap Image
        {
            get
            {
                return baseInfo.Image;
            }
        }

        public override System.Drawing.Point Origin
        {
            get
            {
                return baseInfo.Origin;
            }
        }

        public int rx0
        {
            get
            {
                return _rx0;
            }
            set
            {
                _rx0 = value;
            }
        }

        public int rx1
        {
            get
            {
                return _rx1;
            }
            set
            {
                _rx1 = value;
            }
        }

        public int? MobTime
        {
            get
            {
                return mobTime;
            }
            set
            {
                mobTime = value;
            }
        }

        public string TypeStr { get { return type; } set { type = value; } }
        public int? Info { get { return info; } set { info = value; } }
        public int? Team { get { return team; } set { team = value; } }
    }

    public class PortalInstance : BoardItem
    {
        private PortalInfo baseInfo;
        private string _pn;
        private PortalType _pt;
        private string _tn;
        private int _tm;
        private string _script;
        private int? _delay;
        private MapleBool _hideTooltip;
        private MapleBool _onlyOnce;
        private int? _horizontalImpact;
        private int? _verticalImpact;
        private string _image;
        private int? _hRange;
        private int? _vRange;

        public PortalInstance(PortalInfo baseInfo, Board board, int x, int y, bool beforeAdding, string pn, PortalType pt, string tn, int tm, string script, int? delay, MapleBool hideTooltip, MapleBool onlyOnce, int? horizontalImpact, int? verticalImpact, string image, int? hRange, int? vRange)
            : base(board, x, y, -1, beforeAdding)
        {
            this.baseInfo = baseInfo;
            _pn = pn;
            _pt = pt;
            _tn = tn;
            _tm = tm;
            _script = script;
            _delay = delay;
            _hideTooltip = hideTooltip;
            _onlyOnce = onlyOnce;
            _horizontalImpact = horizontalImpact;
            _verticalImpact = verticalImpact;
            _image = image;
            _hRange = hRange;
            _vRange = vRange;
        }

        public override void Draw(SpriteBatch sprite, Color color, int xShift, int yShift)
        {
            /*if (BeforeAdding)
            {
                Rectangle destinationRectangle = new Rectangle((int)X + xShift - Origin.X, (int)Y + yShift - Origin.Y, Width, Height);
                //if (baseInfo.Texture == null) baseInfo.CreateTexture(sprite.GraphicsDevice);
                sprite.Draw(baseInfo.GetTexture(sprite), destinationRectangle, null, color, 0f, new Vector2(0f, 0f), SpriteEffects.None, 1f);
            }
            else
            {*/
            Rectangle destinationRectangle = new Rectangle((int)X + xShift - Origin.X, (int)Y + yShift - Origin.Y, Width, Height);
            //if (baseInfo.Texture == null) baseInfo.CreateTexture(sprite.GraphicsDevice);
            sprite.Draw(baseInfo.GetTexture(sprite), destinationRectangle, null, color, 0f, new Vector2(0f, 0f), SpriteEffects.None, 1f);
            //}
        }

        public override bool CheckIfLayerSelected(int selectedLayer)
        {
            return true;
        }

        public override MapleDrawableInfo BaseInfo
        {
            get { return baseInfo; }
        }

        public override ItemTypes Type
        {
            get { return ItemTypes.Portals; }
        }

        public override System.Drawing.Bitmap Image
        {
            get
            {
                return baseInfo.Image;
            }
        }

        public override System.Drawing.Point Origin
        {
            get
            {
                return baseInfo.Origin;
            }
        }

        public string image
        {
            get { return _image; }
            set { _image = value; }
        }

        public string pn
        {
            get
            {
                return _pn;
            }
            set
            {
                _pn = value;
            }
        }

        public PortalType pt
        {
            get
            {
                return _pt;
            }
            set
            {
                _pt = value;
                baseInfo = PortalInfo.GetPortalInfoByType(value);
            }
        }

        public string tn
        {
            get
            {
                return _tn;
            }
            set
            {
                _tn = value;
            }
        }

        public int tm
        {
            get
            {
                return _tm;
            }
            set
            {
                _tm = value;
            }
        }

        public string script
        {
            get
            {
                return _script;
            }
            set
            {
                _script = value;
            }
        }

        public int? delay
        {
            get
            {
                return _delay;
            }
            set
            {
                _delay = value;
            }
        }

        public MapleBool hideTooltip
        {
            get
            {
                return _hideTooltip;
            }
            set
            {
                _hideTooltip = value;
            }
        }

        public MapleBool onlyOnce
        {
            get
            {
                return _onlyOnce;
            }
            set
            {
                _onlyOnce = value;
            }
        }

        public int? horizontalImpact
        {
            get
            {
                return _horizontalImpact;
            }
            set
            {
                _horizontalImpact = value;
            }
        }

        public int? verticalImpact
        {
            get
            {
                return _verticalImpact;
            }
            set
            {
                _verticalImpact = value;
            }
        }

        public int? hRange
        {
            get { return _hRange; }
            set { _hRange = value; }
        }

        public int? vRange
        {
            get { return _vRange; }
            set { _vRange = value; }
        }
    }

    public class ReactorInstance : BoardItem, IFlippable
    {
        private ReactorInfo baseInfo;
        private int reactorTime;
        private bool flip;
        private string name;

        public ReactorInstance(ReactorInfo baseInfo, Board board, int x, int y, int reactorTime, string name, bool flip, bool beforeAdding)
            : base(board, x, y, -1, beforeAdding)
        {
            this.baseInfo = baseInfo;
            this.reactorTime = reactorTime;
            this.flip = flip;
            this.name = name;
            if (flip)
                X -= Width - 2 * Origin.X;
        }

        public override void Draw(SpriteBatch sprite, Color color, int xShift, int yShift)
        {
            Rectangle destinationRectangle = new Rectangle((int)X + xShift - Origin.X, (int)Y + yShift - Origin.Y, Width, Height);
            //if (baseInfo.Texture == null) baseInfo.CreateTexture(sprite.GraphicsDevice);
            sprite.Draw(baseInfo.GetTexture(sprite), destinationRectangle, null, color, 0f, new Vector2(0f, 0f), Flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
        }

        public override MapleDrawableInfo BaseInfo
        {
            get { return baseInfo; }
        }

        public bool Flip
        {
            get
            {
                return flip;
            }
            set
            {
                flip = value;
                int xFlipShift = Width - 2 * Origin.X;
                if (flip) X -= xFlipShift;
                else X += xFlipShift;
            }
        }

        public override bool CheckIfLayerSelected(int selectedLayer)
        {
            return true;
        }

        public override ItemTypes Type
        {
            get { return ItemTypes.Reactors; }
        }

        public override System.Drawing.Bitmap Image
        {
            get
            {
                return baseInfo.Image;
            }
        }

        public override System.Drawing.Point Origin
        {
            get
            {
                return baseInfo.Origin;
            }
        }

        public int ReactorTime
        {
            get { return reactorTime; }
            set { reactorTime = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }

    public class BackgroundInstance : BoardItem, IFlippable
    {
        private BackgroundInfo baseInfo;
        private bool flip;
        private int _a; //alpha
        private int _cx; //copy x
        private int _cy; //copy y
        private int _rx;
        private int _ry;
        private bool _front;
        private BackgroundType _type;

        public BackgroundInstance(BackgroundInfo baseInfo, Board board, int x, int y, int z, int rx, int ry, int cx, int cy, BackgroundType type, int a, bool front, bool flip, bool beforeAdding)
            : base(board, x, y, z, beforeAdding)
        {
            this.baseInfo = baseInfo;
            this.flip = flip;
            _rx = rx;
            _ry = ry;
            _cx = cx;
            _cy = cy;
            _a = a;
            _type = type;
            _front = front;
            if (flip)
                X -= Width - 2 * Origin.X;
        }

        public override ItemTypes Type
        {
            get { return ItemTypes.Backgrounds; }
        }

        public bool Flip
        {
            get
            {
                return flip;
            }
            set
            {
                flip = value;
                int xFlipShift = Width - 2 * Origin.X;
                if (flip) X -= xFlipShift;
                else X += xFlipShift;
            }
        }

        public override void Draw(SpriteBatch sprite, Color color, int xShift, int yShift)
        {
            Rectangle destinationRectangle;
            /*if (ApplicationSettings.emulateParallax)
                destinationRectangle = new Rectangle((int)X - Origin.X, (int)Y - Origin.Y, Width, Height);
            else */destinationRectangle = new Rectangle((int)X + xShift - Origin.X, (int)Y + yShift - Origin.Y, Width, Height);
            sprite.Draw(baseInfo.GetTexture(sprite), destinationRectangle, null, color, 0f, new Vector2(0f, 0f), Flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1);
        }

        public override MapleDrawableInfo BaseInfo
        {
            get { return baseInfo; }
        }

        public override bool CheckIfLayerSelected(int selectedLayer)
        {
            return true;
        }

        public override System.Drawing.Bitmap Image
        {
            get
            {
                return baseInfo.Image;
            }
        }

        public override System.Drawing.Point Origin
        {
            get
            {
                return baseInfo.Origin;
            }
        }

        //parallax + undo\redo is shit. I don't like this way either.
        public int BaseX { get { return (int)base.position.X; } set { base.position.X = value; } }
        public int BaseY { get { return (int)base.position.Y; } set { base.position.Y = value; } }

        public int rx
        {
            get { return _rx; }
            set { _rx = value; }
        }

        public int ry
        {
            get { return _ry; }
            set { _ry = value; }
        }

        public int cx
        {
            get { return _cx; }
            set { _cx = value; }
        }

        public int cy
        {
            get { return _cy; }
            set { _cy = value; }
        }

        public int a
        {
            get { return _a; }
            set { _a = value; }
        }

        public BackgroundType type
        {
            get { return _type; }
            set { _type = value; }
        }

        public bool front
        {
            get { return _front; }
            set { _front = value; }
        }

        public int CalculateBackgroundPosX()
        {
            return (rx * (Board.hScroll - Board.CenterPoint.X + 400) / 100) + base.X /*- Origin.X*/ + 400 - Board.CenterPoint.X + Board.hScroll;
        }

        public int CalculateBackgroundPosY()
        {
            return (ry * (Board.vScroll - Board.CenterPoint.Y + 300) / 100) + base.Y /*- Origin.X*/ + 300 - Board.CenterPoint.Y + Board.vScroll;
        }

        public int ReverseBackgroundPosX(int bgPos)
        {
            return bgPos - Board.hScroll + Board.CenterPoint.X - 400 - (rx * (Board.hScroll - Board.CenterPoint.X + 400) / 100);
        }

        public int ReverseBackgroundPosY(int bgPos)
        {
            return bgPos - Board.vScroll + Board.CenterPoint.Y - 300 - (ry * (Board.vScroll - Board.CenterPoint.Y + 300) / 100);
        }

        public override int X
        {
            get
            {
                if (UserSettings.emulateParallax)
                    return CalculateBackgroundPosX();
                else return base.X;
            }
            set
            {
                int newX;
                if (UserSettings.emulateParallax)
                    newX = ReverseBackgroundPosX(value);
                else newX = value;
                base.Move(newX, base.Y);
            }
        }

        public override int Y
        {
            get
            {
                if (UserSettings.emulateParallax)
                    return CalculateBackgroundPosY();
                else return base.Y;
            }
            set
            {
                int newY;
                if (UserSettings.emulateParallax)
                    newY = ReverseBackgroundPosY(value);
                else newY = value;
                base.Move(base.X, newY);
            }
        }

        public override void Move(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}