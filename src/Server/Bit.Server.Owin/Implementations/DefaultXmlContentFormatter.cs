﻿using Bit.Core.Contracts;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Bit.Owin.Implementations
{
    public class DefaultXmlContentFormatter : IContentFormatter
    {
        public virtual string Serialize<T>([AllowNull] T obj)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using StringWriter stringWriter = new StringWriter();
            using XmlWriter xmlWriter = XmlWriter.Create(stringWriter);
            xmlSerializer.Serialize(xmlWriter, obj);
            return stringWriter.ToString();
        }

        public virtual T Deserialize<T>(string objAsStr)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using StringReader stringReader = new StringReader(objAsStr);
            using XmlReader reader = XmlReader.Create(stringReader);
            return (T)xmlSerializer.Deserialize(reader);
        }

        public virtual async Task<T> DeserializeAsync<T>(Stream input, CancellationToken cancellationToken)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using StreamReader streamReader = new StreamReader(input);
            using XmlReader reader = XmlReader.Create(streamReader);
            return (T)xmlSerializer.Deserialize(reader);
        }

        public virtual string ContentType => "application/xml";
    }
}