module Depenses.Types
open Depenses.Shared
open System.Threading.Tasks
open FSharp.Control.Tasks.V2
open System
open FSharp.Data

type DbDepenseError = 
| UtilisateurIdEmpty of int
| SqlException of int

type Api = {
    GetDepenses: unit -> Task<Depense list>
    CreateDepense: DepenseToCreate -> Task<unit>
}

(*

let InMemoryAPI = 
    let mutable depenses = [
        {
            Id = 0
            UtilisateurId = 0
            Date = DateTime(2020,05,17)
            Nature = Hotel
            Montant = 43.8
            Devise = "EUR"
            Commentaire = Some "body once told me"
        }
    ]


    {
        GetDepenses= fun () -> 
            task {
                do! Task.Delay(100)
                return depenses
            }

        CreateDepense= fun depense -> 
            task {
                do! Task.Delay(100)
                depenses <- depenses @ [depense]
            }
    }
    *)


/////////////////////////////////////////////////////commentaire//////////////////////////////////////////////////////////////



module Sql=
    [<Literal>]
    let connectionString = "Server=(local);Database=CleemyDepenses;Trusted_Connection=True;MultipleActiveResultSets=true"
    
    module CreateDepense=
        let private nature depense = 
            match depense.Nature with                
            | Restaurant -> 1
            | Hotel -> 2
            | Misc -> 0

        let private devise depense = 
            match depense.Devise with
            | "RBL" -> 1
            | "EUR" -> 2
            | "USD" -> 0
            | _ -> invalidOp "devise inconnue" 

        let private com depense = 
            match depense.Commentaire with
            | Some v -> v
            | None -> null

        let private insertIntoDb utilisateurId date nature montant devise commentaire = 
            task {
                use cmd = new SqlCommandProvider<"
                INSERT INTO Depenses(
        		        UtilisateurId,
        		        Date,
        		        Nature,
        		        Montant,
        		        Devise,
        		        Commentaire)
                Values
                (
                    @UtilisateurId,
                    @Date,
                    @Nature,
                    @Montant,
                    @Devise,
                    @Commentaire
                )
                " , connectionString>(connectionString)

        

                let! dbResult =
                    cmd.AsyncExecute(
                        UtilisateurId=utilisateurId, 
                        Date=date,
                        Nature=nature,
                        Montant=montant,
                        Devise=devise,
                        Commentaire= commentaire
                    )
                
                match dbResult with
                | 1 -> ()
                | errorCode -> failwithf "%i" errorCode
            }

        let run depense = 
            task{
                return! insertIntoDb 
                        depense.UtilisateurId 
                        depense.Date 
                        (nature depense)
                        (float32 depense.Montant) 
                        (devise depense) 
                        (com depense)
            }
            
    





let InDatabaseAPI = 
    
        

    {
        GetDepenses= fun () -> 
            task {
                use cmd = new SqlCommandProvider<"
                    SELECT  Id,
                    		UtilisateurId,
                    		Date,
                    		Nature,
                    		Montant,
                    		Devise,
                    		Commentaire
                    FROM Depenses
                    " , Sql.connectionString>(Sql.connectionString)

                let! dbDepenses = cmd.AsyncExecute() 
                return 
                    dbDepenses
                    |> Seq.map (fun t -> 
                        match t.UtilisateurId with
                        | None -> invalidOp "UtilisateurIdEmpty" //(UtilisateurIdEmpty t.Id )
                        | Some userId ->
                            let nature = 
                                match t.Nature with                
                                | 1 -> Restaurant
                                | 2 -> Hotel
                                | _ -> Misc

                            let devise = 
                                match t.Devise with
                                | 1 -> "RBL"
                                | 2 -> "EUR"
                                | _ -> "USD"

                            {
                                Id = t.Id
                                UtilisateurId = userId
                                Date = t.Date
                                Devise = devise
                                Montant = float t.Montant
                                Nature = nature
                                Commentaire = t.Commentaire
                            }) 
                    |> Seq.toList 
                
            }

        
        CreateDepense= Sql.CreateDepense.run
            
    }
