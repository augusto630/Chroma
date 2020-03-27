﻿using BS_Utils.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Chroma.Extensions
{
    internal class SaberColourizer
    {
        internal bool warm;

        private SetSaberGlowColor[] glowColors;
        private MeshRenderer[] meshRenderers;
        private MaterialPropertyBlock[] blocks;
        private SetSaberGlowColor.PropertyTintColorPair[][] tintPairs;

        private List<Material> customMats = new List<Material>();

        internal static SaberBurnMarkArea saberBurnMarkArea = null;

        internal static Color? currentAColor = null;
        internal static Color? currentBColor = null;

        private SaberColourizer(Saber saber)
        {
            warm = saber.saberType == SaberType.SaberA;

            glowColors = saber.GetComponentsInChildren<SetSaberGlowColor>();
            meshRenderers = new MeshRenderer[glowColors.Length];
            blocks = new MaterialPropertyBlock[glowColors.Length];
            tintPairs = new SetSaberGlowColor.PropertyTintColorPair[glowColors.Length][];
            for (int i = 0; i < glowColors.Length; i++)
            {
                meshRenderers[i] = glowColors[i].GetPrivateField<MeshRenderer>("_meshRenderer");

                blocks[i] = glowColors[i].GetPrivateField<MaterialPropertyBlock>("_materialPropertyBlock");
                if (blocks[i] == null)
                {
                    blocks[i] = new MaterialPropertyBlock();
                    glowColors[i].SetPrivateField("_materialPropertyBlock", blocks[i]);
                }
                tintPairs[i] = glowColors[i].GetPrivateField<SetSaberGlowColor.PropertyTintColorPair[]>("_propertyTintColorPairs");
                meshRenderers[i].SetPropertyBlock(blocks[i], 0);
            }

            //Custom sabers??
            Renderer[] renderers = saber.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                foreach (Material material in renderers[i].materials)
                {
                    if ((material.HasProperty("_Glow") && material.GetFloat("_Glow") > 0f) || (material.HasProperty("_Bloom") && material.GetFloat("_Bloom") > 0f))
                    {
                        customMats.Add(material);
                    }
                }
            }
        }

        internal static SaberColourizer[] saberColourizers { get; private set; }

        internal static void InitializeSabers(Saber[] sabers)
        {
            saberColourizers = new SaberColourizer[sabers.Length];
            for (int i = 0; i < sabers.Length; i++)
            {
                saberColourizers[i] = new SaberColourizer(sabers[i]);
            }
        }

        internal void Colourize(Color color)
        {
            if (warm) currentAColor = color;
            else currentBColor = color;

            for (int i = 0; i < glowColors.Length; i++)
            {
                for (int j = 0; j < tintPairs[i].Length; j++)
                {
                    blocks[i].SetColor(tintPairs[i][j].property, color * tintPairs[i][j].tintColor);
                }

                meshRenderers[i].SetPropertyBlock(blocks[i], 0);
            }

            foreach (Material material in customMats)
            {
                material.SetColor("_Color", color);
            }

            if (saberBurnMarkArea != null)
            {
                LineRenderer[] _lineRenderers = saberBurnMarkArea.GetPrivateField<LineRenderer[]>("_lineRenderers");
                _lineRenderers[warm ? 0 : 1].startColor = color;
                _lineRenderers[warm ? 0 : 1].endColor = color;
            }
        }
    }
}