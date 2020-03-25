﻿using Chroma.Events;
using Chroma.Settings;
using CustomJSONData;
using CustomJSONData.CustomBeatmap;
using Harmony;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chroma.HarmonyPatches
{
    [HarmonyPriority(Priority.Low)]
    [HarmonyPatch(typeof(ColorNoteVisuals))]
    [HarmonyPatch("Awake")]
    internal class ColorNoteVisualsAwake
    {
        private static void Postfix(ColorNoteVisuals __instance)
        {
            if (ColourManager.TechnicolourBlocks && ChromaConfig.TechnicolourBlocksStyle == ColourManager.TechnicolourStyle.GRADIENT)
                VFX.TechnicolourController.Instance._colorNoteVisuals.Add(__instance);
        }
    }

    [HarmonyPriority(Priority.High)]
    [HarmonyPatch(typeof(ColorNoteVisuals))]
    [HarmonyPatch("HandleNoteControllerDidInitEvent")]
    internal class ColorNoteVisualsHandleNoteControllerDidInitEvent
    {
        public static bool noteColoursActive;

        private static void Prefix(ref NoteController noteController, ref ColorManager ____colorManager)
        {
            NoteData noteData = noteController.noteData;
            bool warm = noteData.noteType == NoteType.NoteA;
            Color? c = null;

            // Technicolour
            if (ColourManager.TechnicolourBlocks && ChromaConfig.TechnicolourBlocksStyle != ColourManager.TechnicolourStyle.GRADIENT)
            {
                try
                {
                    c = ColourManager.GetTechnicolour(noteData.noteType == NoteType.NoteA, noteData.time + noteData.lineIndex + (int)noteData.noteLineLayer, ChromaConfig.TechnicolourBlocksStyle);
                }
                catch (Exception e)
                {
                    ChromaLogger.Log(e);
                }
            }

            // CustomLightColours
            if (ChromaNoteColourEvent.CustomNoteColours.Count > 0)
            {
                Dictionary<float, Color> dictionaryID;
                if (ChromaNoteColourEvent.CustomNoteColours.TryGetValue(noteData.noteType, out dictionaryID))
                {
                    foreach (KeyValuePair<float, Color> d in dictionaryID)
                    {
                        if (d.Key <= noteData.time)
                        {
                            c = d.Value;
                        }
                    }
                }
            }

            // CustomJSONData _customData individual color override
            try
            {
                if (noteData is CustomNoteData customData && ChromaBehaviour.LightingRegistered && ChromaConfig.NoteColourEventsEnabled)
                {
                    dynamic dynData = customData.customData;

                    List<object> color = Trees.at(dynData, "_color");
                    if (color != null)
                    {
                        float r = Convert.ToSingle(color[0]);
                        float g = Convert.ToSingle(color[1]);
                        float b = Convert.ToSingle(color[2]);

                        c = new Color(r, g, b);
                    }
                }
            }
            catch (Exception e)
            {
                ChromaLogger.Log("INVALID _customData", ChromaLogger.Level.WARNING);
                ChromaLogger.Log(e);
            }

            if (c.HasValue)
            {
                ColourManager.SetNoteTypeColourOverride(noteData.noteType, c.Value);
                noteColoursActive = true;
            }

            if (noteColoursActive || ChromaConfig.TechnicolourBlocksStyle == ColourManager.TechnicolourStyle.GRADIENT)
            {
                ChromaNoteColourEvent.SavedNoteColours[noteController] = ____colorManager.ColorForNoteType(noteData.noteType);
                if (!ColourManager.TechnicolourSabers) noteController.noteWasCutEvent += ChromaNoteColourEvent.SaberColour;
            }
        }

        private static void Postfix(ref NoteController noteController)
        {
            ColourManager.RemoveNoteTypeColourOverride(noteController.noteData.noteType);
        }
    }
}