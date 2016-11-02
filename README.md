# search-suggest
Basic improvement of search engine suggestions through a ranking system.

## Setup and Operating Instructions
* Database: Already setup!

* WebPage: [html/index.html](./html/index.html)

### Keyword Server:
To run the C# server, two steps must be taken.
1. First, the .config file must be modified. The config file will already include an entry similar to the following:
```xml
<appSettings>
    <add key="IgnoredWordsTxtPath" value="C:\LocalFilePath\IgnoredWords.txt"/>
</appSettings>
```
This must be changed to reference an existing file that contains a number of words separated by new line characters. These words will be ignored while searching for keywords.
2. Run the .exe file in the same folder. A console window will appear indicating that the server is now running and can now be communicated with through a WebSocket.


Currently the server is hardcoded to operate on localhost. To access it using a standard WebSocket the URL `ws://localhost:1234/service` must be used. This can be easily changed should the need arrise.
## References
* Uses [word frequency data](http://www.wordfrequency.info/free.asp).
* Code for extracting base text from HTML taken from [here](https://consultrikin.wordpress.com/2013/02/21/c-get-plain-text-from-html-string/).