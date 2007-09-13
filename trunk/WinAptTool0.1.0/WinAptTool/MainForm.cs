﻿/*
 * Created by SharpDevelop.
 * User: zhou
 * Date: 2007-8-28
 * Time: 14:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using WinApt.Common;
using WinApt.Client.GUI;

namespace WinApt.Client
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
        public static string configFile = "configDB.xml";
        public static CmdMgr myCmdMgr = null;
        private bool initTag = true;

        public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}

		
		void MainFormLoad(object sender, EventArgs e)
		{
            cbTypes.SelectedIndex = 0;
            lvCatalog.Items[0].Selected = true;
		}

        private void lvCatalog_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateLvApp();
        }

        private void updateLvApp()
        {
            if (lvCatalog.SelectedItems.Count == 0)
            {
                return;
            }
            lvApps.Items.Clear();
            txtDesc.Clear();

            string catalog = lvCatalog.SelectedItems[0].Text;

            ArrayList items = myCmdMgr.FindAppInfoList(cbTypes.Text, catalog, txtSearch.Text);
            initTag = true;
            foreach (Hashtable item in items)
            {
                // 
                //TODO:now there is no icon, we will add later
                //
                ListViewItem lvitem = new ListViewItem();
                int state = (int)item["state"];
                lvitem.Checked = !(state == 0);
                if (state == 2)
                    lvitem.ForeColor = Color.Red;
                lvitem.Text = (string)item["name"];
                lvitem.SubItems.Add((string)item["version"]);
                lvitem.SubItems.Add("★★★");
                lvitem.Tag = (int)item["index"];
                lvApps.Items.Add(lvitem);
            }
            initTag = false;

        }

        private void lvApps_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvApps.SelectedItems.Count == 0)
            {
                return;
            }
            txtDesc.Clear();
            Hashtable item = myCmdMgr.FindAppInfo((int)lvApps.SelectedItems[0].Tag);
            int state = (int)item["state"];
            txtDesc.Text = (string)item["description"];
            txtDesc.AppendText("\n\nDownload Url:\n" + (string)item["url"]);
            if (state == 2)
            {
                txtDesc.AppendText("\nnew version: " + (string)item["version"] + "    " + (string)item["url"]);
            }
        }

        private void cbTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateLvApp();
        }

        private void txtDesc_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            //
            //TODO: download info
            //
            //MessageBox.Show("down load it!" + e.LinkText);
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void lvApps_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (initTag)
                return;
            AppInfoBase i=(AppInfoBase)myCmdMgr.InfoDB.Items[(int)e.Item.Tag];
            if (e.Item.Checked)
            {
                if (myCmdMgr.DelectItems.Contains(e.Item.Tag))
                    myCmdMgr.DelectItems.Remove(e.Item.Tag);
                else
                {
                    myCmdMgr.SelectItems.Add(e.Item.Tag);
                }
                i.setState(1);
            }
            else
            {
                if (myCmdMgr.SelectItems.Contains(e.Item.Tag))
                    myCmdMgr.SelectItems.Remove(e.Item.Tag);
                else
                {
                    myCmdMgr.DelectItems.Add(e.Item.Tag);
                }
                i.setState(0);
            }
            btnApply.Enabled = true;
        }
        /// <summary>
        /// apply the changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApply_Click(object sender, EventArgs e)
        {
            ExecuteForm exeForm = new ExecuteForm(myCmdMgr);
            exeForm.ShowDialog();
            //exeForm.Dispose();
            btnApply.Enabled = false;
            updateLvApp();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (btnApply.Enabled)
            {
                ExecuteForm exeForm = new ExecuteForm(myCmdMgr);
                exeForm.ShowDialog();
                //exeForm.Dispose();
            }
            Close();
        }

        private void btnCancal_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void lvApps_DoubleClick(object sender, EventArgs e)
        {
            if (lvApps.SelectedItems.Count == 0)
                return;
            if (!lvApps.SelectedItems[0].Checked)
            {
                if (myCmdMgr.ExploreFile((int)lvApps.SelectedItems[0].Tag))
                {
                    lvApps.SelectedItems[0].Checked = true;
                }
            }
        }

        private void btnProperty_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Bla, bla...");
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("WinApt tools.\nhttp://code.google.com/winapt");
        }
	}
}