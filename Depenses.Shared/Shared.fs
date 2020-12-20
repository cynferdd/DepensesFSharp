namespace Depenses.Shared
open System

type Nature =
    | Misc
    | Restaurant
    | Hotel


type Depense = {
    Id: int 
    UtilisateurId: int
    Date: DateTime
    Nature: Nature
    Montant: float
    Devise: string
    Commentaire: Option<string>
}

type DepenseToCreate = {
    UtilisateurId: int
    Date: DateTime
    Nature: Nature
    Montant: float
    Devise: string
    Commentaire: Option<string>
}