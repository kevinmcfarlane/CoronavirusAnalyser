open System
open FSharp.Data

[<Literal>]
let url = @"https://www.worldometers.info/coronavirus/"

type ConfirmedCases = HtmlProvider<url>

let explore() =
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

[<EntryPoint>]
let main argv =
    explore()
    0 // return an integer exit code