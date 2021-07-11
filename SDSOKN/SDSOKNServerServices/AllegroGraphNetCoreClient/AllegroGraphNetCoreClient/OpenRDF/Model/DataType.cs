namespace AllegroGraphNetCoreClient.OpenRDF.Model
{
    public class DataType
    {
        public DataType(string kind, string part, string encoding)
        {
            Kind = kind;
            Part = part;
            Encoding = encoding;
        }

        /// <summary>
        /// Datatype or predicate
        /// </summary>
        public string Kind { get; }
        /// <summary>
        /// The resource associated with the mapping
        /// </summary>
        public string Part { get; }
        /// <summary>
        /// Encoding fields.
        /// </summary>
        public string Encoding { get; }
        public override bool Equals(object obj)
        {
            if (obj is DataType)
            {
                DataType dtObj = obj as DataType;
                if (this.Kind == dtObj.Kind && this.Part == dtObj.Part && this.Encoding == dtObj.Encoding)
                    return true;
                else
                    return false;
            }
            else
                // ReSharper disable once BaseObjectEqualsIsObjectEquals
                return base.Equals(obj);
        }

        protected bool Equals(DataType other)
        {
            return Kind == other.Kind && Part == other.Part && Encoding == other.Encoding;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Kind != null ? Kind.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Part != null ? Part.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Encoding != null ? Encoding.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
