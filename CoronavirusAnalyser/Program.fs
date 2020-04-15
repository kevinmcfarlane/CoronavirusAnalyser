open System
open System.IO
open FSharp.Data

[<Literal>]
let url = @"https://www.worldometers.info/coronavirus/"

[<Literal>]
let PercentConverter = 100.0

type ConfirmedCases = HtmlProvider<url>

let printToCsv() =

    let getDouble v =
        if Double.IsNaN(v) then 0.0 else v
    
    let outputPath = Path.Combine(AppContext.BaseDirectory, "output.csv")
    use stream = new StreamWriter(outputPath)

    let data = ConfirmedCases.GetSample()
    let table = data.Tables.Main_table_countries_today
    let rowCount = table.Rows.Length

    // Headers
    // (table.Headers is string [] option, not string [] so we must pattern-match)
    let headers =
        match table.Headers with 
        | Some h -> sprintf "%s, %s, %s, %s, %s, %s, %s" (h.[0].Replace(",", " or")) h.[1] h.[2] h.[3] h.[4] h.[5] (h.[7].Replace(",", " or"))
        | None -> ""    

    let extendedHeaders = headers + ", Total Deaths / Total Cases, Total Recovered / Total Cases, Serious or Critical / Total Cases"
    stream.WriteLine(extendedHeaders)

    // Rows
    table.Rows
    |> Array.take rowCount
    |> Array.map (fun row -> row.``Country, Other``, row.``Total Cases``, row.``New Cases``, row.``Total Deaths``, row.``New Deaths``, row.``Total Recovered``, row.``Serious, Critical``)
    |> Array.iter (fun (country, totalCases, newCases, totalDeaths, newDeaths, totalRecovered, seriousCritical) -> 
        stream.WriteLine(sprintf "%s, %M, %.0f, %.0f, %.0f, %.0f, %.0f, %.2f%%, %.2f%%, %.2f%%" 
            country 
            totalCases 
            (getDouble newCases) 
            (getDouble totalDeaths) 
            (getDouble newDeaths) 
            (getDouble totalRecovered) 
            (getDouble seriousCritical) 
            (PercentConverter * ((getDouble totalDeaths) / float totalCases))
            (PercentConverter * ((getDouble totalRecovered) / float totalCases))
            (PercentConverter * ((getDouble seriousCritical) / float totalCases))
            ))

[<EntryPoint>]
let main argv =
    printToCsv()
    0 