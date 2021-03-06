﻿namespace Chroma.Settings
{
    using BeatSaberMarkupLanguage.Attributes;

    internal class ChromaSettingsUI : PersistentSingleton<ChromaSettingsUI>
    {
        // Events
        [UIValue("lightshowonly")]
        public bool LightshowModifier
        {
            get => ChromaConfig.Instance.LightshowModifier;
            set => ChromaConfig.Instance.LightshowModifier = value;
        }

        [UIValue("rgbevents")]
        public bool CustomColorEventsEnabled
        {
            get => !ChromaConfig.Instance.CustomColorEventsEnabled;
            set => ChromaConfig.Instance.CustomColorEventsEnabled = !value;
        }

        [UIValue("platform")]
        public bool EnvironmentEnhancementsEnabled
        {
            get => !ChromaConfig.Instance.EnvironmentEnhancementsEnabled;
            set => ChromaConfig.Instance.EnvironmentEnhancementsEnabled = !value;
        }

        // Lightshow
        [UIValue("playersplace")]
        public bool PlayersPlace
        {
            get => ChromaConfig.Instance.PlayersPlace;
            set => ChromaConfig.Instance.PlayersPlace = value;
        }

        [UIValue("spectrograms")]
        public bool Spectrograms
        {
            get => ChromaConfig.Instance.Spectrograms;
            set => ChromaConfig.Instance.Spectrograms = value;
        }

        [UIValue("backcolumns")]
        public bool BackColumns
        {
            get => ChromaConfig.Instance.BackColumns;
            set => ChromaConfig.Instance.BackColumns = value;
        }

        [UIValue("buildings")]
        public bool Buildings
        {
            get => ChromaConfig.Instance.Buildings;
            set => ChromaConfig.Instance.Buildings = value;
        }
    }
}
