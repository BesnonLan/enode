﻿using ECommon.IoC;
using ENode.EQueue;
using ENode.Eventing;

namespace DistributeEventStoreSample.Client.EQueueIntegrations
{
    public class EventTopicManager : IEventTopicProvider
    {
        public string GetTopic(EventStream eventStream)
        {
            return "NoteEventTopic";
        }
    }
}
