﻿using System.Collections.Generic;
using CharacterCreator.CommonFunctions;
using CitizenFX.Core;
using MenuAPI;
using static CitizenFX.Core.Native.API;

namespace CharacterCreator.Menus
{
    internal class Clothing
    {
        public static readonly Menu ClothingMenu = new Menu("Character Clothing", "Character Clothing Options");
        public static readonly MenuItem ClothingButton = new MenuItem("Character Clothing", "Character clothing options.");
        
        public static void CreateMenu()
        {
            //Creating clothing Menu
            MenuController.AddMenu(ClothingMenu);
            
            #region clothing options menu
            string[] clothingCategoryNames = new string[12] { "Unused (head)", "Masks", "Unused (hair)", "Upper Body", "Lower Body", "Bags & Parachutes", "Shoes", "Scarfs & Chains", "Shirt & Accessory", "Body Armor & Accessory 2", "Badges & Logos", "Shirt Overlay & Jackets" };
            for (int i = 0; i < 12; i++)
            {
                if (i != 0 && i != 2)
                {
                    int currentVariationIndex = Functions.IsEdidtingPed && Functions.CurrentCharacter.DrawableVariations.clothes.ContainsKey(i) ? Functions.CurrentCharacter.DrawableVariations.clothes[i].Key : GetPedDrawableVariation(Game.PlayerPed.Handle, i);
                    int currentVariationTextureIndex = Functions.IsEdidtingPed && Functions.CurrentCharacter.DrawableVariations.clothes.ContainsKey(i) ? Functions.CurrentCharacter.DrawableVariations.clothes[i].Value : GetPedTextureVariation(Game.PlayerPed.Handle, i);

                    int maxDrawables = GetNumberOfPedDrawableVariations(Game.PlayerPed.Handle, i);

                    List<string> items = new List<string>();
                    for (int x = 0; x < maxDrawables; x++)
                    {
                        items.Add($"Drawable #{x} (of {maxDrawables})");
                    }

                    int maxTextures = GetNumberOfPedTextureVariations(Game.PlayerPed.Handle, i, currentVariationIndex);

                    MenuListItem listItem = new MenuListItem(clothingCategoryNames[i], items, currentVariationIndex, $"Select a drawable using the arrow keys and press ~o~enter~s~ to cycle through all available textures. Currently selected texture: #{currentVariationTextureIndex + 1} (of {maxTextures}).");
                    ClothingMenu.AddMenuItem(listItem);
                }
            }
            #endregion
            
            #region clothes
            ClothingMenu.OnListIndexChange += (_menu, listItem, oldSelectionIndex, newSelectionIndex, realIndex) =>
            {
                int componentIndex = realIndex + 1;
                if (realIndex > 0)
                {
                    componentIndex += 1;
                }

                int textureIndex = GetPedTextureVariation(Game.PlayerPed.Handle, componentIndex);
                int newTextureIndex = 0;
                SetPedComponentVariation(Game.PlayerPed.Handle, componentIndex, newSelectionIndex, newTextureIndex, 0);
                if (Functions.CurrentCharacter.DrawableVariations.clothes == null)
                {
                    Functions.CurrentCharacter.DrawableVariations.clothes = new Dictionary<int, KeyValuePair<int, int>>();
                }

                int maxTextures = GetNumberOfPedTextureVariations(Game.PlayerPed.Handle, componentIndex, newSelectionIndex);

                Functions.CurrentCharacter.DrawableVariations.clothes[componentIndex] = new KeyValuePair<int, int>(newSelectionIndex, newTextureIndex);
                listItem.Description = $"Select a drawable using the arrow keys and press ~o~enter~s~ to cycle through all available textures. Currently selected texture: #{newTextureIndex + 1} (of {maxTextures}).";
            };

            ClothingMenu.OnListItemSelect += (sender, listItem, listIndex, realIndex) =>
            {
                int componentIndex = realIndex + 1; // skip face options as that fucks up with inheritance faces
                if (realIndex > 0) // skip hair features as that is done in the appeareance menu
                {
                    componentIndex += 1;
                }

                int textureIndex = GetPedTextureVariation(Game.PlayerPed.Handle, componentIndex);
                int newTextureIndex = (GetNumberOfPedTextureVariations(Game.PlayerPed.Handle, componentIndex, listIndex) - 1) < (textureIndex + 1) ? 0 : textureIndex + 1;
                SetPedComponentVariation(Game.PlayerPed.Handle, componentIndex, listIndex, newTextureIndex, 0);
                if (Functions.CurrentCharacter.DrawableVariations.clothes == null)
                {
                    Functions.CurrentCharacter.DrawableVariations.clothes = new Dictionary<int, KeyValuePair<int, int>>();
                }

                int maxTextures = GetNumberOfPedTextureVariations(Game.PlayerPed.Handle, componentIndex, listIndex);

                Functions.CurrentCharacter.DrawableVariations.clothes[componentIndex] = new KeyValuePair<int, int>(listIndex, newTextureIndex);
                listItem.Description = $"Select a drawable using the arrow keys and press ~o~enter~s~ to cycle through all available textures. Currently selected texture: #{newTextureIndex + 1} (of {maxTextures}).";
            };
            #endregion
        }
    }
}