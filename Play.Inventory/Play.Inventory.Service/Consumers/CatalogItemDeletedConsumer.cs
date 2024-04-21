using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Play.Catalog.Contracts;
using Play.Catalog.Service.Repositories;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Consumers
{
    public class CatalogItemDeletedConsumer : IConsumer<CatalogItemDeleted>
    {
        private readonly IRepository<CatalogItem> _repository;

        public async Task Consume(ConsumeContext<CatalogItemDeleted> context)
        {
            var message = context.Message;
            var item = await _repository.GetAsync(x => x.CatalogItemId == message.ItemId);
            if (item == null)
            {
                return;
            }
            await _repository.RemoveAsync(message.ItemId);

        }
    }
}