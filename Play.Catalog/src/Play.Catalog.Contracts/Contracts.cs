using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Play.Catalog.Contracts
{
    public record CatalogItemCreated(int ItemId, string Name, string Description);
    public record CatalogItemUpdated(int ItemId, string Name, string Description);
    public record CatalogItemDeleted(int ItemId);
}