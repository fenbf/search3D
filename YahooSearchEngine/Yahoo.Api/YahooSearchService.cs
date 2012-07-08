using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace Yahoo.API
{
	/// <summary>
	/// YahooSearchService wrappes calls to the Yahoo Web Service.
	/// </summary>
	public class YahooSearchService
	{
		private const string _searchYahooApiAddress = "http://search.yahooapis.com/";
		private const string _localYahooApiAddress = "http://local.yahooapis.com/";

		public Yahoo.API.TermExtractionResponse.ResultSet TermExtraction(string appId, string query, string context)
		{
			// Set the uri to send the request to
			string requestUri = _searchYahooApiAddress + "ContentAnalysisService/V1/termExtraction";

			// Create the request object
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUri);
			
			// As the context parameter may be quite long
            // we will use a POST request
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
			{
				sw.Write("appid={0}&context={1}&query={2}", appId, context, query);
			}

			Yahoo.API.TermExtractionResponse.ResultSet resultSet = null;

			// Get the response from Yahoo
			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				using (Stream responseStream = response.GetResponseStream())
				{
					// Convert the response into an object model
					XmlSerializer serializer = new XmlSerializer(typeof(Yahoo.API.TermExtractionResponse.ResultSet));
					resultSet = (Yahoo.API.TermExtractionResponse.ResultSet)serializer.Deserialize(responseStream);
				}
			}

			return resultSet;
		}

		public Yahoo.API.ImageSearchResponse.ResultSet ImageSearch(string appId, string query, string type, short results, int start, string format, bool adultOk)
		{
			string requestUri = String.Format(_searchYahooApiAddress + "ImageSearchService/V1/imageSearch?appid={0}&query={1}&type={2}&results={3}&start={4}&format={5}&adult_ok={6}", appId, query, type, results, start, format, adultOk ? "1" : "0");

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUri);

			Yahoo.API.ImageSearchResponse.ResultSet resultSet = null;
			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				using (Stream responseStream = response.GetResponseStream())
				{
					XmlSerializer serializer = new XmlSerializer(typeof(Yahoo.API.ImageSearchResponse.ResultSet));
					resultSet = (Yahoo.API.ImageSearchResponse.ResultSet)serializer.Deserialize(responseStream);
				}
			}

			return resultSet;
		}

		public Yahoo.API.LocalSearchResponse.ResultSet LocalSearch(string appId, string query, short results, int start, float radius, string street, string city, string state, string zip, string location)
		{
			string requestUri = String.Format(_localYahooApiAddress + "LocalSearchService/V1/localSearch?appid={0}&query={1}&results={2}&start={3}&radious={4}&street={5}&city={6}&state={7}&zip={8}&location={9}", appId, query, results, start, radius, street, city, state, zip, location);

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUri);

			Yahoo.API.LocalSearchResponse.ResultSet resultSet = null;
			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				using (Stream responseStream = response.GetResponseStream())
				{
					XmlSerializer serializer = new XmlSerializer(typeof(Yahoo.API.LocalSearchResponse.ResultSet));
					resultSet = (Yahoo.API.LocalSearchResponse.ResultSet)serializer.Deserialize(responseStream);
				}
			}

			return resultSet;
		}

		public Yahoo.API.LocalSearchResponse2.ResultSet LocalSearch2(string appId, string query, short results, int start, string sort, float radius, string street, string city, string state, string zip, string location, float latitude, float longitude)
		{
			string requestUri = String.Format(_localYahooApiAddress + "LocalSearchService/V2/localSearch?appid={0}&query={1}&results={2}&start={3}&sort={4}&radious={5}&street={6}&city={7}&state={8}&zip={9}&location={10}&latitude={11}&longitude={12}&output=xml", appId, query, results, start, sort, radius, street, city, state, zip, location, latitude, longitude);

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUri);

			Yahoo.API.LocalSearchResponse2.ResultSet resultSet = null;
			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				using (Stream responseStream = response.GetResponseStream())
				{
					XmlSerializer serializer = new XmlSerializer(typeof(Yahoo.API.LocalSearchResponse2.ResultSet));
					resultSet = (Yahoo.API.LocalSearchResponse2.ResultSet)serializer.Deserialize(responseStream);
				}
			}

			return resultSet;
		}

		public Yahoo.API.NewsSearchResponse.ResultSet NewsSearch(string appId, string query, string type, short results, int start, string sort, string language)
		{
			string requestUri = String.Format(_searchYahooApiAddress + "NewsSearchService/V1/newsSearch?appid={0}&query={1}&type={2}&results={3}&start={4}&sort={5}&language={6}", appId, HttpUtility.UrlEncode(query, Encoding.UTF8), type, results, start, sort, language);

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUri);

			Yahoo.API.NewsSearchResponse.ResultSet resultSet = null;
			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				using (Stream responseStream = response.GetResponseStream())
				{
					XmlSerializer serializer = new XmlSerializer(typeof(Yahoo.API.NewsSearchResponse.ResultSet));
					resultSet = (Yahoo.API.NewsSearchResponse.ResultSet)serializer.Deserialize(responseStream);
				}
			}

			return resultSet;
		}

		public Yahoo.API.VideoSearchResponse.ResultSet VideoSearch(string appId, string query, string type, short results, int start, string format, bool adultOk)
		{
			string requestUri = String.Format(_searchYahooApiAddress + "VideoSearchService/V1/videoSearch?appid={0}&query={1}&type={2}&results={3}&start={4}&format={5}&adult_ok={6}", appId, HttpUtility.UrlEncode(query, Encoding.UTF8), type, results, start, format, adultOk  ? "1" : "0");

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUri);

			Yahoo.API.VideoSearchResponse.ResultSet resultSet = null;
			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				using (Stream responseStream = response.GetResponseStream())
				{
					XmlSerializer serializer = new XmlSerializer(typeof(Yahoo.API.VideoSearchResponse.ResultSet));
					resultSet = (Yahoo.API.VideoSearchResponse.ResultSet)serializer.Deserialize(responseStream);
				}
			}

			return resultSet;
		}

		public Yahoo.API.WebSearchRelatedResponse.ResultSet WebSearchRelated(string appId, string query, short results)
		{
			string requestUri = String.Format(_searchYahooApiAddress + "WebSearchService/V1/relatedSuggestion?appid={0}&query={1}&results={2}", appId, HttpUtility.UrlEncode(query, Encoding.UTF8), results);

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUri);

			Yahoo.API.WebSearchRelatedResponse.ResultSet resultSet = null;
			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				using (Stream responseStream = response.GetResponseStream())
				{
					XmlSerializer serializer = new XmlSerializer(typeof(Yahoo.API.WebSearchRelatedResponse.ResultSet));
					resultSet = (Yahoo.API.WebSearchRelatedResponse.ResultSet)serializer.Deserialize(responseStream);
				}
			}

			return resultSet;
		}

		public Yahoo.API.WebSearchResponse.ResultSet WebSearch(string appId, string query, string type, short results, int start, string format, bool adultOk, bool similarOk, string language)
		{
			string requestUri = String.Format(_searchYahooApiAddress + "WebSearchService/V1/webSearch?appid={0}&query={1}&type={2}&results={3}&start={4}&format={5}&adult_ok={6}&similar_ok={7}&language={8}", appId, HttpUtility.UrlEncode(query, Encoding.UTF8) , type, results, start, format, adultOk ? "1" : "0", similarOk ? "1" : "0", language);

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUri);

			Yahoo.API.WebSearchResponse.ResultSet resultSet = null;
			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				using (Stream responseStream = response.GetResponseStream())
				{
					XmlSerializer serializer = new XmlSerializer(typeof(Yahoo.API.WebSearchResponse.ResultSet));
					resultSet = (Yahoo.API.WebSearchResponse.ResultSet)serializer.Deserialize(responseStream);
				}
			}

			return resultSet;
		}

		public Yahoo.API.WebSearchSpellingResponse.ResultSet WebSearchSpelling(string appId, string query)
		{
			string requestUri = String.Format(_searchYahooApiAddress + "WebSearchService/V1/spellingSuggestion?appid={0}&query={1}", appId, HttpUtility.UrlEncode(query, Encoding.UTF8));

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUri);

			Yahoo.API.WebSearchSpellingResponse.ResultSet resultSet = null;
			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				XmlSerializer serializer = new XmlSerializer(typeof(Yahoo.API.WebSearchSpellingResponse.ResultSet));
				resultSet = (Yahoo.API.WebSearchSpellingResponse.ResultSet)serializer.Deserialize(response.GetResponseStream());
			}

			return resultSet;
		}
	}
}
