using System.Collections;
using CypherNet.Serialization;
using Newtonsoft.Json;

namespace CypherNet.Queries
{
    #region

    using System;
    using System.Collections.Generic;

    #endregion

    internal interface ICypherResponse
    {
        Type ResponseType { get; }
    }

    internal class CypherResponse<TResult> : ICypherResponse
    {

        [JsonProperty(PropertyName = "commit")]
        internal string Commit { get; set; }
        
        [JsonProperty(PropertyName = "results")]
        internal CypherResultSet<TResult> Results { get; set; }
            
        [JsonProperty(PropertyName = "errors")]
        internal string[] Errors { get; private set; }

        [JsonProperty(PropertyName = "transaction")]
        internal TransactionDetails Transaction { get; private set; }
        
        internal class TransactionDetails
        {
            [JsonProperty(PropertyName = "expires")]
            [JsonConverter(typeof(IsoDateConverter))]
            public DateTime Expires { get; private set; }
        }

        [JsonIgnore]
        public Type ResponseType
        {
            get { return typeof(TResult); }
        }
    }


    internal class CypherResultSet<TEnumerable> : IEnumerable<TEnumerable>
    {
        private readonly IEnumerable<TEnumerable> _results;

        public CypherResultSet(IEnumerable<TEnumerable> results)
        {
            _results = results;
        }

        public IEnumerator<TEnumerable> GetEnumerator()
        {
            return _results.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _results.GetEnumerator();
        }
    }
}