﻿using Simple.Ecommerce.Domain.EntityDeletionEvents;
using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Infra.Handlers.DeletedEvent
{
    public class OrderItemDeletedEventHandler : IDeleteEventHandler<CartItemDeletedEvent>
    {
        public Task Handle(CartItemDeletedEvent domainEvent)
        {
            return Task.CompletedTask;
        }
    }
}
