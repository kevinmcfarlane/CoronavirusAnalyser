open System
open System.IO
open FSharp.Data

[<Literal>]
let url = @"https://www.worldometers.info/coronavirus/"

type ConfirmedCases = HtmlProvider<url>

let printToConsole() =
    
    let data = ConfirmedCases.GetSample()
    let tables = data.Tables

    // Headers
    match tables.Main_table_countries_today.Headers with 
    | Some headers -> printfn "%s, %s, %s, %s, %s" (headers.[0].Replace(",", " or")) headers.[1] headers.[2] headers.[3] headers.[4]
    | None -> ()     

    // Rows
    tables.Main_table_countries_today.Rows
    |> Array.take 20
    |> Array.map (fun row -> row.``Country, Other``, row.``Total Cases``, row.``New Cases``, row.``Total Deaths``, row.``New Deaths``)
    |> Array.iter (fun (country, totalCases, newCases, totalDeaths, newDeaths) -> 
        printfn "%s, %M, %.0f, %.0f, %.0f" country totalCases (if Double.IsNaN(newCases) then 0.0 else newCases) totalDeaths (if Double.IsNaN(newDeaths) then 0.0 else newDeaths))

let printToCsv() =
    
    let outputPath = Path.Combine(AppContext.BaseDirectory, "output.csv")
    use stream = new StreamWriter(outputPath)

    let data = ConfirmedCases.GetSample()
    let tables = data.Tables

    // Headers
    let headers =
        match tables.Main_table_countries_today.Headers with 
        | Some h -> sprintf "%s, %s, %s, %s, %s" (h.[0].Replace(",", " or")) h.[1] h.[2] h.[3] h.[4]
        | None -> ""    

    stream.WriteLine(headers)

    // Rows
    tables.Main_table_countries_today.Rows
    |> Array.take 20
    |> Array.map (fun row -> row.``Country, Other``, row.``Total Cases``, row.``New Cases``, row.``Total Deaths``, row.``New Deaths``)
    |> Array.iter (fun (country, totalCases, newCases, totalDeaths, newDeaths) -> 
        stream.WriteLine(sprintf "%s, %M, %.0f, %.0f, %.0f" country totalCases (if Double.IsNaN(newCases) then 0.0 else newCases) totalDeaths (if Double.IsNaN(newDeaths) then 0.0 else newDeaths)))

[<EntryPoint>]
let main argv =
    printToCsv()
    0 // return an integer exit code