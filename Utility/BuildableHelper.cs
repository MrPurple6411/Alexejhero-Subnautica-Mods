using SMLHelper.V2.Handlers;
using System;
using UnityEngine;

namespace ModdingAdventCalendar.Utility
{
    public class BuildableHelper
    {
        public TechType techType;

        public BuildableHelper(string id, string displayname, string tooltip, TechGroup group, TechCategory category, TechType after = TechType.None, bool unlockonstart = true, Atlas.Sprite atlasSprite = null, Sprite unitySprite = null)
        {
            if (atlasSprite != null && unitySprite != null) throw new ArgumentException("Both atlasSprite and unitySprite are not null!");

            if (atlasSprite != null) techType = TechTypeHandler.AddTechType(id, displayname, tooltip, atlasSprite, unlockonstart);
            else if (unitySprite != null) techType = TechTypeHandler.AddTechType(id, displayname, tooltip, unitySprite, unlockonstart);
            else techType = TechTypeHandler.AddTechType(id, displayname, tooltip, unlockonstart);

            CraftDataHandler.AddBuildable(techType);
            if (after == TechType.None) CraftDataHandler.AddToGroup(group, category, techType);
            else CraftDataHandler.AddToGroup(group, category, techType, after);
        }

        public static implicit operator TechType(BuildableHelper bh)
        {
            return bh.techType;
        }
    }
}
