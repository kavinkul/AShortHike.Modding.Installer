﻿namespace Blasphemous.Modding.Installer.Mods;

internal class ModUI
{
    private readonly Panel outerPanel;
    private readonly Panel innerPanel;

    private readonly Label nameText;
    private readonly Label authorText;

    private readonly Button updateButton;
    private readonly Button readmeButton;
    private readonly Button installButton;
    private readonly Button enableButton;

    private int _modIdx;
    private bool _isHovering = false;

    public void UpdateUI(string name, string version, string author, bool installed, bool enabled, bool canUpdate)
    {
        // Text
        nameText.Text = $"{name} (v{version})";
        nameText.Size = new Size(nameText.PreferredWidth, 30);
        authorText.Text = "by " + author;
        authorText.Location = new Point(nameText.PreferredWidth + 15, authorText.Location.Y);
        authorText.Size = new Size(authorText.PreferredWidth, 20);

        // Install button
        installButton.Text = installed ? "Installed" : "Not installed";
        installButton.ForeColor = installed ? Colors.GREEN : Colors.RED;
        installButton.FlatAppearance.BorderColor = installed ? Colors.GREEN : Colors.RED;

        // Enable button
        enableButton.Visible = installed;
        enableButton.Text = enabled ? "Enabled" : "Disabled";
        enableButton.ForeColor = enabled ? Color.Yellow : Color.White;
        enableButton.FlatAppearance.BorderColor = enabled ? Color.Yellow : Color.White;

        // Update button
        updateButton.Visible = canUpdate;
    }

    public void ShowDownloadingStatus()
    {
        installButton.Text = "Downloading...";
        installButton.ForeColor = Colors.ORANGE;
        installButton.FlatAppearance.BorderColor = Colors.ORANGE;
    }

    public void SetPosition(int modIdx)
    {
        _modIdx = modIdx;
        outerPanel.Location = new Point(0, (Sizes.MOD_HEIGHT - 2) * modIdx - 2);
        UpdateColor();
    }

    private void UpdateColor()
    {
        Color backgroundColor = _isHovering
            ? Colors.SELECTED_GRAY
            : _modIdx % 2 == 0
                ? Colors.DARK_GRAY
                : Colors.LIGHT_GRAY;

        innerPanel.BackColor = backgroundColor;
        installButton.BackColor = backgroundColor;
        enableButton.BackColor = backgroundColor;
    }

    private bool IsDependencyMod(Mod mod)
    {
        return mod.Data.name == "Modding API" || mod.Data.name.EndsWith("Framework");
    }

    public ModUI(Mod mod, Panel parentPanel)
    {
        parentPanel.AutoScroll = false;

        // Panels

        outerPanel = new Panel
        {
            Name = mod.Data.name,
            Parent = parentPanel,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            BackColor = Color.Black,
            Size = new Size(parentPanel.Width, Sizes.MOD_HEIGHT),
        };

        innerPanel = new Panel
        {
            Name = mod.Data.name,
            Parent = outerPanel,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
            Location = new Point(0, 2),
            Size = new Size(parentPanel.Width, Sizes.MOD_HEIGHT - 4),
        };

        // Left side

        nameText = new Label
        {
            Name = mod.Data.name,
            Parent = innerPanel,
            Anchor = AnchorStyles.Top | AnchorStyles.Left,
            Location = new Point(10, 8),
            Size = new Size(100, 30),
            ForeColor = IsDependencyMod(mod) ? Colors.SPECIAL : Color.LightGray,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = Fonts.MOD_NAME,
        };
        nameText.MouseEnter += mod.MouseEnter;
        nameText.MouseLeave += mod.MouseLeave;

        authorText = new Label
        {
            Name = mod.Data.name,
            Parent = innerPanel,
            Anchor = AnchorStyles.Top | AnchorStyles.Left,
            Location = new Point(200, 13),
            Size = new Size(100, 20),
            ForeColor = Color.LightGray,
            TextAlign = ContentAlignment.BottomLeft,
            Font = Fonts.MOD_AUTHOR,
        };

        // Right side

        updateButton = new Button
        {
            Name = mod.Data.name,
            Parent = innerPanel,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(parentPanel.Width - 450, 11),
            Size = new Size(130, 24),
            BackColor = Color.Black,
            ForeColor = Color.White,
            Font = Fonts.BUTTON,
            Text = "Download update",
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            TabStop = false,
        };
        updateButton.FlatAppearance.BorderColor = Color.White;
        updateButton.Click += mod.ClickedUpdate;
        updateButton.MouseUp += Core.UIHandler.RemoveButtonFocus;
        updateButton.MouseLeave += Core.UIHandler.RemoveButtonFocus;

        readmeButton = new Button
        {
            Name = mod.Data.name,
            Parent = innerPanel,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(parentPanel.Width - 290, 11),
            Size = new Size(70, 24),
            BackColor = Colors.BLUE,
            Font = Fonts.BUTTON,
            Text = "README",
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            TabStop = false,
        };
        readmeButton.FlatAppearance.BorderColor = Color.Black;
        readmeButton.Click += mod.ClickedReadme;
        readmeButton.MouseUp += Core.UIHandler.RemoveButtonFocus;
        readmeButton.MouseLeave += Core.UIHandler.RemoveButtonFocus;

        installButton = new Button
        {
            Name = mod.Data.name,
            Parent = innerPanel,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(parentPanel.Width - 190, 11),
            Size = new Size(100, 24),
            Font = Fonts.BUTTON,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            TabStop = false,
        };
        installButton.Click += mod.ClickedInstall;
        installButton.MouseUp += Core.UIHandler.RemoveButtonFocus;
        installButton.MouseLeave += Core.UIHandler.RemoveButtonFocus;

        enableButton = new Button
        {
            Name = mod.Data.name,
            Parent = innerPanel,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(parentPanel.Width - 80, 11),
            Size = new Size(70, 24),
            Font = Fonts.BUTTON,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            TabStop = false,
        };
        enableButton.Click += mod.ClickedEnable;
        enableButton.MouseUp += Core.UIHandler.RemoveButtonFocus;
        enableButton.MouseLeave += Core.UIHandler.RemoveButtonFocus;

        AddMouseEnterEvent(innerPanel);
        AddMouseLeaveEvent(innerPanel);

        parentPanel.AutoScroll = true;
    }

    private void AddMouseEnterEvent(Control control)
    {
        EventHandler action = (_, __) => ChangeHoverStatus(true);

        control.MouseEnter += action;
        foreach (Control child in control.Controls)
            child.MouseEnter += action;
    }

    private void AddMouseLeaveEvent(Control control)
    {
        EventHandler action = (_, __) => ChangeHoverStatus(false);

        control.MouseLeave += action;
        foreach (Control child in control.Controls)
            child.MouseLeave += action;
    }

    private void ChangeHoverStatus(bool status)
    {
        _isHovering = status;
        UpdateColor();
    }
}
