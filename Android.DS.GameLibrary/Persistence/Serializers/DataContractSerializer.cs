using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;

namespace GameLibrary.Core.Serializers
{
    public class DataContractSerializer<T> : AbstractSerializer<T>
    {
        public DataContractSerializer(String fileName)
            : base(fileName) { }

        public override void Save(T dataToSave)
        {
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myIsolatedStorage.FileExists(FileName))
                {
                    myIsolatedStorage.DeleteFile(FileName);
                }

                using (IsolatedStorageFileStream stream = myIsolatedStorage.OpenFile(FileName, FileMode.Create))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        Dictionary<Type, bool> dictionary = new Dictionary<Type, bool>();
                        StringBuilder builder = new StringBuilder();

                        foreach (Type type in KnownTypes)
                        {
                            if (type != null)
                            {
                                if (!type.IsPrimitive && (type != typeof(string)))
                                {
                                    dictionary[type] = true;
                                    if (builder.Length > 0)
                                    {
                                        builder.Append('\0');
                                    }

                                    builder.Append(type.AssemblyQualifiedName);
                                }
                            }
                        }

                        builder.Append(Environment.NewLine);
                        byte[] bytes = Encoding.UTF8.GetBytes(builder.ToString());
                        memoryStream.Write(bytes, 0, bytes.Length);
                        byte[] buffer = memoryStream.ToArray();
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }
            }
        }

        public override T Load()
        {
            T result = default(T);
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myIsolatedStorage.FileExists(FileName))
                {
                    using (IsolatedStorageFileStream stream = myIsolatedStorage.OpenFile(FileName, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            if (stream.Length > 0L)
                            {
                                try
                                {
                                    List<Type> knownTypes = new List<Type>();
                                    string str = reader.ReadLine();
                                    foreach (string str2 in str.Split(new char[1]))
                                    {
                                        Type item = Type.GetType(str2, false);
                                        if (item != null)
                                        {
                                            knownTypes.Add(item);
                                        }
                                    }
                                    stream.Position = str.Length + Environment.NewLine.Length;
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
            return result;
        }

        #region Properties
        #endregion
    }
}
