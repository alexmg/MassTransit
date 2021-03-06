// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.SubscriptionBuilders
{
    using System;
    using System.Collections.Generic;
    using Magnum.Extensions;
    using Subscriptions.Coordinator;

    public class SubscriptionRouterBuilderImpl :
        SubscriptionRouterBuilder
    {
        readonly IServiceBus _bus;
        readonly IList<Func<IServiceBus, SubscriptionRouter, SubscriptionObserver>> _observers;
        string _network;

        public SubscriptionRouterBuilderImpl(IServiceBus bus, string network)
        {
            _bus = bus;
            _network = network;
            _observers = new List<Func<IServiceBus, SubscriptionRouter, SubscriptionObserver>>
                {
                    (b, c) => new BusSubscriptionConnector(b)
                };
        }

        public string Network
        {
            get { return _network; }
        }

        public void SetNetwork(string network)
        {
            _network = network;
        }

        public void SetObserverFactory(
            Func<IServiceBus, SubscriptionRouter, SubscriptionObserver> observerFactory)
        {
            _observers.Clear();
            _observers.Add(observerFactory);
        }

        public void AddObserverFactory(
            Func<IServiceBus, SubscriptionRouter, SubscriptionObserver> observerFactory)
        {
            _observers.Add(observerFactory);
        }

        public SubscriptionRouterService Build()
        {
            var service = new SubscriptionRouterService(_bus, _network);

            _observers.Each(x => service.AddObserver(x(_bus, service)));

            return service;
        }
    }
}