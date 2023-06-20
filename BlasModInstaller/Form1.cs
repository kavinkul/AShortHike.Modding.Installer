﻿using BlasModInstaller.Pages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlasModInstaller
{
    public partial class MainForm : Form
    {
        private readonly Version CurrentInstallerVersion = new Version(1, 0, 0);

        public static string DownloadsPath => Environment.CurrentDirectory + "\\downloads\\";
        private string ConfigPath => Environment.CurrentDirectory + "\\installer.cfg";

        public static MainForm Instance { get; private set; }

        private Config config;
        public static string BlasRootFolder
        {
            get => Instance.config.BlasRootFolder;
            private set
            {
                Instance.config.BlasRootFolder = value;
                Instance.SaveConfig();
            }
        }

        public int MainSectionWidth => mainSection.Width;

        public BlasModPage BlasModPage { get; private set; }
        public BlasSkinPage BlasSkinPage { get; private set; }
        public BlasIIModPage BlasIIModPage { get; private set; }

        private Octokit.GitHubClient github;
        private string installerLatestReleaseURL;

        public MainForm()
        {
            Directory.CreateDirectory(DownloadsPath);
            if (Instance == null)
                Instance = this;
            InitializeComponent();
            BlasModPage = new BlasModPage(blas1modSection);
            BlasSkinPage = new BlasSkinPage(blas1skinSection);
            BlasIIModPage = new BlasIIModPage(blas2modSection);

            LoadConfig();
            CreateGithubClient();

            CheckForNewerInstallerRelease();
            OpenSection(SectionType.Blas1Mods);
        }

        public static Version CleanSemanticVersion(string version)
        {
            return new Version(version.ToLower().Replace("v", ""));
        }

        public static async Task<Octokit.Release> GetLatestRelease(string owner, string repo)
        {
            return await Instance.github.Repository.Release.GetLatest(owner, repo);
        }

        public static async Task<IReadOnlyList<Octokit.RepositoryContent>> GetRepositoryContents(string owner, string repo)
        {
            return await Instance.github.Repository.Content.GetAllContents(owner, repo);
        }

        private void CreateGithubClient()
        {
            github = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("BlasModInstaller"));
            if (config.GithubToken != null && config.GithubToken != string.Empty)
            {
                github.Credentials = new Octokit.Credentials(config.GithubToken);
            }
        }

        // Config

        private void LoadConfig()
        {
            if (File.Exists(ConfigPath))
            {
                // Load config
                config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigPath));
            }
            else
            {
                // Create new config
                config = new Config();
                SaveConfig();
            }
        }

        private void SaveConfig()
        {
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(config, Formatting.Indented));
        }

        // ...

        private async Task CheckForNewerInstallerRelease()
        {
            Octokit.Release latestRelease = await github.Repository.Release.GetLatest("BrandenEK", "Blasphemous-Mod-Installer");
            Version newestVersion = CleanSemanticVersion(latestRelease.TagName);
            
            if (newestVersion.CompareTo(CurrentInstallerVersion) > 0)
            {
                installerLatestReleaseURL = latestRelease.HtmlUrl;
                warningSectionOuter.Visible = true;
            }
        }

        private void ChooseBlasLocation(object sender, EventArgs e)
        {
            if (blasLocDialog.ShowDialog() == DialogResult.OK)
            {
                string rootPath = Path.GetDirectoryName(blasLocDialog.FileName);
                ValidateBlas1Directory(rootPath);
            }
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            BlasModPage.AdjustPageWidth();
            BlasSkinPage.AdjustPageWidth();
            BlasIIModPage.AdjustPageWidth();
        }

        public void RemoveButtonFocus(object sender, EventArgs e)
        {
            titleLabel.Focus();
        }

        public static void Log(string message)
        {
            Instance.debugLog.Text += message + "\r\n";
        }

        private void ShowSideButtonBorder(object sender, EventArgs e)
        {
            (sender as Button).FlatAppearance.BorderColor = Color.White;
        }

        private void HideSideButtonBorder(object sender, EventArgs e)
        {
            (sender as Button).FlatAppearance.BorderColor = Color.FromArgb(30, 30, 30);
            RemoveButtonFocus(null, null);
        }

        private void blas1modsBtn_Click(object sender, EventArgs e) => OpenSection(SectionType.Blas1Mods);
        private void blas1skinsBtn_Click(object sender, EventArgs e) => OpenSection(SectionType.Blas1Skins);
        private void blas2modsBtn_Click(object sender, EventArgs e) => OpenSection(SectionType.Blas2Mods);
        private void settingsBtn_Click(object sender, EventArgs e) { }

        private void ClickInstallerUpdateLink(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try { Process.Start(installerLatestReleaseURL); }
            catch (Exception) { MessageBox.Show("Link does not exist!", "Invalid Link"); }
        }

        private void OpenSection(SectionType section)
        {
            if (section == SectionType.Blas1Mods)
            {
                titleLabel.Text = "Blasphemous Mods";

                blas1skinSection.Visible = false;
                blas2modSection.Visible = false;

                ValidateBlas1Directory(BlasRootFolder);
            }
            else if (section == SectionType.Blas1Skins)
            {
                titleLabel.Text = "Blasphemous Skins";

                blas1modSection.Visible = false;
                blas1locationSection.Visible = false;
                blas1skinSection.Visible = true;
                blas2modSection.Visible = true;

                BlasSkinPage.LoadData();
            }
            else if (section == SectionType.Blas2Mods)
            {
                titleLabel.Text = "Blasphemous II Mods";

                blas1modSection.Visible = false;
                blas1locationSection.Visible = false;
                blas1skinSection.Visible = false;
                blas2modSection.Visible = true;

                // Load data
            }
        }

        private bool ValidateBlas1Directory(string blasRootPath)
        {
            if (File.Exists(blasRootPath + "\\Blasphemous.exe"))
            {
                Log("Blas1 exe path validated!");
                Directory.CreateDirectory(blasRootPath + "\\Modding\\disabled"); // Only because its not included in the API
                BlasRootFolder = blasRootPath;

                blas1locationSection.Visible = false;
                blas1modSection.Visible = true;
                BlasModPage.LoadData();
                return true;
            }

            Log("Blas1 exe path not found!");
            blas1locationSection.Visible = true;
            blas1modSection.Visible = false;
            return false;
        }
    }
}
