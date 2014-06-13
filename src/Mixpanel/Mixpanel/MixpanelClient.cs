﻿using System;
using System.Collections.Generic;
using System.Text;
using Mixpanel.Core.Message;

namespace Mixpanel
{
    public class MixpanelClient : IMixpanelClient
    {
        public const string UrlFormat = "http://api.mixpanel.com/{0}";
        public const string EndpointTrack = "track";
        public const string EndpointEngage = "engage";

        private readonly string _token;
        private readonly MixpanelConfig _config;

        /// <summary>
        /// Func for getting current utc time. Simplifies testing.
        /// </summary>
        internal Func<DateTime> UtcNow { get; set; }

        public MixpanelClient(string token, MixpanelConfig config = null, object superProperties = null)
        {
            if (String.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException("token");

            _token = token;
            _config = config;

            SetSuperProperties(superProperties);
            
            UtcNow = () => DateTime.UtcNow;
        }

        #region Track

        public bool Track(string @event, object properties)
        {
            return Track(@event, null, properties);
        }

        public bool Track(string @event, object distinctId, object properties)
        {
            return SendMessage(
                CreateTrackMessageObject(@event, distinctId, properties), EndpointTrack, "Track");
        }

        public MixpanelMessageTest TrackTest(string @event, object properties)
        {
            return TrackTest(@event, null, properties);
        }

        public MixpanelMessageTest TrackTest(string @event, object distinctId, object properties)
        {
            return TestMessage(() => CreateTrackMessageObject(@event, distinctId, properties));
        }

        private IDictionary<string, object> CreateTrackMessageObject(
            string @event, object distinctId, object properties)
        {
            return GetMessageObject(
                new TrackMessageBuilder(_config), properties,
                new Dictionary<string, object>
                {
                    {MixpanelProperty.Event, @event},
                    {MixpanelProperty.DistinctId, distinctId}
                });
        }

        #endregion

        #region Alias

        public bool Alias(object distinctId, object alias)
        {
            return SendMessage(CreateAliasMessageObject(distinctId, alias), EndpointTrack, "Alias");
        }

        public MixpanelMessageTest AliasTest(object distinctId, object alias)
        {
            return TestMessage(() => CreateAliasMessageObject(distinctId, alias));
        }

        private IDictionary<string, object> CreateAliasMessageObject(
            object distinctId, object alias)
        {
            return GetMessageObject(
                new AliasMessageBuilder(_config), null,
                new Dictionary<string, object>
                {
                    {MixpanelProperty.DistinctId, distinctId},
                    {MixpanelProperty.Alias, alias}
                });
        }

        #endregion Alias

        #region PeopleSet

        public bool PeopleSet(object properties)
        {
            return PeopleSet(null, properties);
        }

        public bool PeopleSet(object distinctId, object properties)
        {
            return SendMessage(CreatePeopleSetMessageObject(distinctId, properties), EndpointEngage, "PeopleSet");
        }

        public MixpanelMessageTest PeopleSetTest(object properties)
        {
            return PeopleSetTest(null, properties);
        }

        public MixpanelMessageTest PeopleSetTest(object distinctId, object properties)
        {
            return TestMessage(() => CreatePeopleSetMessageObject(distinctId, properties));
        }

        private IDictionary<string, object> CreatePeopleSetMessageObject(object distinctId, object properties)
        {
            return GetMessageObject(
                new PeopleSetMessageBuilder(_config),
                properties, CreateExtraPropertiesForDistinctId(distinctId));
        }

        #endregion PeopleSet

        #region PeopleSetOnce

        public bool PeopleSetOnce(object properties)
        {
            return PeopleSetOnce(null, properties);
        }

        public bool PeopleSetOnce(object distinctId, object properties)
        {
            return SendMessage(
                CreatePeopleSetOnceMessageObject(distinctId, properties), EndpointEngage, "PeopleSetOnce");
        }

        private IDictionary<string, object> CreatePeopleSetOnceMessageObject(object distinctId, object properties)
        {
            return GetMessageObject(
                new PeopleSetOnceMessageBuilder(_config),
                properties, CreateExtraPropertiesForDistinctId(distinctId));
        }

        #endregion PeopleSetOnce

        #region PeopleAdd
        
        public bool PeopleAdd(object properties)
        {
            return PeopleAdd(null, properties);
        }

        public bool PeopleAdd(object distinctId, object properties)
        {
            return SendMessage(
                CreatePeopleAddMessageObject(distinctId, properties), EndpointEngage, "PeopleAdd");
        }

        public MixpanelMessageTest PeopleAddTest(object properties)
        {
            return PeopleAddTest(null, properties);
        }

        public MixpanelMessageTest PeopleAddTest(object distinctId, object properties)
        {
            return TestMessage(() => CreatePeopleAddMessageObject(distinctId, properties));
        }

        private IDictionary<string, object> CreatePeopleAddMessageObject(
            object distinctId, object properties)
        {
            return GetMessageObject(
                new PeopleAddMessageBuilder(_config),
                properties, CreateExtraPropertiesForDistinctId(distinctId),
                MessagePropetyRules.NumericsOnly);
        }

        #endregion PeopleAdd

        #region PeopleAppend

        /// <summary>
        /// Takes an object containing keys and values, and appends each to a list associated with 
        /// the corresponding property name. Appending to a property that doesn't exist will result 
        /// in assigning a list with one element to that property.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        public bool PeopleAppend(object properties)
        {
            return PeopleAppend(null, properties);
        }

        /// <summary>
        /// Takes an object containing keys and values, and appends each to a list associated with 
        /// the corresponding property name. Appending to a property that doesn't exist will result 
        /// in assigning a list with one element to that property.
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        public bool PeopleAppend(object distinctId, object properties)
        {
            return SendMessage(
                CreatePeopleAppendMessageObject(distinctId, properties), EndpointEngage, "PeopleAppend");
        }

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleAppend' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// </summary>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        public MixpanelMessageTest PeopleAppendTest(object properties)
        {
            return PeopleAppendTest(null, properties);
        }

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (message data, JSON,
        /// base64) of building 'PeopleAppend' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        /// <param name="properties">
        /// Object containg keys and values that will be parsed and sent to Mixpanel. Check documentation
        /// on project page https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        public MixpanelMessageTest PeopleAppendTest(object distinctId, object properties)
        {
            return TestMessage(() => CreatePeopleAppendMessageObject(distinctId, properties));
        }

        private IDictionary<string, object> CreatePeopleAppendMessageObject(
            object distinctId, object properties)
        {
            return GetMessageObject(
                new PeopleAppendMessageBuilder(_config),
                properties, CreateExtraPropertiesForDistinctId(distinctId));
        }

        #endregion PeopleAppend

        #region PeopleUnion

        public bool PeopleUnion(object properties)
        {
            return PeopleUnion(null, properties);
        }

        public bool PeopleUnion(object distinctId, object properties)
        {
            return SendMessage(
                CreatePeopleUnionMessageObject(distinctId, properties), EndpointEngage, "PeopleUnion");
        }

        public MixpanelMessageTest PeopleUnionTest(object properties)
        {
            return PeopleUnionTest(null, properties);
        }

        public MixpanelMessageTest PeopleUnionTest(object distinctId, object properties)
        {
            return TestMessage(() => CreatePeopleUnionMessageObject(distinctId, properties));
        }

        private IDictionary<string, object> CreatePeopleUnionMessageObject(
            object distinctId, object properties)
        {
            return GetMessageObject(
                new PeopleUnionMessageBuilder(_config),
                properties, CreateExtraPropertiesForDistinctId(distinctId),
                MessagePropetyRules.ListsOnly);
        }

        #endregion PeopleUnion

        #region PeopleUnset

        /// <summary>
        /// Takes a list of string property names, and permanently removes the properties 
        /// and their values from a profile. Use this method if you have set 'distinct_id'
        /// in super properties.
        /// </summary>
        /// <param name="propertyNames">List of property names to remove.</param>
        public bool PeopleUnset(IEnumerable<string> propertyNames)
        {
            return PeopleUnset(null, propertyNames);
        }

        /// <summary>
        /// Takes a list of string property names, and permanently removes the properties 
        /// and their values from a profile.
        /// </summary>
        /// <param name="distinctId">User unique identifier. Will be converted to string.</param>
        /// <param name="propertyNames">List of property names to remove.</param>
        public bool PeopleUnset(object distinctId, IEnumerable<string> propertyNames)
        {
            return SendMessage(
                CreatePeopleUnsetMessageObject(distinctId, propertyNames), EndpointEngage, "PeopleUnset");
        }

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (dictionary, JSON,
        /// base64) of building 'PeopleUnset' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// </summary>
        /// <param name="propertyNames">List of property names to remove.</param>
        public MixpanelMessageTest PeopleUnsetTest(IEnumerable<string> propertyNames)
        {
            return PeopleUnsetTest(null, propertyNames);
        }

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (dictionary, JSON,
        /// base64) of building 'PeopleUnset' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// </summary>
        /// <param name="distinctId">User unique identifier. Will be converted to string.</param>
        /// <param name="propertyNames">List of property names to remove.</param>
        public MixpanelMessageTest PeopleUnsetTest(object distinctId, IEnumerable<string> propertyNames)
        {
            return TestMessage(() => CreatePeopleUnsetMessageObject(distinctId, propertyNames));
        }

        private IDictionary<string, object> CreatePeopleUnsetMessageObject(object distinctId, IEnumerable<string> propertyNames)
        {
            return GetMessageObject(
                new PeopleUnsetMessageBuilder(_config),
                null, new Dictionary<string, object>
                {
                    {MixpanelProperty.DistinctId, distinctId},
                    {MixpanelProperty.PeopleUnset, propertyNames},
                });
        }

        #endregion PeopleUnset

        #region PeopleDelete

        /// <summary>
        /// Permanently delete the profile from Mixpanel, along with all of its properties. 
        /// Returns true if call was successful, and false otherwise.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        public bool PeopleDelete(object distinctId)
        {
            return SendMessage(CreatePeopleDeleteObject(distinctId), EndpointEngage, "PeopleDelete");
        }

        /// <summary>
        /// Returns <see cref="MixpanelMessageTest"/> that contains all steps (dictionary, JSON,
        /// base64) of building 'PeopleDelete' message. If some error occurs during the process of 
        /// creating a message it can be found in <see cref="MixpanelMessageTest.Exception"/> property.
        /// </summary>
        /// <param name="distinctId">Unique user profile identifier.</param>
        public MixpanelMessageTest PeopleDeleteTest(object distinctId)
        {
            return TestMessage(() => CreatePeopleDeleteObject(distinctId));
        }

        private IDictionary<string, object> CreatePeopleDeleteObject(object distinctId)
        {
            return GetMessageObject(
                new PeopleDeleteMessageBuilder(_config),
                null, CreateExtraPropertiesForDistinctId(distinctId));
        }

        #endregion PeopleDelete

        #region PeopleTrackCharge

        public bool PeopleTrackCharge(object distinctId, decimal amount)
        {
            return PeopleTrackCharge(distinctId, amount, UtcNow());
        }

        public bool PeopleTrackCharge(object distinctId, decimal amount, DateTime time)
        {
            return SendMessage(
                CreatePeopleTrackChargeMessageObject(distinctId, amount, time),
                EndpointEngage, "PeopleTrackCharge");
        }

        public MixpanelMessageTest PeopleTrackChargeTest(object distinctId, decimal amount)
        {
            return PeopleTrackChargeTest(distinctId, amount, UtcNow());
        }

        public MixpanelMessageTest PeopleTrackChargeTest(object distinctId, decimal amount, DateTime time)
        {
            return TestMessage(() => CreatePeopleTrackChargeMessageObject(distinctId, amount, time));
        }

        private IDictionary<string, object> CreatePeopleTrackChargeMessageObject(
            object distinctId, decimal amount, DateTime time)
        {
            return GetMessageObject(
                new PeopleTrackChargeMessageBuilder(),
                null, new Dictionary<string, object>
                {
                    {MixpanelProperty.DistinctId, distinctId},
                    {MixpanelProperty.Time, time},
                    {MixpanelProperty.PeopleAmount, amount},
                });
        }

        #endregion

        #region Super properties

        private object _superProperties;
        
        /// <summary>
        /// Sets super properties that will be attached to every message for the current mixpanel client.
        /// All previosly set super properties will be removed.
        /// </summary>
        /// <param name="superProperties">
        /// Object containg keys and values that will be parsed. If some of the properties are not valid mixpanel 
        /// properties they will be ignored. Check documentation  on project page 
        /// https://github.com/eealeivan/mixpanel-csharp for supported object containers.
        /// </param>
        public void SetSuperProperties(object superProperties)
        {
            _superProperties = superProperties;
        }

        #endregion Super properties

        /// <summary>
        /// Returns dictionary that contains Mixpanel message and is ready to be serialized. 
        /// </summary>
        /// <param name="builder">
        /// An override of <see cref="MessageBuilderBase"/> to use to generate message data.
        /// </param>
        /// <param name="userProperties">Object that contains user defined properties.</param>
        /// <param name="extraProperties">
        /// Object created by calling method. Usually contains properties that are passed to calling method
        /// as arguments.
        /// </param>
        /// <param name="propetyRules">
        /// Additional rules that will be appended to user defined properties.
        /// </param>
        private IDictionary<string, object> GetMessageObject(
            MessageBuilderBase builder, object userProperties, object extraProperties, 
            MessagePropetyRules propetyRules = MessagePropetyRules.None)
        {
            var od = new MessageData(builder.SpecialPropsBindings, propetyRules, _config);
            od.ParseAndSetProperties(userProperties);
            od.SetProperty(MixpanelProperty.Token, _token);
            od.ParseAndSetPropertiesIfNotNull(extraProperties);
            od.ParseAndSetProperties(_superProperties);

            return builder.GetMessageObject(od);
        }

        private IDictionary<string, object> CreateExtraPropertiesForDistinctId(object distinctId)
        {
            return new Dictionary<string, object> { { MixpanelProperty.DistinctId, distinctId } };
        }

        private string ToJson(object obj)
        {
            return ConfigHelper.GetSerializeJsonFn(_config)(obj);
        }

        private string ToBase64(string json)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        }

        private string GetFormData(IDictionary<string, object> obj)
        {
            return "data=" + ToBase64(ToJson(obj));
        }

        private bool SendMessage(IDictionary<string, object> obj, string endpoint, string messageType)
        {
            string url, formData;
            try
            {
                url = string.Format(UrlFormat, endpoint);
                formData = GetFormData(obj);
            }
            catch (Exception e)
            {
                LogError(string.Format("Error creating '{0}' object.", messageType), e);
                return false;
            }

            try
            {
                return ConfigHelper.GetHttpPostFn(_config)(url, formData);
            }
            catch (Exception e)
            {
                LogError(string.Format("POST fails to '{0}' with data '{1}'", url, formData), e);
                return false;
            }
        }

        private MixpanelMessageTest TestMessage(Func<IDictionary<string, object>> getMessageDataFn)
        {
            var res = new MixpanelMessageTest();

            try
            {
                res.Data = getMessageDataFn();
            }
            catch (Exception e)
            {
                res.Exception = e;
                return res;
            }

            try
            {
                res.Json = ToJson(res.Data);
            }
            catch (Exception e)
            {
                res.Exception = e;
                return res;
            }

            try
            {
                res.Base64 = ToBase64(res.Json);
            }
            catch (Exception e)
            {
                res.Exception = e;
                return res;
            }

            return res;
        }

        private void LogError(string msg, Exception exception)
        {
            var logFn = ConfigHelper.GetErrorLogFn(_config);
            if (logFn != null)
            {
                logFn(msg, exception);
            }
        }
    }
}