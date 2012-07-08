path "C:\Program Files\Microsoft Visual Studio .NET 2003\SDK\v1.1\Bin"
xsd.exe schemas\v1\ImageSearchResponse.xsd /c /l:cs /namespace:Yahoo.API.ImageSearchResponse /out:Yahoo.API
xsd.exe schemas\v1\LocalSearchResponse.xsd /c /l:cs /namespace:Yahoo.API.LocalSearchResponse /out:Yahoo.API
xsd.exe schemas\v1\NewsSearchResponse.xsd /c /l:cs /namespace:Yahoo.API.NewsSearchResponse /out:Yahoo.API
xsd.exe schemas\v1\VideoSearchResponse.xsd /c /l:cs /namespace:Yahoo.API.VideoSearchResponse /out:Yahoo.API
xsd.exe schemas\v1\WebSearchRelatedResponse.xsd /c /l:cs /namespace:Yahoo.API.WebSearchRelatedResponse /out:Yahoo.API
xsd.exe schemas\v1\WebSearchResponse.xsd /c /l:cs /namespace:Yahoo.API.WebSearchResponse /out:Yahoo.API
xsd.exe schemas\v1\WebSearchSpellingResponse.xsd /c /l:cs /namespace:Yahoo.API.WebSearchSpellingResponse /out:Yahoo.API
xsd.exe schemas\v1\TermExtractionResponse.xsd /c /l:cs /namespace:Yahoo.API.TermExtractionResponse /out:Yahoo.API

xsd.exe schemas\v2\LocalSearchResponse2.xsd /c /l:cs /namespace:Yahoo.API.LocalSearchResponse2 /out:Yahoo.API

pause
