using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Play.Inventory.Service.Entities
{
    public class CatalogItem : IEntity
    {
        public int Id { get; set; }
        public int CatalogItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    }
}