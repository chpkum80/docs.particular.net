﻿namespace Snippets6
{
    using NServiceBus;

    public class HandlerOrdering
    {
        public void Simple(EndpointConfiguration endpointConfiguration)
        {
            #region HandlerOrderingWithCode

            endpointConfiguration.ExecuteTheseHandlersFirst(typeof(HandlerB), typeof(HandlerA), typeof(HandlerC));

            #endregion
        }

        public void SpecifyingFirst(EndpointConfiguration endpointConfiguration)
        {
            #region HandlerOrderingWithFirst

            endpointConfiguration.ExecuteTheseHandlersFirst(typeof(HandlerB));

            #endregion
        }

        public void SpecifyingOrder(EndpointConfiguration endpointConfiguration)
        {
            #region HandlerOrderingWithMultiple

            endpointConfiguration.ExecuteTheseHandlersFirst(typeof(HandlerB), typeof(HandlerA), typeof(HandlerC));

            #endregion
        }

        public class HandlerA
        {

        }

        public class HandlerB
        {

        }

        public class HandlerC
        {

        }
    }
}