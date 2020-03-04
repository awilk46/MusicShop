using MusicShop.DAL;
using MusicShop.Models;
using MvcSiteMapProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicShop.Infrastructure
{
    public class AlbumySzczegolyDynamicNodeProvider : DynamicNodeProviderBase
    {
        private AlbumContext db = new AlbumContext();

        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode nodee)
        {
            var returnValue = new List<DynamicNode>();

            foreach (Album album in db.Albumy)
            {
                DynamicNode node = new DynamicNode();
                node.Title = album.NazwaAlbumu ;
                node.Key = "Album" + album.AlbumID;
                node.ParentKey = "Kategoria_" + album.KategoriaID;
                node.RouteValues.Add("id", album.AlbumID);
                returnValue.Add(node);
            }

            return returnValue;
        }
    }
}