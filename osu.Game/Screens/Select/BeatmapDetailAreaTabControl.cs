﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Graphics.UserInterface;
using osu.Game.Screens.Select.Filter;
using osu.Game.Screens.Select.Leaderboards;
using osuTK.Graphics;

namespace osu.Game.Screens.Select
{
    public partial class BeatmapDetailAreaTabControl : Container
    {
        public const float HEIGHT = 24;

        public Bindable<BeatmapDetailAreaTabItem> Current
        {
            get => tabs.Current;
            set => tabs.Current = value;
        }

        public Bindable<bool> CurrentModsFilter
        {
            get => modsCheckbox.Current;
            set => modsCheckbox.Current = value;
        }

        public Bindable<ScoreSortMode> CurrentScoreSortMode
        {
            get => sortModeDropdown.Current;
            set => sortModeDropdown.Current = value;
        }

        public Action<BeatmapDetailAreaTabItem, bool, ScoreSortMode> OnFilter; // passed the selected tab and if mods is checked

        public IReadOnlyList<BeatmapDetailAreaTabItem> TabItems
        {
            get => tabs.Items;
            set => tabs.Items = value;
        }

        private readonly OsuTabControlCheckbox modsCheckbox;
        private readonly OsuTabControl<BeatmapDetailAreaTabItem> tabs;
        private readonly SlimEnumDropdown<ScoreSortMode> sortModeDropdown;
        private readonly Container tabsContainer;
        private readonly Container dropdownContainer;

        public BeatmapDetailAreaTabControl()
        {
            Height = HEIGHT;

            Children = new Drawable[]
            {
                new Box
                {
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    RelativeSizeAxes = Axes.X,
                    Height = 1,
                    Colour = Color4.White.Opacity(0.2f),
                },
                tabsContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = tabs = new OsuTabControl<BeatmapDetailAreaTabItem>
                    {
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        RelativeSizeAxes = Axes.Both,
                        IsSwitchable = true,
                    },
                },
                new GridContainer
                {
                    Origin = Anchor.BottomRight,
                    Anchor = Anchor.BottomRight,
                    Height = HEIGHT,
                    AutoSizeAxes = Axes.X,
                    ColumnDimensions = new[]
                    {
                        new Dimension(),
                        new Dimension(GridSizeMode.AutoSize),
                    },
                    RowDimensions = new[] { new Dimension() },
                    Content = new[]
                    {
                        new Drawable[]
                        {
                            modsCheckbox = new OsuTabControlCheckbox
                            {
                                Anchor = Anchor.BottomRight,
                                Origin = Anchor.BottomRight,
                                Text = @"Selected Mods",
                                Alpha = 0,
                            },
                            dropdownContainer = new Container
                            {
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight,
                                RelativeSizeAxes = Axes.Y,
                                Width = 110,
                                Child = sortModeDropdown = new SlimEnumDropdown<ScoreSortMode>
                                {
                                    RelativeSizeAxes = Axes.X,
                                    BypassAutoSizeAxes = Axes.Y,
                                }
                            }
                        }
                    }
                }
            };

            tabs.Current.ValueChanged += _ => invokeOnFilter();
            modsCheckbox.Current.ValueChanged += _ => invokeOnFilter();
            sortModeDropdown.Current.ValueChanged += _ => invokeOnFilter();
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colour)
        {
            modsCheckbox.AccentColour = tabs.AccentColour = colour.YellowLight;
        }

        private void invokeOnFilter()
        {
            OnFilter?.Invoke(tabs.Current.Value, modsCheckbox.Current.Value, sortModeDropdown.Current.Value);

            if (tabs.Current.Value is BeatmapDetailAreaLeaderboardTabItem<BeatmapLeaderboardScope> leaderboard && leaderboard.Scope == BeatmapLeaderboardScope.Local)
            {
                dropdownContainer.ResizeWidthTo(110, 200, Easing.OutQuint);
                dropdownContainer.FadeTo(1, 200, Easing.OutQuint);
                modsCheckbox.Padding = new MarginPadding { Right = 5 };
            }
            else
            {
                dropdownContainer.ResizeWidthTo(0, 200, Easing.OutQuint);
                dropdownContainer.FadeTo(0, 200, Easing.OutQuint);
                modsCheckbox.Padding = new MarginPadding();
            }

            if (tabs.Current.Value.FilterableByMods)
            {
                modsCheckbox.FadeTo(1, 200, Easing.OutQuint);
                tabsContainer.Padding = new MarginPadding { Right = 215 };
            }
            else
            {
                modsCheckbox.FadeTo(0, 200, Easing.OutQuint);
                tabsContainer.Padding = new MarginPadding();
            }
        }
    }
}
