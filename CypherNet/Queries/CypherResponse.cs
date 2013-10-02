using System.Linq;

namespace CypherNet.Queries
{
    #region

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Serialization;

    #endregion

    internal interface ICypherResponse
    {
        Type ResponseType { get; }
    }

    internal class CypherResponse<TResult> : ICypherResponse
    {

        public CypherResponse()
        {
            Errors = Enumerable.Empty<string>().ToArray();
        }

        [JsonProperty(PropertyName = "commit", NullValueHandling = NullValueHandling.Ignore)]
        internal string Commit { get; set; }

        [JsonProperty(PropertyName = "results")]
        internal CypherResultSet<TResult> Results { get; set; }

        [JsonProperty(PropertyName = "errors")]
        internal string[] Errors { get; private set; }

        [JsonProperty(PropertyName = "transaction", NullValueHandling = NullValueHandling.Ignore)]
        internal TransactionDetails Transaction { get; private set; }

        [JsonIgnore]
        public Type ResponseType
        {
            get { return typeof (TResult); }
        }

        internal class TransactionDetails
        {
            [JsonProperty(PropertyName = "expires")]
            [JsonConverter(typeof (IsoDateConverter))]
            public DateTime Expires { get; private set; }
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