# Worldometer Coronavirus Analysis 

Worldometer [Coronavirus] daily statistics. Analysis of the [Countries] table.




#### Results - Excel output for Top 20 countries by Total Cases with extra calculated columns


![Sorted output - deaths/cases](Images/excel-output.png)


#### Technology

[F# Data Library] for data access, in particular the [HTML Type Provider]. This allows us to "screen scrape" a web page and strongly infer the names and data types in a HTML table, for example. These will then appear in the F# IntelliSense with full type-checking.

[Coronavirus]: <https://www.worldometers.info/coronavirus/>
[Countries]: <https://www.worldometers.info/coronavirus/#countries>
[F# Data Library]: <https://fsharp.github.io/FSharp.Data/>
[HTML Type Provider]: <https://fsharp.github.io/FSharp.Data/library/HtmlProvider.html>