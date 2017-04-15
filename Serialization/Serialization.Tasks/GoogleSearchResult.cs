using System.Xml.Serialization;
using System.IO;
using System;
using System.Runtime.Serialization;
namespace Serialization.Tasks
{

    // TODO: Implement GoogleSearchResult class to be deserialized from Google Search API response
    // Specification is available at: https://developers.google.com/custom-search/v1/using_rest#WorkingResults
    // The test json file is at Serialization.Tests\Resources\GoogleSearchJson.txt


    
    [DataContract]
    public class GoogleSearchResult
    {
        [DataMember(Name = "kind")]
        public string Kind { get; set; }

        [DataMember(Name = "url")]
        public ComplexUrl Url { get; set; }

        [DataMember(Name = "queries")]
        public ComplexQueries Queries { get; set; }

        [DataMember(Name = "context")]
        public ComplexContext Context { get; set; }

        [DataMember(Name = "items")]
        public ItemsElements[] Items { get; set; }
    }

    [DataContract]
    public class ComplexUrl
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "template")]
        public string Template { get; set; }
    }

    [DataContract]
    public class ComplexQueries
    {
        [DataMember(Name = "nextPage")]
        public NextPageElements[] NextPage { get; set; }

        [DataMember(Name = "request")]
        public RequestElements[] Request { get; set; }

        [DataMember(Name = "previousPage")]
        public string PreviousPage { get; set; }
    }

    [DataContract]
    public class NextPageElements
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "totalResults")]
        public long TotalResults { get; set; }

        [DataMember(Name = "searchTerms")]
        public string SearchTerms { get; set; }

        [DataMember(Name = "count")]
        public int Count { get; set; }

        [DataMember(Name = "startIndex")]
        public int StartIndex { get; set; }

        [DataMember(Name = "inputEncoding")]
        public string InputEncoding { get; set; }

        [DataMember(Name = "outputEncoding")]
        public string OutputEncoding { get; set; }

        [DataMember(Name = "cx")]
        public string Cx { get; set; }
    }

    [DataContract]
    public class RequestElements
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "totalResults")]
        public long TotalResults { get; set; }

        [DataMember(Name = "searchTerms")]
        public string SearchTerms { get; set; }

        [DataMember(Name = "count")]
        public int Count { get; set; }

        [DataMember(Name = "startIndex")]
        public int StartIndex { get; set; }

        [DataMember(Name = "inputEncoding")]
        public string InputEncoding { get; set; }

        [DataMember(Name = "outputEncoding")]
        public string OutputEncoding { get; set; }

        [DataMember(Name = "cx")]
        public string Cx { get; set; }
    }

    [DataContract]
    public class ComplexContext
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }
    }

    [DataContract]
    public class ItemsElements
    {
        [DataMember(Name = "kind")]
        public string Kind { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "htmlTitle")]
        public string HtmlTitle { get; set; }

        [DataMember(Name = "link")]
        public string Link { get; set; }

        [DataMember(Name = "displayLink")]
        public string DisplayLink { get; set; }

        [DataMember(Name = "snippet")]
        public string Snippet { get; set; }

        [DataMember(Name = "htmlSnippet")]
        public string HtmlSnippet { get; set; }

        [DataMember(Name = "cx")]
        public string Cx { get; set; }
    }
}
