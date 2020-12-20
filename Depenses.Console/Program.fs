// Learn more about F# at http://fsharp.org

open System

open System.IO
open System.Threading.Tasks

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open FSharp.Control.Tasks.V2
open Giraffe
open Saturn

open Microsoft.AspNetCore.Http
open Microsoft.Net.Http.Headers
open FSharp.Control.Tasks.V2.ContextInsensitive
open Giraffe.GiraffeViewEngine
open Depenses.Console.Helper

(*
[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    0 // return an integer exit code
    *)
(*let tryGetEnv = System.Environment.GetEnvironmentVariable >> function null | "" -> None | x -> Some x


let port =
    "SERVER_PORT"
    |> tryGetEnv |> Option.map uint16 |> Option.defaultValue 8085us

let getDepenses () = 
    task {
        let depense = ["toto"] 
        return depense
    }

let mainRouter = router {
    get "api" ( getDepenses |> toJson )
}

let app = application {
    url ("http://0.0.0.0:" + port.ToString() + "/")
    use_router mainRouter
    memory_cache
    use_json_serializer(Thoth.Json.Giraffe.ThothSerializer())
    use_gzip
}

run app*)
open System.Threading.Tasks
open Giraffe
open Saturn
open FSharp.Control.Tasks.V2
open Depenses.Console.Helper
open Depenses.Console
open Depenses.Types

let port = 8085us





let mainRouter =
    router {
        forward "/depenses" (DepenseRoutes.route InDatabaseAPI)
    }

let app =
    application {
        use_router mainRouter
        url ("http://0.0.0.0:" + port.ToString() + "/")
        memory_cache
        use_json_serializer(Thoth.Json.Giraffe.ThothSerializer())
        use_gzip
    }

let exitCode = 0

[<EntryPoint>]
let main _ =        
    run app
    exitCode