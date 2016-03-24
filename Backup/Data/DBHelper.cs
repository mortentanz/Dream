using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Dream.Data
{
  /// <summary>
  /// Provides static general purpose methods for interacting with database store.
  /// </summary>
  internal static class DBHelper
  {

    /// <summary>
    /// Creates a new SqlConnection using a configured connection string.
    /// </summary>
    /// <returns>An SqlConnection object configured with a connection string.</returns>
    internal static SqlConnection CreateConnection()
    {
      SqlConnection conn = null;
      #if DEBUG
      conn = new SqlConnection(Properties.Settings.Default.ConnectionStringLocal);
      #else
      conn = new SqlConnection(Properties.Settings.Default.ConnectionString);
      #endif
      return conn;
    }

    /// <summary>
    /// Serializes an object graph to an SqlXml object.
    /// </summary>
    /// <typeparam name="T">The type of object to be serialized.</typeparam>
    /// <param name="o">The object to be serialized.</param>
    /// <returns>An SqlXml object containing the serialized object graph.</returns>
    internal static SqlXml XmlSerialize<T>(T o)
    {
      XmlSerializer xs = new XmlSerializer(typeof(T));
      using (MemoryStream ms = new MemoryStream())
      {
        xs.Serialize(ms, o);
        SqlXml xml = new SqlXml(ms);
        return xml;
      }
    }

    /// <summary>
    /// Deserializes the content of an SqlXml object.
    /// </summary>
    /// <typeparam name="T">The type of object to be deserialized.</typeparam>
    /// <param name="xml">An SqlXml object containing the xml representation of the object.</param>
    /// <returns>An object of the specified type.</returns>
    /// <exception cref="SqlNullValueException">Thrown if the supplied SqlXml is null valued.</exception>
    internal static T XmlDeserialize<T>(SqlXml xml)
    {
      return DBHelper.XmlDeserialize<T>(xml, null);
    }

    /// <summary>
    /// Deserializes the content of an SqlXml object.
    /// </summary>
    /// <typeparam name="T">The type of object to be deserialized.</typeparam>
    /// <param name="xml">An SqlXml object containing the xml representation of the object.</param>
    /// <param name="unknownXmlElementHandler">An XmlElementEventHandler delegate to be called if unknown elements are found.</param>
    /// <returns>An object of the specified type.</returns>
    /// <exception cref="SqlNullValueException">Thrown if the supplied SqlXml is null valued.</exception>
    internal static T XmlDeserialize<T>(SqlXml xml, XmlElementEventHandler unknownXmlElementHandler)
    {
      XmlSerializer xs = new XmlSerializer(typeof(T));
      if (unknownXmlElementHandler != null)
      {
        xs.UnknownElement += unknownXmlElementHandler;
      }
      object o = xs.Deserialize(xml.CreateReader());
      return (T)o;
    }

  }


}
