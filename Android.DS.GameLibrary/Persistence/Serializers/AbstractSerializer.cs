using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLibrary.Core.Serializers
{
    /// <summary>
    /// Represents a abstract serializer for data serializarion. 
    /// </summary>
    public abstract class AbstractSerializer<T>
    {
        public AbstractSerializer(String fileName)
        {
            FileName = fileName;
            KnownTypes = new List<Type>();
        }

        /// <summary>
        /// Saves given data.
        /// </summary>
        /// <param name="dataToSave">Data to be saved.</param>
        public abstract void Save(T dataToSave);

        /// <summary>
        /// Loads data from file.
        /// </summary>
        /// <returns>Data if file and data exists or null if some error occurs.</returns>
        public abstract T Load();

        /// <summary>
        /// Adds new Known Type for serialization.
        /// </summary>
        /// <param name="type"></param>
        public void AddKnownType(Type type)
        {
            if (type != null && !KnownTypes.Contains(type))
            {
                KnownTypes.Add(type);
            }
        }

        #region Properties

        public String FileName { get; protected set; }
        protected List<Type> KnownTypes { get; set; }

        #endregion
    }
}
