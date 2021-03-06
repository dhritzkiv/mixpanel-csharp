using System;
using System.Collections.Generic;
using Mixpanel.Core.Message;
using NUnit.Framework;

namespace Mixpanel.Tests
{
    [TestFixture]
    public class PeopleSetMessageBuilderTests
    {
        private PeopleSetMessageBuilder _builder;
        private MessageData _md;

        [SetUp]
        public void SetUp()
        {
            MixpanelConfig.Global.Reset();
            _builder = new PeopleSetMessageBuilder();
            _md = new MessageData(
                _builder.SpecialPropsBindings, 
                _builder.DistinctIdPropsBindings, 
                _builder.MessagePropetiesRules, 
                _builder.SuperPropertiesRules);
        }

        [Test]
        public void GetObject_BuildSimpleObject_Builded()
        {
            const string token = "aa11qq";
            _md.SetProperty(MixpanelProperty.Token, token);

            const string distinctId = "123";
            _md.SetProperty(MixpanelProperty.DistinctId, distinctId);

            const string ip = "111.111.111.111";
            _md.SetProperty(MixpanelProperty.Ip, ip);

            var time = new DateTime(2013, 9, 26, 22, 33, 44, DateTimeKind.Utc);
            _md.SetProperty(MixpanelProperty.Time, time);

            const bool ignoreTime = true;
            _md.SetProperty(MixpanelProperty.IgnoreTime, ignoreTime);

            const string firstName = "Darth";
            _md.SetProperty(MixpanelProperty.FirstName, firstName);

            const string lastName = "Vader";
            _md.SetProperty(MixpanelProperty.LastName, lastName);

            const string name = "Darth Vader";
            _md.SetProperty(MixpanelProperty.Name, name);

            var created = new DateTime(2013, 5, 12, 11, 10, 9, DateTimeKind.Utc);
            _md.SetProperty(MixpanelProperty.Created, created);

            const string email = "darth.vader@mail.com";
            _md.SetProperty(MixpanelProperty.Email, email);

            const string phone = "123456";
            _md.SetProperty(MixpanelProperty.Phone, phone);

            const string testProp1 = "test 1";
            _md.SetProperty("TestProp1", testProp1);

            const int testProp2 = 6;
            _md.SetProperty("TestProp2", testProp2);

            var obj = _builder.GetMessageObject(_md);
            Assert.That(obj.Count, Is.EqualTo(6));
            Assert.That(obj["$token"], Is.EqualTo(token));
            Assert.That(obj["$distinct_id"], Is.EqualTo(distinctId));
            Assert.That(obj["$ip"], Is.EqualTo(ip));
            Assert.That(obj["$time"], Is.TypeOf<long>());
            Assert.That(obj["$time"], Is.EqualTo(1380234824L));
            Assert.That(obj["$ignore_time"], Is.EqualTo(true));

            Assert.That(obj["$set"], Is.InstanceOf<IDictionary<string, object>>());
            var set = (IDictionary<string, object>)obj["$set"];

            Assert.That(set.Count, Is.EqualTo(8));
            Assert.That(set["$first_name"], Is.EqualTo(firstName));
            Assert.That(set["$last_name"], Is.EqualTo(lastName));
            Assert.That(set["$name"], Is.EqualTo(name));
            Assert.That(set["$created"], Is.EqualTo("2013-05-12T11:10:09"));
            Assert.That(set["$email"], Is.EqualTo(email));
            Assert.That(set["$phone"], Is.EqualTo(phone));
            Assert.That(set["TestProp1"], Is.EqualTo(testProp1));
            Assert.That(set["TestProp2"], Is.EqualTo(testProp2));
        }
    }
}