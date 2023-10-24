﻿using BlasModInstaller.Grouping;
using BlasModInstaller.Loading;
using BlasModInstaller.Mods;
using BlasModInstaller.Properties;
using BlasModInstaller.Skins;
using BlasModInstaller.Sorting;
using BlasModInstaller.UIHolding;
using BlasModInstaller.Validation;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BlasModInstaller
{
    static class Core
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            UIHandler = new UIHandler();
            SettingsHandler = new SettingsHandler(Environment.CurrentDirectory + "\\installer.cfg");
            GithubHandler = new GithubHandler(SettingsHandler.Config.GithubToken);

            List<Mod> blas1mods = new List<Mod>();
            List<Skin> blas1skins = new List<Skin>();
            List<Mod> blas2mods = new List<Mod>();

            string blas1modTitle = "Blasphemous Mods";
            string blas1skinTitle = "Blasphemous Skins";
            string blas2modTitle = "Blasphemous II Mods";

            string blas1modLocalPath = Environment.CurrentDirectory + "\\downloads\\BlasphemousMods.json";
            string blas1skinLocalPath = Environment.CurrentDirectory + "\\downloads\\BlasphemousSkins.json";
            string blas2modLocalPath = Environment.CurrentDirectory + "\\downloads\\BlasphemousIIMods.json";

            string blas1modRemotePath = "https://raw.githubusercontent.com/BrandenEK/Blasphemous-Mod-Installer/main/BlasphemousMods.json";
            string blas2modRemotePath = "https://raw.githubusercontent.com/BrandenEK/Blasphemous-Mod-Installer/main/BlasphemousIIMods.json";

            var blas1modGrouper = new ModGrouper(blas1modTitle, blas1mods);
            var blas1skinGrouper = new SkinGrouper(blas1skinTitle, blas1skins);
            var blas2modGrouper = new ModGrouper(blas2modTitle, blas2mods);

            var blas1modUI = new GenericUIHolder<Mod>(UIHandler.GetUIElementByType(SectionType.Blas1Mods), blas1mods);
            var blas1skinUI = new GenericUIHolder<Skin>(UIHandler.GetUIElementByType(SectionType.Blas1Skins), blas1skins);
            var blas2modUI = new GenericUIHolder<Mod>(UIHandler.GetUIElementByType(SectionType.Blas2Mods), blas2mods);

            var blas1modSorter = new ModSorter(blas1modUI, blas1mods);
            var blas1skinSorter = new SkinSorter(blas1skinUI, blas1skins);
            var blas2modSorter = new ModSorter(blas2modUI, blas2mods);

            var blas1modLoader = new ModLoader(blas1modLocalPath, blas1modRemotePath, blas1modUI, blas1modSorter, blas1mods, SectionType.Blas1Mods);
            var blas1skinLoader = new SkinLoader(blas1skinLocalPath, "blasphemous1", blas1skinUI, blas1skinSorter, blas1skins, SectionType.Blas1Skins);
            var blas2modLoader = new ModLoader(blas2modLocalPath, blas2modRemotePath, blas2modUI, blas2modSorter, blas2mods, SectionType.Blas2Mods);

            var blas1Validator = new Blas1Validator();
            var blas2Validator = new Blas2Validator();

            var blas1modPage = new InstallerPage(blas1modTitle, Resources.background1,
                blas1modGrouper,
                blas1modLoader,
                blas1modSorter,
                blas1modUI,
                blas1Validator);

            var blas1skinPage = new InstallerPage(blas1skinTitle, Resources.background1,
                blas1skinGrouper,
                blas1skinLoader,
                blas1skinSorter,
                blas1skinUI,
                blas1Validator);

            var blas2modPage = new InstallerPage(blas2modTitle, Resources.background2,
                blas2modGrouper,
                blas2modLoader,
                blas2modSorter,
                blas2modUI,
                blas2Validator);

            _pages.Add(SectionType.Blas1Mods, blas1modPage);
            _pages.Add(SectionType.Blas1Skins, blas1skinPage);
            _pages.Add(SectionType.Blas2Mods, blas2modPage);

            Application.Run(UIHandler);
        }

        public static UIHandler UIHandler { get; private set; }
        public static SettingsHandler SettingsHandler { get; private set; }
        public static GithubHandler GithubHandler { get; private set; }

        private static Dictionary<SectionType, InstallerPage> _pages = new Dictionary<SectionType, InstallerPage>();

        public static InstallerPage CurrentPage => _pages[SettingsHandler.Config.LastSection];
        public static IEnumerable<InstallerPage> AllPages => _pages.Values;

        public static InstallerPage Blas1ModPage => _pages[SectionType.Blas1Mods];
        public static InstallerPage Blas1SkinPage => _pages[SectionType.Blas1Skins];
        public static InstallerPage Blas2ModPage => _pages[SectionType.Blas2Mods];

        // Don't forget to increase this when releasing an update!  Have to do it here
        // because I'm not sure how to increase file version for windows forms
        public static Version CurrentInstallerVersion => new Version(1, 2, 1);
    }
}