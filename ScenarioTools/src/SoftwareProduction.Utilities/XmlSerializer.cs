#region Using directives

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Xml.Serialization;

#endregion

namespace SoftwareProductions.Utilities
{

	/// <summary>
	/// XmlSerializer serializes and deserializes object graphs to XML.
	/// It can handle many types that the System.XmlSerializer cannot, and 
	/// it attempts to produce compact readable XML, in contrast to the SoapFormatter.
	/// This is a single use helper class. Once an instance is used it should be discarded. 
	/// </summary>
	public class XmlSerializer
	{

		private string _defaultAssemblyName = "";
		private string _defaultNamespace = "";

		private bool _useFields; // = false;
		private int _nextID = -1;
		private ArrayList _assemblies;

		private ArrayList _skipList;

		private Hashtable _types;
		
		private const int BINARY_CHUNK_SIZE = 10240; //10K

		/// <summary>
		/// Initializes a new instance of the XmlSerializer class.
		/// </summary>
		public XmlSerializer() 
		{

		}

		/// <summary>
		/// Initializes a new instance of the XmlSerializer class with the specified 
		/// DefaultAssemblyName, DefaultNamespace, and value for UseFields.
		/// </summary>
		/// <param name="useFields">The intended value of the UseFields property.</param>
		/// <param name="defaultAssemblyName">The intended value of the DefaultAssemblyName property.</param>
		/// <param name="defaultNamespace">The intended value of the DefaultNamespace property.</param>
		public XmlSerializer(bool useFields, string defaultAssemblyName, string defaultNamespace) 
		{
			_defaultAssemblyName = defaultAssemblyName;
			_defaultNamespace = defaultNamespace;
			_useFields = useFields;

			_assemblies = new ArrayList();
			
		}

		/// <summary>
		/// Initializes a new instance of the XmlSerializer class with the specified 
		/// DefaultAssemblyName, DefaultNamespace, assembly, and value for UseFields.
		/// </summary>
		/// <param name="useFields">The intended value of the UseFields property.</param>
		/// <param name="defaultAssemblyName">The intended value of the DefaultAssemblyName property.</param>
		/// <param name="defaultNamespace">The intended value of the DefaultNamespace property.</param>
		/// <param name="assembly">An assembly that may be required to deserialise objects.</param>
		public XmlSerializer(bool useFields, string defaultAssemblyName, string defaultNamespace, Assembly assembly) 
					: this(useFields, defaultAssemblyName, defaultNamespace)
		{	
			if (assembly != null) 
			{
				_assemblies.Add(assembly);
			}
		}

		/// <summary>
		/// Initializes a new instance of the XmlSerializer class with the specified 
		/// DefaultAssemblyName, DefaultNamespace, assemblies, and value for UseFields.
		/// </summary>
		/// <param name="useFields">The intended value of the UseFields property.</param>
		/// <param name="defaultAssemblyName">The intended value of the DefaultAssemblyName property.</param>
		/// <param name="defaultNamespace">The intended value of the DefaultNamespace property.</param>
		/// <param name="assemblies">A collection of assemblies that may be required to deserialise objects.</param>
		public XmlSerializer(bool useFields, string defaultAssemblyName, string defaultNamespace, ICollection assemblies) 
		{
			_defaultAssemblyName = defaultAssemblyName;
			_defaultNamespace = defaultNamespace;
			_useFields = useFields;
			if (assemblies != null) 
			{
				_assemblies = new ArrayList(assemblies.Count);
				_assemblies.AddRange(assemblies);
			}
			else
			{
				_assemblies = new ArrayList();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating if serialization and de-serialization
		/// operations will use Fields or Properties on the objects being processed.
		/// </summary>
		/// <remarks>
		/// Properties are sometimes hard to serialize (e.g. some may be read-only)
		/// but the result is usually more readable and is a closer match for the 
		/// public interface of the classes.
		/// </remarks>
		public bool UseFields 
		{
			get { return _useFields; }
			set { _useFields = value; }
		}

		/// <summary>
		/// Gets or sets the default assembly names, as a semi-colon separated list. 
		/// Types in the default assemblies are serialized with just the type name, 
		/// without the assembly qualification. During de-serialization Types without 
		/// assembly information are assumed to be from one of these assemblies. This 
		/// allows these types to be deserialized as a different version than they were 
		/// serialized. It also makes the resulting XML more compact and human readable.
		/// </summary>
		public string DefaultAssemblyNames 
		{
			get { return _defaultAssemblyName; }
			set { _defaultAssemblyName = value; }
		}

		/// <summary>
		/// Gets or sets the default namespace. Types in the default namespace are serilaized with
		/// just the type name, without the namespace. During de-serialization
		/// Types without a namespace specified are assumed to be from this namespace. 
		/// This makes the resulting XML more compact and human readable.
		/// </summary>
		public string DefaultNamespace 
		{
			get { return _defaultNamespace; }
			set { _defaultNamespace = value; }
		}

		/// <summary>
		/// Gets the collection of Assemblies containing types that may
		/// be required to deserialise objects. Assemblies loaded in the
		/// current AppDomain, or in the GAC, or referenced by an 
		/// assembly in this collection, will automatically be searched,
		/// so they do not need to be added to this collection.
		/// </summary>
		public IList Assemblies
		{
			get { return _assemblies; }
		}

		/// <summary>
		/// Serializes the target object graph into the specified XmlWriter.
		/// Uses the Type.Name of the target as the name of the root element.
		/// </summary>
		/// <param name="target">The object or graph of objects to serialize to XML.</param>
		/// <param name="writer">The XmlWriter to write the XML to that represents the serialized objects.</param>
		/// <exception cref="ArgumentNullException">Thrown if <i>target</i> or <i>writer</i> are null (Nothing in Visual Basic).</exception>
		public void Serialize(object target, XmlWriter writer) 
		{
			ExceptionHelper.ExceptionIfNull(target, "target");
			ExceptionHelper.ExceptionIfNull(writer, "writer");

			Serialize(target, writer, target.GetType().Name);
		}

		/// <summary>
		/// Serializes the target object graph and returns a string containing the 
		/// resulting XML.
		/// </summary>
		/// <param name="target">The object or graph of objects to serialize to XML.</param>
		/// <returns>A string containing the serialized state of the target as XML.</returns>
		public string SerializeToString(object target)
		{
			using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture)) 
			{
				XmlTextWriter writer = new XmlTextWriter(stringWriter);
				writer.Formatting = System.Xml.Formatting.Indented;

				writer.WriteStartDocument();
				Serialize(target, writer);
				writer.Close();

				return stringWriter.ToString();
			}
		}

		/// <summary>
		/// Serializes the target object graph and returns an XmlDocument containing the 
		/// resulting XML.
		/// </summary>
		/// <param name="target">The object or graph of objects to serialize to XML.</param>
		/// <returns>An XmlDocument containing the serialized state of the target as XML.</returns>
		public XmlDocument Serialize(object target)
		{
			MemoryStream stream = new MemoryStream();
			XmlTextWriter writer = new XmlTextWriter(stream, Encoding.Unicode);
			writer.Formatting = Formatting.Indented;
			this.Serialize(target, writer);
			XmlDocument document = new XmlDocument();
			document.PreserveWhitespace = true;

			writer.Flush();

			stream.Position = 0;

			XmlTextReader reader = new XmlTextReader(stream);

			document.Load(reader);
			
			return document;
		}

		/// <summary>
		/// Serializes the target object graph into the specified XmlWriter.
		/// Uses the specified name of the root element.
		/// </summary>
		/// <param name="target">The object or graph of objects to serialize to Xml.</param>
		/// <param name="writer">The XmlWriter to write the XML to that represents the serialized objects.</param>
		/// <param name="name">The name of the root Xml element.</param>
		public void Serialize(object target, XmlWriter writer, string name) 
		{
			Serialize( target, writer, name, new Hashtable() ); 
		}
		
		private void Serialize(object target, XmlWriter writer, string name, IDictionary doneObjects) 
		{

			//Create an element with the name of this Object
			writer.WriteStartElement(name);

			if (doneObjects.Contains(target)) 
			{
				//If there are 2 references to the same object instance in the object graph
				//being serialised, or there is a loop, we just note a reference in the output
				//to the object id we defined when we encountered the first reference to the instance.
				//This prevents infinite looping and allows the deserialiser to create a reference.
				
				writer.WriteAttributeString( "idref", doneObjects[target].ToString() );
				
				writer.WriteEndElement();
			}
			else 
			{


				//Write the type of the target
				writer.WriteAttributeString( "type", GetTypeName(target.GetType()) );

				//For simple value types we just store the value as a string.
				if (TypeChecker.TypeIsAtomic(target.GetType()))
				{
					writer.WriteAttributeString( "value", Format(target) );
					writer.WriteEndElement(); 
					return;		//<--- Early Exit
				}

				//Create and write a unique ref for the object
				int objectRef = GetNextID();

				writer.WriteAttributeString( "id", objectRef.ToString(CultureInfo.InvariantCulture) );

				doneObjects.Add( target, objectRef.ToString(CultureInfo.InvariantCulture) );

				if (target is byte[]) 
				{
					WriteArrayProperties(target, writer);

					writer.WriteStartElement("Base64");

					//Write the byte array out as base64
					int doneBytes = 0;
					int totalBytes = ((byte[])target).Length;
					while (doneBytes < totalBytes) 
					{
						int bytesToWrite = Math.Min(BINARY_CHUNK_SIZE, totalBytes - doneBytes);
						writer.WriteBase64((byte[])target, doneBytes, bytesToWrite);
						doneBytes += bytesToWrite;
					}

					writer.WriteEndElement();
				}
                else if (target is IXmlSerializable)
                {
                    ((IXmlSerializable)target).WriteXml(writer);
                }
				else if (target.GetType().IsArray) 
				{
					WriteArrayProperties(target, writer);

					//Write the values of the collection.
					WriteCollection(target, writer, doneObjects);

				}
				else if (target is Type) 
				{
					//If the target is an instance of System.Type we just store enough information
					//to identify it rather than serialising all its properties.
					writer.WriteStartElement("Properties");
					//writer.WriteAttributeString( "Name", ((Type)target).Name );
					writer.WriteAttributeString("Name", GetTypeName((Type)target));
					
					writer.WriteEndElement();
				}
				else 
				{
					//Write the atomic properties as attributes
					IDictionary children = WriteAttributes(target, writer);

					//Handle the items if this class is a collection
					WriteCollection(target, writer, doneObjects);

					//Write each sub-object (child) into a child element
					WriteNamedList(children, writer, doneObjects);
				}

				writer.WriteEndElement();
			}
			
		}

		private bool TypeIsInDefaultAssembly(Type type) 
		{
			//mscorlib is always a default assembly.
			string fullName = type.AssemblyQualifiedName.ToLower(CultureInfo.InvariantCulture);
			if (fullName.StartsWith("system.") && fullName.IndexOf(", mscorlib,") > 0)
			{
				return true;
			}

			string[] defaultAssemblies = _defaultAssemblyName.Split(';');

			foreach (string defaultAssembly in defaultAssemblies) 
			{
				if (defaultAssembly != null && defaultAssembly.Length > 0 && 
					type.Assembly.FullName.StartsWith(defaultAssembly)) 
				{
					return true;
				}
			}

			return false;

		}

		/// <summary>
		/// Writes the element type and diminsions of the array to 
		/// a Properties XML element.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="writer"></param>
		private void WriteArrayProperties(object target, XmlWriter writer) 
		{
			writer.WriteStartElement("Properties");
			writer.WriteAttributeString( "ElementType", GetTypeName( target.GetType().GetElementType() ) );

			string lengths = "";
			string lowerBounds = "";

			Array array = (Array)target;
			for (int i = 0; i < array.Rank; i++) 
			{
				lengths += "," + array.GetLength(i);
				lowerBounds += "," + array.GetLowerBound(i);
			}

			writer.WriteAttributeString("Lengths", lengths.TrimStart(new char[] {','}));
			writer.WriteAttributeString("LowerBounds", lowerBounds.TrimStart(new char[] {','}));
			writer.WriteEndElement();
		}

		private string GetTypeName(Type type) 
		{
			if (TypeIsInDefaultAssembly(type)) 
			{
				string typeName = type.FullName;

				if (_defaultNamespace != null && _defaultNamespace.Length > 0 && typeName.StartsWith(_defaultNamespace)) 
				{
					typeName = typeName.Remove(0, _defaultNamespace.Length + 1); 
				}

				return typeName;
			}
			else 
			{
				return type.AssemblyQualifiedName;
			}
		}

		/// <summary>
		/// Writes the atomic properties of an object into the XML as attributes.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="writer"></param>
		/// <returns></returns>
		private IDictionary WriteAttributes(object target, XmlWriter writer) 
		{
			Hashtable attributes = new Hashtable();
			Hashtable children = new Hashtable();
			Hashtable types = new Hashtable();

			//Get the list of properties or fields
			if (_useFields) 
			{
				foreach (FieldInfo fieldInfo in target.GetType().GetFields()) 
				{
					if (fieldInfo.GetCustomAttributes(typeof(XmlIgnoreAttribute), true).Length == 0) 
					{
						attributes.Add(fieldInfo.Name, fieldInfo.GetValue(target));
						types.Add(fieldInfo.Name, fieldInfo.FieldType);
					}
				}
			}
			else 
			{
				foreach (PropertyInfo propInfo in target.GetType().GetProperties()) 
				{ 
					if (! SkipProperty(target, propInfo) && propInfo.GetIndexParameters().Length == 0) 
					//if (propInfo.GetIndexParameters().Length == 0) 
					{
						if (propInfo.GetCustomAttributes(typeof(XmlIgnoreAttribute), true).Length == 0) 
						{
							attributes.Add(propInfo.Name, propInfo.GetValue(target, null));
							types.Add(propInfo.Name, propInfo.PropertyType);
						}
					}
				}
			}

			writer.WriteStartElement("Properties");

			//For each item in the list, write it as an attribute or add it to the collection of children.
			foreach (DictionaryEntry item in attributes) 
			{				
				Type type = (Type)types[item.Key];
				if (item.Value == null) 
				{
					//Do nothing.
				}
				else if (TypeChecker.TypeIsAtomic(type)) 
				{
					//writer.WriteAttributeString((string)item.Key, item.Value.ToString());
					writer.WriteAttributeString((string)item.Key, Format(item.Value));
				}
				else 
				{
					children.Add(item.Key, item.Value);
				}
			}

			writer.WriteEndElement();

			//Return the list of children
			return children;
		}

		/// <summary>
		/// Returns true if the specified property should be left out of the serialisation.
		/// E.g. the Keys, Values and SynchRoot properties of a Hashtable.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="testPropInfo"></param>
		/// <returns></returns>
		private bool SkipProperty( object target, PropertyInfo testPropInfo ) 
		{
			if (_skipList == null) 
			{
				_skipList = new ArrayList();

				foreach (PropertyInfo propInfo in typeof(IDictionary).GetProperties()) 
				{
					if (propInfo.Name != "Count") 
					{
						_skipList.Add( propInfo.Name );
					}
				}
				foreach (PropertyInfo propInfo in typeof(ICollection).GetProperties()) 
				{
					if (propInfo.Name != "Count") 
					{
						_skipList.Add( propInfo.Name );
					}
				}
			}

			return (target is IDictionary || target is ICollection) && _skipList.Contains(testPropInfo.Name);
		}

		/// <summary>
		/// Formats the specified atomic value so it can be written out as XML.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private string Format(object value) 
		{
			if (value is DateTime) 
			{
				return ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss.fffffff", Culture);
			}
			if (value is Guid) 
			{
				return ((Guid)value).ToString();
			}
			else 
			{
				return Convert.ToString(value, Culture);
			}
		}

		/// <summary>
		/// Sets the value of the specified property of the specified object instance.
		/// </summary>
		/// <param name="propInfo"></param>
		/// <param name="obj"></param>
		/// <param name="value"></param>
		private void SetValue(PropertyInfo propInfo, object obj, string value) 
		{
			propInfo.SetValue(obj, GetValue(propInfo.PropertyType, value), null);
		}

		/// <summary>
		/// Shortcut for CultureInfo.InvariantCulture.
		/// </summary>
		private CultureInfo Culture
		{
			get
			{
				CultureInfo culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
				culture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
				culture.DateTimeFormat.DateSeparator = "-";
				return culture;
			}
		}

		/// <summary>
		/// Parses the specified string read form the XML and returns the value
		/// it represents as the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private object GetValue(Type type, string value) 
		{
			if (type == typeof(string)) 
			{
				return value;
			}
			else if (type == typeof(bool)) 
			{
				return Convert.ToBoolean(value, Culture);
			}
			else if (type == typeof(DateTime)) 
			{
				if (value.Length == 0) 
				{
					return DateTime.MinValue;
				}
				else
				{
					return DateTime.Parse(value, Culture); 
					//return Convert.ToDateTime(value, Culture);
				}
			}
			else if (type == typeof(TimeSpan)) 
			{
				return  TimeSpan.Parse(value); //Convert.Tot(value, Culture);
			}
			else if (type == typeof(Int16)) 
			{
				return  Convert.ToInt16(value, Culture);
			}
			else if (type == typeof(Int32)) 
			{
				return  Convert.ToInt32(value, Culture);
			}
			else if (type == typeof(Int64)) 
			{
				return  Convert.ToInt64(value, Culture);
			}
			else if (type == typeof(float)) 
			{
				return  Convert.ToSingle(value, Culture);
			}
			else if (type == typeof(double)) 
			{
				return  Convert.ToDouble(value, Culture);
			}
			else if (type == typeof(decimal)) 
			{
				return  Convert.ToDecimal(value, Culture);
			}
			else if (type == typeof(char)) 
			{
				return  Convert.ToChar(value, Culture);
			}
			else if (type == typeof(byte)) 
			{
				return  Convert.ToByte(value, Culture);
			}
			else if (type == typeof(UInt16)) 
			{
				return  Convert.ToUInt16(value, Culture);
			}
			else if (type == typeof(UInt32)) 
			{
				return  Convert.ToUInt32(value, Culture);
			}
			else if (type == typeof(UInt64)) 
			{
				return  Convert.ToUInt64(value, Culture);
			}
			else if (type.IsEnum) 
			{
				return Enum.Parse(type, value, true);
			}
			else if (type == typeof(Guid)) 
			{
				return new Guid(value);
			}
			else 
			{
				return null;
			}
		}

		/// <summary>
		/// Writes out the items of the specified ICollection into 
		/// the specified XmlWriter.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="writer"></param>
		/// <param name="doneObjects"></param>
		private void WriteCollection(object target, XmlWriter writer, IDictionary doneObjects) 
		{
			if (target is IDictionary) 
			{
				writer.WriteStartElement("Items");

				foreach(DictionaryEntry entry in (IDictionary)target) 
				{
					writer.WriteStartElement("Item");
					Serialize(entry.Key, writer, "Key", doneObjects);  
					Serialize(entry.Value, writer, "Value", doneObjects);  
					writer.WriteEndElement();
				}
				writer.WriteEndElement();
			}
			else if (target is IList) 
			{
				writer.WriteStartElement("Items");

				foreach(object item in (ICollection)target) 
				{
					Serialize(item, writer, "Item", doneObjects);  
				}
				writer.WriteEndElement();
			} 
		}

		/// <summary>
		/// Writes the sub-objects contained in the properties of a parent 
		/// instance to the specified XmlWriter.
		/// </summary>
		/// <param name="list"></param>
		/// <param name="writer"></param>
		/// <param name="doneObjects"></param>
		private void WriteNamedList(IDictionary list, XmlWriter writer, IDictionary doneObjects) 
		{

			foreach (DictionaryEntry item in list) 
			{
				this.Serialize(item.Value, writer, (string)item.Key, doneObjects);
			}
		}

		/// <summary>
		/// Gets an ID to assign the object in the Xml. The ID is used to
		/// refer to the object if it occurres more than once in the graph.
		/// </summary>
		/// <returns></returns>
		private int GetNextID() 
		{
			_nextID++;
			return _nextID;
		}

		/// <summary>
		/// Deserialises an object from the specified XmlReader.
		/// </summary>
		/// <param name="reader">The XmlReader containing the serialised state.</param>
		/// <returns>An object graph fully populated (deserilaised) from the XmlReader.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <i>reader</i> is null (Nothing in Visual Basic).</exception>
		/// <exception cref="TypeNotFoundException">
		/// Thrown if a type named encountered in the serialised data cannot be located in an assembly or the ExtraTypes list.
		/// </exception>
		public object Deserialize(XmlTextReader reader) 
		{
			ExceptionHelper.ExceptionIfNull(reader, "reader");

			return Deserialize(reader, new Hashtable());
		}

		/// <summary>
		/// Deserialises an object from the specified XmlNode.
		/// </summary>
		/// <param name="node">The XmlNode containing the serialised state.</param>
		/// <returns>An object graph fully populated (deserilaised) from the XmlNode.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <i>node</i> is null (Nothing in Visual Basic).</exception>
		/// <exception cref="TypeNotFoundException">
		/// Thrown if a type named encountered in the serialised data cannot be located in an assembly or the ExtraTypes list.
		/// </exception>
		public object Deserialize(XmlNode node) 
		{
			ExceptionHelper.ExceptionIfNull(node, "node");

			MemoryStream stream = new MemoryStream();
			XmlTextWriter writer = new XmlTextWriter(stream, Encoding.Unicode);
			node.WriteTo(writer);
			writer.Flush();

			stream.Position = 0;

			XmlTextReader reader = new XmlTextReader(stream);

			return Deserialize(reader);
		}

		/// <summary>
		/// Deserialises an object from the specified XmlReader.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="doneObjects"></param>
		/// <returns></returns>
		private object Deserialize(XmlTextReader reader, IDictionary doneObjects) 
		{

			object obj = null;

			//Scroll the reader forward until it is on the start of an element
			//We will then attempt to read data from that element
			while (reader.NodeType != XmlNodeType.Element && !reader.EOF)
				reader.Read();

			//Check if we have already done this object, and if so,
			//return the stored instance rather than re-creating another instance.
			string refID = reader.GetAttribute("idref");
			if (refID != null && refID.Length > 0) 
			{
				obj = doneObjects[Convert.ToInt32(refID, CultureInfo.InvariantCulture)];
				ReadToEndOfCurrentElement(reader);
				return obj;		//<--- Early Exit
			}

			//Get the type of the object we are about to de-serialise.
			string typeName = reader.GetAttribute("type");

			//Record the depth so we know if the next element is a sibling or a child
			int depth = reader.Depth;
			string name = reader.LocalName;
			int objID = Convert.ToInt32(reader.GetAttribute("id"), CultureInfo.InvariantCulture);

			if (typeName.Length == 0) 
			{
				//Error? Skip this entire node. Read until the reader is positioned on whatever comes
				//after our end tag (should be the start of a sibling or the end of our parent)
				ReadToEndOfCurrentElement(reader);
			}
			else if (typeName.StartsWith("System.RuntimeType")) 
			{
				//If the serialised object is an Instance of System.Type we just find and return the type.

				//Find the properties
				if (ReadToStartOfElement(reader, "Properties", reader.Depth)) 
				{ 
					obj = GetTypeFromName(reader.GetAttribute("Name"));

					doneObjects.Add(objID, obj);

					//Move off the properties node
					ReadToEndOfCurrentElement(reader);
				}
			}
			else 
			{
				//Create an object for the current element
				obj = DesirialiseNormalObject(typeName, reader, doneObjects, objID, depth);
			}

			//To complete the read we want to consume the end tag for this element if it exists 
			//and any whitespace after it. This is so we do not leave the reader in the middle of
			//an element when we return.

			//Read to the next element only if the end tag is for this node. 
			//Not all nodes will necessarily have an end tag depending on how they were serialized.
			//Also there may or may not be whitespace between nodes, again depending on how they were serialized.
			if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == name) 
			{
				reader.Read();

				//Now may be on Whitespace
				if (reader.NodeType == XmlNodeType.Whitespace)
					reader.Read();
			}

			//Now on next element
			return obj;

		}

		/// <summary>
		/// Deserializes an object that is not an instance of System.Type.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="reader"></param>
		/// <param name="doneObjects"></param>
		/// <param name="objID"></param>
		/// <param name="depth"></param>
		/// <returns></returns>
		private object DesirialiseNormalObject(string typeName, XmlTextReader reader, IDictionary doneObjects, int objID, int depth)
		{
			Object obj;
			
			if (typeName.IndexOf("[") >= 0) //Handle Arrays
			{
				//Create a blank array
				obj = CreateArray(reader,depth);
			}
			else 
			{
				obj = CreateObject(typeName);
			}

            if (obj is IXmlSerializable)
            {
                int currentDepth = reader.Depth;

                ((IXmlSerializable)obj).ReadXml(reader);
            }
            else if (TypeChecker.TypeIsAtomic(obj.GetType()))
			{
				obj = GetValue(obj.GetType(), reader.GetAttribute("value"));
				reader.Read();
			}
			else
			{
				doneObjects.Add(objID, obj);

				if (obj.GetType().IsArray) 
				{
					ReadArray((Array)obj, reader, depth, doneObjects);
				}
				else 
				{
					//Find and read the properties
					reader.Read();
					ReadProperties(reader, depth, obj);

					//Read the items if it is a collection
					ReadCollectionItems(obj, reader, depth, doneObjects);

					//Read children
					ReadChildren(reader, depth, obj, doneObjects);
				}

			}

			return obj;
		}

		/// <summary>
		/// Reads and populates the atomic properties of the current object. 
		/// (I.e. those stored as attributes in the properties element 
		/// rather than as children in child elements.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="depth"></param>
		/// <param name="obj"></param>
		private void ReadProperties(XmlReader reader, int depth, object obj)
		{
			if (ReadToStartOfElement(reader, "Properties", depth + 1))
			{
				//Read the properties
				while (reader.MoveToNextAttribute()) 
				{
					PropertyInfo propInfo = obj.GetType().GetProperty(reader.Name);

					if (propInfo.CanWrite) 
					{
						//propInfo.SetValue(obj, reader.Value, null);
						SetValue(propInfo, obj, reader.Value); 
					}
				}

				//Move off the properties node
				ReadToEndOfCurrentElement(reader);
			}
		}

		/// <summary>
		/// If the current object being de-serialised is a collection, this routine reads its 
		/// serialised items and populates the collection.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="reader"></param>
		/// <param name="depth"></param>
		/// <param name="doneObjects"></param>
		private void ReadCollectionItems(object obj, XmlTextReader reader, int depth, IDictionary doneObjects)
		{
			IDictionary dictionary = obj as IDictionary;

			if (dictionary != null) 
			{
				if (ReadToStartOfElement(reader, "Items", depth + 1))
				{
					if (! reader.IsEmptyElement) 
					{
						reader.Read();
						while (ReadToStartOfElement(reader, "Item", reader.Depth)) 
						{
							ReadToStartOfElement(reader, "Key", reader.Depth);

							object key = Deserialize(reader, doneObjects);
							object value = Deserialize(reader, doneObjects);

							dictionary.Add(key, value);

                            ReadToEndOfCurrentElement(reader);

						}
					}
					else
					{
						reader.Read();
					}

					//Consume the Items end tag if there is one.
					if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "Items") 
					{
						ReadToEndOfCurrentElement(reader);
					}

				}
			}
			else if (obj is IList) 
			{
				if (ReadToStartOfElement(reader, "Items", depth + 1))
				{
					IList list = (IList)obj;
					
					while (ReadToStartOfElement(reader, "Item", reader.Depth)) 
					{
						string typeName = reader.GetAttribute("type");

						//typeName could be null if the item is an idref, in which case it is not an atomic type.
						if (typeName != null && TypeChecker.TypeIsAtomic(GetTypeFromName(typeName))) 
						{
							list.Add(GetValue(GetTypeFromName(typeName), reader.GetAttribute("value")));
							ReadToEndOfCurrentElement(reader);
						}
						else 
						{
							list.Add(Deserialize(reader, doneObjects));
						}

					}

					//Consume the Items end tag if there is one
					if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "Items") 
					{
						ReadToEndOfCurrentElement(reader);
					}
				}
			}
		}

		/// <summary>
		/// Reads the child elements and desirialises the object they represent.
		/// Then populates the properties on the current parent object with the de-serialised child objects.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="depth"></param>
		/// <param name="obj"></param>
		/// <param name="doneObjects"></param>
		private void ReadChildren(XmlTextReader reader, int depth, object obj, IDictionary doneObjects)
		{
			while (! reader.EOF && reader.Depth > depth) 
			{
				PropertyInfo propInfo = obj.GetType().GetProperty(reader.LocalName);

				if (propInfo != null) 
				{
					if (propInfo.CanWrite) 
					{
						propInfo.SetValue(obj, Deserialize(reader, doneObjects), null);
					}
					else if (TypeChecker.TypeIsCollection(propInfo.PropertyType))
					{
						//If the property is readonly, and a collection type, we may be able 
						//to populate the collection one item at a time.
						PopulateReadOnlyCollection(reader, doneObjects, propInfo, obj);
					}
					else 
					{
						ReadToEndOfCurrentElement(reader);
					}
				}
				else 
				{
					ReadToEndOfCurrentElement(reader);
				}
			}
		}

		/// <summary>
		/// If the property is readonly, and a collection type, we may be able 
		/// to populate the collection one item at a time.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="doneObjects"></param>
		/// <param name="propInfo"></param>
		/// <param name="obj"></param>
		private void PopulateReadOnlyCollection(XmlTextReader reader, IDictionary doneObjects, PropertyInfo propInfo, object obj)
		{
			object child = Deserialize(reader, doneObjects);
	
			if (child is IList) 
			{
				IList list = (IList)propInfo.GetValue(obj, null);
				if (list != null) 
				{
					list.Clear();
					foreach (object item in (IList)child) 
					{
						list.Add(item);
					}
				}
			}
			else if (child is IDictionary) 
			{
				IDictionary list = (IDictionary)propInfo.GetValue(obj, null);
				if (list != null) 
				{
					list.Clear();
					foreach (DictionaryEntry item in (IDictionary)child) 
					{
						list.Add(item.Key, item.Value);
					}
				}
			}
		}

		/// <summary>
		/// Creates and returns an instance of the specified type.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		private object CreateObject(string typeName) 
		{
			if (typeName.StartsWith("System.String"))
			{
				return string.Empty;
			}
			else 
			{
				return Activator.CreateInstance(GetTypeFromName(typeName));
			}
		}

		/// <summary>
		/// Creates an array by reading the element type and dimensions of the 
		/// array from the xmlReader and instanciating an array with the specified properties.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		private object CreateArray(XmlTextReader reader, int depth) 
		{
			object array = null;

			if (ReadToStartOfElement(reader, "Properties", depth))
			{
				string elementType = reader.GetAttribute("ElementType");
				string lengths = reader.GetAttribute("Lengths");
				string lowerBounds = reader.GetAttribute("LowerBounds");

				string[] lengthArray = lengths.Split(',');
				string[] lowerBoundArray = lowerBounds.Split(',');

				int[] lengthIntArray = new int[lengthArray.Length];
				int[] lowerBoundIntArray = new int[lowerBoundArray.Length];

				for (int i = 0; i < lengthArray.Length; i++) 
				{
					lengthIntArray[i] = int.Parse(lengthArray[i], CultureInfo.InvariantCulture);
				}

				for (int i = 0; i < lengthArray.Length; i++) 
				{
					lowerBoundIntArray[i] = int.Parse(lowerBoundArray[i], CultureInfo.InvariantCulture);
				}

				Type type = GetTypeFromName(elementType);

				array = Array.CreateInstance(type, lengthIntArray, lowerBoundIntArray);

				ReadToEndOfCurrentElement(reader);
			}

			return array;
		}

		/// <summary>
		/// Reads the items of an (possibly multi-dimensional) array from the Xml
		/// into the specified array instance, which has already been created with the correct size.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="reader"></param>
		/// <param name="depth"></param>
		/// <param name="doneObjects"></param>
		private void ReadArray(Array array, XmlTextReader reader, int depth, IDictionary doneObjects) 
		{
			//Array array = (Array)obj;

			if (array is byte[]) 
			{
				ReadToStartOfElement(reader, "Base64", depth);

				//Read a byte array from base64 encoding.
				int doneBytes = 0;
				byte[] bytes = (byte[])array;
				int totalBytes = bytes.Length;
				while (doneBytes < totalBytes) 
				{
					int bytesToRead = Math.Min(BINARY_CHUNK_SIZE, totalBytes - doneBytes);
					doneBytes += reader.ReadBase64(bytes, doneBytes, bytesToRead);
					//doneBytes += bytesToRead;
				}

				ReadToEndOfCurrentElement(reader); 
			}
			else 
			{

				//Get all the items into an ArrayList
				ArrayList items = new ArrayList(array.Length);
				ReadCollectionItems(items, reader, depth, doneObjects);

				//Initialise a set of counter arrays for the array based on its rank.
				int[] lengths = new int[array.Rank];
				int[] lowerBounds = new int[array.Rank];
				int[] counters = new int[array.Rank];

				for ( int i = 0; i < array.Rank; i++) 
				{
					lengths[i] = array.GetLength(i);
					lowerBounds[i] = array.GetLowerBound(i);
					counters[i] = array.GetLowerBound(i);
				}

				//
				//Set all the items in the array.
				//
				foreach (object item in items) 
				{	
					//Set the value at the current set of indicies.
					array.SetValue(item, counters);

					//bump the last index
					counters[counters.Length -1]++;

					//go from the last to the 1st index, and if the index has gone past
					//its upper bound, reset it to its lower bound and bump the one before it.
					for (int i = counters.Length -1; i >= 0; i--) 
					{
						if (i > 0 && counters[i] == lowerBounds[i] + lengths[i]) 
						{
							counters[i] = lowerBounds[i];
							counters[i - 1]++;
						}
					}
				}

			}
		}

		/// <summary>
		/// Given the name of a type, works out and returns the System.Type it represents.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		private Type GetTypeFromName(string typeName) 
		{

			Type type = null;

			if (_types == null) 
			{
				_types = new Hashtable();
			}

			//Check if the type is in the cache
			if (_types.Contains(typeName)) 
			{
				return (Type)_types[typeName];
			}

			//Load types that are in the default namespace in the default assembly
			if (_defaultNamespace.Length > 0 && ! typeName.StartsWith(_defaultNamespace)) 
			{
				string fullTypeName = _defaultNamespace + "." + typeName;

				type = GetTypeFromDefaultAssembly(fullTypeName);
			}

			//Load types in the default assembly but not the default namespace
			if (type == null) 
			{
				type = GetTypeFromDefaultAssembly(typeName);
			}

			//Load types that are not in the default assembly
			if (type == null) 
			{
				type = GetTypeFromAssemblyQualifiedName(typeName);
			}

			//If we still have not found the type, try to load it ignoring the version on the assembly
			if (type == null) 
			{
				int versionIndex = typeName.IndexOf(", Version=");
				if (versionIndex >= 0)
				{
					string shortTypeName = GetTypeNameWithoutVersion(typeName); 
					type = GetTypeFromAssemblyQualifiedName(shortTypeName);
				}
			}

			if (type == null)
			{
				throw new TypeNotFoundException(string.Format(Culture, TypeNotFoundException.MessageText, typeName));
			}

			//If we found the type, add it to the cache for performance.
			_types.Add(typeName, type);

			return type;
		}

		/// <summary>
		/// Returns the type name that includes all elements of the full name except the version.
		/// This allows objects to be de-serialised from a stream that contains a different version.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		private static string GetTypeNameWithoutVersion(string typeName) 
		{
			string[] nameElements = typeName.Split(',');
			string newName = "";

			foreach (string nameElement in nameElements) 
			{		
				if (nameElement.IndexOf("Version=") < 0) 
				{
					if (newName.Length == 0) 
					{
						newName = nameElement;
					}
					else 
					{
						newName += "," + nameElement;
					}
				}
			}

			return newName;
		}

		/// <summary>
		/// Searches for the Type matching the specified Assembly Qualified Type name.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		private Type GetTypeFromAssemblyQualifiedName(string typeName)
		{
			Type type = null;

			string[] nameElements = typeName.Split(',');

			type = Type.GetType(typeName);

			if (type == null && this.Assemblies != null)
			{
				//Search the Assemblies List for a type whos name contains each 
				//part of the typeName that we are looking for. The elements of the
				//name may not be in the same order, and they may not all be present.
				foreach (Assembly assembly in this.Assemblies)
				{
					type = SearchAssembly(assembly, nameElements);

					//If we have found the type, bailout of the assembly loop.
					if (type != null)
					{
						break;
					}
				}
			}

			//If the type was not found in one of the base assemblies we are
			//using for deserialisation, search for it in any of the assemblies
			//loaded in the current AppDomain.
			if (type == null) 
			{
				type = SearchLoadedAssemblies(nameElements);
			}

			//If we did not find the type in the assembly list, search in 
			//their referenced assemblies. 
			if (type == null && this.Assemblies != null)
			{
				foreach (Assembly assembly in this.Assemblies)
				{
					type = SearchReferencedAssemblies(assembly, nameElements);

					if (type != null)
					{
						break;
					}
				}
			}

			return type;
		}

		/// <summary>
		/// Searches the specified assembly for the given type.
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="nameElements"></param>
		/// <returns></returns>
		private static Type SearchAssembly(Assembly assembly, string[] nameElements)
		{
			Type type = null;

			foreach (Type testType in assembly.GetTypes())
			{
				bool found = true;
				foreach (string nameElement in nameElements) 
				{
					if (testType.AssemblyQualifiedName.IndexOf(nameElement) < 0)
					{
						found = false;
					}
				}

				if (found)
				{
					type = testType;
					break;
				}
			}

			return type;
		}

		/// <summary>
		/// Searches for a specified type in the loaded assemblies.
		/// </summary>
		/// <param name="nameElements"></param>
		/// <returns></returns>
		private static Type SearchLoadedAssemblies(string[] nameElements) 
		{ 
			Type type = null;

			foreach (Assembly loadedAssembly in AppDomain.CurrentDomain.GetAssemblies()) 
			{
				if (! (loadedAssembly.FullName.StartsWith("mscorlib") || loadedAssembly.FullName == "System" 
							|| loadedAssembly.FullName.StartsWith("System.")) ) 
				{
					type = SearchAssembly(loadedAssembly, nameElements);
					if (type != null) 
					{
						break;
					}
				}
			}

			return type;
		}

		/// <summary>
		/// Searches the assemblies referenced by a specified assembly for a given type.
		/// Does not search system assemblies, assemblies in the GAC, or assemblies that are 
		/// already loaded, since these assemblies will already have been searched.
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="nameElements"></param>
		/// <returns></returns>
		private Type SearchReferencedAssemblies(Assembly assembly, string[] nameElements)
		{
			Type type = null;

			foreach (AssemblyName assemblyName in assembly.GetReferencedAssemblies())
			{
				bool search = true;

				//If the assembly has already been search because it is in the GAC
				//or in the list of assemblies, skip it here.
				if (assemblyName.FullName.StartsWith("mscorlib") || assemblyName.FullName.StartsWith("System.")) 
				{
					search = false;
				}
				else 
				{
					foreach (Assembly baseAssembly in this.Assemblies)
					{
						if (baseAssembly.FullName == assemblyName.FullName)
						{
							search = false;
						}
					}

					if (search) 
					{
						foreach (Assembly loadedAssembly in AppDomain.CurrentDomain.GetAssemblies())
						{
							if (loadedAssembly.FullName == assemblyName.FullName)
							{
								search = false;
							}
						}
					}
				}

				if (search)
				{
					try 
					{
						Assembly referencedAssembly = null;
							
						if (assemblyName.CodeBase == null || assemblyName.CodeBase.Length == 0) 
						{
							string directory = Path.GetDirectoryName(assembly.Location);

							//Add the culture to the assembly if it is in the 
							if (assemblyName.CultureInfo != null && 
								assemblyName.CultureInfo.LCID != CultureInfo.InvariantCulture.LCID && 
								! assemblyName.CultureInfo.IsNeutralCulture) 
							{
								referencedAssembly = SearchForAssembly(directory + Path.DirectorySeparatorChar + assemblyName.CultureInfo.TwoLetterISOLanguageName, assemblyName);
							}

							//If there was no culture or we could not find it, search for a neutral culture version.
							if (referencedAssembly == null)
							{
								referencedAssembly = SearchForAssembly(directory, assemblyName);
							}
						}

						//If there is a CodeBase defined, or we could not find it by probing, try to use 
						//the AssemblyName to load the referenced assembly. This will throw a FileNotFoundException if it fails.
						if (referencedAssembly == null) 
						{
							referencedAssembly = Assembly.Load(assemblyName);
						}

						//Search for the type in the assembly.
						type = SearchAssembly(referencedAssembly, nameElements);

						//If we have not found the type, recursively search through the reference chain.
						if (type == null)
						{
							type = SearchReferencedAssemblies(referencedAssembly, nameElements);
						}
					}
					catch (FileNotFoundException) 
					{
						//Do Nothing. If we cannot find the type in any Assembly,
						//A TypeNotFoundException will be thrown.
					}
				}
			}

			return type;
		}

		/// <summary>
		/// Probes in the specified directory for an assembly matching the given AssemblyName.
		/// </summary>
		/// <param name="directory"></param>
		/// <returns></returns>
		private static Assembly SearchForAssembly(string directory, AssemblyName assemblyName) 
		{
			Assembly assembly = CheckPath(directory + Path.DirectorySeparatorChar + assemblyName.Name + ".dll");

			if (assembly == null) 
			{
				assembly = CheckPath(directory + Path.DirectorySeparatorChar + assemblyName.Name + Path.DirectorySeparatorChar + assemblyName.Name + ".dll");
			}

			return assembly;
		}

		private static Assembly CheckPath(string testPath) 
		{
			if (File.Exists(testPath))
			{
				return Assembly.LoadFrom(testPath);
			}
			else 
			{
				return null;
			}
		}

		private Type GetTypeFromDefaultAssembly(string typeName) 
		{
            //Bailout if the typeName already has assembly information.
            if (typeName.IndexOf(", Culture=") > 0)
            {
                return null;
            }

			string[] defaultAssemblies = _defaultAssemblyName.Split(';');

			foreach (string defaultAssembly in defaultAssemblies) 
			{
				Type type = Type.GetType(typeName + ", " + defaultAssembly);
				if (type != null) 
				{
					return type;
				}
			}

			return null;
		}

		/// <summary>
		/// Reads until the end of the current element is reached.
		/// Returns true if there is anything after the end of the element.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		private bool ReadToEndOfCurrentElement(XmlReader reader) 
		{

			int depth = reader.Depth; 

			if (reader.NodeType == XmlNodeType.Element) 
			{
				reader.Read();
			}

			while (!reader.EOF && reader.Depth > depth || (reader.Depth == depth && reader.NodeType != XmlNodeType.Element  && reader.NodeType != XmlNodeType.Text)) 
			{
				reader.Read();
			}

			if (reader.NodeType == XmlNodeType.EndElement && reader.Depth >= depth) 
			{
				reader.Read();
			}

			while (!reader.EOF && reader.NodeType == XmlNodeType.Whitespace) 
			{
				reader.Read();
			}

			return ! reader.EOF;
		}

		/// <summary>
		/// Read until we find the start of the named element or we leave the specified depth.
		/// Returns true if the element is found.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="elementName"></param>
		/// <param name="depth"></param>
		/// <returns></returns>
		private bool ReadToStartOfElement(XmlReader reader, string elementName, int depth) 
		{

			while (!reader.EOF && reader.Depth >= depth && (reader.NodeType != XmlNodeType.Element || reader.LocalName != elementName)) 
			{
				reader.Read();
			}

			return reader.LocalName == elementName && reader.NodeType == XmlNodeType.Element;
				
		}
	}

}

