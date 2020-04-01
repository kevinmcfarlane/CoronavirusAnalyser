open FSharp.Data

[<Literal>]
let url = @"https://www.worldometers.info/coronavirus/#countries"

type ConfirmedCases = HtmlProvider<url>

let explore() =
    let data = ConfirmedCases.GetSample()
    let tables = data.Tables
    tables.Main_table_countries_today.Rows
    |> Array.take 20
    |> Array.map (fun row -> row.``Country, Other``, row.``Total Cases``, row.``New Cases``, row.``Total Deaths``, row.``New Deaths``)
    |> Array.iter (fun (country, totalCases, newCases, totalDeaths, newDeaths) -> 
        printfn "Country: %s, Total Cases: %M, New Cases: %.0f, Total Deaths: %.0f, New Deaths: %.0f" country totalCases newCases totalDeaths newDeaths)

[<EntryPoint>]
let main argv =
    explore()
    0 // return an integer exit code