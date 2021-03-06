﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECommon.IoC;
using ENode.Infrastructure;

namespace ENode.Eventing.Impl
{
    /// <summary>The default implementation of IEventSynchronizerProvider.
    /// </summary>
    public class DefaultEventSynchronizerProvider : IEventSynchronizerProvider, IAssemblyInitializer
    {
        private readonly IDictionary<Type, IList<IEventSynchronizer>> _eventSynchronizerDict = new Dictionary<Type, IList<IEventSynchronizer>>();

        /// <summary>Initialize from the given assemblies.
        /// </summary>
        /// <param name="assemblies"></param>
        /// <exception cref="Exception"></exception>
        public void Initialize(params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                foreach (var synchronizerType in assembly.GetTypes().Where(IsEventSynchronizer))
                {
                    if (!TypeUtils.IsComponent(synchronizerType))
                    {
                        throw new ENodeException("Event synchronizer type [{0}] should be marked as component.", synchronizerType.FullName);
                    }
                    RegisterSynchronizer(synchronizerType);
                }
            }
        }

        /// <summary>Get all the event synchronizers for the given event type.
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public IEnumerable<IEventSynchronizer> GetSynchronizers(Type eventType)
        {
            var eventSynchronizers = new List<IEventSynchronizer>();
            foreach (var key in _eventSynchronizerDict.Keys.Where(key => key.IsAssignableFrom(eventType)))
            {
                eventSynchronizers.AddRange(_eventSynchronizerDict[key]);
            }
            return eventSynchronizers;
        }

        private void RegisterSynchronizer(Type synchronizerType)
        {
            foreach (var synchronizerInterface in ScanSynchronizerInterfaces(synchronizerType))
            {
                var eventType = GetEventType(synchronizerInterface);
                var synchronizerWrapperType = typeof(EventSynchronizerWrapper<>).MakeGenericType(eventType);
                IList<IEventSynchronizer> eventSynchronizers;
                if (!_eventSynchronizerDict.TryGetValue(eventType, out eventSynchronizers))
                {
                    eventSynchronizers = new List<IEventSynchronizer>();
                    _eventSynchronizerDict.Add(eventType, eventSynchronizers);
                }

                if (eventSynchronizers.Any(x => x.GetInnerSynchronizer().GetType() == synchronizerType)) continue;

                var synchronizer = ObjectContainer.Resolve(synchronizerType);
                var synchronizerWrapper = Activator.CreateInstance(synchronizerWrapperType, new[] { synchronizer }) as IEventSynchronizer;
                eventSynchronizers.Add(synchronizerWrapper);
            }
        }
        private static bool IsEventSynchronizer(Type type)
        {
            return type.IsClass && !type.IsAbstract && ScanSynchronizerInterfaces(type).Any();
        }
        private static IEnumerable<Type> ScanSynchronizerInterfaces(Type type)
        {
            return type.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventSynchronizer<>));
        }
        private static Type GetEventType(Type synchronizerInterface)
        {
            return synchronizerInterface.GetGenericArguments().Single();
        }
    }
}
