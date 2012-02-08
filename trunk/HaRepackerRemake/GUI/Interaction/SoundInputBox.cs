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

namespace HaRepacker.GUI.Interaction
{
    public partial class SoundInputBox : Form
    {
        public static bool Show(string title, out string name, out string path)
        {
            SoundInputBox form = new SoundInputBox(title);
            bool result = form.ShowDialog() == DialogResult.OK;
            name = form.nameResult;
            path = form.soundResult;
            return result;
        }

        private string nameResult = null;
        private string soundResult = null;

        public SoundInputBox(string title)
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;
            Text = title;
        }

        private void nameBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                okButton_Click(null, null);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (pathBox.Text != "" && pathBox.Text != null && nameBox.Text != "" && nameBox.Text != null && System.IO.File.Exists(pathBox.Text))
            {
                nameResult = nameBox.Text;
                soundResult = pathBox.Text;
                DialogResult = DialogResult.OK;
                Close();
            }
            else MessageBox.Show("Please enter valid input", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog { Title = "Select MP3 File", Filter = "MPEG-1 Audio Layer 3(*.mp3)|*.mp3" };
            if (dialog.ShowDialog() == DialogResult.OK) pathBox.Text = dialog.FileName;
        }
    }
}
