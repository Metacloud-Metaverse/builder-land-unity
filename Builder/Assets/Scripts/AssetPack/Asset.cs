using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetPacks
{
    public class Asset
    {
        public int id;
        public object asset;
        public string url;
        public string name;
        public string[] tags;

        public Asset(int id, object asset, string url, string name, string[] tags)
        {
            this.id = id;
            this.asset = asset;
            this.url = url;
            this.name = name;
            this.tags = tags;
        }
    }
}
