using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CleanArch_Products.Application.Messaging
{
    public interface IMessageBus
    {
        Task PublishAsync<T>(string topic, T message);
    }
}