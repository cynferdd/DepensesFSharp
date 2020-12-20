module Depenses.Console.DepenseRoutes

open Depenses.Types
open System.Threading.Tasks
open Giraffe
open Saturn
open FSharp.Control.Tasks.V2
open Depenses.Console.Helper
open Depenses.Shared
open System


    
let route api=
    router {
        get "" (api.GetDepenses |> toJson)
        post "" (api.CreateDepense |> withJsonBody)
    }

