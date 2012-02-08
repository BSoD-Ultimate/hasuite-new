﻿/*  HaRepacker - WZ extractor and repacker
 * Copyright (C) 2009, 2010 haha01haha01
   
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
using System.Windows.Forms;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using System.Collections;

namespace HaRepackerLib
{
    public class WzNode : TreeNode
    {
        public WzNode(IWzObject SourceObject)
            : base(SourceObject.Name)
        {
            ParseChilds(SourceObject);
        }

        private void ParseChilds(IWzObject SourceObject)
        {
            if (SourceObject == null) throw new NullReferenceException("Cannot create a null WzNode");
            Tag = SourceObject;
            SourceObject.HRTag = this;
            if (SourceObject is WzFile) SourceObject = ((WzFile)SourceObject).WzDirectory;
            if (SourceObject is WzDirectory)
            {
                foreach (WzDirectory dir in ((WzDirectory)SourceObject).WzDirectories)
                    Nodes.Add(new WzNode(dir));
                foreach (WzImage img in ((WzDirectory)SourceObject).WzImages)
                    Nodes.Add(new WzNode(img));
            }
            else if (SourceObject is WzImage)
            {
                if (((WzImage)SourceObject).Parsed)
                    foreach (IWzImageProperty prop in ((WzImage)SourceObject).WzProperties)
                        Nodes.Add(new WzNode(prop));
            }
            else if (SourceObject is IPropertyContainer)
            {
                foreach (IWzImageProperty prop in ((IPropertyContainer)SourceObject).WzProperties)
                    Nodes.Add(new WzNode(prop));
            }
        }

        public void Delete()
        {
            Remove();
            if (Tag is IWzImageProperty) ((IWzImageProperty)Tag).ParentImage.Changed = true;
            ((IWzObject)Tag).Remove();
        }

        public bool CanHaveChilds
        {
            get
            {
                return (Tag is WzFile ||
                    Tag is WzDirectory ||
                    Tag is WzImage ||
                    Tag is IPropertyContainer);
            }
        }

        public static WzNode GetChildNode(WzNode parentNode, string name)
        {
            foreach (WzNode node in parentNode.Nodes)
                if (node.Text == name) 
                    return node;
            return null;
        }

        public static bool CanNodeBeInserted(WzNode parentNode, string name)
        {
            IWzObject obj = (IWzObject)parentNode.Tag;
            if (obj is IPropertyContainer) return ((IPropertyContainer)obj)[name] == null;
            else if (obj is WzDirectory) return ((WzDirectory)obj)[name] == null;
            else if (obj is WzFile) return ((WzFile)obj).WzDirectory[name] == null;
            else return false;
        }

        private void addObjInternal(IWzObject obj)
        {
            IWzObject TaggedObject = (IWzObject)Tag;
            if (TaggedObject is WzFile) TaggedObject = ((WzFile)TaggedObject).WzDirectory;
            if (TaggedObject is WzDirectory)
            {
                if (obj is WzDirectory)
                    ((WzDirectory)TaggedObject).AddDirectory((WzDirectory)obj);
                else if (obj is WzImage)
                    ((WzDirectory)TaggedObject).AddImage((WzImage)obj);
                else return;
            }
            else if (TaggedObject is WzImage)
            {
                if (!((WzImage)TaggedObject).Parsed) ((WzImage)TaggedObject).ParseImage();
                if (obj is IWzImageProperty)
                    ((WzImage)TaggedObject).AddProperty((IWzImageProperty)obj);
                else return;
            }
            else if (TaggedObject is IPropertyContainer)
            {
                if (obj is IWzImageProperty)
                    ((IPropertyContainer)TaggedObject).AddProperty((IWzImageProperty)obj);
                else return;
            }
            else return;
        }

        public bool MoveWzObjectInto(WzNode newParent)
        {
            if (CanNodeBeInserted(newParent, Text))
            {
                addObjInternal((IWzObject)newParent.Tag);
                return true;
            }
            else
            {
                MessageBox.Show("Cannot insert object \"" + Text + "\" because an object with the same name already exists. Skipping.", "Skipping Object", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }

        public bool MoveInto(WzNode newParent)
        {
            if (CanNodeBeInserted(newParent, Text))
            {
                addObjInternal((IWzObject)newParent.Tag);
                Parent.Nodes.Remove(this);
                newParent.Nodes.Add(this);
                return true;
            }
            else
            {
                MessageBox.Show("Cannot insert object \"" + Text + "\" because an object with the same name already exists. Skipping.", "Skipping Object", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }

        public bool AddNode(WzNode node)
        {
            if (CanNodeBeInserted(this, node.Text))
            {
                TryParseImage();
                this.Nodes.Add(node);
                addObjInternal((IWzObject)node.Tag);
                return true;
            }
            else
            {
                MessageBox.Show("Cannot insert node \"" + node.Text + "\" because a node with the same name already exists. Skipping.", "Skipping Node", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }

        private void TryParseImage()
        {
            if (Tag is WzImage && !((WzImage)Tag).Parsed)
            {
                ((WzImage)Tag).ParseImage();
                Reparse();
            }
        }

        public bool AddObject(IWzObject obj, UndoRedoManager undoRedoMan)
        {
            if (CanNodeBeInserted(this, obj.Name))
            {
                TryParseImage();
                addObjInternal(obj);
                WzNode node = new WzNode(obj);
                Nodes.Add(node);
                if (node.Tag is IWzImageProperty) ((IWzImageProperty)node.Tag).ParentImage.Changed = true;
                undoRedoMan.AddUndoBatch(new System.Collections.Generic.List<UndoRedoAction> { UndoRedoManager.ObjectAdded(this, node) });
                node.EnsureVisible();
                return true;
            }
            else
            {
                MessageBox.Show("Cannot insert object \"" + obj.Name + "\" because an object with the same name already exists. Skipping.", "Skipping Object", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }

        public void Reparse()
        {
            Nodes.Clear();
            ParseChilds((IWzObject)Tag);
        }

        public string GetTypeName()
        {
            return Tag.GetType().Name;
        }

        public void ChangeName(string name)
        {
            Text = name;
            ((IWzObject)Tag).Name = name;
            if (Tag is IWzImageProperty) ((IWzImageProperty)Tag).ParentImage.Changed = true;
        }

        public WzNode TopLevelNode
        {
            get
            {
                WzNode parent = this;
                while (parent.Level > 0)
                {
                    parent = (WzNode)parent.Parent;
                }
                return parent;
            }
        }
    }
}
