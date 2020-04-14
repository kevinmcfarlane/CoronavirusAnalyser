open System
open System.IO
open FSharp.Data

[<Literal>]
let url = @"https://www.worldometers.info/coronavirus/"

[<Literal>]
let PercentConverter = 100.0

type ConfirmedCases = HtmlProvider<url>

let printToConsole() =
    
    let data = ConfirmedCases.GetSample()
    let tables = data.Tables

    // Headers
    match tables.Main_table_countries_today.Headers with 
    | Some headers -> printfn "%s, %s, %s, %s, %s, %s, %s" (headers.[0].Replace(",", " or")) headers.[1] headers.[2] headers.[3] headers.[4] headers.[5] (headers.[7].Replace(",", " or"))
    | None -> ()     

    // Rows
    tables.Main_table_countries_today.Rows
    |> Array.take 21
    |> Array.map (fun row -> row.``Country, Other``, row.``Total Cases``, row.``New Cases``, row.``Total Deaths``, row.``New Deaths``, row.``Total Recovered``, row.``Serious, Critical``)
    |> Array.iter (fun (country, totalCases, newCases, totalDeaths, newDeaths, totalRecovered, seriousCritical) -> 
        printfn "%s, %M, %.0f, %.0f, %.0f, %.0f, %.0f" 
            country 
            totalCases 
            (if Double.IsNaN(newCases) then 0.0 else newCases) 
            totalDeaths 
            (if Double.IsNaN(newDeaths) then 0.0 else newDeaths)
            totalRecovered
            seriousCritical)

let printToCsv() =
    
    let outputPath = Path.Combine(AppContext.BaseDirectory, "output.csv")
    use stream = new StreamWriter(outputPath)

    let data = ConfirmedCases.GetSample()
    let tables = data.Tables

    // Headers
    // (tables.Main_table_countries_today.Headers is string [] option, not string [] so we must pattern-match)
    let headers =
        match tables.Main_table_countries_today.Headers with 
        | Some h -> sprintf "%s, %s, %s, %s, %s, %s, %s" (h.[0].Replace(",", " or")) h.[1] h.[2] h.[3] h.[4] h.[5] (h.[7].Replace(",", " or"))
        | None -> ""    

    let extendedHeaders = headers + ", Total Deaths / Total Cases, Total Recovered / Total Cases, Serious or Critical / Total Cases"
    stream.WriteLine(extendedHeaders)

    // Rows
    tables.Main_table_countries_today.Rows
    |> Array.take 21
    |> Array.map (fun row -> row.``Country, Other``, row.``Total Cases``, row.``New Cases``, row.``Total Deaths``, row.``New Deaths``, row.``Total Recovered``, row.``Serious, Critical``)
    |> Array.iter (fun (country, totalCases, newCases, totalDeaths, newDeaths, totalRecovered, seriousCritical) -> 
        stream.WriteLine(sprintf "%s, %M, %.0f, %.0f, %.0f, %.0f, %.0f, %.2f%%, %.2f%%, %.2f%%" 
            country 
            totalCases 
            (if Double.IsNaN(newCases) then 0.0 else newCases) 
            totalDeaths 
            (if Double.IsNaN(newDeaths) then 0.0 else newDeaths) 
            totalRecovered
            seriousCritical
            (PercentConverter * (totalDeaths / float totalCases))
            (PercentConverter * (totalRecovered / float totalCases))
            (PercentConverter * (seriousCritical / float totalCases))
            ))

[<EntryPoint>]
let main argv =
    printToCsv()
    printToConsole()
    0 