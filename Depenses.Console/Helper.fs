module Depenses.Console.Helper

open System.Threading.Tasks
open FSharp.Control.Tasks.V2
open Microsoft.AspNetCore.Http
open Saturn
open Giraffe
    
let toJson (get: unit -> Task<'b>) (r: 'c) ctx =
    task {
        let! v = get ()
        return! Controller.json ctx v
    }

let withJsonBody (execute: 'a -> Task<'b>) (r: 'c) (ctx: HttpContext) =
    task {
        let! model = ctx.BindJsonAsync<'a>()
        let! response = execute model
        return! Controller.json ctx response
    }
