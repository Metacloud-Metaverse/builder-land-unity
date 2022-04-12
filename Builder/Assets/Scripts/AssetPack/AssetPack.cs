using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetPacks
{
    public class AssetPack
    {
        public string name;
        public List<Section> sections = new List<Section>();
        public delegate void Hook();
        private List<Hook> _hooks = new List<Hook>();

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

        public void AddAsset(object asset, string sectionName)
        {
            Section section;
            if(SectionExists(sectionName))
            {
                section = GetSection(sectionName);
            }
            else
            {
                section = new Section();
                section.name = sectionName;
                sections.Add(section);
            }
            section.assets.Add(asset);
        }

        public void AddSection(string sectionName)
        {
            if (SectionExists(sectionName)) return;

            var section = new Section();
            section.name = sectionName;
            sections.Add(section);
        }

        public void AddHook(Hook hook)
        {
            _hooks.Add(hook);
        }

        public void RemoveHook(Hook hook)
        {
            _hooks.Remove(hook);
        }

        public void CallHooks()
        {
            foreach (var hook in _hooks)
            {
                hook();
            }
        }
    }
}
