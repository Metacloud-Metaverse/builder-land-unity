using System.Collections.Generic;
using UnityEngine;

namespace AssetPacks
{
    public class AssetPack
    {
        public string name;
        public List<Section> sections = new List<Section>();
        public Sprite image;
        
        public bool SectionExists(string name)
        {
            foreach (var s in sections)
            {
                if (s.name == name) return true;
            }
            return false;
        }

        public Section GetSection(string name)
        {
            foreach (var section in sections)
            {
                if (section.name == name) return section;
            }
            return null;
        }

        public void AddAsset(object asset, string sectionName, string url)
        {
            Section section;
            if(SectionExists(sectionName))
            {
                section = GetSection(sectionName);
            }
            else
            {
                section = AddSection(sectionName);
            }
            section.assets.Add(asset);
            section.urls.Add(url);
        }

        public Section AddSection(string sectionName)
        {
            if (SectionExists(sectionName)) return GetSection(sectionName);

            var section = new Section();
            section.name = sectionName;
            sections.Add(section);

            return section;
        }
    }
}
