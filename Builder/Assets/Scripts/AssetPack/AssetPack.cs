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


        public Asset AddAsset(AssetData assetData, object download)
        {
            Section section;
            if (SectionExists(assetData.section))
            {
                section = GetSection(assetData.section);
            }
            else
            {
                section = AddSection(assetData.section);
            }
            var asset = new Asset(
                assetData.id,
                download,
                assetData.url,
                assetData.name,
                assetData.tags
            );
            
            section.assets.Add(asset);

            return asset;
        }

        public Asset GetAsset(int id)
        {
            foreach (var section in sections)
            {
                foreach (var asset in section.assets)
                {
                    if(asset.id == id)
                    {
                        return asset;
                    }
                }
            }
            return null;
        }

        public bool ExistsAsset(AssetData data)
        {
            if (!SectionExists(data.section)) return false;

            var assets = GetSection(data.section).assets;
            Debug.Log("---------");
            foreach (var asset in assets)
            {
                Debug.Log($"Received: {data.id}, asset: {asset.id}");
                if (data.id == asset.id) return true;
            }
            return false;
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
