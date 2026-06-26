using System;
using System.Drawing;
using System.Windows.Forms;
using ScumChecker.Controls;

namespace ScumChecker
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelToolsListHost;



        // ===== Root layout
        private System.Windows.Forms.Panel panelSidebar;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelPages;

        // Sidebar buttons
        private System.Windows.Forms.Label lblAppTitle;
        private System.Windows.Forms.Button btnNavNative;
        private System.Windows.Forms.Button btnNavSteam;
        private System.Windows.Forms.Button btnNavTools;
        private System.Windows.Forms.Button btnNavQuick;
        private System.Windows.Forms.Panel sidebarSpacer;

        // Footer
        private System.Windows.Forms.LinkLabel linkGitHub;
        private System.Windows.Forms.LinkLabel linkBio;

        // ===== Pages
        private System.Windows.Forms.Panel pageNative;
        private System.Windows.Forms.Panel pageSteam;
        private System.Windows.Forms.Panel pageTools;
        private System.Windows.Forms.Panel pageQuick;

        // ===== Native (Summary/Top/Split)
        private System.Windows.Forms.Panel panelSummary;
        private System.Windows.Forms.Label lblVerdictTitle;
        private System.Windows.Forms.Label lblVerdict;
        private System.Windows.Forms.Label lblCountHigh;
        private System.Windows.Forms.Label lblCountMedium;
        private System.Windows.Forms.Label lblCountLow;
        private System.Windows.Forms.Label lblCountInfo;

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnCopyReport;
        private System.Windows.Forms.CheckBox chkHigh;
        private System.Windows.Forms.CheckBox chkMedium;
        private System.Windows.Forms.CheckBox chkLow;
        private System.Windows.Forms.CheckBox chkInfo;
        private System.Windows.Forms.Label lblStatus;

        private System.Windows.Forms.TableLayoutPanel steamTopLayout;


        // Flat progress bar = 2 panels
        private System.Windows.Forms.Panel panelProgressTrack;
        private System.Windows.Forms.Panel panelProgressFill;

        private System.Windows.Forms.SplitContainer splitNative;
        private System.Windows.Forms.DataGridView dgvFindings;
        private System.Windows.Forms.TextBox txtLog;

        private System.Windows.Forms.DataGridViewTextBoxColumn colSeverity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCategory;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWhat;
        private System.Windows.Forms.DataGridViewTextBoxColumn colReason;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAction;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDetails;

        // ===== Steam page
        private System.Windows.Forms.Panel panelSteamTop;
        private System.Windows.Forms.Label lblSteamTitle;
        private System.Windows.Forms.Label lblSteamHint;
        private System.Windows.Forms.Panel panelSteamGridHost;
        private System.Windows.Forms.DataGridView dgvSteam;

        private System.Windows.Forms.DataGridViewTextBoxColumn colSteamId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPersona;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAccountName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMostRecent;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTimestamp;

        // ===== Tools page
        private System.Windows.Forms.Panel panelToolsTop;
        private System.Windows.Forms.Label lblToolsTitle;
        private System.Windows.Forms.Label lblToolsDesc;

        private System.Windows.Forms.Panel panelToolsGridHost;
        private System.Windows.Forms.DataGridView dgvTools;

        private System.Windows.Forms.Panel panelToolsBottom;
        private System.Windows.Forms.Button btnOpenTool;
        private System.Windows.Forms.Button btnLocateTool;
        private System.Windows.Forms.Button btnDownloadTool;
        private System.Windows.Forms.Label lblToolsHint;

        private System.Windows.Forms.DataGridViewTextBoxColumn colTool;
        private System.Windows.Forms.DataGridViewTextBoxColumn colToolStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colToolPath;

        // ===== Quick page
        // Ну типо открытие быстрых мест и прочая херня

        private System.Windows.Forms.Button btnOpenPrefetch;
        private System.Windows.Forms.Button btnOpenEventViewer;
        private System.Windows.Forms.Button btnOpenLocalAppData;

        private System.Windows.Forms.Button btnOpenChromeProfile;
        private System.Windows.Forms.Button btnOpenEdgeProfile;
        private System.Windows.Forms.Button btnOpenFirefoxProfiles;
        private System.Windows.Forms.Button btnOpenBrowserCache;

        private System.Windows.Forms.Button btnOpenRecent;
        private System.Windows.Forms.Button btnOpenDesktop;

        private System.Windows.Forms.Panel panelQuickTop;
        private System.Windows.Forms.Label lblQuickTitle;
        private System.Windows.Forms.Label lblQuickDesc;
        private System.Windows.Forms.FlowLayoutPanel flowQuick;

        // Проверка на покупок читов 
        private Button btnOpenYougame = null!;
        private Button btnOpenDragonHack = null!;
        private Button btnOpenUpGame = null!;
        private Button btnOpenIndustries = null!;
        private Button btnOpenCheatrise = null!;
        private Button btnOpenCyberhack = null!;
        private Button btnOpenSoftix = null!;
        private Button btnOpenScumFolder = null!;

        // Ну типо открытие быстрых мест 
        private System.Windows.Forms.Button btnOpenRegedit;
        private System.Windows.Forms.Button btnOpenTemp;
        private System.Windows.Forms.Button btnOpenDownloads;
        private System.Windows.Forms.Button btnOpenWindowsUpdate;
        private System.Windows.Forms.Button btnOpenAppData;
        private System.Windows.Forms.Button btnOpenSteamConfig;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            btnOpenPrefetch = new Button();
            btnOpenEventViewer = new Button();
            btnOpenLocalAppData = new Button();
            btnOpenChromeProfile = new Button();
            btnOpenEdgeProfile = new Button();
            btnOpenFirefoxProfiles = new Button();
            btnOpenBrowserCache = new Button();
            btnOpenRecent = new Button();
            btnOpenDesktop = new Button();
            panelSidebar = new Panel();
            ezbio = new Button();
            Git_button = new Button();
            panelFooter = new Panel();
            lblLang = new Label();
            cmbLang = new ComboBox();
            btnNavQuick = new Button();
            btnNavTools = new Button();
            btnNavSteam = new Button();
            btnNavNative = new Button();
            sidebarSpacer = new Panel();
            panel1 = new Panel();
            lblAppTitle = new Label();
            button1 = new Button();
            panelToolsListHost = new Panel();
            panelMain = new Panel();
            panelPages = new Panel();
            pageQuick = new Panel();
            flowQuick = new FlowLayoutPanel();
            btnOpenRegedit = new Button();
            btnOpenTemp = new Button();
            btnOpenDownloads = new Button();
            btnOpenWindowsUpdate = new Button();
            btnOpenAppData = new Button();
            btnOpenSteamConfig = new Button();
            panelQuickTop = new Panel();
            lblQuickTitle = new Label();
            lblQuickDesc = new Label();
            pageTools = new Panel();
            panelToolsGridHost = new Panel();
            dgvTools = new DataGridView();
            colTool = new DataGridViewTextBoxColumn();
            colToolStatus = new DataGridViewTextBoxColumn();
            colToolPath = new DataGridViewTextBoxColumn();
            panelToolsBottom = new Panel();
            btnOpenTool = new Button();
            btnLocateTool = new Button();
            btnDownloadTool = new Button();
            lblToolsHint = new Label();
            panelToolsTop = new Panel();
            lblToolsTitle = new Label();
            lblToolsDesc = new Label();
            pageSteam = new Panel();
            panelSteamGridHost = new Panel();
            dgvSteam = new DataGridView();
            colSteamId = new DataGridViewTextBoxColumn();
            colPersona = new DataGridViewTextBoxColumn();
            colAccountName = new DataGridViewTextBoxColumn();
            colMostRecent = new DataGridViewTextBoxColumn();
            colTimestamp = new DataGridViewTextBoxColumn();
            panelSteamTop = new Panel();
            lblSteamTitle = new Label();
            lblSteamHint = new Label();
            pageNative = new Panel();
            splitNative = new SplitContainer();
            dgvFindings = new DataGridView();
            colSeverity = new DataGridViewTextBoxColumn();
            colCategory = new DataGridViewTextBoxColumn();
            colWhat = new DataGridViewTextBoxColumn();
            colReason = new DataGridViewTextBoxColumn();
            colAction = new DataGridViewTextBoxColumn();
            colDetails = new DataGridViewTextBoxColumn();
            txtLog = new TextBox();
            panelTop = new Panel();
            btnScan = new Button();
            btnCancel = new Button();
            btnCopyReport = new Button();
            chkHigh = new CheckBox();
            chkMedium = new CheckBox();
            chkLow = new CheckBox();
            chkInfo = new CheckBox();
            lblStatus = new Label();
            panelProgressTrack = new Panel();
            panelProgressFill = new Panel();
            panelSummary = new Panel();
            lblVerdictTitle = new Label();
            lblVerdict = new Label();
            lblCountHigh = new Label();
            lblCountMedium = new Label();
            lblCountLow = new Label();
            lblCountInfo = new Label();
            PanelLogo = new Panel();
            button3akrit = new Button();
            buttonSvernut = new Button();
            linkGitHub = new LinkLabel();
            linkBio = new LinkLabel();
            panelSidebar.SuspendLayout();
            panelFooter.SuspendLayout();
            panel1.SuspendLayout();
            panelMain.SuspendLayout();
            panelPages.SuspendLayout();
            pageQuick.SuspendLayout();
            flowQuick.SuspendLayout();
            panelQuickTop.SuspendLayout();
            pageTools.SuspendLayout();
            panelToolsGridHost.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTools).BeginInit();
            panelToolsBottom.SuspendLayout();
            panelToolsTop.SuspendLayout();
            pageSteam.SuspendLayout();
            panelSteamGridHost.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSteam).BeginInit();
            panelSteamTop.SuspendLayout();
            pageNative.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitNative).BeginInit();
            splitNative.Panel1.SuspendLayout();
            splitNative.Panel2.SuspendLayout();
            splitNative.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvFindings).BeginInit();
            panelTop.SuspendLayout();
            panelProgressTrack.SuspendLayout();
            panelSummary.SuspendLayout();
            PanelLogo.SuspendLayout();
            SuspendLayout();
            // 
            // btnOpenPrefetch
            // 
            btnOpenPrefetch.Location = new Point(570, 15);
            btnOpenPrefetch.Name = "btnOpenPrefetch";
            btnOpenPrefetch.Size = new Size(75, 23);
            btnOpenPrefetch.TabIndex = 7;
            // 
            // btnOpenEventViewer
            // 
            btnOpenEventViewer.Location = new Point(651, 15);
            btnOpenEventViewer.Name = "btnOpenEventViewer";
            btnOpenEventViewer.Size = new Size(75, 23);
            btnOpenEventViewer.TabIndex = 8;
            // 
            // btnOpenLocalAppData
            // 
            btnOpenLocalAppData.Location = new Point(489, 15);
            btnOpenLocalAppData.Name = "btnOpenLocalAppData";
            btnOpenLocalAppData.Size = new Size(75, 23);
            btnOpenLocalAppData.TabIndex = 6;
            // 
            // btnOpenChromeProfile
            // 
            btnOpenChromeProfile.Location = new Point(732, 15);
            btnOpenChromeProfile.Name = "btnOpenChromeProfile";
            btnOpenChromeProfile.Size = new Size(75, 23);
            btnOpenChromeProfile.TabIndex = 9;
            // 
            // btnOpenEdgeProfile
            // 
            btnOpenEdgeProfile.Location = new Point(813, 15);
            btnOpenEdgeProfile.Name = "btnOpenEdgeProfile";
            btnOpenEdgeProfile.Size = new Size(75, 23);
            btnOpenEdgeProfile.TabIndex = 10;
            // 
            // btnOpenFirefoxProfiles
            // 
            btnOpenFirefoxProfiles.Location = new Point(894, 15);
            btnOpenFirefoxProfiles.Name = "btnOpenFirefoxProfiles";
            btnOpenFirefoxProfiles.Size = new Size(75, 23);
            btnOpenFirefoxProfiles.TabIndex = 11;
            // 
            // btnOpenBrowserCache
            // 
            btnOpenBrowserCache.Location = new Point(3, 44);
            btnOpenBrowserCache.Name = "btnOpenBrowserCache";
            btnOpenBrowserCache.Size = new Size(75, 23);
            btnOpenBrowserCache.TabIndex = 12;
            // 
            // btnOpenRecent
            // 
            btnOpenRecent.Location = new Point(84, 44);
            btnOpenRecent.Name = "btnOpenRecent";
            btnOpenRecent.Size = new Size(75, 23);
            btnOpenRecent.TabIndex = 13;
            // 
            // btnOpenDesktop
            // 
            btnOpenDesktop.Location = new Point(165, 44);
            btnOpenDesktop.Name = "btnOpenDesktop";
            btnOpenDesktop.Size = new Size(75, 23);
            btnOpenDesktop.TabIndex = 14;
            // 
            // panelSidebar
            // 
            panelSidebar.BackColor = Color.FromArgb(1, 1, 9);
            panelSidebar.Controls.Add(ezbio);
            panelSidebar.Controls.Add(Git_button);
            panelSidebar.Controls.Add(panelFooter);
            panelSidebar.Controls.Add(btnNavQuick);
            panelSidebar.Controls.Add(btnNavTools);
            panelSidebar.Controls.Add(btnNavSteam);
            panelSidebar.Controls.Add(btnNavNative);
            panelSidebar.Controls.Add(sidebarSpacer);
            panelSidebar.Controls.Add(panel1);
            panelSidebar.Dock = DockStyle.Left;
            panelSidebar.Location = new Point(0, 0);
            panelSidebar.Name = "panelSidebar";
            panelSidebar.Padding = new Padding(12);
            panelSidebar.Size = new Size(190, 720);
            panelSidebar.TabIndex = 1;
            // 
            // ezbio
            // 
            ezbio.BackColor = Color.Transparent;
            ezbio.BackgroundImage = Properties.Resources.ezbio1;
            ezbio.BackgroundImageLayout = ImageLayout.Zoom;
            ezbio.Cursor = Cursors.Hand;
            ezbio.Dock = DockStyle.Bottom;
            ezbio.FlatAppearance.BorderSize = 0;
            ezbio.FlatAppearance.MouseDownBackColor = Color.FromArgb(70, 120, 110, 255);
            ezbio.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 120, 110, 255);
            ezbio.FlatStyle = FlatStyle.Flat;
            ezbio.ForeColor = Color.Transparent;
            ezbio.Location = new Point(12, 564);
            ezbio.Name = "ezbio";
            ezbio.Size = new Size(166, 56);
            ezbio.TabIndex = 7;
            ezbio.TabStop = false;
            ezbio.UseVisualStyleBackColor = false;
            ezbio.Click += Git_button_Click;
            // 
            // Git_button
            // 
            Git_button.BackColor = Color.Transparent;
            Git_button.BackgroundImage = Properties.Resources.Github_ico2;
            Git_button.BackgroundImageLayout = ImageLayout.Zoom;
            Git_button.Cursor = Cursors.Hand;
            Git_button.Dock = DockStyle.Bottom;
            Git_button.FlatAppearance.BorderSize = 0;
            Git_button.FlatAppearance.MouseDownBackColor = Color.FromArgb(70, 120, 110, 255);
            Git_button.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 120, 110, 255);
            Git_button.FlatStyle = FlatStyle.Flat;
            Git_button.ForeColor = Color.Transparent;
            Git_button.Location = new Point(12, 620);
            Git_button.Name = "Git_button";
            Git_button.Size = new Size(166, 56);
            Git_button.TabIndex = 0;
            Git_button.TabStop = false;
            Git_button.UseVisualStyleBackColor = false;
            Git_button.Click += ezka_button_Click;
            // 
            // panelFooter
            // 
            panelFooter.BackColor = Color.FromArgb(1, 1, 9);
            panelFooter.Controls.Add(lblLang);
            panelFooter.Controls.Add(cmbLang);
            panelFooter.Dock = DockStyle.Bottom;
            panelFooter.Location = new Point(12, 676);
            panelFooter.Name = "panelFooter";
            panelFooter.Padding = new Padding(12, 8, 12, 8);
            panelFooter.Size = new Size(166, 32);
            panelFooter.TabIndex = 1;
            panelFooter.Paint += panelFooter_Paint;
            // 
            // lblLang
            // 
            lblLang.AutoSize = true;
            lblLang.BackColor = Color.FromArgb(1, 1, 9);
            lblLang.ForeColor = Color.Gainsboro;
            lblLang.Location = new Point(9, 9);
            lblLang.Name = "lblLang";
            lblLang.Size = new Size(36, 15);
            lblLang.TabIndex = 2;
            lblLang.Text = "Lang:";
            // 
            // cmbLang
            // 
            cmbLang.BackColor = Color.FromArgb(1, 1, 9);
            cmbLang.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLang.FlatStyle = FlatStyle.Flat;
            cmbLang.ForeColor = Color.Gainsboro;
            cmbLang.FormattingEnabled = true;
            cmbLang.Items.AddRange(new object[] { "RU", "EN" });
            cmbLang.Location = new Point(51, 6);
            cmbLang.Name = "cmbLang";
            cmbLang.Size = new Size(112, 23);
            cmbLang.TabIndex = 3;
            // 
            // btnNavQuick
            // 
            btnNavQuick.BackColor = Color.FromArgb(12, 12, 18);
            btnNavQuick.Dock = DockStyle.Top;
            btnNavQuick.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 80);
            btnNavQuick.FlatStyle = FlatStyle.Flat;
            btnNavQuick.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnNavQuick.ForeColor = Color.Gainsboro;
            btnNavQuick.Location = new Point(12, 230);
            btnNavQuick.Name = "btnNavQuick";
            btnNavQuick.Size = new Size(166, 44);
            btnNavQuick.TabIndex = 0;
            btnNavQuick.Text = "Быстрый доступ";
            btnNavQuick.UseVisualStyleBackColor = false;
            // 
            // btnNavTools
            // 
            btnNavTools.BackColor = Color.FromArgb(12, 12, 18);
            btnNavTools.Dock = DockStyle.Top;
            btnNavTools.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 80);
            btnNavTools.FlatStyle = FlatStyle.Flat;
            btnNavTools.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnNavTools.ForeColor = Color.Gainsboro;
            btnNavTools.Location = new Point(12, 186);
            btnNavTools.Name = "btnNavTools";
            btnNavTools.Size = new Size(166, 44);
            btnNavTools.TabIndex = 1;
            btnNavTools.Text = "Программы";
            btnNavTools.UseVisualStyleBackColor = false;
            // 
            // btnNavSteam
            // 
            btnNavSteam.BackColor = Color.FromArgb(12, 12, 18);
            btnNavSteam.Dock = DockStyle.Top;
            btnNavSteam.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 80);
            btnNavSteam.FlatStyle = FlatStyle.Flat;
            btnNavSteam.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnNavSteam.ForeColor = Color.Gainsboro;
            btnNavSteam.Location = new Point(12, 142);
            btnNavSteam.Name = "btnNavSteam";
            btnNavSteam.Size = new Size(166, 44);
            btnNavSteam.TabIndex = 2;
            btnNavSteam.Text = "Steam";
            btnNavSteam.UseVisualStyleBackColor = false;
            // 
            // btnNavNative
            // 
            btnNavNative.BackColor = Color.FromArgb(16, 12, 28);
            btnNavNative.Dock = DockStyle.Top;
            btnNavNative.FlatAppearance.BorderColor = Color.FromArgb(120, 110, 255);
            btnNavNative.FlatStyle = FlatStyle.Flat;
            btnNavNative.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnNavNative.ForeColor = Color.White;
            btnNavNative.Location = new Point(12, 98);
            btnNavNative.Name = "btnNavNative";
            btnNavNative.Size = new Size(166, 44);
            btnNavNative.TabIndex = 3;
            btnNavNative.Text = "Нативная проверка";
            btnNavNative.UseVisualStyleBackColor = false;
            // 
            // sidebarSpacer
            // 
            sidebarSpacer.BackColor = Color.Transparent;
            sidebarSpacer.Dock = DockStyle.Top;
            sidebarSpacer.Location = new Point(12, 88);
            sidebarSpacer.Name = "sidebarSpacer";
            sidebarSpacer.Size = new Size(166, 10);
            sidebarSpacer.TabIndex = 4;
            // 
            // panel1
            // 
            panel1.Controls.Add(lblAppTitle);
            panel1.Controls.Add(button1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(12, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(166, 76);
            panel1.TabIndex = 8;
            // 
            // lblAppTitle
            // 
            lblAppTitle.Dock = DockStyle.Top;
            lblAppTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblAppTitle.ForeColor = Color.White;
            lblAppTitle.Location = new Point(45, 0);
            lblAppTitle.Name = "lblAppTitle";
            lblAppTitle.Size = new Size(121, 52);
            lblAppTitle.TabIndex = 5;
            lblAppTitle.Text = "            ScumChecker";
            lblAppTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // button1
            // 
            button1.Dock = DockStyle.Left;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Image = Properties.Resources.ico;
            button1.Location = new Point(0, 0);
            button1.Name = "button1";
            button1.Size = new Size(45, 76);
            button1.TabIndex = 6;
            button1.UseVisualStyleBackColor = true;
            // 
            // panelToolsListHost
            // 
            panelToolsListHost.BackColor = Color.FromArgb(12, 12, 18);
            panelToolsListHost.Dock = DockStyle.Fill;
            panelToolsListHost.Location = new Point(0, 0);
            panelToolsListHost.Name = "panelToolsListHost";
            panelToolsListHost.Size = new Size(1010, 533);
            panelToolsListHost.TabIndex = 1;
            // 
            // panelMain
            // 
            panelMain.BackColor = Color.Transparent;
            panelMain.Controls.Add(panelPages);
            panelMain.Dock = DockStyle.Fill;
            panelMain.Location = new Point(190, 27);
            panelMain.Name = "panelMain";
            panelMain.Padding = new Padding(12, 12, 12, 0);
            panelMain.Size = new Size(1034, 693);
            panelMain.TabIndex = 0;
            // 
            // panelPages
            // 
            panelPages.BackColor = Color.Transparent;
            panelPages.Controls.Add(pageQuick);
            panelPages.Controls.Add(pageTools);
            panelPages.Controls.Add(pageSteam);
            panelPages.Controls.Add(pageNative);
            panelPages.Dock = DockStyle.Fill;
            panelPages.Location = new Point(12, 12);
            panelPages.Name = "panelPages";
            panelPages.Size = new Size(1010, 681);
            panelPages.TabIndex = 0;
            // 
            // pageQuick
            // 
            pageQuick.Controls.Add(flowQuick);
            pageQuick.Controls.Add(panelQuickTop);
            pageQuick.Dock = DockStyle.Fill;
            pageQuick.Location = new Point(0, 0);
            pageQuick.Name = "pageQuick";
            pageQuick.Size = new Size(1010, 681);
            pageQuick.TabIndex = 0;
            pageQuick.Visible = false;
            // 
            // flowQuick
            // 
            flowQuick.BackColor = Color.Transparent;
            flowQuick.Controls.Add(btnOpenRegedit);
            flowQuick.Controls.Add(btnOpenTemp);
            flowQuick.Controls.Add(btnOpenDownloads);
            flowQuick.Controls.Add(btnOpenWindowsUpdate);
            flowQuick.Controls.Add(btnOpenAppData);
            flowQuick.Controls.Add(btnOpenSteamConfig);
            flowQuick.Controls.Add(btnOpenLocalAppData);
            flowQuick.Controls.Add(btnOpenPrefetch);
            flowQuick.Controls.Add(btnOpenEventViewer);
            flowQuick.Controls.Add(btnOpenChromeProfile);
            flowQuick.Controls.Add(btnOpenEdgeProfile);
            flowQuick.Controls.Add(btnOpenFirefoxProfiles);
            flowQuick.Controls.Add(btnOpenBrowserCache);
            flowQuick.Controls.Add(btnOpenRecent);
            flowQuick.Controls.Add(btnOpenDesktop);
            flowQuick.Dock = DockStyle.Fill;
            flowQuick.Location = new Point(0, 78);
            flowQuick.Name = "flowQuick";
            flowQuick.Padding = new Padding(0, 12, 0, 0);
            flowQuick.Size = new Size(1010, 603);
            flowQuick.TabIndex = 0;
            // 
            // btnOpenRegedit
            // 
            btnOpenRegedit.Location = new Point(3, 15);
            btnOpenRegedit.Name = "btnOpenRegedit";
            btnOpenRegedit.Size = new Size(75, 23);
            btnOpenRegedit.TabIndex = 0;
            // 
            // btnOpenTemp
            // 
            btnOpenTemp.Location = new Point(84, 15);
            btnOpenTemp.Name = "btnOpenTemp";
            btnOpenTemp.Size = new Size(75, 23);
            btnOpenTemp.TabIndex = 1;
            // 
            // btnOpenDownloads
            // 
            btnOpenDownloads.Location = new Point(165, 15);
            btnOpenDownloads.Name = "btnOpenDownloads";
            btnOpenDownloads.Size = new Size(75, 23);
            btnOpenDownloads.TabIndex = 2;
            // 
            // btnOpenWindowsUpdate
            // 
            btnOpenWindowsUpdate.Location = new Point(246, 15);
            btnOpenWindowsUpdate.Name = "btnOpenWindowsUpdate";
            btnOpenWindowsUpdate.Size = new Size(75, 23);
            btnOpenWindowsUpdate.TabIndex = 3;
            // 
            // btnOpenAppData
            // 
            btnOpenAppData.Location = new Point(327, 15);
            btnOpenAppData.Name = "btnOpenAppData";
            btnOpenAppData.Size = new Size(75, 23);
            btnOpenAppData.TabIndex = 4;
            // 
            // btnOpenSteamConfig
            // 
            btnOpenSteamConfig.Location = new Point(408, 15);
            btnOpenSteamConfig.Name = "btnOpenSteamConfig";
            btnOpenSteamConfig.Size = new Size(75, 23);
            btnOpenSteamConfig.TabIndex = 5;
            // 
            // panelQuickTop
            // 
            panelQuickTop.BackColor = Color.FromArgb(18, 14, 36);
            panelQuickTop.Controls.Add(lblQuickTitle);
            panelQuickTop.Controls.Add(lblQuickDesc);
            panelQuickTop.Dock = DockStyle.Top;
            panelQuickTop.Location = new Point(0, 0);
            panelQuickTop.Name = "panelQuickTop";
            panelQuickTop.Padding = new Padding(14, 12, 14, 12);
            panelQuickTop.Size = new Size(1010, 78);
            panelQuickTop.TabIndex = 1;
            // 
            // lblQuickTitle
            // 
            lblQuickTitle.AutoSize = true;
            lblQuickTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblQuickTitle.ForeColor = Color.White;
            lblQuickTitle.Location = new Point(14, 12);
            lblQuickTitle.Name = "lblQuickTitle";
            lblQuickTitle.Size = new Size(128, 20);
            lblQuickTitle.TabIndex = 0;
            lblQuickTitle.Text = "Быстрый доступ";
            // 
            // lblQuickDesc
            // 
            lblQuickDesc.AutoSize = true;
            lblQuickDesc.ForeColor = Color.Gainsboro;
            lblQuickDesc.Location = new Point(14, 42);
            lblQuickDesc.Name = "lblQuickDesc";
            lblQuickDesc.Size = new Size(329, 15);
            lblQuickDesc.TabIndex = 1;
            lblQuickDesc.Text = "Реестр, Temp, Downloads, Update и другие быстрые места.";
            // 
            // pageTools
            // 
            pageTools.Controls.Add(panelToolsGridHost);
            pageTools.Controls.Add(panelToolsBottom);
            pageTools.Controls.Add(panelToolsTop);
            pageTools.Dock = DockStyle.Fill;
            pageTools.Location = new Point(0, 0);
            pageTools.Name = "pageTools";
            pageTools.Size = new Size(1010, 681);
            pageTools.TabIndex = 1;
            pageTools.Visible = false;
            // 
            // panelToolsGridHost
            // 
            panelToolsGridHost.BackColor = Color.FromArgb(12, 12, 18);
            panelToolsGridHost.Controls.Add(panelToolsListHost);
            panelToolsGridHost.Controls.Add(dgvTools);
            panelToolsGridHost.Dock = DockStyle.Fill;
            panelToolsGridHost.Location = new Point(0, 78);
            panelToolsGridHost.Name = "panelToolsGridHost";
            panelToolsGridHost.Size = new Size(1010, 533);
            panelToolsGridHost.TabIndex = 0;
            // 
            // dgvTools
            // 
            dgvTools.AllowUserToAddRows = false;
            dgvTools.AllowUserToDeleteRows = false;
            dgvTools.AllowUserToResizeRows = false;
            dgvTools.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTools.BackgroundColor = Color.FromArgb(12, 12, 18);
            dgvTools.BorderStyle = BorderStyle.None;
            dgvTools.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvTools.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(22, 22, 34);
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = Color.Gainsboro;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgvTools.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvTools.Columns.AddRange(new DataGridViewColumn[] { colTool, colToolStatus, colToolPath });
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(10, 10, 14);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = Color.Gainsboro;
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(34, 24, 60);
            dataGridViewCellStyle2.SelectionForeColor = Color.White;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dgvTools.DefaultCellStyle = dataGridViewCellStyle2;
            dgvTools.Dock = DockStyle.Fill;
            dgvTools.EnableHeadersVisualStyles = false;
            dgvTools.GridColor = Color.FromArgb(32, 32, 44);
            dgvTools.Location = new Point(0, 0);
            dgvTools.MultiSelect = false;
            dgvTools.Name = "dgvTools";
            dgvTools.ReadOnly = true;
            dgvTools.RowHeadersVisible = false;
            dgvTools.RowTemplate.Height = 28;
            dgvTools.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTools.Size = new Size(1010, 533);
            dgvTools.TabIndex = 0;
            dgvTools.Visible = false;
            // 
            // colTool
            // 
            colTool.FillWeight = 26F;
            colTool.HeaderText = "Tool";
            colTool.Name = "colTool";
            colTool.ReadOnly = true;
            // 
            // colToolStatus
            // 
            colToolStatus.FillWeight = 14F;
            colToolStatus.HeaderText = "Status";
            colToolStatus.Name = "colToolStatus";
            colToolStatus.ReadOnly = true;
            // 
            // colToolPath
            // 
            colToolPath.FillWeight = 60F;
            colToolPath.HeaderText = "Path";
            colToolPath.Name = "colToolPath";
            colToolPath.ReadOnly = true;
            // 
            // panelToolsBottom
            // 
            panelToolsBottom.BackColor = Color.FromArgb(12, 12, 18);
            panelToolsBottom.Controls.Add(btnOpenTool);
            panelToolsBottom.Controls.Add(btnLocateTool);
            panelToolsBottom.Controls.Add(btnDownloadTool);
            panelToolsBottom.Controls.Add(lblToolsHint);
            panelToolsBottom.Dock = DockStyle.Bottom;
            panelToolsBottom.Location = new Point(0, 611);
            panelToolsBottom.Name = "panelToolsBottom";
            panelToolsBottom.Padding = new Padding(12);
            panelToolsBottom.Size = new Size(1010, 70);
            panelToolsBottom.TabIndex = 1;
            // 
            // btnOpenTool
            // 
            btnOpenTool.BackColor = Color.FromArgb(16, 12, 28);
            btnOpenTool.FlatAppearance.BorderColor = Color.FromArgb(120, 110, 255);
            btnOpenTool.FlatStyle = FlatStyle.Flat;
            btnOpenTool.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnOpenTool.ForeColor = Color.White;
            btnOpenTool.Location = new Point(12, 16);
            btnOpenTool.Name = "btnOpenTool";
            btnOpenTool.Size = new Size(120, 36);
            btnOpenTool.TabIndex = 0;
            btnOpenTool.Text = "Open";
            btnOpenTool.UseVisualStyleBackColor = false;
            // 
            // btnLocateTool
            // 
            btnLocateTool.BackColor = Color.FromArgb(12, 12, 18);
            btnLocateTool.FlatAppearance.BorderColor = Color.FromArgb(120, 110, 255);
            btnLocateTool.FlatStyle = FlatStyle.Flat;
            btnLocateTool.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnLocateTool.ForeColor = Color.White;
            btnLocateTool.Location = new Point(140, 16);
            btnLocateTool.Name = "btnLocateTool";
            btnLocateTool.Size = new Size(120, 36);
            btnLocateTool.TabIndex = 1;
            btnLocateTool.Text = "Locate";
            btnLocateTool.UseVisualStyleBackColor = false;
            // 
            // btnDownloadTool
            // 
            btnDownloadTool.BackColor = Color.FromArgb(12, 12, 18);
            btnDownloadTool.FlatAppearance.BorderColor = Color.FromArgb(120, 110, 255);
            btnDownloadTool.FlatStyle = FlatStyle.Flat;
            btnDownloadTool.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnDownloadTool.ForeColor = Color.White;
            btnDownloadTool.Location = new Point(268, 16);
            btnDownloadTool.Name = "btnDownloadTool";
            btnDownloadTool.Size = new Size(120, 36);
            btnDownloadTool.TabIndex = 2;
            btnDownloadTool.Text = "Download";
            btnDownloadTool.UseVisualStyleBackColor = false;
            // 
            // lblToolsHint
            // 
            lblToolsHint.AutoSize = true;
            lblToolsHint.ForeColor = Color.Gainsboro;
            lblToolsHint.Location = new Point(410, 26);
            lblToolsHint.Name = "lblToolsHint";
            lblToolsHint.Size = new Size(218, 15);
            lblToolsHint.TabIndex = 3;
            lblToolsHint.Text = "Select tool → Open / Locate / Download";
            // 
            // panelToolsTop
            // 
            panelToolsTop.BackColor = Color.FromArgb(18, 14, 36);
            panelToolsTop.Controls.Add(lblToolsTitle);
            panelToolsTop.Controls.Add(lblToolsDesc);
            panelToolsTop.Dock = DockStyle.Top;
            panelToolsTop.Location = new Point(0, 0);
            panelToolsTop.Name = "panelToolsTop";
            panelToolsTop.Padding = new Padding(14, 12, 14, 12);
            panelToolsTop.Size = new Size(1010, 78);
            panelToolsTop.TabIndex = 2;
            // 
            // lblToolsTitle
            // 
            lblToolsTitle.AutoSize = true;
            lblToolsTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblToolsTitle.ForeColor = Color.White;
            lblToolsTitle.Location = new Point(14, 12);
            lblToolsTitle.Name = "lblToolsTitle";
            lblToolsTitle.Size = new Size(157, 20);
            lblToolsTitle.TabIndex = 0;
            lblToolsTitle.Text = "Tools for moderation";
            // 
            // lblToolsDesc
            // 
            lblToolsDesc.AutoSize = true;
            lblToolsDesc.ForeColor = Color.Gainsboro;
            lblToolsDesc.Location = new Point(14, 42);
            lblToolsDesc.Name = "lblToolsDesc";
            lblToolsDesc.Size = new Size(354, 15);
            lblToolsDesc.TabIndex = 1;
            lblToolsDesc.Text = "Detect / open tools. Ничего не устанавливается автоматически.";
            // 
            // pageSteam
            // 
            pageSteam.Controls.Add(panelSteamGridHost);
            pageSteam.Controls.Add(panelSteamTop);
            pageSteam.Dock = DockStyle.Fill;
            pageSteam.Location = new Point(0, 0);
            pageSteam.Name = "pageSteam";
            pageSteam.Size = new Size(1010, 681);
            pageSteam.TabIndex = 2;
            pageSteam.Visible = false;
            // 
            // panelSteamGridHost
            // 
            panelSteamGridHost.BackColor = Color.FromArgb(12, 12, 18);
            panelSteamGridHost.Controls.Add(dgvSteam);
            panelSteamGridHost.Dock = DockStyle.Fill;
            panelSteamGridHost.Location = new Point(0, 78);
            panelSteamGridHost.Name = "panelSteamGridHost";
            panelSteamGridHost.Size = new Size(1010, 603);
            panelSteamGridHost.TabIndex = 0;
            // 
            // dgvSteam
            // 
            dgvSteam.AllowUserToAddRows = false;
            dgvSteam.AllowUserToDeleteRows = false;
            dgvSteam.AllowUserToResizeRows = false;
            dgvSteam.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSteam.BackgroundColor = Color.FromArgb(12, 12, 18);
            dgvSteam.BorderStyle = BorderStyle.None;
            dgvSteam.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvSteam.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.FromArgb(22, 22, 34);
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridViewCellStyle3.ForeColor = Color.Gainsboro;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            dgvSteam.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            dgvSteam.Columns.AddRange(new DataGridViewColumn[] { colSteamId, colPersona, colAccountName, colMostRecent, colTimestamp });
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = Color.FromArgb(10, 10, 14);
            dataGridViewCellStyle4.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle4.ForeColor = Color.Gainsboro;
            dataGridViewCellStyle4.SelectionBackColor = Color.FromArgb(34, 24, 60);
            dataGridViewCellStyle4.SelectionForeColor = Color.White;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.False;
            dgvSteam.DefaultCellStyle = dataGridViewCellStyle4;
            dgvSteam.Dock = DockStyle.Fill;
            dgvSteam.EnableHeadersVisualStyles = false;
            dgvSteam.GridColor = Color.FromArgb(32, 32, 44);
            dgvSteam.Location = new Point(0, 0);
            dgvSteam.MultiSelect = false;
            dgvSteam.Name = "dgvSteam";
            dgvSteam.ReadOnly = true;
            dgvSteam.RowHeadersVisible = false;
            dgvSteam.RowTemplate.Height = 28;
            dgvSteam.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSteam.Size = new Size(1010, 603);
            dgvSteam.TabIndex = 0;
            // 
            // colSteamId
            // 
            colSteamId.FillWeight = 22F;
            colSteamId.HeaderText = "SteamID64";
            colSteamId.Name = "colSteamId";
            colSteamId.ReadOnly = true;
            // 
            // colPersona
            // 
            colPersona.FillWeight = 22F;
            colPersona.HeaderText = "PersonaName";
            colPersona.Name = "colPersona";
            colPersona.ReadOnly = true;
            // 
            // colAccountName
            // 
            colAccountName.FillWeight = 18F;
            colAccountName.HeaderText = "AccountName";
            colAccountName.Name = "colAccountName";
            colAccountName.ReadOnly = true;
            // 
            // colMostRecent
            // 
            colMostRecent.FillWeight = 10F;
            colMostRecent.HeaderText = "MostRecent";
            colMostRecent.Name = "colMostRecent";
            colMostRecent.ReadOnly = true;
            // 
            // colTimestamp
            // 
            colTimestamp.FillWeight = 28F;
            colTimestamp.HeaderText = "Timestamp";
            colTimestamp.Name = "colTimestamp";
            colTimestamp.ReadOnly = true;
            // 
            // panelSteamTop
            // 
            panelSteamTop.BackColor = Color.FromArgb(18, 14, 36);
            panelSteamTop.Controls.Add(lblSteamTitle);
            panelSteamTop.Controls.Add(lblSteamHint);
            panelSteamTop.Dock = DockStyle.Top;
            panelSteamTop.Location = new Point(0, 0);
            panelSteamTop.Name = "panelSteamTop";
            panelSteamTop.Padding = new Padding(14, 12, 14, 12);
            panelSteamTop.Size = new Size(1010, 78);
            panelSteamTop.TabIndex = 1;
            // 
            // lblSteamTitle
            // 
            lblSteamTitle.AutoSize = true;
            lblSteamTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblSteamTitle.ForeColor = Color.White;
            lblSteamTitle.Location = new Point(14, 12);
            lblSteamTitle.Name = "lblSteamTitle";
            lblSteamTitle.Size = new Size(125, 20);
            lblSteamTitle.TabIndex = 0;
            lblSteamTitle.Text = "Steam аккаунты";
            // 
            // lblSteamHint
            // 
            lblSteamHint.AutoSize = true;
            lblSteamHint.ForeColor = Color.Gainsboro;
            lblSteamHint.Location = new Point(14, 42);
            lblSteamHint.Name = "lblSteamHint";
            lblSteamHint.Size = new Size(437, 15);
            lblSteamHint.TabIndex = 1;
            lblSteamHint.Text = "Двойной клик по строке открывает steamcommunity профиль (loginusers.vdf)";
            // 
            // pageNative
            // 
            pageNative.Controls.Add(splitNative);
            pageNative.Controls.Add(panelTop);
            pageNative.Controls.Add(panelSummary);
            pageNative.Dock = DockStyle.Fill;
            pageNative.Location = new Point(0, 0);
            pageNative.Name = "pageNative";
            pageNative.Size = new Size(1010, 681);
            pageNative.TabIndex = 3;
            // 
            // splitNative
            // 
            splitNative.BackColor = Color.FromArgb(10, 10, 14);
            splitNative.Dock = DockStyle.Fill;
            splitNative.Location = new Point(0, 154);
            splitNative.Name = "splitNative";
            splitNative.Orientation = Orientation.Horizontal;
            // 
            // splitNative.Panel1
            // 
            splitNative.Panel1.Controls.Add(dgvFindings);
            splitNative.Panel1MinSize = 260;
            // 
            // splitNative.Panel2
            // 
            splitNative.Panel2.Controls.Add(txtLog);
            splitNative.Panel2MinSize = 80;
            splitNative.Size = new Size(1010, 527);
            splitNative.SplitterDistance = 415;
            splitNative.SplitterWidth = 6;
            splitNative.TabIndex = 0;
            // 
            // dgvFindings
            // 
            dgvFindings.AllowUserToAddRows = false;
            dgvFindings.AllowUserToDeleteRows = false;
            dgvFindings.AllowUserToResizeRows = false;
            dgvFindings.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvFindings.BackgroundColor = Color.FromArgb(12, 12, 18);
            dgvFindings.BorderStyle = BorderStyle.None;
            dgvFindings.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvFindings.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = Color.FromArgb(22, 22, 34);
            dataGridViewCellStyle5.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridViewCellStyle5.ForeColor = Color.Gainsboro;
            dataGridViewCellStyle5.WrapMode = DataGridViewTriState.True;
            dgvFindings.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            dgvFindings.Columns.AddRange(new DataGridViewColumn[] { colSeverity, colCategory, colWhat, colReason, colAction, colDetails });
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = Color.FromArgb(10, 10, 14);
            dataGridViewCellStyle6.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle6.ForeColor = Color.Gainsboro;
            dataGridViewCellStyle6.SelectionBackColor = Color.FromArgb(34, 24, 60);
            dataGridViewCellStyle6.SelectionForeColor = Color.White;
            dataGridViewCellStyle6.WrapMode = DataGridViewTriState.False;
            dgvFindings.DefaultCellStyle = dataGridViewCellStyle6;
            dgvFindings.Dock = DockStyle.Fill;
            dgvFindings.EnableHeadersVisualStyles = false;
            dgvFindings.GridColor = Color.FromArgb(32, 32, 44);
            dgvFindings.Location = new Point(0, 0);
            dgvFindings.MultiSelect = false;
            dgvFindings.Name = "dgvFindings";
            dgvFindings.ReadOnly = true;
            dgvFindings.RowHeadersVisible = false;
            dgvFindings.RowTemplate.Height = 28;
            dgvFindings.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvFindings.Size = new Size(1010, 415);
            dgvFindings.TabIndex = 0;
            // 
            // colSeverity
            // 
            colSeverity.FillWeight = 10F;
            colSeverity.HeaderText = "Severity";
            colSeverity.Name = "colSeverity";
            colSeverity.ReadOnly = true;
            // 
            // colCategory
            // 
            colCategory.FillWeight = 12F;
            colCategory.HeaderText = "Category";
            colCategory.Name = "colCategory";
            colCategory.ReadOnly = true;
            // 
            // colWhat
            // 
            colWhat.FillWeight = 16F;
            colWhat.HeaderText = "What";
            colWhat.Name = "colWhat";
            colWhat.ReadOnly = true;
            // 
            // colReason
            // 
            colReason.FillWeight = 22F;
            colReason.HeaderText = "Why flagged";
            colReason.Name = "colReason";
            colReason.ReadOnly = true;
            // 
            // colAction
            // 
            colAction.FillWeight = 16F;
            colAction.HeaderText = "Recommended action";
            colAction.Name = "colAction";
            colAction.ReadOnly = true;
            // 
            // colDetails
            // 
            colDetails.FillWeight = 24F;
            colDetails.HeaderText = "Details";
            colDetails.Name = "colDetails";
            colDetails.ReadOnly = true;
            // 
            // txtLog
            // 
            txtLog.BackColor = Color.FromArgb(10, 10, 14);
            txtLog.BorderStyle = BorderStyle.None;
            txtLog.Dock = DockStyle.Fill;
            txtLog.ForeColor = Color.Gainsboro;
            txtLog.Location = new Point(0, 0);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(1010, 106);
            txtLog.TabIndex = 0;
            // 
            // panelTop
            // 
            panelTop.BackColor = Color.FromArgb(12, 12, 20);
            panelTop.Controls.Add(btnScan);
            panelTop.Controls.Add(btnCancel);
            panelTop.Controls.Add(btnCopyReport);
            panelTop.Controls.Add(chkHigh);
            panelTop.Controls.Add(chkMedium);
            panelTop.Controls.Add(chkLow);
            panelTop.Controls.Add(chkInfo);
            panelTop.Controls.Add(lblStatus);
            panelTop.Controls.Add(panelProgressTrack);
            panelTop.Dock = DockStyle.Top;
            panelTop.Location = new Point(0, 62);
            panelTop.Name = "panelTop";
            panelTop.Padding = new Padding(14, 14, 14, 10);
            panelTop.Size = new Size(1010, 92);
            panelTop.TabIndex = 1;
            // 
            // btnScan
            // 
            btnScan.BackColor = Color.FromArgb(16, 12, 28);
            btnScan.FlatAppearance.BorderColor = Color.FromArgb(120, 110, 255);
            btnScan.FlatStyle = FlatStyle.Flat;
            btnScan.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnScan.ForeColor = Color.White;
            btnScan.Location = new Point(14, 14);
            btnScan.Name = "btnScan";
            btnScan.Size = new Size(130, 36);
            btnScan.TabIndex = 0;
            btnScan.Text = "Скан";
            btnScan.UseVisualStyleBackColor = false;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.FromArgb(12, 12, 18);
            btnCancel.Enabled = false;
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 80);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnCancel.ForeColor = Color.Gainsboro;
            btnCancel.Location = new Point(154, 14);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(130, 36);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Отмена";
            btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnCopyReport
            // 
            btnCopyReport.BackColor = Color.FromArgb(12, 12, 18);
            btnCopyReport.FlatAppearance.BorderColor = Color.FromArgb(120, 110, 255);
            btnCopyReport.FlatStyle = FlatStyle.Flat;
            btnCopyReport.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnCopyReport.ForeColor = Color.White;
            btnCopyReport.Location = new Point(294, 14);
            btnCopyReport.Name = "btnCopyReport";
            btnCopyReport.Size = new Size(160, 36);
            btnCopyReport.TabIndex = 2;
            btnCopyReport.Text = "Копировать отчёт";
            btnCopyReport.UseVisualStyleBackColor = false;
            // 
            // chkHigh
            // 
            chkHigh.AutoSize = true;
            chkHigh.Checked = true;
            chkHigh.CheckState = CheckState.Checked;
            chkHigh.ForeColor = Color.Gainsboro;
            chkHigh.Location = new Point(470, 22);
            chkHigh.Name = "chkHigh";
            chkHigh.Size = new Size(52, 19);
            chkHigh.TabIndex = 3;
            chkHigh.Text = "High";
            // 
            // chkMedium
            // 
            chkMedium.AutoSize = true;
            chkMedium.Checked = true;
            chkMedium.CheckState = CheckState.Checked;
            chkMedium.ForeColor = Color.Gainsboro;
            chkMedium.Location = new Point(535, 22);
            chkMedium.Name = "chkMedium";
            chkMedium.Size = new Size(71, 19);
            chkMedium.TabIndex = 4;
            chkMedium.Text = "Medium";
            // 
            // chkLow
            // 
            chkLow.AutoSize = true;
            chkLow.Checked = true;
            chkLow.CheckState = CheckState.Checked;
            chkLow.ForeColor = Color.Gainsboro;
            chkLow.Location = new Point(630, 22);
            chkLow.Name = "chkLow";
            chkLow.Size = new Size(48, 19);
            chkLow.TabIndex = 5;
            chkLow.Text = "Low";
            // 
            // chkInfo
            // 
            chkInfo.AutoSize = true;
            chkInfo.Checked = true;
            chkInfo.CheckState = CheckState.Checked;
            chkInfo.ForeColor = Color.Gainsboro;
            chkInfo.Location = new Point(695, 22);
            chkInfo.Name = "chkInfo";
            chkInfo.Size = new Size(47, 19);
            chkInfo.TabIndex = 6;
            chkInfo.Text = "Info";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = Color.Gainsboro;
            lblStatus.Location = new Point(860, 22);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(106, 15);
            lblStatus.TabIndex = 7;
            lblStatus.Text = "Статус: Ожидание";
            // 
            // panelProgressTrack
            // 
            panelProgressTrack.BackColor = Color.FromArgb(18, 18, 30);
            panelProgressTrack.Controls.Add(panelProgressFill);
            panelProgressTrack.Dock = DockStyle.Bottom;
            panelProgressTrack.Location = new Point(14, 72);
            panelProgressTrack.Name = "panelProgressTrack";
            panelProgressTrack.Size = new Size(982, 10);
            panelProgressTrack.TabIndex = 8;
            // 
            // panelProgressFill
            // 
            panelProgressFill.BackColor = Color.FromArgb(150, 90, 255);
            panelProgressFill.Dock = DockStyle.Left;
            panelProgressFill.Location = new Point(0, 0);
            panelProgressFill.Name = "panelProgressFill";
            panelProgressFill.Size = new Size(0, 10);
            panelProgressFill.TabIndex = 0;
            // 
            // panelSummary
            // 
            panelSummary.BackColor = Color.FromArgb(18, 14, 36);
            panelSummary.Controls.Add(lblVerdictTitle);
            panelSummary.Controls.Add(lblVerdict);
            panelSummary.Controls.Add(lblCountHigh);
            panelSummary.Controls.Add(lblCountMedium);
            panelSummary.Controls.Add(lblCountLow);
            panelSummary.Controls.Add(lblCountInfo);
            panelSummary.Dock = DockStyle.Top;
            panelSummary.Location = new Point(0, 0);
            panelSummary.Name = "panelSummary";
            panelSummary.Padding = new Padding(14, 12, 14, 12);
            panelSummary.Size = new Size(1010, 62);
            panelSummary.TabIndex = 2;
            // 
            // lblVerdictTitle
            // 
            lblVerdictTitle.AutoSize = true;
            lblVerdictTitle.ForeColor = Color.Gainsboro;
            lblVerdictTitle.Location = new Point(14, 22);
            lblVerdictTitle.Name = "lblVerdictTitle";
            lblVerdictTitle.Size = new Size(54, 15);
            lblVerdictTitle.TabIndex = 0;
            lblVerdictTitle.Text = "Вердикт:";
            // 
            // lblVerdict
            // 
            lblVerdict.AutoSize = true;
            lblVerdict.ForeColor = Color.White;
            lblVerdict.Location = new Point(78, 22);
            lblVerdict.Name = "lblVerdict";
            lblVerdict.Size = new Size(106, 15);
            lblVerdict.TabIndex = 1;
            lblVerdict.Text = "No data (run Scan)";
            // 
            // lblCountHigh
            // 
            lblCountHigh.AutoSize = true;
            lblCountHigh.ForeColor = Color.White;
            lblCountHigh.Location = new Point(610, 22);
            lblCountHigh.Name = "lblCountHigh";
            lblCountHigh.Size = new Size(45, 15);
            lblCountHigh.TabIndex = 2;
            lblCountHigh.Text = "High: 0";
            // 
            // lblCountMedium
            // 
            lblCountMedium.AutoSize = true;
            lblCountMedium.ForeColor = Color.White;
            lblCountMedium.Location = new Point(690, 22);
            lblCountMedium.Name = "lblCountMedium";
            lblCountMedium.Size = new Size(64, 15);
            lblCountMedium.TabIndex = 3;
            lblCountMedium.Text = "Medium: 0";
            // 
            // lblCountLow
            // 
            lblCountLow.AutoSize = true;
            lblCountLow.ForeColor = Color.White;
            lblCountLow.Location = new Point(800, 22);
            lblCountLow.Name = "lblCountLow";
            lblCountLow.Size = new Size(41, 15);
            lblCountLow.TabIndex = 4;
            lblCountLow.Text = "Low: 0";
            // 
            // lblCountInfo
            // 
            lblCountInfo.AutoSize = true;
            lblCountInfo.ForeColor = Color.White;
            lblCountInfo.Location = new Point(880, 22);
            lblCountInfo.Name = "lblCountInfo";
            lblCountInfo.Size = new Size(40, 15);
            lblCountInfo.TabIndex = 5;
            lblCountInfo.Text = "Info: 0";
            // 
            // PanelLogo
            // 
            PanelLogo.BackColor = Color.FromArgb(1, 1, 9);
            PanelLogo.Controls.Add(buttonSvernut);
            PanelLogo.Controls.Add(button3akrit);
            PanelLogo.Dock = DockStyle.Top;
            PanelLogo.Location = new Point(190, 0);
            PanelLogo.Name = "PanelLogo";
            PanelLogo.Size = new Size(1034, 27);
            PanelLogo.TabIndex = 2;
            // 
            // button3akrit
            // 
            button3akrit.Dock = DockStyle.Right;
            button3akrit.FlatAppearance.BorderSize = 0;
            button3akrit.FlatStyle = FlatStyle.Flat;
            button3akrit.Font = new Font("Segoe UI", 13F);
            button3akrit.ForeColor = Color.White;
            button3akrit.Location = new Point(1011, 0);
            button3akrit.Name = "button3akrit";
            button3akrit.Size = new Size(23, 27);
            button3akrit.TabIndex = 0;
            button3akrit.Text = "×";
            button3akrit.UseVisualStyleBackColor = true;
            // 
            // buttonSvernut
            // 
            buttonSvernut.Dock = DockStyle.Right;
            buttonSvernut.FlatAppearance.BorderSize = 0;
            buttonSvernut.FlatStyle = FlatStyle.Flat;
            buttonSvernut.Font = new Font("Segoe UI", 13F);
            buttonSvernut.ForeColor = Color.White;
            buttonSvernut.Location = new Point(988, 0);
            buttonSvernut.Name = "buttonSvernut";
            buttonSvernut.Size = new Size(23, 27);
            buttonSvernut.TabIndex = 1;
            buttonSvernut.Text = "-";
            buttonSvernut.UseVisualStyleBackColor = true;
            // 
            // linkGitHub
            // 
            linkGitHub.ActiveLinkColor = Color.FromArgb(190, 130, 255);
            linkGitHub.AutoSize = true;
            linkGitHub.LinkColor = Color.FromArgb(160, 120, 255);
            linkGitHub.Location = new Point(12, 9);
            linkGitHub.Name = "linkGitHub";
            linkGitHub.Size = new Size(120, 15);
            linkGitHub.TabIndex = 0;
            linkGitHub.TabStop = true;
            linkGitHub.Text = "github.com/ifny75";
            linkGitHub.VisitedLinkColor = Color.FromArgb(160, 120, 255);
            // 
            // linkBio
            // 
            linkBio.ActiveLinkColor = Color.FromArgb(190, 130, 255);
            linkBio.AutoSize = true;
            linkBio.LinkColor = Color.FromArgb(160, 120, 255);
            linkBio.Location = new Point(170, 9);
            linkBio.Name = "linkBio";
            linkBio.Size = new Size(92, 15);
            linkBio.TabIndex = 1;
            linkBio.TabStop = true;
            linkBio.Text = "github.com/ifny75/ScumChecker";
            linkBio.VisitedLinkColor = Color.FromArgb(160, 120, 255);
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(8, 8, 12);
            BackgroundImage = Properties.Resources.fon13;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1224, 720);
            ControlBox = false;
            Controls.Add(panelMain);
            Controls.Add(PanelLogo);
            Controls.Add(panelSidebar);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ScumChecker";
            Load += Form1_Load;
            panelSidebar.ResumeLayout(false);
            panelFooter.ResumeLayout(false);
            panelFooter.PerformLayout();
            panel1.ResumeLayout(false);
            panelMain.ResumeLayout(false);
            panelPages.ResumeLayout(false);
            pageQuick.ResumeLayout(false);
            flowQuick.ResumeLayout(false);
            panelQuickTop.ResumeLayout(false);
            panelQuickTop.PerformLayout();
            pageTools.ResumeLayout(false);
            panelToolsGridHost.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvTools).EndInit();
            panelToolsBottom.ResumeLayout(false);
            panelToolsBottom.PerformLayout();
            panelToolsTop.ResumeLayout(false);
            panelToolsTop.PerformLayout();
            pageSteam.ResumeLayout(false);
            panelSteamGridHost.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvSteam).EndInit();
            panelSteamTop.ResumeLayout(false);
            panelSteamTop.PerformLayout();
            pageNative.ResumeLayout(false);
            splitNative.Panel1.ResumeLayout(false);
            splitNative.Panel2.ResumeLayout(false);
            splitNative.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitNative).EndInit();
            splitNative.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvFindings).EndInit();
            panelTop.ResumeLayout(false);
            panelTop.PerformLayout();
            panelProgressTrack.ResumeLayout(false);
            panelSummary.ResumeLayout(false);
            panelSummary.PerformLayout();
            PanelLogo.ResumeLayout(false);
            ResumeLayout(false);
        }

        // fallback init for non-custom quick buttons
        private void InitQuickButton(System.Windows.Forms.Button b, string text)
        {
            b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 1;
            b.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(120, 110, 255);
            b.BackColor = System.Drawing.Color.FromArgb(12, 12, 18);
            b.ForeColor = System.Drawing.Color.White;
            b.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            b.Margin = new System.Windows.Forms.Padding(0, 0, 12, 12);
            b.Size = new System.Drawing.Size(260, 48);
            b.Text = text;
            b.UseVisualStyleBackColor = false;
        }
        #endregion

        private Button button1;
        private Button Git_button;
        private Button ezbio;
        private Panel panelFooter;
        private Label lblLang;
        private ComboBox cmbLang;
        private Panel PanelLogo;
        private Button button3akrit;
        private Button buttonSvernut;
        private Panel panel1;
    }
}
