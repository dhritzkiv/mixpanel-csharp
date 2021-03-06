using System.Collections.Generic;

namespace Mixpanel.Core.Message
{
    internal abstract class TrackMessageBuilderBase : MessageBuilderBase
    {
        private static readonly Dictionary<string, string> DistinctIdPropsBindingsInternal =
            new Dictionary<string, string>(2)
            {
                {MixpanelProperty.TrackDistinctId, MixpanelProperty.TrackDistinctId},
                {"distinctid", MixpanelProperty.TrackDistinctId}
            };

        protected static readonly Dictionary<string, string> CoreSpecialPropsBindings =
            new Dictionary<string, string>
            {
                {MixpanelProperty.TrackEvent, MixpanelProperty.TrackEvent},
                {MixpanelProperty.TrackToken, MixpanelProperty.TrackToken}
            };

        static TrackMessageBuilderBase()
        {
            foreach (var binding in DistinctIdPropsBindingsInternal)
            {
                CoreSpecialPropsBindings.Add(binding.Key, binding.Value);
            }
        }

        public override IDictionary<string, string> DistinctIdPropsBindings
        {
            get { return DistinctIdPropsBindingsInternal; }
        }

        protected IDictionary<string, object> GetCoreMessageObject(MessageData messageData)
        {
            var msg = new Dictionary<string, object>(2);

            // event
            SetSpecialRequiredProperty(msg, messageData, MixpanelProperty.TrackEvent,
                x => ThrowIfPropertyIsNullOrEmpty(x, MixpanelProperty.TrackEvent),
                x => x.ToString());

            var properties = new Dictionary<string, object>();
            msg[MixpanelProperty.TrackProperties] = properties;

            // token
            SetSpecialRequiredProperty(properties, messageData, MixpanelProperty.TrackToken,
                x => ThrowIfPropertyIsNullOrEmpty(x, MixpanelProperty.TrackToken),
                x => x.ToString());

            return msg;
        }
    }
}