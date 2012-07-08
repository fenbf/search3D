//
// simpleSearch.cs -- simple web services search in C#
// Author: Daniel Jones
// Copyright 2006 Daniel Jones
// Licensed under BSD open source license
// http://www.opensource.org/licenses/bsd-license.php
//
class YahooWebServiceExample
{
        static void Main(string[] args)
        {
                System.Net.WebClient webClient = new System.Net.WebClient();

                const string request =
"http://search.yahooapis.com/WebSearchService/V1/webSearch?appid=YahooDemo&query=madonna&results=2";
                byte[] responseXML;
                try
                {
                        responseXML = webClient.DownloadData(request);
                        System.Text.UTF8Encoding objUTF8 = new
                        System.Text.UTF8Encoding();

System.Console.WriteLine(objUTF8.GetString(responseXML));
                }
                catch (System.Exception)
                {
                        System.Console.WriteLine("Web services request failed");
                }
        }
}
